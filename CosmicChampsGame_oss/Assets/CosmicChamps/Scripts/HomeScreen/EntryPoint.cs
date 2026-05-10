#define RESTART_BY_SPACE

using System;
using CosmicChamps.Bootstrap.Client.UI;
using CosmicChamps.Common;
using CosmicChamps.Data;
using CosmicChamps.HomeScreen.UI;
using CosmicChamps.Services;
using CosmicChamps.Signals;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace CosmicChamps.HomeScreen
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private float _splashScreenMinDelay = 3f;

        [SerializeField]
        private LocalizedString _startMessage;

        [Inject]
        private SignPresentersGroup _signPresentersGroup;

        [Inject]
        private GamePresentersGroup _gamePresentersGroup;

        [Inject]
        private AuthService _authService;

        [Inject]
        private SplashScreenPresenter _splashScreenPresenter;

        [Inject]
        private DisclaimerPopup _disclaimerPopup;

        [Inject]
        private GameRepository _gameRepository;

        [Inject]
        private NewsPresenter _newsPresenter;
        
        [Inject]
        private UILocker _uiLocker;

        [Inject]
        private IMessageBroker _messageBroker;

        private async void Start ()
        {
            _splashScreenPresenter.Display (await _startMessage.GetLocalizedStringAsync ());

            var loadingStartTime = Time.realtimeSinceStartup;
            var isSignedIn = await _authService.IsSignedIn ();
            if (!isSignedIn)
            {
                await _signPresentersGroup.DisplayAsync ();
                await HideSplashScreen (loadingStartTime);
                return;
            }

            await _gamePresentersGroup.DisplayAsync ();
            
            var news = await _gameRepository.GetNews ();
            await HideSplashScreen (loadingStartTime);

            if (news != null)
                await _newsPresenter.DisplayAsync (news);
        }

        private async UniTask HideSplashScreen (float loadingStartTime)
        {
            var diff = Time.realtimeSinceStartup - loadingStartTime;
            if (diff < _splashScreenMinDelay)
                await UniTask.Delay (TimeSpan.FromSeconds (diff));

            _disclaimerPopup.DisplayIfNeeded ();
            _splashScreenPresenter.Hide ();
        }
        
        private void Update ()
        {
            #if RESTART_BY_SPACE
            if (Input.GetKeyUp (KeyCode.Space))
                _messageBroker.Publish (new RestartSignal ());
            #endif
        }
    }
}