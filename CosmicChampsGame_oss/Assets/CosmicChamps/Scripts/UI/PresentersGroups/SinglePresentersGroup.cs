namespace CosmicChamps.UI.PresentersGroups
{
    public class SinglePresentersGroup : PresentersGroup
    {
        protected override void OnPresenterDisplaying (PresenterVisibilityEventArgs args)
        {
            base.OnPresenterDisplaying (args);

            foreach (var presenter in Presenters)
            {
                if (presenter == args.Presenter)
                    continue;

                presenter.Hide (PresenterDisplayOptions.Immediate);
            }
        }
    }
}