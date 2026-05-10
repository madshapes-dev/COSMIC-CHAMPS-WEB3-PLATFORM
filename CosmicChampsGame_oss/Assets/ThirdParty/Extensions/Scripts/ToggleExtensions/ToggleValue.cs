using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.ToggleExtensions
{
    [RequireComponent (typeof (Toggle))]
    public abstract class ToggleValue<TValue> : MonoBehaviour
    {
        [SerializeField]
        private TValue value;

        private Toggle toggle;

        public TValue Value
        {
            set => this.value = value;
            get => value;
        }

        public Toggle Toggle => toggle == null ? toggle = GetComponent<Toggle> () : toggle;
    }
}