using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.ToggleExtensions
{
    [RequireComponent (typeof (Toggle))]
    public class ToggleValueWithInteractableSync : MonoBehaviour
    {
        private Toggle toggle;

        private void Awake ()
        {
            toggle = GetComponent<Toggle> ();
            toggle
                .OnValueChangedAsObservable ()
                .Select (x => !x)
                .SubscribeToInteractable (toggle)
                .AddTo (this);
        }
    }
}