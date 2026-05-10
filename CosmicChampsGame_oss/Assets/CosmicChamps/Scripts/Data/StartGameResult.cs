using OneOf;

namespace CosmicChamps.Data
{
    public class StartGameResult : OneOfBase<PlayerGameSession, MatchmakingTicket, MatchmakingTimeout, MatchmakingCancellation>
    {
        private StartGameResult (
            OneOf<PlayerGameSession, MatchmakingTicket, MatchmakingTimeout, MatchmakingCancellation> _) : base (_)
        {
        }

        public static implicit operator StartGameResult (PlayerGameSession _) => new(_);
        public static implicit operator StartGameResult (MatchmakingTicket _) => new(_);
        public static implicit operator StartGameResult (MatchmakingTimeout _) => new(_);
        public static implicit operator StartGameResult (MatchmakingCancellation _) => new(_);
    }
}