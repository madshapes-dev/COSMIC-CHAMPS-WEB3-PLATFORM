using System.Collections.Generic;

namespace CosmicChamps.Data
{
    public class GameSessionMatchmakerData
    {
        public class PlayerAttribute
        {
            public object valueAttribute { set; get; }
        }
        
        public class Player
        {
            public string playerId { set; get; }
            public Dictionary<string, PlayerAttribute> attributes { set; get; }
        }

        public class Team
        {
            public string name { set; get; }
            public List<Player> players { set; get; }
        }

        public string matchId { set; get; }
        public string matchmakingConfigurationArn { set; get; }
        public List<Team> teams { set; get; }
    }
}