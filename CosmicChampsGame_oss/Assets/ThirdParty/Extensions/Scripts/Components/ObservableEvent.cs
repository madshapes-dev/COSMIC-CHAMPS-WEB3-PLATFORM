using System;
using UniRx;

namespace ThirdParty.Extensions.Components
{
    public class ObservableEvent<T> : IObservable<T>
    {
        private class Proxy : IObservable<T>
        {
            private readonly ObservableEvent<T> _event;

            public Proxy (ObservableEvent<T> @event)
            {
                _event = @event;
            }

            public IDisposable Subscribe (IObserver<T> observer) => _event.Subscribe (observer);
        }

        private readonly Subject<T> _subject = new();
        private Proxy _proxy;

        public IObservable<T> AsObservable () => _proxy ??= new Proxy (this);
        public void Fire (T data) => _subject.OnNext (data);
        public void Fire (Exception e) => _subject.OnError (e);
        public IDisposable Subscribe (IObserver<T> observer) => _subject.Subscribe (observer);
    }
}