using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmicChamps.Data
{
    public class GameSession
    {
        public string Id { set; get; }
        public string IpAddress { set; get; }
        public string DnsName { set; get; }
        public int Port { set; get; }
        public ICollection<Player> Players { set; get; }
        public string WinnerId { set; get; }
        public GameMode GameMode { set; get; }
        public string TournamentId { set; get; }
        public string Level { set; get; }

        public Player GetPlayer (string id)
        {
            var player = Players.FirstOrDefault (x => x.Id == id);
            if (player == null)
                throw new InvalidOperationException ($"Cannot find player {id}");

            return player;
        }

        public string[] GetUnitIds () => Players
            .SelectMany (x => x.ActiveDeck.Cards.Select (y => y.Skin))
            .Distinct ()
            .ToArray ();

        public PlayerDeckCard[] GetCards () => Players
            .SelectMany (x => x.ActiveDeck.Cards)
            .Distinct ()
            .ToArray ();
    }
}