using CosmicChamps.Auth.Services;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Data
{
    public class PlayerRepository
    {
        private readonly Auth.Services.Auth.AuthClient authClient;

        public PlayerRepository (Auth.Services.Auth.AuthClient authClient)
        {
            this.authClient = authClient;
        }

        public UniTask SignUp (string email, string password) =>
            authClient
                .SignUpAsync (
                    new SignUpRequest
                    {
                        Email = email,
                        Password = password
                    })
                .ResponseAsync
                .AsUniTask ();

        public UniTask ConfirmSignUp (string email, string code) =>
            authClient
                .ConfirmSignUpAsync (
                    new ConfirmSignUpRequest ()
                    {
                        Email = email,
                        Code = code
                    })
                .ResponseAsync
                .AsUniTask ();

        public UniTask<Tokens> SignIn (string email, string password) =>
            authClient
                .SignInAsync (
                    new SignInRequest
                    {
                        Email = email,
                        Password = password
                    })
                .ResponseAsync
                .ContinueWith (
                    x => new Tokens
                    {
                        IdToken = x.Result.Tokens.IdToken,
                        AccessToken = x.Result.Tokens.AccessToken,
                        RefreshToken = x.Result.Tokens.RefreshToken,
                        ExpirationDate = x.Result.Tokens.AccessTokenExpireTime
                    })
                .AsUniTask ();

        public UniTask<string> GetResetPasswordCodeAsync (string email) =>
            authClient
                .GetResetPasswordCodeAsync (new GetResetPasswordCodeRequest { Email = email })
                .ResponseAsync
                .ContinueWith (x => x.Result.Email)
                .AsUniTask ();

        public UniTask<string> ResetPassword (string email, string password, string code) =>
            authClient
                .ResetPasswordAsync (
                    new ResetPasswordRequest
                    {
                        Email = email,
                        Password = password,
                        Code = code
                    })
                .ResponseAsync
                .ContinueWith (x => x.Result.Error)
                .AsUniTask ();

        public UniTask<Tokens> GuestSignIn (string deviceId) =>
            authClient
                .GuestSignInAsync (
                    new GuestSignInRequest
                    {
                        DeviceId = deviceId
                    })
                .ResponseAsync
                .ContinueWith (
                    x => new Tokens
                    {
                        IdToken = x.Result.Tokens.IdToken,
                        AccessToken = x.Result.Tokens.AccessToken,
                        RefreshToken = x.Result.Tokens.RefreshToken,
                        ExpirationDate = x.Result.Tokens.AccessTokenExpireTime
                    })
                .AsUniTask ();

        public UniTask<Tokens> ImmutableSignIn (string immutableAccessToken) =>
            authClient
                .ImmutableSignInAsync (
                    new ImmutableSignInRequest
                    {
                        ImmutableAccessToken = immutableAccessToken
                    })
                .ResponseAsync
                .ContinueWith (
                    x => new Tokens
                    {
                        IdToken = x.Result.Tokens.IdToken,
                        AccessToken = x.Result.Tokens.AccessToken,
                        RefreshToken = x.Result.Tokens.RefreshToken,
                        ExpirationDate = x.Result.Tokens.AccessTokenExpireTime
                    })
                .AsUniTask ();
    }
}