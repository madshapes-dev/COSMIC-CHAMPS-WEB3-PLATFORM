using System.Collections.Generic;

namespace CosmicChamps.Data
{
    public class LocalGameSessionData
    {
        public GameMode GameMode { set; get; }
        public string TournamentId { set; get; }
        public List<string> PlayerIds { set; get; }
        public string MatchmakingConfigurationArn { set; get; }
    }
}