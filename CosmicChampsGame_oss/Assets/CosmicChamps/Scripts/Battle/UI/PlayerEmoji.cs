using CosmicChamps.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThirdParty.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CosmicChamps.Battle.UI
{
    public class PlayerEmoji : MonoBehaviour
    {
        private const float TweenAmplitude = 1.1f;
        private const float TweenPeriod = 0f;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private float _tweenDuration = 0.1f;

        private void KillTween ()
        {
            transform.DOKill ();
            _image.DOKill ();
        }

        public void Display (IEmojisProvider emojisProvider, string emoji)
        {
            async UniTaskVoid InternalChangeEmoji ()
            {
                KillTween ();

                if (string.IsNullOrEmpty (emoji))
                {
                    Hide (false);
                    return;
                }

                var sprite = await emojisProvider.GetEmoji (emoji);

                if (transform.localScale == Vector3.one)
                {
                    await _image.DOSpriteFade (sprite, _tweenDuration);
                    return;
                }

                _image.sprite = sprite;
                await transform
                    .DOScale (1f, _tweenDuration)
                    .SetEase (Ease.OutElastic, TweenAmplitude, TweenPeriod);
            }

            InternalChangeEmoji ().Forget ();
        }

        public void Hide (bool immediate)
        {
            KillTween ();

            if (immediate)
                transform.localScale = Vector3.zero;
            else
                transform
                    .DOScale (0f, _tweenDuration)
                    .SetEase (Ease.InElastic, TweenAmplitude, TweenPeriod)
                    .OnComplete (() => _image.sprite = null);
        }

        public void Align (Vector3 position) => _image.rectTransform.AlignToWorldPosition (position);
    }
}