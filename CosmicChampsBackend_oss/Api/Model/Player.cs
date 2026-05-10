using Amazon.DynamoDBv2.DataModel;
using CosmicChamps.Common.Utils;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (Player))]
public class Player
{
    [DynamoDBHashKey]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string? Nickname { set; get; }

    [DynamoDBProperty]
    public string Email { set; get; }

    [DynamoDBProperty]
    public PlayerUnit[] Units { set; get; }

    [DynamoDBProperty]
    public PlayerCard[] Cards { set; get; }

    [DynamoDBProperty]
    public PlayerDeck[]? Decks { set; get; }

    [DynamoDBProperty]
    public int ActiveDeckIndex { set; get; }

    [DynamoDBProperty]
    public string WalletId { set; get; }

    [DynamoDBProperty]
    public string? LinkedWalletId { set; get; }

    [DynamoDBProperty]
    public string? ImmutableWalletId { set; get; }

    [DynamoDBProperty]
    public string? ImmutableId { set; get; }

    [DynamoDBProperty]
    public string LatestReadNews { set; get; }

    [DynamoDBProperty]
    public string? HUDSkin { set; get; }

    [DynamoDBProperty]
    public int Rating { set; get; }

    [DynamoDBProperty]
    public UnitBoost[]? Boosts { set; get; }

    [DynamoDBProperty]
    public string? TournamentId { set; get; }

    [DynamoDBProperty]
    public int GamesPlayed { set; get; }
    
    [DynamoDBProperty]
    public int MissionGamesPlayed { set; get; }
    
    [DynamoDBProperty]
    public int PrizeBotGamesPlayed { set; get; }

    [DynamoDBProperty]
    public string? PrizeBotGamesPlayedTimestamp { set; get; }

    [DynamoDBProperty]
    public string? LastPlayedGameDate { set; get; }

    [DynamoDBProperty]
    public PlayerShipCard[]? ShipCards { set; get; }

    [DynamoDBProperty]
    public PlayerShipSlot ShipSlot { set; get; }

    [DynamoDBProperty]
    public int NicknameChangeCount { set; get; }

    [DynamoDBProperty]
    public bool SignUpCompleted { set; get; }

    [DynamoDBProperty]
    public string[] Emojis { set; get; }

    [DynamoDBProperty]
    public int UniversalShards { set; get; }

    [DynamoDBProperty]
    public PlayerCardShards[] CardShards { set; get; }

    [DynamoDBProperty]
    public PlayerBattleReward[]? BattleRewards { set; get; }

    [DynamoDBProperty]
    public int Level { set; get; }

    [DynamoDBProperty]
    public int Exp { set; get; }

    [DynamoDBProperty]
    public int GamesBeforePrizeBot { set; get; }

    [DynamoDBProperty]
    public List<string>? PromoCodes { set; get; }

    [DynamoDBProperty (typeof (EnumStringConverter<AccountType>))]
    public AccountType AccountType { set; get; }
}