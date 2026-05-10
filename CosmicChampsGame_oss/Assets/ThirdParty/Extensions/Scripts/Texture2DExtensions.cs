using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class Texture2DExtensions
    {
        public static Sprite ToSprite (this Texture2D texture2D) => texture2D == null
            ? null
            : Sprite.Create (
                texture2D,
                new Rect (0f, 0f, texture2D.width, texture2D.height),
                Vector2.one * 0.5f);
    }
}