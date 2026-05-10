using Microsoft.AspNetCore.Authentication;

namespace CosmicChamps.Api.HMACAuthentication
{
    public class HMACAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Key { set; get; }
    }
}