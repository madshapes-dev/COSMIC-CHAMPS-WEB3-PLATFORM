using ThirdParty.Extensions.CanvasGroupFader;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.UI
{
    public class PopupPresenter : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private float _fadeInDuration = 0.2f;

        private void Awake ()
        {
            _closeButton
                .OnClickAsObservable ()
                .Subscribe (_ => Hide ())
                .AddTo (this);
        }

        public void Display () => this.FadeIn (_fadeInDuration);
        public void Hide () => this.FadeOut (_fadeInDuration);
    }
}