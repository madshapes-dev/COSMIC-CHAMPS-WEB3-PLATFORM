using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.ToggleExtensions
{
    [RequireComponent (typeof (ToggleGroup))]
    public class ToggleGroupValue<TValue> : MonoBehaviour
    {
        private ToggleGroup toggleGroup;
        private ToggleValue<TValue>[] toggleValues;

        public ToggleGroup ToggleGroup => toggleGroup;

        public ToggleValue<TValue>[] ToggleValues => toggleValues ??= GetComponentsInChildren<ToggleValue<TValue>> ();

        public TValue Value
        {
            get
            {
                if (toggleGroup == null)
                    toggleGroup = GetComponent<ToggleGroup> ();

                var activeToggle = toggleGroup.GetFirstActiveToggle ();
                if (activeToggle == null)
                    return default;

                var toggleValue = activeToggle.GetComponent<ToggleValue<TValue>> ();
                return toggleValue == null ? default : toggleValue.Value;
            }
        }
    }
}