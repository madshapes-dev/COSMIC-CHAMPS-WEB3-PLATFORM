using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.Components
{
    [RequireComponent (typeof (ScrollRect))]
    public class ScrollRectDisabler : MonoBehaviour
    {
        private ScrollRect scrollRect;

        private void Awake ()
        {
            scrollRect = GetComponent<ScrollRect> ();

            scrollRect
                .content
                .ObserveEveryValueChanged (x => x.rect)
                .Merge (scrollRect.viewport.ObserveEveryValueChanged (x => x.rect))
                .Delay (TimeSpan.FromMilliseconds (500))
                .Select (_ => scrollRect.content.rect.height < scrollRect.viewport.rect.height)
                .Subscribe (x => scrollRect.enabled = !x)
                .AddTo (this);
        }
    }
}