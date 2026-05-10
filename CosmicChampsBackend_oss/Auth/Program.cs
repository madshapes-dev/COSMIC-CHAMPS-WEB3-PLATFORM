using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using CosmicChamps.Auth;
using CosmicChamps.Auth.Configs;
using CosmicChamps.Auth.Services;
using CosmicChamps.Common.Configs;
using CosmicChamps.Common.Model;
using CosmicChamps.Common.Model.DynamoDB;
using CosmicChamps.Common.Services;

var builder = WebApplication.CreateBuilder (args);
var macOS = Environment.GetEnvironmentVariable ("MACOS");
if (!string.IsNullOrEmpty (macOS))
    builder
        .Host
        .ConfigureAppConfiguration ((_, configurationBuilder) => configurationBuilder.AddJsonFile ("appsettings.macOS.json"));

var awsConfig = builder
    .Configuration
    .GetSection ("AWS")
    .Get<AWSConfig> ();

var awsCredentials = new BasicAWSCredentials (awsConfig.Key, awsConfig.Secret);

var appConfigConfig = builder
    .Configuration
    .GetSection ("AppConfig")
    .Get<AppConfigConfig> ();

var dynamoDbConfig = builder
    .Configuration
    .GetSection ("DynamoDB")
    .Get<DynamoDBConfig> ();

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
services.AddGrpc (options => { options.Interceptors.Add<ClientFilterInterceptor.ClientFilterInterceptor> (); });
services.AddSingleton<IAmazonCognitoIdentityProvider> (
    new AmazonCognitoIdentityProviderClient (
        new BasicAWSCredentials (awsConfig.Key, awsConfig.Secret),
        RegionEndpoint.GetBySystemName (awsConfig.Cognito.Region)));
services.AddSingleton (awsConfig.Cognito);

services.Configure<ClientFilterInterceptor.ClientFilterConfig> (builder.Configuration.GetSection ("ClientFilter"));
services.Configure<WalletBridgesConfig> (builder.Configuration.GetSection ("WalletBridges"));
services.Configure<GuestAccountsConfig> (builder.Configuration.GetSection ("GuestAccounts"));

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

services.AddSingleton (typeof (IGuestCredentialsRepository), typeof (GuestCredentialsRepository));
services.AddSingleton (typeof (IImmutableCredentialsRepository), typeof (ImmutableCredentialsRepository));

services.AddSingleton<ImmutableService> ();

services.AddHttpClient ();

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
            .UseGrpcWeb ()
            .UseCors (CorsPolicies.Default)
            .UseEndpoints (endpoints => { endpoints.MapGrpcService<AWSAuthService> ().EnableGrpcWeb (); });
    });
app.MapGet ("/", () => "Invalid client");
app.MapGet ("/Auth/Healthcheck", () => "ok");
app.UseCors (CorsPolicies.Default);

app.Run ();