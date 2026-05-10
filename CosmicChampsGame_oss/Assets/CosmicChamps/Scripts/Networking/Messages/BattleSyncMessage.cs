using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct BattleSyncMessage : NetworkMessage
    {
        public BattleSyncData BattleSyncData;
    }
}