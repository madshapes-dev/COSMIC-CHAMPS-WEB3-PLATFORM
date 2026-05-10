using CosmicChamps.Api.Services;

namespace CosmicChamps.Api.Model.Validation;

public static class InventoryExtensions
{
    public static void ApplyToPlayer (
        this Inventory inventory,
        Player player,
        GameData gameData,
        List<HUDSkin> hudSkins)
    {
        string GetUnitId (InventoryItem item)
        {
            var inventoryItemIdChunks = item._id.Split ('_');
            return inventoryItemIdChunks.Length == 0
                ? string.Empty
                : inventoryItemIdChunks[0];
        }

        var cardInventoryItems = inventory
            .Items
            .Distinct (InventoryItem.Comparer)
            .Select (x => (unitId: GetUnitId (x), item: x))
            .Where (x => !string.IsNullOrEmpty (x.unitId))
            .GroupBy (x => x.unitId)
            .ToArray ();

        foreach (var unitData in gameData.Units)
        {
            var playerUnit = player.Units.FirstOrDefault (x => x.Id == unitData.Id);
            if (playerUnit == null)
            {
                playerUnit = new PlayerUnit { Id = unitData.Id };
                player.Units = player.Units.Append (playerUnit).ToArray ();
            }

            var cardInventoryItem = cardInventoryItems.FirstOrDefault (x => x.Key == playerUnit.Id)?.Select (x => x.item) ??
                                    Enumerable.Empty<InventoryItem> ();
            playerUnit.Skins = cardInventoryItem
                .Select (x => x._id)
                .Intersect (unitData.Aliases)
                .Prepend (playerUnit.Id)
                .ToArray ();
        }

        var spacerocks = inventory.Items.Where (x => x._id.StartsWith ("spacerock")).ToList ();

        player.HUDSkin = hudSkins.LastOrDefault (x => spacerocks.Exists (y => y._id == x.Ntf))?.Skin ?? string.Empty;
        player.Boosts = inventory
            .Boosts
            .Select (
                x => new UnitBoost
                {
                    Id = x.id,
                    Hp = x.hp,
                    Damage = x.dmg,
                    DeathDamage = x.deathDmg
                })
            .ToArray ();
    }
}