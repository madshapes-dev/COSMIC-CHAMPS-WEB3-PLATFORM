using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.Components
{
    [RequireComponent (typeof (Graphics))]
    public class GraphicsPressScaleAnimation : MonoBehaviour
    {
        [SerializeField]
        private float animationStepDuration = 0.05f;

        [SerializeField]
        private float minFactor = 0.95f;

        [SerializeField]
        private float magFactor = 1.05f;

        [SerializeField]
        private Transform target;

        private readonly CompositeDisposable pointerDownDisposables = new();
        private IDisposable pointerDownSubscription;

        private Logger logger;

        private Graphic graphics;
        private IObservable<bool> componentEnabledObservable;
        private Vector3 baseScale;

        private Transform Target => target != null ? target : transform;

        private void Awake ()
        {
            graphics = GetComponent<Graphic> ();

            componentEnabledObservable = this
                .ObserveEveryValueChanged (x => x.enabled)
                .TakeUntilDestroy (this)
                .ToReactiveProperty ();

            componentEnabledObservable
                .Where (x => x)
                .Subscribe (_ => SubscribeToPointerDown ());
        }

        private void OnDestroy ()
        {
            pointerDownDisposables.Dispose ();
        }

        private void ResetPointerDownSubscriptions ()
        {
            pointerDownDisposables.Clear ();
        }

        private void SubscribeToPointerDown ()
        {
            pointerDownSubscription = graphics
                .OnPointerDownAsObservable ()
                .Where (_ => graphics.raycastTarget)
                .TakeUntil (componentEnabledObservable.Where (x => !x))
                .Subscribe (_ => PointerDown ());
        }

        private void PointerDown ()
        {
            pointerDownSubscription.Dispose ();

            baseScale = transform.localScale;

            Target.DOScale (baseScale * minFactor, animationStepDuration);

            var pointerClickObservable = graphics.OnPointerClickAsObservable ();
            pointerClickObservable
                .Subscribe (_ => PointerClick ())
                .AddTo (pointerDownDisposables);

            graphics
                .OnPointerUpAsObservable ()
                .DelayFrame (0, FrameCountType.EndOfFrame)
                .TakeUntil (pointerClickObservable)
                .Subscribe (_ => PointerUp ())
                .AddTo (pointerDownDisposables);
        }

        private void PointerUp ()
        {
            ResetPointerDownSubscriptions ();

            DOTween
                .Sequence ()
                .Append (Target.DOScale (baseScale, animationStepDuration))
                .AppendCallback (SubscribeToPointerDown);
        }

        private void PointerClick ()
        {
            ResetPointerDownSubscriptions ();

            DOTween
                .Sequence ()
                .Append (Target.DOScale (baseScale * magFactor, animationStepDuration * 2f))
                .Append (Target.DOScale (baseScale, animationStepDuration))
                .AppendCallback (SubscribeToPointerDown);
        }
    }
}