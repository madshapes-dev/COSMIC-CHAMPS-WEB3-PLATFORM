#if !UNITY_SERVER
using Cysharp.Threading.Tasks;
using System;
using CosmicChamps.Settings;
using Immutable.Passport;
using Immutable.Passport.Model;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Services
{
    public class ImmutableService : IImmutableService
    {
        private readonly AppProfile _appProfile;
        private readonly ILogger _logger;
        private Passport _passport;

        public ImmutableService (AppProfile appProfile, ILogger logger)
        {
            _appProfile = appProfile;
            _logger = logger;
        }

        public async UniTask Initialize ()
        {
            _logger.Information ("Initialize...");

            if (_passport != null)
            {
                _logger.Information ("Initialize already initialized");
                return;
            }

            var config = _appProfile.Immutable;

            #if UNITY_WEBGL && !UNITY_EDITOR
            var clientId = config.WebClientId;
            var url = UnityEngine.Application.absoluteURL;
            var uri = new Uri(url);
            var scheme = uri.Scheme;
            var hostWithPort = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
            var fullPath = System.IO.Path.GetDirectoryName (uri.AbsolutePath);
            var redirectUri = $"{scheme}://{hostWithPort}{fullPath}/{config.RedirectUriPath}";
            var logoutRedirectUri = $"{scheme}://{hostWithPort}{fullPath}/{config.LogoutRedirectUriPath}";
            #else
            var clientId = config.ClientId;
            var redirectUri = $"{config.DeeplinkUri}/{config.RedirectUriPath}";
            var logoutRedirectUri = $"{config.DeeplinkUri}/{config.LogoutRedirectUriPath}";
            #endif

            _logger.Information ("Initialize Passport.Init...");
            _passport = await Passport.Init (
                clientId,
                config.Environment,
                redirectUri,
                logoutRedirectUri);
            _logger.Information ("Initialize Passport.Init Done");
        }

        public async UniTask Login ()
        {
            try
            {
                await Initialize ();
                /*var hasCredentialsSaved = await _passport.HasCredentialsSaved ();
                _logger.Information ("Login hasCredentialsSaved: {HasCredentialsSaved}", hasCredentialsSaved);
                if (hasCredentialsSaved)
                {
                    _logger.Information ("Login Using cached session...");
                    await _passport.Login (useCachedSession: true);
                    _logger.Information ("Login Using cached session Done");
                    return;
                }*/

                #if (false && UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX || UNITY_WEBGL
                _logger.Information ("Login LoginPKCE...");
                await _passport.LoginPKCE ();
                #else
                _logger.Information ("Login Login...");
                await _passport.Login ();
                #endif
                _logger.Information ("Login Done");
            } catch (OperationCanceledException)
            {
                _logger.Information ("Login Canceled");
            } catch (Exception)
            {
                await Logout ();
                throw;
            }
        }

        public async UniTask RequestAccounts ()
        {
            await Initialize ();
            await _passport.ConnectEvm ();
            await _passport.ZkEvmRequestAccounts ();
        }

        public async UniTask<string> GetAccessToken ()
        {
            await Initialize ();
            return await _passport.GetAccessToken ();
        }

        public async UniTask Logout ()
        {
            try
            {
                await Initialize ();

                var hasCredentialsSaved = await _passport.HasCredentialsSaved ();
                if (!hasCredentialsSaved)
                    return;

                #if (false && UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX || UNITY_WEBGL
                _logger.Information ("Logout LogoutPKCE...");
                await _passport.LogoutPKCE ();
                #else
                _logger.Information ("Logout Logout...");
                await _passport.Logout ();
                #endif
                _logger.Information ("Logout Done");
            } catch (PassportException)
            {
                _logger.Information ("Logout PassportException");
            } catch (TimeoutException)
            {
                _logger.Information ("Logout TimeoutException");
            }
        }
    }
}
#endif