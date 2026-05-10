using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using CosmicChamps.Auth.Configs;
using CosmicChamps.Common.Configs;
using CosmicChamps.Common.Model;
using CosmicChamps.Common.Services;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace CosmicChamps.Auth.Services;

public class AWSAuthService : Auth.AuthBase
{
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digit = "1234567890";
    private const string Special = "!@#$%^&*_-=+";

    private static readonly Random _random = new();
    private static readonly StringBuilder _passwordBuilder = new();

    private static readonly string[] PasswordMasks =
    {
        "llllLLLsddLlsds",
        "dlllLsLLLdldlss",
        "slddlLLLllLlsds",
        "dsldLLlllLLldss",
        "LdLslldllLdLsls",
        "sdlLLddlLslLlls",
        "lsLdLddllLLslsl",
        "lsdddlLlLLsllLs",
        "dslLsddlllLLLsl",
        "LdsdslldllLlLsL"
    };

    private readonly ILogger<AWSAuthService> _logger;
    private readonly IAmazonCognitoIdentityProvider _identityProvider;
    private readonly AWSCognitoConfig _config;
    private readonly IOptionsMonitor<WalletBridgesConfig> _walletBridgesConfigOption;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IGuestCredentialsRepository _guestCredentialsRepository;
    private readonly IImmutableCredentialsRepository _immutableCredentialsRepository;
    private readonly IOptionsMonitor<GuestAccountsConfig> _guestAccountsConfigOption;
    private readonly ImmutableService _immutableService;

    public AWSAuthService (
        ILogger<AWSAuthService> logger,
        IAmazonCognitoIdentityProvider identityProvider,
        AWSCognitoConfig config,
        IOptionsMonitor<WalletBridgesConfig> walletBridgesConfigOption,
        IHttpClientFactory httpClientFactory,
        IGuestCredentialsRepository guestCredentialsRepository,
        IOptionsMonitor<GuestAccountsConfig> guestAccountsConfigOption,
        IImmutableCredentialsRepository immutableCredentialsRepository,
        ImmutableService immutableService)
    {
        _logger = logger;
        _identityProvider = identityProvider;
        _config = config;
        _walletBridgesConfigOption = walletBridgesConfigOption;
        _httpClientFactory = httpClientFactory;
        _guestCredentialsRepository = guestCredentialsRepository;
        _guestAccountsConfigOption = guestAccountsConfigOption;
        _immutableCredentialsRepository = immutableCredentialsRepository;
        _immutableService = immutableService;
    }

    private long GetAuthenticationResultExpireTime (AuthenticationResultType authenticationResult) =>
        DateTimeOffset.Now.ToUnixTimeSeconds () + authenticationResult.ExpiresIn;

    private string GetRandomGuestPassword ()
    {
        var passwordMask = PasswordMasks[_random.Next (0, PasswordMasks.Length)];

        foreach (var maskSymbol in passwordMask)
        {
            var symbols = maskSymbol switch
            {
                'l' => Lower,
                'L' => Upper,
                's' => Special,
                'd' => Digit,
                _ => throw new InvalidOperationException ($"Invalid password mask {passwordMask}")
            };

            _passwordBuilder.Append (symbols[_random.Next (0, symbols.Length)]);
        }

        var password = _passwordBuilder.ToString ();
        _passwordBuilder.Clear ();
        return password;
    }

