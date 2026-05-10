using CosmicChamps.Signals;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class NetworkUnreachablePresenter : MonoBehaviour
    {
        [Inject]
        private IMessageBroker _messageBroker;

        private void Awake ()
        {
            _messageBroker
                .Receive<NetworkReachabilitySignal> ()
                .Subscribe (OnNetworkReachabilitySignal)
                .AddTo (this);
        }

        private void OnNetworkReachabilitySignal (NetworkReachabilitySignal signal)
        {
            switch (this.IsVisible ())
            {
                case true when signal.Reachable:
                    this.SetVisible (false);
                    break;
                case false when !signal.Reachable:
                    this.SetVisible (true);
                    break;
            }
        }
    }
}