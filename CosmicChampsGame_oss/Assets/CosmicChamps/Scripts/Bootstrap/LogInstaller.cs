using CosmicChamps.Log;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Unity3D;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Bootstrap
{
    public class LogInstaller : MonoInstaller
    {
        [SerializeField]
        protected LogEventLevel _logLevel;

        private ILogger _baseLogger;

        protected virtual LoggerConfiguration CreateBaseLoggerConfiguration (InjectContext _)
        {
            var messageTemplate = Application.isEditor ? SerilogOutputTemplates.Editor : SerilogOutputTemplates.Build;
            return new LoggerConfiguration ()
                .MinimumLevel.ControlledBy (new LoggingLevelSwitch (_logLevel))
                .WriteTo.Unity3D (outputTemplate: messageTemplate);
        }

        protected virtual ILogger CreateLogger (InjectContext injectContext)
        {
            _baseLogger ??= CreateBaseLoggerConfiguration (injectContext).CreateLogger ();

            var logger = _baseLogger;
            if (injectContext.ObjectInstance is Object unityObjectInstance)
                logger = logger.ForContext (unityObjectInstance);

            return logger
                .WithUnityCustomTag (injectContext.ObjectType.Name)
                #if UNITY_EDITOR
                .WithUnityEditorColorLogLevelsTag (
                    SerilogLogLevelColors.Verbose,
                    SerilogLogLevelColors.Debug,
                    SerilogLogLevelColors.Information,
                    SerilogLogLevelColors.Warning,
                    SerilogLogLevelColors.Error)
                #endif
                .WithFrameCount ();
        }

        public override void InstallBindings ()
        {
            Container
                .Bind<ILogger> ()
                .FromMethod (CreateLogger)
                .AsTransient ();
        }
    }
}