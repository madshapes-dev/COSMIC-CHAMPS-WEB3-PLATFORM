using System.Security.Claims;

namespace CosmicChamps.Api.Services;

public static class ClaimsPrincipalExtensions
{
    private const string NicknameClaimIdentifier = "nickname";
    
    public static string GetPlayerId (this ClaimsPrincipal player) => player
        .Claims
        .First (x => x.Type == ClaimTypes.NameIdentifier)
        .Value;
}