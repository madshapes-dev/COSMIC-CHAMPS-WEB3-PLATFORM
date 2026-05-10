using CosmicChamps.Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace CosmicChamps.Bootstrap.Client.UI
{
    public class VersionCaption : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _caption;

        [Inject]
        private AppProfile _appProfile;

        private void Awake ()
        {
            _caption.text = $"{BuildInfo.AppVersionString} {_appProfile.Name}";
        }
    }
}