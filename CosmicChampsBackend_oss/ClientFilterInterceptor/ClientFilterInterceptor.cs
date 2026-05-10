using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClientFilterInterceptor;

public class ClientFilterInterceptor : Interceptor
{
    private const string VersionHeader = "Cosmic-Champs-Version";
    private const string BuildHeader = "Cosmic-Champs-Build";
    private const string PlatformHeader = "Cosmic-Champs-Plaform";
    private const string MaintenanceHeader = "Cosmic-Champs-Maintenance";
    private const string XUrlHeader = "Cosmic-Champs-XUrl";
    private const string TelegramUrlHeader = "Cosmic-Champs-TelegramUrl";
    private const string DiscordUrlHeader = "Cosmic-Champs-DiscordUrl";
    private const string UpdateLinkHeader = "Cosmic-Champs-Update-Link";
    private const string SoftUpdateHeader = "Cosmic-Champs-Soft-Update";

    private readonly IOptionsMonitor<ClientFilterConfig> _configOption;
    private readonly ILogger<ClientFilterInterceptor> _logger;

    public ClientFilterInterceptor (
        IOptionsMonitor<ClientFilterConfig> configOption,
        ILogger<ClientFilterInterceptor> logger)
    {
        _configOption = configOption;
        _logger = logger;
    }

    private void CheckForMaintenance (ClientFilterConfig clientFilterConfig)
    {
        if (!clientFilterConfig.Maintenance)
            return;

        var metaData = new Metadata
        {
            { MaintenanceHeader, "true" },
            { XUrlHeader, clientFilterConfig.XUrl },
            { TelegramUrlHeader, clientFilterConfig.TelegramUrl },
            { DiscordUrlHeader, clientFilterConfig.DiscordUrl }
        };
        throw new RpcException (
            new Status (StatusCode.ResourceExhausted, "The app is down for a maintenance, please try again later"),
            metaData);
    }

    private void CheckForUpdate (ServerCallContext context, ClientFilterConfig clientFilterConfig, int build, string platform)
    {
        _logger.LogDebug (
            "CheckForUpdate config.BuildVersions.Platforms: {Plaforms}",
            clientFilterConfig.BuildVersions.Platforms == null
                ? "null"
                : string.Join (", ", clientFilterConfig.BuildVersions.Platforms.Keys));

        if (clientFilterConfig.BuildVersions.Platforms == null ||
            !clientFilterConfig.BuildVersions.Platforms.TryGetValue (platform, out var buildVersion))
            buildVersion = clientFilterConfig.BuildVersions.Default;

        _logger.LogDebug (
            "CheckForUpdate buildVersion min: {Min}; buildVersion current {Current}",
            buildVersion.Min,
            buildVersion.Current);

        if (build < buildVersion.Min)
            throw new RpcException (
                new Status (
                    StatusCode.ResourceExhausted,
                    "Your app is outdated. Please install an update from beta.cosmicchamps.com"),
                new Metadata { { UpdateLinkHeader, "https://beta.cosmicchamps.com" } });

        if (build >= buildVersion.Current)
            return;

        context.ResponseTrailers.Add (SoftUpdateHeader, "There is a new version of the app available at beta.cosmicchamps.com");
        context.ResponseTrailers.Add (UpdateLinkHeader, "https://beta.cosmicchamps.com");
    }

    private void PerformChecks (ServerCallContext context)
    {
        var version = context.RequestHeaders.GetValue (VersionHeader) ?? string.Empty;
        var buildStr = context.RequestHeaders.GetValue (BuildHeader) ?? string.Empty;
        var platform = context.RequestHeaders.GetValue (PlatformHeader) ?? string.Empty;

        _logger.LogDebug (
            "PerformChecks version: {Version}; buildStr {BuildStr}; platform {Platform}",
            version,
            buildStr,
            platform);

        var config = _configOption.CurrentValue;

        CheckForMaintenance (config);
        CheckForUpdate (context, config, int.TryParse (buildStr, out var build) ? build : 0, platform);
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse> (
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        PerformChecks (context);
        return base.UnaryServerHandler (request, context, continuation);
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse> (
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        PerformChecks (context);
        return base.ServerStreamingServerHandler (request, responseStream, context, continuation);
    }
}