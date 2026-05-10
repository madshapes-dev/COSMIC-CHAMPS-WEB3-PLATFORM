using System;
using DG.Tweening;
using UniRx;

namespace ThirdParty.Extensions
{
    public static class TweenExtensions
    {
        private class TweenObservable : IObservable<Unit>
        {
            private readonly Subject<Unit> subject = new();

            public TweenObservable (Tween tween)
            {
                tween.onUpdate += () => subject.OnNext (Unit.Default);
                tween.onComplete += () => subject.OnCompleted ();
                tween.onKill += () => subject.OnCompleted ();
            }

            public IDisposable Subscribe (IObserver<Unit> observer) => subject.Subscribe (observer);
        }

        private class TweenDisposable : IDisposable
        {
            private readonly Tween _tween;

            public TweenDisposable (Tween tween)
            {
                _tween = tween;
            }

            public void Dispose () => _tween?.Kill ();
        }

        public static IObservable<Unit> AsObservable (this Tween tween) => new TweenObservable (tween);
        public static IDisposable AsDisposable (this Tween tween) => new TweenDisposable (tween);
    }
}