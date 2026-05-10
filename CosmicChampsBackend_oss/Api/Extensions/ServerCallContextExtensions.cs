using System.Security.Claims;
using Grpc.Core;

namespace CosmicChamps.Api.Extensions;

public static class ServerCallContextExtensions
{
    public static string GetPlayerId (this ServerCallContext context)
    {
        var playerId = context
            .GetHttpContext ()
            .User
            .FindFirst (ClaimTypes.NameIdentifier)
            ?.Value;

        if (string.IsNullOrEmpty (playerId))
            throw new RpcException (new Status (StatusCode.FailedPrecondition, "Unable to get player id"));

        return playerId;
    }
}