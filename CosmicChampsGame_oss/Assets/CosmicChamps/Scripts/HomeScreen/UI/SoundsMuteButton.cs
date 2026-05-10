using System;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CosmicChamps.HomeScreen.UI
{
    public class SoundsMuteButton : MonoBehaviour
    {
        [SerializeField]
        private Sprite _onSprite;

        [SerializeField]
        private Sprite _offSprite;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private float _fadeDuration = 0.2f;

        [Inject]
        private SoundsService _soundsService;

        private void Awake ()
        {
            _button
                .OnClickAsObservable ()
                .Subscribe (OnClicked)
                .AddTo (this);
        }

        private void Start ()
        {
            _soundsService
                .IsMuted
                .Subscribe (OnIsMuted)
                .AddTo (this);
        }

        private void OnIsMuted (bool isMuted) => _image.DOSpriteFade (isMuted ? _offSprite : _onSprite, _fadeDuration);
        private void OnClicked (Unit unit) => _soundsService.ToggleMute ();
    }
}