using System.Linq;
using CosmicChamps.Data;
using CosmicChamps.UI;
using ThirdParty.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.HomeScreen.UI
{
    public class NewsPresenter : AbstractPresenter<News, Unit, NewsPresenter>
    {
        [SerializeField]
        private TextMeshProUGUI _headerText;

        [SerializeField]
        protected TextMeshProUGUI _messageText;

        [SerializeField]
        private Button _linkButton;

        [SerializeField]
        private Button[] _closeButtons;

        protected override void Awake ()
        {
            base.Awake ();

            _closeButtons
                .Select (x => x.OnClickAsObservable ())
                .Merge ()
                .Subscribe (_ => Hide ())
                .AddTo (this);

            _linkButton
                .OnClickAsObservable ()
                .Subscribe (OpenLink)
                .AddTo (this);
        }

        private void OpenLink (Unit _)
        {
            Application.OpenURL (model.Link);
            Hide ();
        }

        protected override void Refresh ()
        {
            _headerText.text = model.Header;
            _messageText.text = model.Text;

            _linkButton.SetVisible (!string.IsNullOrEmpty (model.Link));
        }
    }
}