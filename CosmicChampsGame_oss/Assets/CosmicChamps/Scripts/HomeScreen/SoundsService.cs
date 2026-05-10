using CosmicChamps.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace CosmicChamps.HomeScreen
{
    public class SoundsService : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AssetReferenceT<AudioClip> _homeScreenMusic;

        [SerializeField]
        private AssetReferenceT<AudioClip> _battleMusic;

        [SerializeField]
        private float _fadeDuration = 0.5f;

        [Inject]
        private PlayerPrefsService _playerPrefsService;

        private Tween _tween;

        public readonly BoolReactiveProperty IsMuted = new();

        private Tween FadeOutAudioSource () => _audioSource.DOFade (0f, _audioSource.volume * _fadeDuration);

        private Tween FadeInAudioSource () => _audioSource.DOFade (1f, (1f - _audioSource.volume) * _fadeDuration);

        private void Awake ()
        {
            var muteSound = _playerPrefsService.MuteSound.Value;

            _audioSource.enabled = !muteSound;
            IsMuted.Value = muteSound;

            IsMuted
                .Skip (1)
                .Subscribe (
                    x =>
                    {
                        _audioSource.enabled = !x;
                        _playerPrefsService.MuteSound.Value = x;
                    })
                .AddTo (this);
        }

        private void OnDestroy ()
        {
            KillTween ();
            _audioSource.Stop ();
        }

        private void KillTween ()
        {
            _tween?.Kill ();
            _tween = null;
        }

        private async UniTaskVoid PlayMusic (AssetReferenceT<AudioClip> audioClipReference)
        {
            var handle = audioClipReference.OperationHandle;
            var audioClip = handle.IsValid ()
                ? handle.Result as AudioClip
                : await audioClipReference.LoadAssetAsync ();

            if (IsMuted.Value)
            {
                _audioSource.clip = audioClip;
                return;
            }

            KillTween ();

            var sequence = DOTween.Sequence ();
            if (_audioSource.isPlaying)
                sequence.Append (FadeOutAudioSource ());

            _tween = sequence
                .AppendCallback (
                    () =>
                    {
                        _audioSource.clip = audioClip;
                        _audioSource.volume = 0f;
                        if (!_audioSource.isPlaying)
                            _audioSource.Play ();
                        else
                            _audioSource.time = 0f;
                    })
                .Append (FadeInAudioSource ());
        }

        public void StopMusic ()
        {
            if (_audioSource == null || !_audioSource.isPlaying)
                return;

            KillTween ();

            _tween = DOTween
                .Sequence ()
                .Append (FadeOutAudioSource ())
                .AppendCallback (() => _audioSource.Stop ());
        }

        public void PlayHomeScreenMusic () => PlayMusic (_homeScreenMusic).Forget ();
        public void PlayBattleMusic () => PlayMusic (_battleMusic).Forget ();
        public void ToggleMute () => IsMuted.Value = !IsMuted.Value;
    }
}