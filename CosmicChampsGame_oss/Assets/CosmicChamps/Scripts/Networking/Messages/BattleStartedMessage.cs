using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct BattleStartedMessage : NetworkMessage
    {
        public BattleStartedData BattleStartedData;
    }
}