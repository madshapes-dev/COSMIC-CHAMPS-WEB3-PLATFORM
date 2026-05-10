using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace ThirdParty.Extensions
{
    public static class TextMeshProUGUIExtensions
    {
        private static readonly StringBuilder dotsStringBuilder = new();
        private const char Dot = '.';
        private const string Transparent = "<alpha=#00>";

        private static string GetLoadingDots (long tick)
        {
            dotsStringBuilder.Length = 0;
            for (var i = 0; i < tick % 4; i++)
            {
                dotsStringBuilder.Append (Dot);
            }

            dotsStringBuilder.Append (Transparent);
            for (var i = 0; i < 3 - tick % 4; i++)
            {
                dotsStringBuilder.Append (Dot);
            }

            return dotsStringBuilder.ToString ();
        }

        public static IDisposable AnimateTextWithEndingDots (this TextMeshProUGUI textMeshProUgui, string text)
        {
            void RefreshText (long tick = 0)
            {
                textMeshProUgui.text = text + GetLoadingDots (tick);
            }

            RefreshText ();

            var timerInterval = TimeSpan.FromMilliseconds (350);
            return Observable
                .Timer (timerInterval, timerInterval)
                .Subscribe (RefreshText);
        }

        public static Tween AnimateTextChangeThroughFade (
            this TextMeshProUGUI textMeshPro,
            string text,
            float duration = 0.2f)
        {
            if (textMeshPro.text == text)
                return null;

            var stepDuration = duration / 2f;

            textMeshPro.DOKill ();

            return DOTween
                .Sequence ()
                .Append (textMeshPro.DOFade (0f, stepDuration))
                .AppendCallback (() => textMeshPro.text = text)
                .Append (textMeshPro.DOFade (1f, stepDuration))
                .SetId (textMeshPro);
        }

        public static Tween AnimateTextChangeThroughRotation (
            this TextMeshProUGUI textMeshPro,
            string text,
            float duration = 0.2f,
            float pulseScale = 1f)
        {
            if (textMeshPro.text == text)
                return null;

            var stepDuration = duration / 5f;

            textMeshPro.DOKill ();

            return DOTween
                .Sequence ()
                .Append (textMeshPro.transform.DOLocalRotate (Vector3.zero.WithY (90f), stepDuration))
                .AppendCallback (() => textMeshPro.text = text)
                .Append (textMeshPro.transform.DOLocalRotate (Vector3.zero, stepDuration))
                .AppendInterval (stepDuration)
                .Append (textMeshPro.transform.DOScale (pulseScale, stepDuration))
                .Append (textMeshPro.transform.DOScale (1f, stepDuration))
                .SetId (textMeshPro);
        }

        public static void FadeInTextUntilInputChanged (
            this TextMeshProUGUI textMeshProUGUI,
            string text,
            float duration,
            params TMP_InputField[] inputFields)
        {
            FadeInTextUntilInputChanged (
                textMeshProUGUI,
                text,
                duration,
                Array.ConvertAll (inputFields, x => x.OnValueChangedAsUnitObservable ()));
        }

        public static void FadeInTextUntilInputChanged (
            this TextMeshProUGUI textMeshProUGUI,
            string text,
            float duration,
            params IObservable<Unit>[] resetObservables)
        {
            textMeshProUGUI.text = text;
            FadeInTextUntilInputChanged (textMeshProUGUI, duration, resetObservables);
        }

        public static void FadeInTextUntilInputChanged (
            this TextMeshProUGUI textMeshProUGUI,
            float duration,
            params TMP_InputField[] inputFields)
        {
            FadeInTextUntilInputChanged (
                textMeshProUGUI,
                duration,
                Array.ConvertAll (inputFields, x => x.OnValueChangedAsUnitObservable ()));
        }

        public static void FadeInTextUntilInputChanged (
            this TextMeshProUGUI textMeshProUGUI,
            float duration,
            params IObservable<Unit>[] resetObservables)
        {
            textMeshProUGUI.DOKill ();
            textMeshProUGUI.DOFade (1f, duration);

            resetObservables
                .Merge ()
                .Take (1)
                .Subscribe (_ => textMeshProUGUI.DOFade (0f, duration));
        }

        public static void FadeOutTextAndResetText (this TextMeshProUGUI textMeshProUGUI, float duration)
        {
            textMeshProUGUI.DOKill ();
            textMeshProUGUI
                .DOFade (0f, duration)
                .onComplete += () => textMeshProUGUI.text = null;
        }
    }
}