    public override async Task<SignInReply> ImmutableSignIn (ImmutableSignInRequest request, ServerCallContext context)
    {
        /*using var httpClient = _httpClientFactory.CreateClient ();
        httpClient.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue ("Bearer", request.ImmutableAccessToken);

        var immutableUserInfo =
            await httpClient.GetFromJsonAsync<ImmutableUserInfoResponse> (
                "https://api.sandbox.immutable.com/passport-profile/v1/user/info");*/
        var immutableUserInfo = await _immutableService.GetUserInfo (request.ImmutableAccessToken);
        if (immutableUserInfo == null)
            throw new RpcException (new Status (StatusCode.InvalidArgument, "Invalid Immutable Passport token"));

        var immutableCredentials = await _immutableCredentialsRepository.GetAsync (immutableUserInfo.sub);
        try
        {
            do
            {
                if (immutableCredentials == null)
                {
                    immutableCredentials = new ImmutableCredentials
                    {
                        ImmutableId = immutableUserInfo.sub,
                        Email = immutableUserInfo.email,
                        Password = GetRandomGuestPassword (),
                        WalletId = immutableUserInfo.passport_address
                    };

                    var signUpResponse = await _identityProvider.SignUpAsync (
                        new Amazon.CognitoIdentityProvider.Model.SignUpRequest
                        {
                            ClientId = _config.ClientId,
                            Username = immutableCredentials.Email,
                            Password = immutableCredentials.Password,
                        });

                    if (signUpResponse.HttpStatusCode != HttpStatusCode.OK)
                        break;

                    var confirmResponse = await _identityProvider.AdminConfirmSignUpAsync (
                        new AdminConfirmSignUpRequest
                        {
                            Username = immutableCredentials.Email,
                            UserPoolId = _config.UserPoolId
                        });

                    if (confirmResponse.HttpStatusCode != HttpStatusCode.OK)
                        break;

                    immutableCredentials.PlayerId = signUpResponse.UserSub;
                    await _immutableCredentialsRepository.CreateAsync (immutableCredentials);
                }

                return await InternalSignIn (immutableCredentials.Email, immutableCredentials.Password);
            } while (false);
        } catch (Exception e)
        {
            _logger.LogError ("Failed to sign in using Immutable Passport: {Exception}", e.Message);
            throw new RpcException (new Status (StatusCode.InvalidArgument, e.Message));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to sign in using Immutable Passport"));
    }

    public override async Task<SignInReply> GuestSignIn (GuestSignInRequest request, ServerCallContext context)
    {
        var deviceId = request.DeviceId;
        var guestCredentials = await _guestCredentialsRepository.GetAsync (deviceId);

        try
        {
            do
            {
                if (guestCredentials == null)
                {
                    guestCredentials = new GuestCredentials
                    {
                        DeviceId = deviceId,
                        Email =
                            $"{Guid.NewGuid ().ToString ().Substring (0, 18)}@{_guestAccountsConfigOption.CurrentValue.EmailDomain}",
                        Password = GetRandomGuestPassword ()
                    };

                    var signUpResponse = await _identityProvider.SignUpAsync (
                        new Amazon.CognitoIdentityProvider.Model.SignUpRequest
                        {
                            ClientId = _config.ClientId,
                            Username = guestCredentials.Email,
                            Password = guestCredentials.Password,
                        });

                    if (signUpResponse.HttpStatusCode != HttpStatusCode.OK)
                        break;

                    var confirmResponse = await _identityProvider.AdminConfirmSignUpAsync (
                        new AdminConfirmSignUpRequest
                        {
                            Username = guestCredentials.Email,
                            UserPoolId = _config.UserPoolId
                        });

                    if (confirmResponse.HttpStatusCode != HttpStatusCode.OK)
                        break;

                    guestCredentials.PlayerId = signUpResponse.UserSub;
                    await _guestCredentialsRepository.CreateAsync (guestCredentials);
                }

                return await InternalSignIn (guestCredentials.Email, guestCredentials.Password);
            } while (false);
        } catch (Exception e)
        {
            _logger.LogError ("Failed to guest sign in: {Exception}", e.Message);
            throw new RpcException (new Status (StatusCode.InvalidArgument, e.Message));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to guest sign in"));
    }

    public override async Task<SignUpReply> SignUp (SignUpRequest request, ServerCallContext context)
    {
        var email = request.Email;
        var password = request.Password;

        if (email.EndsWith ($"@{_guestAccountsConfigOption.CurrentValue.EmailDomain}"))
            throw new RpcException (new Status (StatusCode.InvalidArgument, "Forbidden email domain"));

        void ProcessException (Exception e)
        {
            _logger.LogError ("Failed to sign up: {Exception}", e.Message);
            throw new RpcException (
                new Status (
                    StatusCode.InvalidArgument,
                    e
                        .Message
                        .Replace ("Password did not conform with policy: ", string.Empty)
                        .Replace ("Username should be an email", "Please enter valid email")));
        }

        try
        {
            do
            {
                var signUpResponse = await _identityProvider.SignUpAsync (
                    new Amazon.CognitoIdentityProvider.Model.SignUpRequest
                    {
                        ClientId = _config.ClientId,
                        Username = email,
                        Password = password,
                    });

                if (signUpResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;

                return new SignUpReply ();
            } while (false);
        } catch (UsernameExistsException e)
        {
            do
            {
                var getUserResponse = await _identityProvider.AdminGetUserAsync (
                    new AdminGetUserRequest
                    {
                        Username = email,
                        UserPoolId = _config.UserPoolId
                    });

                if (getUserResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;

                if (getUserResponse.UserStatus != UserStatusType.UNCONFIRMED)
                    ProcessException (e);

                var deleteUserAsync = await _identityProvider.AdminDeleteUserAsync (
                    new AdminDeleteUserRequest
                    {
                        Username = email,
                        UserPoolId = _config.UserPoolId
                    });

                if (deleteUserAsync.HttpStatusCode != HttpStatusCode.OK)
                    break;

                return await SignUp (request, context);
            } while (false);
        } catch (Exception e)
        {
            ProcessException (e);
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to sign up"));
    }

    public override async Task<ConfirmSignUpReply> ConfirmSignUp (ConfirmSignUpRequest request, ServerCallContext context)
    {
        try
        {
            do
            {
                var confirmResponse = await _identityProvider.ConfirmSignUpAsync (
                    new Amazon.CognitoIdentityProvider.Model.ConfirmSignUpRequest
                    {
                        ClientId = _config.ClientId,
                        ConfirmationCode = request.Code,
                        Username = request.Email
                    });

                if (confirmResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;

                return new ConfirmSignUpReply ();
            } while (false);
        } catch (Exception e)
        {
            _logger.LogError ("Failed to confirm sign up: {Exception}", e.Message);
            throw new RpcException (new Status (StatusCode.InvalidArgument, e.Message));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to confirm sign up"));
    }

    private async Task<SignInReply> InternalSignIn (string email, string password)
    {
        try
        {
            var response = await _identityProvider.InitiateAuthAsync (
                new InitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                    ClientId = _config.ClientId,
                    AuthParameters = new Dictionary<string, string>
                    {
                        { "USERNAME", email },
                        { "PASSWORD", password! }
                    }
                });

            if (response.HttpStatusCode == HttpStatusCode.OK && response.AuthenticationResult != null)
                return new SignInReply
                {
                    Tokens = new Tokens
                    {
                        IdToken = response.AuthenticationResult.IdToken,
                        AccessToken = response.AuthenticationResult.AccessToken,
                        RefreshToken = response.AuthenticationResult.RefreshToken,
                        AccessTokenExpireTime = GetAuthenticationResultExpireTime (response.AuthenticationResult)
                    }
                };
        } catch (Exception e)
        {
            _logger.LogError ("Failed to sign in: {Exception}", e.Message);
            throw new RpcException (
                new Status (
                    StatusCode.InvalidArgument,
                    e
                        .Message
                        .Replace ("Incorrect username or password", "Incorrect email or password")));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to sign in"));
    }

    public override async Task<SignInReply> SignIn (SignInRequest request, ServerCallContext context)
    {
        var immutableCredentials = await _immutableCredentialsRepository.GetByEmailAsync (request.Email);
        if (immutableCredentials != null)
            throw new RpcException (
                new Status (StatusCode.InvalidArgument, "Player able to sign in only by Immutable Passport"));

        return await InternalSignIn (request.Email, request.Password);
    }

    public override async Task<SignOutReply> SignOut (SignOutRequest request, ServerCallContext context)
    {
        try
        {
            var response = await _identityProvider.GlobalSignOutAsync (
                new GlobalSignOutRequest
                {
                    AccessToken = request.AccessToken
                });

            if (response.HttpStatusCode == HttpStatusCode.OK)
                return new SignOutReply ();
        } catch (Exception e)
        {
            _logger.LogError ("Failed to sign out: {Exception}", e.Message);
            throw new RpcException (new Status (StatusCode.InvalidArgument, $"Failed to sign out: {e.Message}"));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to sign out"));
    }

    public override async Task<RefreshTokenReply> RefreshToken (RefreshTokenRequest request, ServerCallContext context)
    {
        try
        {
            var response = await _identityProvider.InitiateAuthAsync (
                new InitiateAuthRequest
                {
                    ClientId = _config.ClientId,
                    AuthFlow = AuthFlowType.REFRESH_TOKEN,
                    AuthParameters = new Dictionary<string, string>
                    {
                        { "REFRESH_TOKEN", request.RefreshToken }
                    }
                });

            if (response.HttpStatusCode == HttpStatusCode.OK && response.AuthenticationResult != null)
                return new RefreshTokenReply
                {
                    Tokens = new Tokens
                    {
                        IdToken = response.AuthenticationResult.IdToken,
                        AccessToken = response.AuthenticationResult.AccessToken,
                        AccessTokenExpireTime = GetAuthenticationResultExpireTime (response.AuthenticationResult)
                    }
                };
        } catch (NotAuthorizedException e) when (e.Message == "Refresh Token has expired")
        {
            return new RefreshTokenReply ();
        } catch (UserNotFoundException _)
        {
            return new RefreshTokenReply ();
        } catch (Exception e)
        {
            _logger.LogError ("Failed to refresh token: {Exception}", e.Message);
            throw new RpcException (new Status (StatusCode.Unauthenticated, $"Failed to refresh token: {e.Message}"));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to sign in"));
    }

    public override async Task<VersionCheckReply> VersionCheck (VersionCheckRequest request, ServerCallContext context)
    {
        string walletBridgeUrl;
        try
        {
            using var client = _httpClientFactory.CreateClient ();
            var walletBridgesList =
                await client.GetFromJsonAsync<WalletBridgesList> (_walletBridgesConfigOption.CurrentValue.ListUrl);

            walletBridgeUrl = walletBridgesList is { servers.Length: > 0 }
                ? walletBridgesList.servers[_random.Next (walletBridgesList.servers.Length)]
                : _walletBridgesConfigOption.CurrentValue.FallbackBridge;
        } catch
        {
            walletBridgeUrl = _walletBridgesConfigOption.CurrentValue.FallbackBridge;
        }

        return new VersionCheckReply
        {
            WalletBridgeUrl = walletBridgeUrl
        };
    }

    public override async Task<GetResetPasswordCodeReply> GetResetPasswordCode (
        GetResetPasswordCodeRequest request,
        ServerCallContext context)
    {
        var forgotPasswordResponse = await _identityProvider.ForgotPasswordAsync (
            new ForgotPasswordRequest
            {
                ClientId = _config.ClientId,
                Username = request.Email
            });

        return new GetResetPasswordCodeReply
        {
            Email = forgotPasswordResponse.CodeDeliveryDetails.Destination
        };
    }

    public override async Task<ResetPasswordReply> ResetPassword (ResetPasswordRequest request, ServerCallContext context)
    {
        try
        {
            var response = await _identityProvider.ConfirmForgotPasswordAsync (
                new ConfirmForgotPasswordRequest
                {
                    ClientId = _config.ClientId,
                    Username = request.Email,
                    Password = request.Password,
                    ConfirmationCode = request.Code
                });

            if (response.HttpStatusCode == HttpStatusCode.OK)
                return new ResetPasswordReply ();
        } catch (CodeMismatchException)
        {
            return new ResetPasswordReply
            {
                Error = "Invalid Confirmation Code"
            };
        } catch (Exception e)
        {
            _logger.LogError ("Failed to reset password: {Message}", e.Message);
            throw new RpcException (new Status (StatusCode.Unknown, $"Failed to reset password: {e.Message}"));
        }

        throw new RpcException (new Status (StatusCode.Unknown, "Failed to reset password"));
    }
}