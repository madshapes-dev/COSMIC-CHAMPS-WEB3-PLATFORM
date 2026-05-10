using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions.Components
{
    [RequireComponent (typeof (LayoutGroup))]
    public class LayoutGroupDisabler : MonoBehaviour, ILayoutGroup
    {
        [SerializeField]
        private LayoutGroup layoutGroup;

        private void OnEnable ()
        {
            layoutGroup.enabled = true;
        }

        private void ScheduleDisabling ()
        {
            if (!Application.isPlaying)
                return;

            Observable
                .EveryLateUpdate ()
                .Take (1)
                .Subscribe (_ => layoutGroup.enabled = false);
        }

        public void SetLayoutHorizontal ()
        {
            ScheduleDisabling ();
        }

        public void SetLayoutVertical ()
        {
            ScheduleDisabling ();
        }
    }
}