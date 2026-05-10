using System;
using UnityEngine.EventSystems;

namespace CosmicChamps.UI
{
    public class UILocker
    {
        private readonly EventSystem _eventSystem;
        private ProgressIcon[] _progressIcons;

        public UILocker (EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public void Lock (params ProgressIcon[] progressIcons)
        {
            _progressIcons = progressIcons;

            if (progressIcons != null)
                Array.ForEach (progressIcons, x => x.FadeIn ());

            _eventSystem.enabled = false;
        }

        public void Unlock ()
        {
            if (_progressIcons != null)
                Array.ForEach (_progressIcons, x => x.FadeOut ());

            _eventSystem.enabled = true;
        }
    }
}