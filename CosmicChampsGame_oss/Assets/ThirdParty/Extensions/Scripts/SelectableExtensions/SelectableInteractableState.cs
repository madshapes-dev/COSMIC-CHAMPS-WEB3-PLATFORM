using UniRx;

namespace ThirdParty.Extensions.SelectableExtensions
{
    public class SelectableInteractableState : SelectableState
    {
        private void OnEnable ()
        {
            if (Selectable.interactable)
                Apply (0f);

            Selectable
                .ObserveEveryValueChanged (x => x.interactable)
                .Skip (1)
                .Where (x => x)
                .TakeUntilDisable (this)
                .Subscribe (_ => Apply ());
        }
    }
}