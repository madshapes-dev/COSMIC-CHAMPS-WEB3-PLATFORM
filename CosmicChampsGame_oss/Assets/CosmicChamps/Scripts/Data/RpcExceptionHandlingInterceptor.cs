using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmicChamps.Bootstrap.Client;
using CosmicChamps.Data.Sources.Tokens;
using CosmicChamps.Signals;
using Grpc.Core;
using Grpc.Core.Interceptors;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using ILogger = Serilog.ILogger;


namespace CosmicChamps.Data
{
    public class RpcExceptionHandlingInterceptor : Interceptor
    {
        private class RpcExceptionHandlingResponseStream<TResponse> : IAsyncStreamReader<TResponse>
        {
            private readonly AsyncServerStreamingCall<TResponse> _call;
            private readonly Action<RpcException> _exceptionHandler;

            public RpcExceptionHandlingResponseStream (
                AsyncServerStreamingCall<TResponse> call,
                Action<RpcException> exceptionHandler)
            {
                _call = call;
                _exceptionHandler = exceptionHandler;
            }

            public TResponse Current => _call.ResponseStream.Current;

            public async Task<bool> MoveNext (CancellationToken cancellationToken)
            {
                try
                {
                    return await _call.ResponseStream.MoveNext (cancellationToken);
                } catch (RpcException rpcException)
                {
                    _exceptionHandler (rpcException);
                    if (Application.platform.IsServer ())
                        throw;

                    throw new SilentException ();
                }
            }
        }

        private const string VersionHeader = "Cosmic-Champs-Version";
        private const string BuildHeader = "Cosmic-Champs-Build";
        private const string PlatformHeader = "Cosmic-Champs-Plaform";
        private const string MaintenanceHeader = "Cosmic-Champs-Maintenance";
        private const string UpdateLinkHeader = "Cosmic-Champs-Update-Link";
        private const string SoftUpdateHeader = "Cosmic-Champs-Soft-Update";
        private const string XUrlHeader = "Cosmic-Champs-XUrl";
        private const string TelegramUrlHeader = "Cosmic-Champs-TelegramUrl";
        private const string DiscordUrlHeader = "Cosmic-Champs-DiscordUrl";

        private readonly IMessageBroker _messageBroker;
        private readonly ITokensDataSource _tokensDataSource;
        private readonly ILogger _logger;

        public RpcExceptionHandlingInterceptor (
            IMessageBroker messageBroker,
            ILogger logger,
            ITokensDataSource tokensDataSource)
        {
            _messageBroker = messageBroker;
            _logger = logger;
            _tokensDataSource = tokensDataSource;
        }

        private async Task<TResponse> HandleResponse<TResponse> (AsyncUnaryCall<TResponse> call)
        {
            try
            {
                var response = await call.ResponseAsync;
                HandleResponseTrailers (call.GetTrailers ());

                return response;
            } catch (RpcException rpcException)
            {
                HandleRpcException (rpcException);
                if (Application.platform.IsServer ())
                    throw;

                throw new SilentException ();
            }
        }

        private bool GetHeader (Metadata metadata, string key, out string value)
        {
            var entry = metadata.FirstOrDefault (x => x.Key.Equals (key, StringComparison.InvariantCultureIgnoreCase));
            value = entry?.Value;
            return value != null;
        }

        private void HandleResponseTrailers (Metadata trailers)
        {
            if (GetHeader (trailers, SoftUpdateHeader, out var softUpdateHeader) &&
                GetHeader (trailers, UpdateLinkHeader, out var updateLinkHeader))
            {
                _messageBroker.Publish (new UpdateRequiredSignal (softUpdateHeader, updateLinkHeader, true));
            }
        }

        private void AddCallerMetadata<TRequest, TResponse> (ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var headers = context.Options.Headers;
            if (headers == null)
            {
                headers = new Metadata ();
                var options = context.Options.WithHeaders (headers);
                context = new ClientInterceptorContext<TRequest, TResponse> (context.Method, context.Host, options);
            }

            headers.Add (VersionHeader, BuildInfo.Version);
            headers.Add (BuildHeader, BuildInfo.BuildVersion.ToString ());
            headers.Add (PlatformHeader, Application.platform.ToString ());
        }

        private void HandleRpcException (RpcException rpcException)
        {
            _logger.Error (rpcException, "HandleRpcException {StatusStatusCode}", rpcException.Status.StatusCode);
            var trailers = rpcException.Trailers;

            switch (rpcException.Status.StatusCode)
            {
                case StatusCode.ResourceExhausted when GetHeader (trailers, MaintenanceHeader, out _):
                    /*_messageBroker.Publish (
                        new ErrorSignal (rpcException.Status.Detail, rpcException.StackTrace, true, false, false));*/

                    GetHeader (trailers, XUrlHeader, out var xUrl);
                    GetHeader (trailers, TelegramUrlHeader, out var telegramUrl);
                    GetHeader (trailers, DiscordUrlHeader, out var discordUrl);
                    _messageBroker.Publish (new MaintenanceSignal (xUrl, telegramUrl, discordUrl));
                    break;

                case StatusCode.ResourceExhausted when GetHeader (trailers, UpdateLinkHeader, out var updateHeader):
                    _messageBroker.Publish (
                        new UpdateRequiredSignal (rpcException.Status.Detail, updateHeader, false));
                    break;
                case StatusCode.InvalidArgument:
                case StatusCode.PermissionDenied:
                    _messageBroker.Publish (
                        new ErrorSignal (rpcException.Status.Detail, rpcException.StackTrace, false, false, false));
                    break;

                case StatusCode.Unauthenticated:
                    _tokensDataSource.Clear ();
                    _messageBroker.Publish (new RestartSignal ());
                    break;

                default:
                    _messageBroker.Publish (
                        new ErrorSignal (rpcException.Status.Detail, rpcException.StackTrace, true, false));
                    break;
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse> (
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata (ref context);

            var call = continuation (request, context);
            var result = new AsyncUnaryCall<TResponse> (
                HandleResponse (call),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose);

            return result;
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse> (
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata (ref context);

            var call = continuation (request, context);
            return new AsyncServerStreamingCall<TResponse> (
                new RpcExceptionHandlingResponseStream<TResponse> (call, HandleRpcException),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose);
        }
    }
}