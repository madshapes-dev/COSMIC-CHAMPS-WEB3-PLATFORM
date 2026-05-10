using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.UI
{
    public class LinkButton : MonoBehaviour
    {
        [SerializeField]
        private string _link;

        private Button _button;

        private void Awake ()
        {
            _button = GetComponent<Button> ();
            if (_button != null)
                _button
                    .OnClickAsObservable ()
                    .Subscribe (_ => Application.OpenURL (_link))
                    .AddTo (this);
        }
    }
}