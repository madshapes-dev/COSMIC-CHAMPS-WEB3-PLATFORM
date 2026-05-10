using System;
using System.Net.Http;
using CosmicChamps.Api.Services;
using CosmicChamps.Data;
using CosmicChamps.Data.Sources.Tokens;
using CosmicChamps.HomeScreen;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Logging;
using ThirdParty.Extensions;
using TransformsAI.Unity.Grpc.Web;
using Zenject;

namespace CosmicChamps.Bootstrap.Client
{
    public class DataLayerInstaller : Installer<NetworkingConfig, DataLayerInstaller>
    {
        [Inject]
        private NetworkingConfig _networkingConfig;

        private void RegisterNetworkDataSources ()
        {
            Container
                .BindAsSingle<BearerAuthorizationHttpClientHandler> ()
                .WithArguments (new HttpClientHandler ());

            var serviceConfig = new ServiceConfig
            {
                MethodConfigs =
                {
                    new MethodConfig
                    {
                        Names = { MethodName.Default },
                        RetryPolicy = new RetryPolicy
                        {
                            MaxAttempts = int.MaxValue,
                            InitialBackoff = TimeSpan.FromSeconds (_networkingConfig.RequestRetryRate),
                            MaxBackoff = TimeSpan.FromSeconds (_networkingConfig.RequestRetryRate),
                            BackoffMultiplier = double.MaxValue,
                            RetryableStatusCodes = { StatusCode.Internal, StatusCode.Unavailable }
                        }
                    }
                }
            };

            ILoggerFactory CreateLoggerFactory (InjectContext context)
            {
                var loggerFactory = new GrpcLoggerFactory ();
                loggerFactory.AddProvider (context.Container.Instantiate<NetworkReachabilityGrpcLogger.Provider> ());
                if (_networkingConfig.LogGrpcCalls)
                    loggerFactory.AddProvider (new UnityDebugGrpcLogger.Provider ());

                return loggerFactory;
            }

            Container
                .Bind<Auth.Services.Auth.AuthClient> ()
                .FromMethod (
                    context =>
                    {
                        var channel = GrpcChannel.ForAddress (
                            context
                                .Container
                                .Resolve<AppProfile> ()
                                .AuthServiceUrl,
                            new GrpcChannelOptions
                            {
                                MaxRetryAttempts = int.MaxValue,
                                HttpHandler = UnityGrpcWebHandler.Create (GrpcWebMode.GrpcWeb),
                                ServiceConfig = serviceConfig,
                                LoggerFactory = CreateLoggerFactory (context)
                            });

                        return new Auth.Services.Auth.AuthClient (
                            channel.Intercept (context.Container.Instantiate<RpcExceptionHandlingInterceptor> ()));
                    })
                .AsSingle ();

            Container
                .Bind<Game.GameClient> ()
                .FromMethod (
                    context =>
                    {
                        var channel = GrpcChannel.ForAddress (
                            context
                                .Container
                                .Resolve<AppProfile> ()
                                .APIServiceUrl,
                            new GrpcChannelOptions
                            {
                                MaxRetryAttempts = int.MaxValue,
                                HttpHandler = context.Container.Instantiate<BearerAuthorizationHttpClientHandler> (
                                    new[] { UnityGrpcWebHandler.Create (GrpcWebMode.GrpcWeb) }),
                                ServiceConfig = serviceConfig,
                                LoggerFactory = CreateLoggerFactory (context)
                            });

                        return new Game.GameClient (
                            channel.Intercept (context.Container.Instantiate<RpcExceptionHandlingInterceptor> ()));
                    })
                .AsSingle ();
        }

        private void RegisterRepositories ()
        {
            Container.BindAsSingle<PlayerRepository> ();
            Container.BindAsSingle<TokensRepository> ();
            Container.BindAsSingle<GameRepository> ();
            Container.BindAsSingle<GameDataRepository> ();
        }

        private void RegisterDataSources ()
        {
            Container.BindAsSingle<ITokensDataSource, TokensDataSource> ();
            Container.BindAsSingle<MemoryTokensDataSource> ();
            Container.BindAsSingle<PlayerPrefsTokensDataSource> ();
            Container.BindAsSingle<AppProfileTokensDataSource> ();
        }

        private void RegisterDataLayer ()
        {
            RegisterRepositories ();
            RegisterDataSources ();
            RegisterNetworkDataSources ();
        }

        private void RegisterServices ()
        {
            Container.BindAsSingle<AuthService> ();
            Container.Bind<IGameService> ().To<ClientGameService> ().AsSingle ();
        }

        public override void InstallBindings ()
        {
            RegisterDataLayer ();
            RegisterServices ();
        }
    }
}