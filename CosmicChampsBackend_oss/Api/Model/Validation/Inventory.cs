namespace CosmicChamps.Api.Model.Validation;

public class Inventory
{
    public static Inventory Empty = new()
    {
        Items = Array.Empty<InventoryItem> (),
        Boosts = Array.Empty<Boost> ()
    };

    public InventoryItem[] Items { set; get; }
    public Boost[] Boosts { set; get; }
}