using System.Linq;
using ThirdParty.Extensions;
using ThirdParty.Extensions.CanvasGroupFader;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.UI.PresentersGroups
{
    public class SinglePresentersWithHeaderGroup : SinglePresentersGroup
    {
        [SerializeField]
        private float _headerTextChangeAnimationDuration = 0.3f;

        [SerializeField]
        private Transform _withBackHeader;

        [SerializeField]
        private TextMeshProUGUI _withBackText;

        [SerializeField]
        private Transform _withoutBackHeader;

        [SerializeField]
        private TextMeshProUGUI _withoutBackText;

        [SerializeField]
        private Button _backButton;

        protected override void Awake ()
        {
            base.Awake ();

            _backButton
                .OnClickAsObservable ()
                .Subscribe (_ => OnBackClicked ())
                .AddTo (this);
        }

        private void OnBackClicked ()
        {
            var visiblePresenter = Presenters.FirstOrDefault (x => x.IsVisible ());
            if (visiblePresenter == null)
                return;

            visiblePresenter.ClosedWithBack ();

            var presenterToDisplay = visiblePresenter.BackPresenter;
            if (presenterToDisplay != null)
                presenterToDisplay.Display ();
        }

        protected override void OnPresenterDisplaying (PresenterVisibilityEventArgs args)
        {
            base.OnPresenterDisplaying (args);

            Observable
                .NextFrame ()
                .Subscribe (
                    _ =>
                    {
                        var currentWithBackState = _withBackHeader.IsVisible ();
                        var newWithBackState = args.Presenter.BackPresenter != null;

                        if (currentWithBackState == newWithBackState)
                        {
                            (currentWithBackState ? _withBackText : _withoutBackText).AnimateTextChangeThroughFade (
                                args.Presenter.Header,
                                _headerTextChangeAnimationDuration);
                        } else
                        {
                            (newWithBackState ? _withBackText : _withoutBackText).text = args.Presenter.Header;
                            var (headerToHide, headerToShow) = newWithBackState
                                ? (_withoutBackHeader, _withBackHeader)
                                : (_withBackHeader, _withoutBackHeader);

                            headerToHide.FadeOut (_headerTextChangeAnimationDuration);
                            headerToShow.FadeIn (_headerTextChangeAnimationDuration);
                        }
                    });
        }
    }
}