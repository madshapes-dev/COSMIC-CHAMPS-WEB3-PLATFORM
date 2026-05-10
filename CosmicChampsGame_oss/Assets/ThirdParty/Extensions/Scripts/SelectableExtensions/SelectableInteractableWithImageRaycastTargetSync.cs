using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.SelectableExtensions
{
    [RequireComponent (typeof (Selectable), typeof (Image))]
    public class SelectableInteractableWithImageRaycastTargetSync : MonoBehaviour
    {
        private Selectable selectable;
        private Image image;

        private void Awake ()
        {
            selectable = GetComponent<Selectable> ();
            image = GetComponent<Image> ();

            selectable
                .ObserveEveryValueChanged (x => x.interactable)
                .Subscribe (x => image.raycastTarget = x)
                .AddTo (this);
        }
    }
}