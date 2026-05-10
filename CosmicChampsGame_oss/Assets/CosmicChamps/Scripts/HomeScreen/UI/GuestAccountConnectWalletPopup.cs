using CosmicChamps.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class GuestAccountConnectWalletPopup : AbstractPresenter
    {
        [SerializeField]
        private Button _bindEmailButton;

        [SerializeField]
        private Button _closeButton;

        [Inject]
        private BindEmailPresenter _bindEmailPresenter;

        protected override void Awake ()
        {
            base.Awake ();

            _bindEmailButton
                .OnClickAsObservable ()
                .Subscribe (OnBindEmailClicked)
                .AddTo (this);

            _closeButton
                .OnClickAsObservable ()
                .Subscribe (OnCloseClicked)
                .AddTo (this);
        }

        private void OnBindEmailClicked (Unit _)
        {
            _bindEmailPresenter.Display (new BindEmailPresenter.Model (null));
            Hide ();
        }

        private void OnCloseClicked (Unit _)
        {
            Hide ();
        }

        public override void ForceRefresh ()
        {
        }

        public override void ForceClear ()
        {
        }
    }
}