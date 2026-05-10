using System;
using CosmicChamps.Common;
using CosmicChamps.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.Battle.UI
{
    public class EmojiButton : AbstractPresenter<EmojiButton.Model, EmojiButton.Callbacks, EmojiButton>
    {
        public class Factory : PlaceholderFactory<EmojiButton>
        {
        }

        public readonly struct Model
        {
            public readonly string Id;

            public Model (string id)
            {
                Id = id;
            }
        }

        public readonly struct Callbacks
        {
            public readonly Action<string> OnClicked;

            public Callbacks (Action<string> onClicked)
            {
                OnClicked = onClicked;
            }
        }

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Button _button;

        [Inject]
        private IEmojisProvider _emojisProvider;

        protected override void Refresh ()
        {
            async UniTaskVoid RefreshAsync ()
            {
                _image.sprite = await _emojisProvider.GetEmoji (model.Id);
            }

            base.Refresh ();
            RefreshAsync ().Forget ();
        }

        public override void SetCallbacks (Callbacks callbacks)
        {
            base.SetCallbacks (callbacks);
            _button
                .OnClickAsObservable ()
                .Subscribe (_ => callbacks.OnClicked.Invoke (model.Id))
                .AddTo (_callbacksDisposables);
        }
    }
}