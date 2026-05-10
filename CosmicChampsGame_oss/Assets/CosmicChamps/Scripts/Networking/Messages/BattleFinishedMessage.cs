using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct BattleFinishedMessage : NetworkMessage
    {
        public BattleFinishData BattleFinishData;
    }
}