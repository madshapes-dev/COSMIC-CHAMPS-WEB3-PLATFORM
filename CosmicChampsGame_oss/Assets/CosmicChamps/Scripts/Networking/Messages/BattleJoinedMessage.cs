using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct BattleJoinedMessage : NetworkMessage
    {
        public BattleJoinData BattleJoinData;
    }
}