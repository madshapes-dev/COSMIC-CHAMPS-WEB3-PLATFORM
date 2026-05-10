using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;
using Logger = Serilog.Core.Logger;

namespace CosmicChamps.Bootstrap.Client
{
    public class LogInstaller : Bootstrap.LogInstaller
    {
        protected override ILogger CreateLogger (InjectContext injectContext)
        {
            return Debug.isDebugBuild
                ? base.CreateLogger (injectContext)
                : Logger.None;
        }
    }
}