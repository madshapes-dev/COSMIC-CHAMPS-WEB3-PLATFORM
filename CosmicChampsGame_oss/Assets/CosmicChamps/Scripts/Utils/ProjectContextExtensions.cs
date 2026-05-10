using CosmicChamps.Settings;
using Zenject;

namespace CosmicChamps.Utils
{
    public static class ProjectContextExtensions
    {
        public static AppProfile GetAppProfile (this ProjectContext projectContext) =>
            projectContext.GetComponent<RootInstaller> ().AppProfile;
    }
}