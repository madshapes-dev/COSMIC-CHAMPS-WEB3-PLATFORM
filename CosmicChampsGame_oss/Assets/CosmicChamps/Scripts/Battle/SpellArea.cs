using DG.Tweening;
using ThirdParty.Extensions;
using UnityEngine;

namespace CosmicChamps.Battle
{
    public class SpellArea : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private float _fadeDuration = 0.2f;

        private float _fadeLevel;
        private Tween _tween;

        private void Awake ()
        {
            _fadeLevel = _spriteRenderer.color.a;
            FadeOut (true);
        }

        private void OnDestroy () => KillTween ();

        private void KillTween () => _tween?.Kill ();

        private void Fade (float value, bool immediate = false)
        {
            KillTween ();

            if (immediate)
                _spriteRenderer.color = _spriteRenderer.color.WithA (value);
            else
                _tween = _spriteRenderer.DOFade (value, _fadeDuration);
        }

        public void FadeIn (bool immediate = false) => Fade (_fadeLevel, immediate);

        public void FadeOut (bool immediate = false) => Fade (0f, immediate);

        public void Adjust (Vector3 position, float? size = null)
        {
            transform.position = position;
            if (size.HasValue)
                transform.localScale = Vector3.one * size.Value;
        }
    }
}