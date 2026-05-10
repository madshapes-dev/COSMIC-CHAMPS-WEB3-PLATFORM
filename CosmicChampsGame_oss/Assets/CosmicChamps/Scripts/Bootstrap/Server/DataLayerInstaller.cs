using System.Net.Http;
using CosmicChamps.Api.Services;
using CosmicChamps.Data;
using CosmicChamps.Data.Sources.Tokens;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using ThirdParty.Extensions;
using Zenject;

namespace CosmicChamps.Bootstrap.Server
{
    public class DataLayerInstaller : Installer<DataLayerInstaller>
    {
        public override void InstallBindings ()
        {
            Container.BindAsSingle<ITokensDataSource, DummyTokensDataSource> ();
            Container.BindAsSingle<GameDataRepository> ();
            Container.BindAsSingle<GameRepository> ();
            Container.Bind<IGameService> ().To<ServerGameService> ().AsSingle ();
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
                                HttpClient = new HttpClient (
                                    new GrpcWebHandler (
                                        GrpcWebMode.GrpcWeb,
                                        context.Container.Instantiate<HMACAuthorizationHttpClientHandler> (
                                            new object[]
                                            {
                                                context.Container.Resolve<AppProfile> ().APIServiceSecretKey,
                                                new HttpClientHandler ()
                                            }))),
                                // LoggerFactory = GrpcLoggerFactory.Instance
                            });

                        return new Game.GameClient (
                            channel.Intercept (Container.Instantiate<RpcExceptionHandlingInterceptor> ()));
                    })
                .AsSingle ();
        }
    }
}