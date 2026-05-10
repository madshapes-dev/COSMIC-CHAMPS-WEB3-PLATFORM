using DG.Tweening;
using TMPro;

namespace ThirdParty.Extensions
{
    public static class DOTweenModuleTextMeshPRO
    {
        public static Tween DOText (this TextMeshProUGUI target, string text, float duration)
        {
            return DOTween
                .Sequence ()
                .Append (target.DOFade (0f, duration / 2f))
                .AppendCallback (() => target.text = text)
                .Append (target.DOFade (1f, duration / 2f));
        }
    }
}