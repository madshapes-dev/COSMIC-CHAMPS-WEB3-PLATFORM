using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ThirdParty.Extensions.Components
{
    [DisallowMultipleComponent]
    public class ObservablePostRenderTrigger : ObservableTriggerBase
    {
        private Subject<Unit> onPostRender;

        private void OnPostRender ()
        {
            onPostRender?.OnNext (Unit.Default);
        }

        public IObservable<Unit> OnPostRenderAsObservable ()
        {
            return onPostRender ?? (onPostRender = new Subject<Unit> ());
        }

        protected override void RaiseOnCompletedOnDestroy ()
        {
            onPostRender?.OnCompleted ();
        }
    }

    public static class ObservablePostRenderTriggerExtensions
    {
        public static IObservable<Unit> OnPostRenderAsObservable (this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit> ();

            var trigger = component.GetComponent<ObservablePostRenderTrigger> ();
            if (trigger == null)
                trigger = component.gameObject.AddComponent<ObservablePostRenderTrigger> ();

            return trigger.OnPostRenderAsObservable ();
        }
    }
}