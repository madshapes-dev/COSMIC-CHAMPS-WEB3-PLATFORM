using UnityEngine;

namespace CosmicChamps
{
    public class SystemInfoDeviceIdProvider : IDeviceIdProvider
    {
        public string DeviceId => SystemInfo.deviceUniqueIdentifier;
    }
}