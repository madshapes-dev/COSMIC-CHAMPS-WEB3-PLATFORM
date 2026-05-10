using System.Net.Http;
using CosmicChamps.Api.Services;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.Settings;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using ThirdParty.Extensions;
using Zenject;

namespace CosmicChamps.Bootstrap.Simulator
{
    public class DataLayerInstaller : Installer<DataLayerInstaller>
    {
        public override void InstallBindings ()
        {
            Container.BindAsSingle<GameDataRepository> ();
            Container.BindAsSingle<GameRepository> ();
            Container.BindAsSingle<TokensRepository> ();
            Container.Bind<IGameService> ().To<ClientGameService> ().AsSingle ();

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
                                HttpClient = new HttpClient (new GrpcWebHandler (GrpcWebMode.GrpcWeb))
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
                                HttpClient = new HttpClient (
                                    new GrpcWebHandler (
                                        GrpcWebMode.GrpcWeb,
                                        context.Container.Instantiate<BearerAuthorizationHttpClientHandler> (
                                            new[] { new HttpClientHandler () }))),
                            });

                        return new Game.GameClient (
                            channel.Intercept (Container.Instantiate<RpcExceptionHandlingInterceptor> ()));
                    })
                .AsSingle ();
        }
    }
}