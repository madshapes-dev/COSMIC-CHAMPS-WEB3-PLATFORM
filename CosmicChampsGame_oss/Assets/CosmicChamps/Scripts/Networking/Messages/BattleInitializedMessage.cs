using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct BattleInitializedMessage : NetworkMessage
    {
        public BattleInitData BattleInitData;
    }
}