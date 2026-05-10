using UniRx;

namespace ThirdParty.Extensions.ToggleExtensions
{
    public class ToggleOffState : ToggleState
    {
        private void OnEnable ()
        {
            if (!Toggle.isOn)
                Apply (0f);

            Toggle
                .OnValueChangedAsObservable ()
                .Where (x => !x)
                .TakeUntilDisable (this)
                .Subscribe (_ => Apply ());
        }
    }
}