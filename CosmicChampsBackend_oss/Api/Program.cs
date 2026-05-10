using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.GameLift;
using Amazon.Runtime;
using ClientFilterInterceptor;
using CosmicChamps.Api;
using CosmicChamps.Api.Configs;
using CosmicChamps.Api.HMACAuthentication;
using CosmicChamps.Api.Model;
using CosmicChamps.Api.Model.DynamoDB;
using CosmicChamps.Api.Services;
using CosmicChamps.Api.Services.Matchmaking;
using CosmicChamps.Common.Configs;
using CosmicChamps.Common.Model;
using CosmicChamps.Common.Model.DynamoDB;
using CosmicChamps.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using JwtBearerDefaults = CosmicChamps.Api.JwtBearerAuthentication.JwtBearerDefaults;
using WalletRequest = CosmicChamps.Api.Configs.WalletRequest;


var builder = WebApplication.CreateBuilder (args);

var macOS = Environment.GetEnvironmentVariable ("MACOS");
if (!string.IsNullOrEmpty (macOS))
    builder
        .Host
        .ConfigureAppConfiguration ((_, configurationBuilder) => configurationBuilder.AddJsonFile ("appsettings.macOS.json"));

var jwtConfig = builder
    .Configuration
    .GetSection ("JWT")
    .Get<JWTConfig> ();

var walletRequestConfig = builder
    .Configuration
    .GetSection ("WalletRequest")
    .Get<WalletRequest> ();

var hmacConfig = builder
    .Configuration
    .GetSection ("HMAC")
    .Get<HMAConfig> ();

var awsConfig = builder
    .Configuration
    .GetSection ("AWS")
    .Get<AWSConfig> ();

var dynamoDbConfig = builder
    .Configuration
    .GetSection ("DynamoDB")
    .Get<DynamoDBConfig> ();

var appConfigConfig = builder
    .Configuration
    .GetSection ("AppConfig")
    .Get<AppConfigConfig> ();

var gameLiftConfig = builder
    .Configuration
    .GetSection ("GameLift")
    .Get<GameLiftConfig> ();

var awsCredentials = new BasicAWSCredentials (awsConfig.Key, awsConfig.Secret);

var localAppConfig = Environment.GetEnvironmentVariable ("LOCAL_APPCONFIG");
if (!string.IsNullOrEmpty (localAppConfig))
    builder
        .Host
        .ConfigureAppConfiguration (
            (_, configurationBuilder) => configurationBuilder.AddJsonFile ("appconfig.json", false, true));
else
    builder
        .Host
        .ConfigureAppConfiguration (
            (_, configurationBuilder) => configurationBuilder.AddAppConfig (
                appConfigConfig.ApplicationId,
                appConfigConfig.EnvironmentId,
                appConfigConfig.ProfileId,
                new AWSOptions { Credentials = awsCredentials },
                TimeSpan.FromSeconds (60)));

var services = builder.Services;
var localGamelift = Environment.GetEnvironmentVariable ("LOCAL_GAMELIFT");
var useLocalGamelift = !string.IsNullOrEmpty (localGamelift);
if (useLocalGamelift)
{
    services.AddSingleton (typeof (IMatchmakingService), typeof (LocalMatchmakingService));
    services.AddSingleton (
        new AmazonGameLiftClient (
            awsCredentials,
            new AmazonGameLiftConfig
            {
                UseHttp = true,
                ServiceURL = localGamelift
            }));
} else
{
    services.AddSingleton (typeof (IMatchmakingService), typeof (FlexmatchMatchmakingService));
    services.AddSingleton (
        new AmazonGameLiftClient (
            awsCredentials,
            RegionEndpoint.GetBySystemName (gameLiftConfig.Region)));
}

services.AddSingleton (gameLiftConfig);

var localDynamoDb = Environment.GetEnvironmentVariable ("LOCAL_DYNAMODB");
var useLocalDynamoDb = !string.IsNullOrEmpty (localDynamoDb);
var amazonDynamoDBClient = useLocalDynamoDb
    ? new AmazonDynamoDBClient (new AmazonDynamoDBConfig { ServiceURL = localDynamoDb })
    : new AmazonDynamoDBClient (
        awsCredentials,
        RegionEndpoint.GetBySystemName (dynamoDbConfig.Region));
services.AddSingleton (amazonDynamoDBClient);
services.AddSingleton (new DynamoDBContext (amazonDynamoDBClient));
services.AddSingleton (dynamoDbConfig);

