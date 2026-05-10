namespace CosmicChamps.UI
{
    public readonly struct PresenterVisibilityEventArgs
    {
        public readonly AbstractPresenter Presenter;
        public readonly PresenterDisplayOptions Options;

        public PresenterVisibilityEventArgs (AbstractPresenter presenter, PresenterDisplayOptions options)
        {
            Presenter = presenter;
            Options = options;
        }
    }
}