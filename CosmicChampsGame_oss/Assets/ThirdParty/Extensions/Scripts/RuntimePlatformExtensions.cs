using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class RuntimePlatformExtensions
    {
        public static bool IsServer (this RuntimePlatform platform) =>
            platform is RuntimePlatform.LinuxServer or RuntimePlatform.WindowsServer or RuntimePlatform.OSXServer;
    }
}