services.AddSingleton (typeof (IGameSessionRepository), typeof (GameSessionRepository));
services.AddSingleton (typeof (IErrorReportRepository), typeof (ErrorReportRepository));
services.AddSingleton (typeof (IPlayerRepository), typeof (PlayerRepository));
services.AddSingleton (typeof (INewsRepository), typeof (NewsRepository));
services.AddSingleton (typeof (ITournamentRepository), typeof (TournamentRepository));
services.AddSingleton (typeof (IStatisticsRepository), typeof (StatisticsRepository));
services.AddSingleton (typeof (IMatchReportRepository), typeof (MatchReportRepository));
services.AddSingleton (typeof (IGuestCredentialsRepository), typeof (GuestCredentialsRepository));
services.AddSingleton (typeof (IImmutableCredentialsRepository), typeof (ImmutableCredentialsRepository));

services.AddSingleton<IAmazonCognitoIdentityProvider> (
    new AmazonCognitoIdentityProviderClient (
        new BasicAWSCredentials (awsConfig.Key, awsConfig.Secret),
        RegionEndpoint.GetBySystemName (awsConfig.Cognito.Region)));
services.AddSingleton (awsConfig.Cognito);

services.AddHttpClient ();
services.AddGrpc (options => { options.Interceptors.Add<ClientFilterInterceptor.ClientFilterInterceptor> (); });
services
    .AddAuthentication ()
    .AddJwtBearer (
        JwtBearerDefaults.CustomAuthenticationScheme,
        options =>
        {
            var customJwtConfig = builder
                .Configuration
                .GetSection ("CustomJWT")
                .Get<CustomJWTConfig> ();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = customJwtConfig.Audience,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = customJwtConfig.Issuer,
                IssuerSigningKeys = new[]
                {
                    new SymmetricSecurityKey (Encoding.UTF8.GetBytes (customJwtConfig.Secret))
                }
            };
        })
    .AddJwtBearer (
        JwtBearerDefaults.AWSAuthenticationScheme,
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                AudienceValidator = (_, _, _) => true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = jwtConfig.Audience,
                ValidIssuer = jwtConfig.Issuer,
                IssuerSigningKeys = new JsonWebKeySet (jwtConfig.Keys).GetSigningKeys (),
            };
        })
    .AddHMACAuthentication (options => options.Key = hmacConfig.Key);

services.AddAuthorization (
    options =>
    {
        var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder (
            JwtBearerDefaults.CustomAuthenticationScheme,
            JwtBearerDefaults.AWSAuthenticationScheme,
            HMACBearerDefaults.AuthenticationScheme);
        defaultAuthorizationPolicyBuilder =
            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser ();
        options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build ();
    });

services.Configure<RemoteStatsServiceConfig> (builder.Configuration.GetSection ("StatsService"));
services.AddSingleton<RemoteStatisticsService> ();
services.AddSingleton<StatisticsService> ();

services.Configure<ValidationServiceConfig> (builder.Configuration.GetSection ("ValidationService"));
services.AddSingleton (walletRequestConfig);
services.AddSingleton<ValidationService> ();
services.AddSingleton<ImmutableService> ();

services.Configure<GameData> (builder.Configuration.GetSection ("GameData"));
services.Configure<List<HUDSkin>> (builder.Configuration.GetSection ("HUDSkins"));
services.Configure<GuestAccountsConfig> (builder.Configuration.GetSection ("GuestAccounts"));
services.Configure<ClientFilterConfig> (builder.Configuration.GetSection ("ClientFilter"));
services.Configure<LocalMatchmakingConfig> (builder.Configuration.GetSection ("LocalMatchmaking"));
services.Configure<List<string>> (builder.Configuration.GetSection ("RestrictedNicknames"));

services.AddSingleton<InitService> ();

services.AddCors (
    options =>
    {
        options.AddPolicy (
            CorsPolicies.Default,
            configurePolicy => configurePolicy
                .AllowAnyOrigin ()
                .AllowAnyMethod ()
                .AllowAnyHeader ()
                .WithExposedHeaders ("Grpc-Status", "Grpc-Message"));
    });

var app = builder.Build ();
app.MapWhen (
    context => context.Request.Method == HttpMethod.Post.Method,
    appBuilder =>
    {
        appBuilder
            .UseRouting ()
            .UseAuthentication ()
            .UseAuthorization ()
            .UseGrpcWeb ()
            .UseCors (CorsPolicies.Default)
            .UseEndpoints (endpoints => { endpoints.MapGrpcService<GameService> ().EnableGrpcWeb (); });
    });
app.MapGet ("/", () => "Invalid client");
app.MapGet ("/Api/Healthcheck", () => "ok");
app.UseCors (CorsPolicies.Default);

await app.Services.GetRequiredService<InitService> ().Run ();

app.Run ();