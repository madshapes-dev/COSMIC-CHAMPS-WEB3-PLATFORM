using System;
using CosmicChamps.Signals;
using ThirdParty.Extensions;
using UniRx;
using Zenject;

namespace CosmicChamps.Utils
{
    public class NetworkReachableReporterProgress : IProgress<float>
    {
        public class Factory : PlaceholderFactory<NetworkReachableReporterProgress>
        {
        }

        private readonly IMessageBroker _messageBroker;
        private float? _lastValue;

        public NetworkReachableReporterProgress (IMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public void Report (float value)
        {
            if (!_lastValue.HasValue)
            {
                _lastValue = value;
                return;
            }

            if (_lastValue.Value.CompareWithEps (value) == 0)
                return;

            _lastValue = value;
            _messageBroker.Publish (NetworkReachabilitySignal.ReachableSignal);
        }
    }
}