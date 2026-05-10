using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmicChamps.Data
{
    public class HMACAuthorizationHttpClientHandler : DelegatingHandler
    {
        private readonly string _key;

        public HMACAuthorizationHttpClientHandler (string key, HttpMessageHandler innerHandler) : base (innerHandler)
        {
            _key = key;
        }

        protected override async Task<HttpResponseMessage> SendAsync (
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder (request.RequestUri)
            {
                Scheme = Uri.UriSchemeHttp,
                Port = -1
            };
            var requestUri = System.Web.HttpUtility.UrlEncode (uriBuilder.Uri.AbsoluteUri.ToLower ());
            var requestHttpMethod = request.Method.Method;
            var requestTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds ();
            var nonce = Guid.NewGuid ().ToString ("N");
            var requestContentBase64String = string.Empty;
            
            // Logger.Log (this, $"--->>>HMACAuthorizationHttpClientHandler requestUri {requestUri} requestHttpMethod {requestHttpMethod} requestTimeStamp {requestTimeStamp} nonce {nonce}");

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsByteArrayAsync ();
                var md5 = MD5.Create ();
                var requestContentHash = md5.ComputeHash (content);
                requestContentBase64String = Convert.ToBase64String (requestContentHash);
            }

            var signatureRawData = $"{requestHttpMethod}{requestUri}{requestTimeStamp}{nonce}{requestContentBase64String}";
            // Logger.Log (this, $"--->>>HMACAuthorizationHttpClientHandler signatureRawData {signatureRawData}");
            var secretKeyByteArray = Encoding.UTF8.GetBytes (_key);
            var signature = Encoding.UTF8.GetBytes (signatureRawData);

            using (var hmac = new HMACSHA256 (secretKeyByteArray))
            {
                var signatureBytes = hmac.ComputeHash (signature);
                var requestSignatureBase64String = Convert.ToBase64String (signatureBytes);
                request.Headers.Authorization = new AuthenticationHeaderValue (
                    "HMAC",
                    $"{requestSignatureBase64String}:{nonce}:{requestTimeStamp}");
                /*Logger.Log (
                    this,
                    $"--->>>HMACAuthorizationHttpClientHandler request.Headers.Authorization {request.Headers.Authorization}");*/
            }

            var response = await base.SendAsync (request, cancellationToken);
            return response;
        }
    }
}