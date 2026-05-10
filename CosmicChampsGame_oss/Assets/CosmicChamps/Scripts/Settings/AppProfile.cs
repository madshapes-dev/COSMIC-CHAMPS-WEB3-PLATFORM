using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CosmicChamps.Settings
{
    [CreateAssetMenu (menuName = "Cosmic Champs/App Profile")]
    public class AppProfile : ScriptableObject
    {
        [Serializable]
        private struct AdMobUnit
        {
            [SerializeField]
            private string _androidId;

            [SerializeField]
            private string _iosId;

            public string AndroidId => _androidId;

            public string iOSId => _iosId;
        }

        [Serializable]
        public struct ImmutableConfig
        {
            [SerializeField]
            private string _clientId;

            [SerializeField]
            private string _webClientId;

            [SerializeField]
            private string _environment;

            [SerializeField]
            private string _deeplinkUri;

            [SerializeField]
            private string _redirectUriPath;

            [SerializeField]
            private string _logoutRedirectUriPath;

            public string WebClientId => _webClientId;

            public string ClientId => _clientId;

            public string Environment => _environment;
            
            public string DeeplinkUri => _deeplinkUri;

            public string RedirectUriPath => _redirectUriPath;

            public string LogoutRedirectUriPath => _logoutRedirectUriPath;
        }

        [Header ("Common"), FormerlySerializedAs ("authServiceUrl")]
        [SerializeField]
        private string _authServiceUrl;

        [FormerlySerializedAs ("apiServiceUrl")]
        [SerializeField]
        private string _apiServiceUrl;

        [SerializeField]
        private string _apiServiceSecretKey;

        [SerializeField]
        private string _bundlesLoadUrl;

        [SerializeField]
        private string _customToken;

        [SerializeField]
        private string _connectWalletUrl;

        [FormerlySerializedAs ("_adMob")]
        [SerializeField]
        private AdMobUnit _adMobUnit;

        [SerializeField]
        private ImmutableConfig _immutable;

        public string AuthServiceUrl => _authServiceUrl;

        public string APIServiceUrl => _apiServiceUrl;

        public string APIServiceSecretKey => _apiServiceSecretKey;

        public string BundlesLoadUrl => _bundlesLoadUrl;

        public string CustomToken => _customToken;

        public string ConnectWalletUrl => _connectWalletUrl;

        public string Name => name.Replace (nameof (AppProfile), string.Empty);

        public string AdMobUnitId
        {
            get
            {
                #if UNITY_IOS
                return _adMobUnit.iOSId;
                #elif UNITY_ANDROID
                return _adMobUnit.AndroidId;
                #else
                return "unsupported";
                #endif
            }
        }

        public ImmutableConfig Immutable => _immutable;
    }
}