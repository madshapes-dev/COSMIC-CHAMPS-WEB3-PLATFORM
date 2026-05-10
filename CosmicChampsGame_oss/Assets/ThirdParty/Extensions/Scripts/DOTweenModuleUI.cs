using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdParty.Extensions
{
    public static class DOTweenModuleUI
    {
        private static readonly int _blendFromTex = Shader.PropertyToID ("_BlendFromTex");
        private static readonly int _blendValue = Shader.PropertyToID ("_BlendValue");

        public static readonly Sprite TransparentPixelSprite;

        static DOTweenModuleUI ()
        {
            var transparentPixelTexture = new Texture2D (1, 1);
            transparentPixelTexture.SetPixel (0, 0, Color.clear);

            TransparentPixelSprite = Sprite.Create (transparentPixelTexture, new Rect (0f, 0f, 1f, 1f), Vector2.one / 2f);
        }


        public static Tweener DOSpriteFade (this Image target, Sprite sprite, float duration = 0.2f)
        {
            if (target.sprite == sprite)
                return null;

            if (!target.material.shader.name.StartsWith ("Blendable Sprite"))
                target.material = new Material (Shader.Find (Shaders.BlendableSpriteShader));

            target.material.SetTexture (_blendFromTex, target.sprite.texture);
            target.material.SetFloat (_blendValue, 0f);
            target.sprite = sprite;

            var mask = target.GetComponentInParent<Mask> ();

            return DOTween
                .To (
                    x =>
                    {
                        target.material.SetFloat (_blendValue, x);
                        if (mask != null)
                            MaskUtilities.NotifyStencilStateChanged (mask);
                    },
                    0f,
                    1f,
                    duration)
                .SetId (target);
        }

        public static Tween DoSpriteFlip (this Image target, Sprite sprite, float scale, float duration = 0.2f)
        {
            if (target.sprite == sprite)
                return null;

            var initialScale = target.transform.localScale.x;

            return DOTween
                .Sequence ()
                .Append (target.transform.DOLocalRotate (Vector3.zero.WithY (90f), duration / 3f))
                .AppendCallback (() => target.sprite = sprite)
                .Append (target.transform.DOLocalRotate (Vector3.zero, duration / 3f))
                .Append (target.transform.DOScale (scale, duration / 6f))
                .Append (target.transform.DOScale (initialScale, duration / 6f))
                .SetId (target);
        }

        public static Tween DoSpriteFlip (this Image target, Sprite sprite, float duration = 0.2f)
        {
            return DoSpriteFlip (target, sprite, 1.2f, duration);
        }
    }
}