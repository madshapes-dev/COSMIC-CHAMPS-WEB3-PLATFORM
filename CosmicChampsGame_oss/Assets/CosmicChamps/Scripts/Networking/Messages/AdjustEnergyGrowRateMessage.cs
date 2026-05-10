using CosmicChamps.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct AdjustEnergyGrowRateMessage : NetworkMessage
    {
        public EnergyGrowRate EnergyGrowRate;
    }
}