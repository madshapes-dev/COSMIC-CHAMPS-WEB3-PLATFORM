using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CosmicChamps.Data
{
    public class BearerAuthorizationHttpClientHandler : DelegatingHandler
    {
        private readonly TokensRepository _tokensRepository;

        public BearerAuthorizationHttpClientHandler (HttpMessageHandler innerHandler, TokensRepository tokensRepository)
            : base (innerHandler)
        {
            _tokensRepository = tokensRepository;
        }

        protected override async Task<HttpResponseMessage> SendAsync (
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var tokens = await _tokensRepository.Get ();
            request.Headers.Authorization = new AuthenticationHeaderValue ("Bearer", tokens.IdToken);

            var response = await base.SendAsync (request, cancellationToken);
            return response;
        }
    }
}