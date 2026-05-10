using CosmicChamps.Utils;
using Zenject;

namespace CosmicChamps.Settings
{
    public static class AppProfileAddressablesSettings
    {
        public static string BundlesLoadUrl => ProjectContext.Instance.GetAppProfile ().BundlesLoadUrl;
    }
}