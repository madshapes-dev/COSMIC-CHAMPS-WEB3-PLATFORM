using System;
using CosmicChamps.Auth.Services;
using CosmicChamps.Data.Sources.Tokens;
using Cysharp.Threading.Tasks;

namespace CosmicChamps.Data
{
    public class TokensRepository
    {
        private readonly Auth.Services.Auth.AuthClient _authClient;
        private readonly ITokensDataSource _tokensDataSource;

        public TokensRepository (Auth.Services.Auth.AuthClient authClient, ITokensDataSource tokensDataSource)
        {
            _authClient = authClient;
            _tokensDataSource = tokensDataSource;
        }

        public UniTask<Tokens> Refresh () =>
            _authClient
                .RefreshTokenAsync (
                    new RefreshTokenRequest
                    {
                        RefreshToken = _tokensDataSource
                            .Get ()
                            .RefreshToken
                    })
                .ResponseAsync
                .ContinueWith (
                    x => x.Result.Tokens == null
                        ? null
                        : new Tokens
                        {
                            IdToken = x.Result.Tokens.IdToken,
                            AccessToken = x.Result.Tokens.AccessToken,
                            RefreshToken = _tokensDataSource.Get ().RefreshToken,
                            ExpirationDate = x.Result.Tokens.AccessTokenExpireTime
                        })
                .AsUniTask ();

        public async UniTask<Tokens> Get ()
        {
            var tokens = _tokensDataSource.Get ();
            if (tokens == null)
                return null;

            var now = DateTimeOffset.Now.ToUnixTimeSeconds ();
            if (tokens.ExpirationDate > now)
                return tokens;

            tokens = await Refresh ();
            if (tokens != null)
                Set (tokens);
            else
                Clear ();

            return tokens;
        }

        public void Set (Tokens tokens) => _tokensDataSource.Set (tokens);

        public void Clear () => _tokensDataSource.Clear ();
    }
}