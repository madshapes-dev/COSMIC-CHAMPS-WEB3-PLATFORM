using System;
using CosmicChamps.Battle.Client;
using CosmicChamps.Services;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Battle.UI
{
    public class EmojisPanel : MonoBehaviour
    {
        [SerializeField]
        private Button _showButton;

        [SerializeField]
        private Button _hideButton;

        [SerializeField]
        private Transform _pane;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        [Inject]
        private ILogger _logger;

        [Inject]
        private BattleService _battleService;

        [Inject]
        private EmojiButton.Factory _buttonsFactory;

        [Inject]
        private IGameService _gameService;

        private void Awake ()
        {
            _showButton.OnClickAsObservable ().Subscribe (_ => ShowPane ()).AddTo (this);
            _hideButton.OnClickAsObservable ().Subscribe (_ => HidePane (false)).AddTo (this);

            var player = _gameService.GetCachedPlayer ();

            foreach (var emoji in player.Emojis)
            {
                var button = _buttonsFactory.Create ();
                button.model = new EmojiButton.Model (emoji);
                button.SetCallbacks (new EmojiButton.Callbacks (OnEmojiClicked));
                button.SetParent (transform);
            }

            HidePane (true);
        }

        private void OnEmojiClicked (string emoji)
        {
            _logger.Information ("OnEmojiClicked {Id}", emoji);
            _battleService.SetEmoji (emoji);

            HidePane (false);
        }

        private void ShowPane ()
        {
            _pane.FadeIn ();
            _showButton.interactable = false;
            _hideButton.interactable = true;
        }

        private void HidePane (bool immediate)
        {
            _pane.FadeOut (immediate);
            _showButton.interactable = true;
            _hideButton.interactable = false;
        }
        #endif
    }
}