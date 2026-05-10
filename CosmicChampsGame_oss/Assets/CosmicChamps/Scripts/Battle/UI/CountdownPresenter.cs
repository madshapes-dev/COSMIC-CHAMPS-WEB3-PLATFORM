using System;
using System.Linq;
using CosmicChamps.Battle.Data;
using CosmicChamps.Data;
using CosmicChamps.Services;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;
using DisposableExtensions = ThirdParty.Extensions.DisposableExtensions;
using ILogger = Serilog.ILogger;
using Player = CosmicChamps.Battle.Data.Client.Player;
using Opponent = CosmicChamps.Battle.Data.Client.Opponent;

namespace CosmicChamps.Battle.UI
{
    public class CountdownPresenter : AbstractPresenter<CountdownPresenter.Model, Unit, CountdownPresenter>
    {
        public readonly struct Model
        {
            public readonly Player Player;
            public readonly Opponent Opponent;

            public Model (Player player, Opponent opponent)
            {
                Player = player;
                Opponent = opponent;
            }
        }

        [Serializable]
        private class PlayerInfoViewSettings
        {
            [SerializeField]
            private PlayerTeam _playerTeam;

            [SerializeField]
            private Sprite _nameBackground;

            [SerializeField]
            private Color _fadeColor;

            public PlayerTeam PlayerTeam => _playerTeam;

            public Sprite NameBackground => _nameBackground;

            public Color FadeColor => _fadeColor;
        }

        [Serializable]
        private class PlayerInfo
        {
            [SerializeField]
            private TextMeshProUGUI _nameCaption;

            [SerializeField]
            private TextMeshProUGUI _ratingCaption;

            [SerializeField]
            private Image _nameBackground;

            [SerializeField]
            private Image _fade;

            public TextMeshProUGUI NameCaption => _nameCaption;

            public TextMeshProUGUI RatingCaption => _ratingCaption;

            public void ApplySettings (PlayerInfoViewSettings playerInfoViewSettings)
            {
                _nameBackground.sprite = playerInfoViewSettings.NameBackground;
                _fade.color = playerInfoViewSettings.FadeColor;
            }
        }

        [SerializeField]
        private TextMeshProUGUI _timerCaption;

        [SerializeField]
        private Image _progressBar;

        [SerializeField]
        private Image _background;

        [SerializeField]
        private CanvasGroup _playersDataCanvasGroup;

        [SerializeField]
        private string _vsMessage = "vs";

        [SerializeField]
        private string _ratingPrefix;

        [SerializeField]
        private float _timerChangeFadeDuration = 0.2f;

        [SerializeField]
        private float _backgroundDefaultAlpha = 0.9f;

        [SerializeField]
        private float _playersDataHideDuration = 0.5f;

        [FormerlySerializedAs ("_playerView")]
        [SerializeField]
        private PlayerInfo _playerInfo;

        [FormerlySerializedAs ("_opponentView")]
        [SerializeField]
        private PlayerInfo _opponentInfo;

        [FormerlySerializedAs ("_playerViewSettings")]
        [SerializeField]
        private PlayerInfoViewSettings[] _playerInfoViewSettings;

        [Inject]
        private GameDataRepository _gameDataRepository;

        [Inject]
        private IBrandingFactory _brandingFactory;

        [Inject]
        private ILogger _logger;

        [Inject]
        private IGameService _gameService;

        private readonly CompositeDisposable _countdownDisposables = new();
        private IDisposable _countdownAnimation;

        protected override void Awake ()
        {
            base.Awake ();
            _countdownDisposables.AddTo (this);
        }

        public void StartCountdown (float delay, int seconds)
        {
            _countdownDisposables.Clear ();

            var timerCaptionSequence = DOTween.Sequence ();
            var interval = 0f;
            for (var i = 0; i < seconds; i++)
            {
                var countdownText = (seconds - i).ToString ();
                timerCaptionSequence
                    .AppendInterval (interval)
                    .AppendCallback (() => _timerCaption.AnimateTextChangeThroughFade (countdownText, _timerChangeFadeDuration));

                interval = 1f;
            }

            _countdownAnimation = DOTween
                .Sequence ()
                .AppendInterval (delay)
                .Append (_playersDataCanvasGroup.DOFade (0f, _playersDataHideDuration))
                .Join (
                    DOTween
                        .Sequence ()
                        .Append (_progressBar.DOFillAmount (0f, seconds).SetEase (Ease.Linear))
                        .Join (_background.DOFade (0f, seconds).SetEase (Ease.InCubic))
                        .Join (timerCaptionSequence)
                        .AppendCallback (() => Hide ()))
                .AsDisposable ()
                .AddTo (_countdownDisposables);
        }

        protected override void Refresh ()
        {
            DisposableExtensions.DisposeAndReset (ref _countdownAnimation);

            _timerCaption.text = _vsMessage;
            _progressBar.fillAmount = 1f;
            _background.color = _background.color.WithA (_backgroundDefaultAlpha);
            _playersDataCanvasGroup.alpha = 1f;

            var playerTeam = model.Player.Team;
            var opponentTeam = playerTeam.GetOpposite ();

            var playerViewSettings = _playerInfoViewSettings.FirstOrDefault (x => x.PlayerTeam == playerTeam);
            var opponentViewSettings = _playerInfoViewSettings.FirstOrDefault (x => x.PlayerTeam == opponentTeam);

            if (playerViewSettings == null)
                throw new InvalidOperationException ($"Unable to find view settings for team {playerTeam}");

            if (opponentViewSettings == null)
                throw new InvalidOperationException ($"Unable to find view settings for team {opponentTeam}");

            var me = _gameDataRepository.GetCachedPlayer ();
            _playerInfo.ApplySettings (playerViewSettings);
            _playerInfo.NameCaption.text = me.DisplayName;
            _playerInfo.RatingCaption.text = $"{_ratingPrefix}{me.Rating}";

            _opponentInfo.ApplySettings (opponentViewSettings);
            _opponentInfo.NameCaption.text = model.Opponent.Name;
            _opponentInfo.RatingCaption.text = $"{_ratingPrefix}{model.Opponent.Rating}";
        }

        protected override void Clear ()
        {
            base.Clear ();
            _countdownDisposables.Clear ();
        }

        public override async UniTask DisplayAsync (Model model, PresenterDisplayOptions options = PresenterDisplayOptions.Notify)
        {
            _logger.Information ("DisplayAsync");

            var opponentBot = _gameService
                .GetCachedGameData ()
                .Bots
                .FirstOrDefault (x => x.PlayerId == model.Opponent.Id);

            if (opponentBot != null)
                await _brandingFactory.GetCountdown (opponentBot.Id, _playersDataCanvasGroup.transform);

            await base.DisplayAsync (model, options);
        }
    }
}