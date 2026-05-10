using DG.Tweening;
using ThirdParty.Extensions;
using UnityEngine;

namespace CosmicChamps.Battle
{
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField]
        private string _type;
        
        [SerializeField]
        private Collider _collider;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private float _fadeDuration = 0.2f;

        private Tween _tween;
        private float _fadeLevel;

        public string Type => _type;

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

        public bool Raycast (Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            // Debug.DrawRay (ray.origin, ray.direction * 100f, Color.red, float.MaxValue);
            return _collider.Raycast (ray, out hitInfo, maxDistance);
        }

        public void FadeIn (bool immediate = false) => Fade (_fadeLevel, immediate);

        public void FadeOut (bool immediate = false) => Fade (0f, immediate);

        public Vector3 GetRandomPosition () => _collider.bounds.GetRandomPosition ();
    }
}