using System;
using CosmicChamps.Services;

namespace CosmicChamps
{
    public class WebGLDeviceIdeProvider : IDeviceIdProvider
    {
        private readonly PlayerPrefsService _playerPrefsService;

        public WebGLDeviceIdeProvider (PlayerPrefsService playerPrefsService)
        {
            _playerPrefsService = playerPrefsService;
        }

        public string DeviceId
        {
            get
            {
                var deviceId = _playerPrefsService.WebGLDeviceId.Value;
                if (string.IsNullOrEmpty (deviceId))
                {
                    deviceId = Guid.NewGuid ().ToString ();
                    _playerPrefsService.WebGLDeviceId.Value = deviceId;
                }

                return deviceId;
            }
        }
    }
}