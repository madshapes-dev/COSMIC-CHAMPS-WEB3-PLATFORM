using System.Collections.Generic;
using CosmicChamps.Data;

namespace CosmicChamps.Bootstrap.Server
{
    public class PlayersProvider
    {
        public ICollection<Player> Players { set; get; }
    }
}