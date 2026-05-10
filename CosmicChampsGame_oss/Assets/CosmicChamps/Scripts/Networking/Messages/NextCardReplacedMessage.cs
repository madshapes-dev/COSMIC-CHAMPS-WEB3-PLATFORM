using CosmicChamps.Battle.Data;
using Mirror;

namespace CosmicChamps.Networking.Messages
{
    public struct NextCardReplacedMessage : NetworkMessage
    {
        public NextCardReplacement NextCardReplacement;
    }
}