using System.Linq;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions;
using ThirdParty.Extensions.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


namespace CosmicChamps.UI.PresentersGroups
{
    public abstract class PresentersGroup : AbstractPresenter
    {
        [FormerlySerializedAs ("presenters")]
        [SerializeField,
         Space (20f),
         HelpBox (
             "Please specify certain AbstractPresenter component, but not the GameObject as reference. When referencing GameObject with several AbstractPresenters attached it is possible that wrong one will be referenced",
             HelpBoxMessageType.Info)]
        private AbstractPresenter[] _presenters;

        private AbstractPresenter _defaultPresenter;

        protected AbstractPresenter[] Presenters => _presenters;

        protected override void Awake ()
        {
            _presenters
                .Select (x => x.OnDisplaying)
                .Merge ()
                .Subscribe (OnPresenterDisplaying)
                .AddTo (this);

            if (_defaultPresenter == null)
                _defaultPresenter = _presenters?[0];
        }

        private async UniTask SelfDisplay (PresenterDisplayOptions options = PresenterDisplayOptions.Default) =>
            await base.DisplayAsync (options);

        protected virtual void OnPresenterDisplaying (PresenterVisibilityEventArgs args)
        {
            if (!this.IsVisible ())
                SelfDisplay (args.Options).Forget ();
        }

        public override void ForceRefresh ()
        {
        }

        public override void ForceClear ()
        {
        }

        public UniTask DisplayAsync (
            AbstractPresenter presenter,
            PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            var presenterToDisplay = presenter == null ? _defaultPresenter : presenter;
            // Logger.LogWarning (this, $"DisplayAsync {presenterToDisplay}");
            return UniTask.WhenAll (
                SelfDisplay (options),
                presenterToDisplay == null ? UniTask.CompletedTask : presenterToDisplay.DisplayAsync (options));
        }

        public override UniTask DisplayAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            // Logger.LogWarning (this, $"DisplayAsync");
            return DisplayAsync (_defaultPresenter, options);
        }

        public override async UniTask HideAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            async UniTaskVoid HidePresenters () =>
                await UniTask.WhenAll (_presenters.Select (x => x.HideAsync (options | PresenterDisplayOptions.Immediate)));

            if (!this.IsVisible ())
            {
                HidePresenters ().Forget ();
                return;
            }

            await base.HideAsync (options);
            HidePresenters ().Forget ();
        }
    }
}