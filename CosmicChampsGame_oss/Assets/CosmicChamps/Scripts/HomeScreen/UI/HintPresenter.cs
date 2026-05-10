using CosmicChamps.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class HintPresenter : AbstractPresenter<LocalizedString, Unit, HintPresenter>
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private Button _closeButton;

        protected override void Awake ()
        {
            _closeButton.OnClickAsObservable ().Subscribe (_ => Hide ()).AddTo (this);
        }

        protected override void Refresh ()
        {
            _text.text = model.GetLocalizedString ();
        }
    }
}