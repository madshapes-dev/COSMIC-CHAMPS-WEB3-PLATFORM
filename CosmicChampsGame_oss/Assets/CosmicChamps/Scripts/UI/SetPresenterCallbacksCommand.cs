using System;
using CosmicChamps.UI;

namespace CosmicChamps.Utils
{
    public readonly struct SetPresenterCallbacksCommand<TPresenter, TModel, TCallbacks> : IDisposable
        where TPresenter : AbstractPresenter<TModel, TCallbacks, TPresenter>
    {
        private readonly TPresenter _presenter;
        private readonly TCallbacks _callbacks;

        public SetPresenterCallbacksCommand (TPresenter presenter, TCallbacks callbacks) : this ()
        {
            _presenter = presenter;
            _callbacks = callbacks;
        }

        public IDisposable Run ()
        {
            _presenter.SetCallbacks (_callbacks);
            return this;
        }

        public void Dispose ()
        {
            _presenter.SetCallbacks (default);
        }
    }
}