using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CosmicChamps.Api.HMACAuthentication
{
    public sealed class HMACAuthenticationHandler : AuthenticationHandler<HMACAuthenticationOptions>
    {
        private const string AuthorizationHeader = "Authorization";
        private const string Identity = "SimulationService";

        public HMACAuthenticationHandler (
            IOptionsMonitor<HMACAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base (options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync ()
        {
            var header = Request.Headers[AuthorizationHeader].SingleOrDefault ();
            if (string.IsNullOrEmpty (header))
                return AuthenticateResult.NoResult ();

            var headerChunks = header
                .Replace (HMACBearerDefaults.AuthenticationScheme, string.Empty)
                .Split (':', ' ');

            if (headerChunks.Length != 4)
                return AuthenticateResult.NoResult ();

            var requestSignature = headerChunks[1];
            var nonce = headerChunks[2];
            var requestTimeStamp = headerChunks[3];
            var uriBuilder = new UriBuilder
            {
                Host = Request.Host.Host,
                Port = -1,
                Path = Request.Path.Value ?? string.Empty,
                Query = Request.QueryString.Value,
                Scheme = Uri.UriSchemeHttp,
            };

            var requestUri = HttpUtility.UrlEncode (uriBuilder.Uri.AbsoluteUri.ToLower ());
            var requestHttpMethod = Request.Method;

            /*Logger.LogDebug (
                "--->>>HandleAuthenticateAsync requestUri {RequestUri} requestHttpMethod {RequestHttpMethod} requestTimeStamp {RequestTimeStamp} nonce {Nonce}",
                requestUri,
                requestHttpMethod,
                requestTimeStamp,
                nonce);*/

            var requestContentBase64String = string.Empty;
            var contentLength = (int)Request.ContentLength;
            if (contentLength > 0)
            {
                Request.EnableBuffering ();

                // using var reader = new StreamReader (Request.Body, Encoding.UTF8, false, 1024, true);
                // using var reader = new BinaryReader (Request.Body);
                var bodyBytes = new byte[contentLength];
                var offset = 0;
                while (offset < contentLength)
                {
                    offset = await Request.Body.ReadAsync (bodyBytes, offset, contentLength);
                }
                /*var contentBytes = new byte[(int)Request.ContentLength];
                await Request.Body.ReadAsync (contentBytes, 0, (int)Request.ContentLength);*/

                var md5 = MD5.Create ();
                var requestContentHash = md5.ComputeHash (bodyBytes);
                requestContentBase64String = Convert.ToBase64String (requestContentHash);

                Request.Body.Position = 0;
            }

            var signatureRawData = $"{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}";
            // Logger.LogDebug ("--->>>signatureRawData {SignatureRawData}", signatureRawData);
            var secretKeyByteArray = Encoding.UTF8.GetBytes (Options.Key);
            var signature = Encoding.UTF8.GetBytes (signatureRawData);

            using var hmac = new HMACSHA256 (secretKeyByteArray);
            var signatureBytes = hmac.ComputeHash (signature);
            var signatureBase64String = Convert.ToBase64String (signatureBytes);
            // Logger.LogDebug ("--->>> signatureBase64String {SignatureBase64String}", signatureBase64String);

            if (!requestSignature.Equals (signatureBase64String))
                return AuthenticateResult.Fail ("Invalid signature");

            return AuthenticateResult.Success (
                new AuthenticationTicket (
                    new GenericPrincipal (new GenericIdentity (Identity), null),
                    new AuthenticationProperties { IsPersistent = false, AllowRefresh = false },
                    HMACBearerDefaults.AuthenticationScheme));
        }
    }
}