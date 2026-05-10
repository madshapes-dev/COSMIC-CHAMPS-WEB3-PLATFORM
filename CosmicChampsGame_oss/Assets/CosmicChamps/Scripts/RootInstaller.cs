using System;
using System.IO;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using CosmicChamps.UI;
using Newtonsoft.Json;
using Oddworm.Framework;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CosmicChamps
{
    public class RootInstaller : MonoInstaller
    {
        public const string AppProfileField = nameof (_appProfile);

        [SerializeField]
        private AppProfile _appProfile;

        [SerializeField]
        private ServerProcessOptions _serverProcessOptions;

        public AppProfile AppProfile => _appProfile;

        public override void InstallBindings ()
        {
            CommandLine.Init (Environment.CommandLine);

            Container.BindInstance<AppProfile> (_appProfile);
            Container.BindInstance<IMessageBroker> (MessageBroker.Default);
            Container.BindInstance<IAsyncMessageBroker> (AsyncMessageBroker.Default);
            Container.BindInstance<UILocker> (new UILocker (EventSystem.current));

            Container.BindAsSingle<PlayerPrefsService> ();
            #if UNITY_WEBGL && !UNITY_EDITOR
            Container.Bind<IDeviceIdProvider>().To<WebGLDeviceIdeProvider>().AsSingle();
            #else
            Container.Bind<IDeviceIdProvider> ().To<SystemInfoDeviceIdProvider> ().AsSingle ();
            #endif

            #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || UNITY_SERVER
            var logPath = CommandLine.GetString (CommandLineOptions.LogFile, _serverProcessOptions.LogFile);
            var port = CommandLine.GetInt (CommandLineOptions.Port, _serverProcessOptions.Port);

            Container.Bind<ServerProcessOptions> ().FromInstance (new ServerProcessOptions (logPath, port));
            // Container.Bind<AWSCredentials> ().FromResources ("AWSCredentials");

            var gameLiftMetadataPath = GameLiftMetadata.Path;
            Container
                .Bind<GameLiftMetadata> ()
                .FromInstance (
                    !string.IsNullOrEmpty (gameLiftMetadataPath)
                        ? JsonConvert.DeserializeObject<GameLiftMetadata> (File.ReadAllText (gameLiftMetadataPath))
                        : null);

            var gameLiftInstanceMetadataPath = GameLiftInstanceMetadata.Path;
            Container
                .Bind<GameLiftInstanceMetadata> ()
                .FromInstance (
                    !string.IsNullOrEmpty (gameLiftInstanceMetadataPath)
                        ? JsonConvert.DeserializeObject<GameLiftInstanceMetadata> (
                            File.ReadAllText (gameLiftInstanceMetadataPath))
                        : null);
            #endif
        }
    }
}