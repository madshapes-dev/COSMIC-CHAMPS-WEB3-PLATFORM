using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct UnitSpawningMessage : NetworkMessage
    {
        public UnitSpawningData UnitSpawningData;
    }
}