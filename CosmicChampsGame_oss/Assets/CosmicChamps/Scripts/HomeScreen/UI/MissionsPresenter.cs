using System.Linq;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class MissionsPresenter : AbstractPresenter<Player, Unit, MissionsPresenter>
    {
        [SerializeField]
        private TMP_Text _missionTitle;
        
        [SerializeField]
        private TMP_Text _missionReward;
        
        [SerializeField]
        private Progressbar _missionProgressbar;
        
        [SerializeField]
        private TMP_Text _missionProgressCaption;

        [SerializeField]
        private GameObject _walletConnected;

        [SerializeField]
        private GameObject _allMissionsCompleted;

        [SerializeField]
        private GameObject _walletNotConnected;

        [SerializeField]
        private Button _walletsButton;
        
        [SerializeField]
        private Button _readMoreButton;
        
        [SerializeField]
        private LocalizedString _missionGamesPlayedString;
        
        [SerializeField]
        private LocalizedString _missionRewardString;
        

        [Inject]
        private IGameService _gameService;
        
        [Inject]
        private WalletsPresenter _walletsPresenter;

        protected override void Awake()
        {
            base.Awake();
            
            _walletsButton
                .OnClickAsObservable ()
                .Subscribe (_ => OnWalletsClicked ())
                .AddTo (this);

            _readMoreButton
                .OnClickAsObservable()
                .Subscribe(_ => OnReadMoreClicked())
                .AddTo(this);
        }

        private void OnReadMoreClicked()
        {
            Application.OpenURL(_gameService.GetCachedGameData().Missions.Url);
        }

        private void OnWalletsClicked() => _walletsPresenter.Display ();

        protected override void Refresh()
        {
            var missionsEnabled = _gameService.GetCachedGameData().Missions.Enabled; 
            this.SetVisible(missionsEnabled);
            
            if (!missionsEnabled)
                return;
            
            model
                .ObserveEveryValueChanged(x => x.LinkedWalletId)
                .ToReactiveProperty()
                .Subscribe(OnLinedWallet)
                .AddTo(_modelDisposables);
        }

        private void OnLinedWallet(string linkedWallet)
        {
            var walletConnected = !string.IsNullOrEmpty(linkedWallet);
            var gameData = _gameService.GetCachedGameData();
            var missions = gameData.Missions.List;
            var allMissionsCompleted = missions.Length == 0 || missions.Last().GamesCount <= model.MissionGamesPlayed; 
            
            _walletConnected.SetActive(walletConnected && !allMissionsCompleted);
            _allMissionsCompleted.SetActive(walletConnected && allMissionsCompleted);
            _walletNotConnected.SetActive(!walletConnected);
            
            if (allMissionsCompleted || !walletConnected)
                return;
            
            var missionIndex = missions.Length - 1;
            for (var i = 0; i < missions.Length; i++)
            {
                if (model.MissionGamesPlayed >= missions[i].GamesCount)
                    continue;
                
                missionIndex = i;
                break;
            }
            
            var prevMissionGamesCount = missionIndex == 0 ? 0 : missions[missionIndex - 1].GamesCount;
            var mission = missions[missionIndex];
            var missionGamesCount = mission.GamesCount - prevMissionGamesCount;
            var missionGamesPlayed = Mathf.Min(model.MissionGamesPlayed - prevMissionGamesCount, missionGamesCount);
            
            _missionTitle.text = mission.Title;
            _missionReward.text = $"{_missionRewardString.GetLocalizedString()}: <sprite name=\"{mission.Reward.Kind}\"> {mission.Reward.Amount}";
            _missionProgressCaption.text = $"{_missionGamesPlayedString.GetLocalizedString()}: {missionGamesPlayed}/{missionGamesCount}";
            _missionProgressbar.SetValue((float)missionGamesPlayed/missionGamesCount, true);
        }
    }
}