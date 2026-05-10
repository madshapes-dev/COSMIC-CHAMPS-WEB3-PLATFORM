using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct BattleStartingMessage : NetworkMessage
    {
        public BattleStartingData BattleStartingData;
    }
}