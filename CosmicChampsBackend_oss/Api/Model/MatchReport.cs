using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

[DynamoDBTable (nameof (MatchReport))]
public class MatchReport
{
    public class Player
    {
        public class Card
        {
            public string Id { set; get; }
            public string Skin { set; get; }
        }
        
        public class Boost
        {
            public string Id { set; get; }
            public int Hp { set; get; }
            public int Damage { set; get; }
            public int DeathDamage { set; get; }
        }        
        
        public string Id { set; get; }
        public string WalletId { set; get; }
        public string? LinkedWalletId { set; get; }
        public string? Nickname { set; get; }
        public Card[] Deck { set; get; }
        public Boost[] Boosts { set; get; }
        public string? HUDSkin { set; get; }
        public int Rating { set; get; }
        public int PrizeBotGamesPlayed { set; get; }
    }
    
    [DynamoDBHashKey]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string PlayerA { set; get; }
    
    [DynamoDBProperty]
    public string PlayerB { set; get; } 
    
    [DynamoDBProperty]
    public string WinnerId { set; get; }
    
    [DynamoDBProperty]
    public Player[] Players { set; get; }
    
    [DynamoDBProperty]
    public string? Date { set; get; }
    
    [DynamoDBProperty]
    public int Duration { set; get; }
    
    [DynamoDBProperty]
    public string? Environment { set; get; }
}