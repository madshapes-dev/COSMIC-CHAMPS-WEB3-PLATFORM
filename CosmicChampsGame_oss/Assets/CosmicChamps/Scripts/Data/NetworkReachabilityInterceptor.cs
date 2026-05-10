using Grpc.Core.Interceptors;
using UniRx;

namespace CosmicChamps.Data
{
    public class NetworkReachabilityInterceptor : Interceptor
    {
        private readonly IMessageBroker _messageBroker;

        public NetworkReachabilityInterceptor (IMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
        }
    }
}