using CosmicChamps.Api.HMACAuthentication;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddHMACAuthentication (this AuthenticationBuilder builder)
        {
            return builder.AddHMACAuthentication ((_) => { });
        }

        public static AuthenticationBuilder AddHMACAuthentication (
            this AuthenticationBuilder builder,
            Action<HMACAuthenticationOptions> options)
        {
            return builder.AddScheme<HMACAuthenticationOptions, HMACAuthenticationHandler> (
                HMACBearerDefaults.AuthenticationScheme,
                options);
        }
    }
}