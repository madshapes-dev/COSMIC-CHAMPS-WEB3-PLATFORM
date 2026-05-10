using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    [RequireComponent (typeof (Button))]
    public class HintTrigger : MonoBehaviour
    {
        [SerializeField]
        private LocalizedString _hint;

        [Inject]
        private HintPresenter _hintPresenter;

        private void Awake ()
        {
            GetComponent<Button> ()
                .OnClickAsObservable ()
                .Subscribe (_ => _hintPresenter.Display (_hint))
                .AddTo (this);
        }
    }
}