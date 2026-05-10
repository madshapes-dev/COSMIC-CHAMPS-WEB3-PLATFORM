using System.Collections.Generic;
using CosmicChamps.Data;
using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    /*public class DecksPresenter : AbstractPresenter<Unit, DeckPresenter.Callbacks, DecksPresenter>
    {
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private DeckPresenter _deckPresenter;
        
        /*[SerializeField]
        private TextMeshProUGUI _headerCaption;

        [SerializeField]
        private HorizontalScrollSnap _scrollSnap;

        [SerializeField]
        private Button _prevButton;

        [SerializeField]
        private Button _nextButton;#1#

        [SerializeField]
        private LocalizedString _deckHeaderPrefix;

        #if UNITY_EDITOR && !UNITY_EDITOR_MIRROR_STRIP || !UNITY_SERVER
        /*[Inject]
        private DeckPresenter.Factory _deckFactory;#1#

        [Inject]
        private GameDataRepository _gameDataRepository;

        private DeckPresenter.Callbacks _callbacks;
        private PlayerDeck[] _decks;
        private bool _initialized;
        private int _prevPage;

        protected override void Awake ()
        {
            base.Awake ();

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);

            /*_scrollSnap.OnSelectionChangeStartEvent.AddListener (OnPageChanging);
            _scrollSnap.OnSelectionChangeEndEvent.AddListener (OnPageChanged);#1#
        }

        /*protected override void OnDestroy ()
        {
            base.OnDestroy ();
            _scrollSnap.OnSelectionChangeStartEvent.RemoveListener (OnPageChanging);
            _scrollSnap.OnSelectionChangeEndEvent.RemoveListener (OnPageChanged);
        }

        private void OnPageChanging ()
        {
            _prevPage = _scrollSnap.CurrentPage;
        }

        private void OnPageChanged (int page)
        {
            _decks = _gameDataRepository.GetCachedPlayer ().Decks;

            _prevButton.targetGraphic.Fade (page != 0);
            _nextButton.targetGraphic.Fade (page != _decks.Length - 1);

            RefreshHeader (page);

            if (_prevPage != page)
            {
                _scrollSnap
                    .ChildObjects[_prevPage]
                    .GetComponent<DeckPresenter> ()
                    .ResetScrollAndSelection ();
            }

            _gameDataRepository
                .GetCachedPlayer ()
                .ActiveDeckIndex = page;
        }
        
        private void RefreshHeader (int index)
        {
            _deckHeaderPrefix.Arguments = new object[]
                { new Dictionary<string, int> { { "index", index + 1 } } };

            _headerCaption.AnimateTextChangeThroughFade (_deckHeaderPrefix.GetLocalizedString ());
        }        

        private async UniTaskVoid RefreshAsync ()
        {
            var player = _gameDataRepository.GetCachedPlayer ();

            _decks = player.Decks;
            RefreshHeader (player.ActiveDeckIndex);

            foreach (var deck in _decks)
            {
                var deckPresenter = _newDeckFactory
                    .Create ()
                    .AddTo (_modelDisposables);

                deckPresenter.SetCallbacks (_callbacks);
                deckPresenter.model = deck;

                _scrollSnap.AddChild (deckPresenter.gameObject);
            }

            var deckIndex = player.ActiveDeckIndex;
            var transitionSpeedBackup = _scrollSnap.transitionSpeed;
            //Let the scroll snap pass through all Awake/OnEnable/Start logic which could break desired behaviour when running first time
            await UniTask.DelayFrame (1);
            _scrollSnap.transitionSpeed = float.MaxValue;
            _scrollSnap.ChangePage (deckIndex);
            //Letting the Update loop scroll with max transition speed
            await UniTask.DelayFrame (2);
            _scrollSnap.transitionSpeed = transitionSpeedBackup;
            OnPageChanged (deckIndex);
        }

        protected override void Clear ()
        {
            _scrollSnap.RemoveAllChildren (out _);
            base.Clear ();
        }

        protected override void Refresh ()
        {
            RefreshAsync ().Forget ();
        }
        
        public override void SetCallbacks (DeckPresenter.Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _callbacks = callbacks;
        }#1#
        #endif
    }*/
}