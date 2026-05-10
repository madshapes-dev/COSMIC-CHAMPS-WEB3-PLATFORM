using CosmicChamps.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CosmicChamps.Services
{
    public class AuthService
    {
        private readonly PlayerRepository _playerRepository;
        private readonly TokensRepository _tokensRepository;
        private readonly IImmutableService _immutableService;

        public AuthService (
            PlayerRepository playerRepository,
            TokensRepository tokensRepository,
            IImmutableService immutableService)
        {
            _playerRepository = playerRepository;
            _tokensRepository = tokensRepository;
            _immutableService = immutableService;
        }

        public async UniTask<bool> IsSignedIn ()
        {
            var tokens = await _tokensRepository.Get ();
            return tokens != null;
        }

        public UniTask SignIn (string email, string password) =>
            _playerRepository
                .SignIn (email, password)
                .ContinueWith (x => _tokensRepository.Set (x));

        public UniTask SignUp (string email, string password) =>
            _playerRepository.SignUp (email, password);

        public UniTask ConfirmSignUp (string email, string code) =>
            _playerRepository.ConfirmSignUp (email, code);

        public UniTask GuestSignIn (string deviceId) =>
            _playerRepository
                .GuestSignIn (deviceId)
                .ContinueWith (x => _tokensRepository.Set (x));

        public async UniTask ImmutableSignIn ()
        {
            await _immutableService.Login ();
            await _immutableService.RequestAccounts ();

            var immutableAccessToken = await _immutableService.GetAccessToken ();
            var tokens = await _playerRepository.ImmutableSignIn (immutableAccessToken);

            _tokensRepository.Set (tokens);
        }

        public void SignOut ()
        {
            async UniTaskVoid ImmutableLogout ()
            {
                Debug.Log ("SignOutAsync _immutableService.Logout...");
                await _immutableService.Logout ();
                Debug.Log ("SignOutAsync _immutableService.Logout Done");
            }

            _tokensRepository.Clear ();
            ImmutableLogout ().Forget ();
        }

        public UniTask<string> GetResetPasswordCodeAsync (string email) =>
            _playerRepository.GetResetPasswordCodeAsync (email);

        public UniTask<string> ResetPassword (string email, string password, string code) =>
            _playerRepository.ResetPassword (email, password, code);
    }
}