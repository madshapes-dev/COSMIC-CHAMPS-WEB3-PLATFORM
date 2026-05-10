using CosmicChamps.Services;
using CosmicChamps.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class DisclaimerPopup : PopupPresenter
    {
        [Inject]
        private PlayerPrefsService _playerPrefsService;

        public void DisplayIfNeeded ()
        {
            if (_playerPrefsService.DisclaimerShown.Value)
                return;

            _playerPrefsService.DisclaimerShown.Value = true;
            Display ();
        }
    }
}