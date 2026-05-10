using UnityEngine;
using UnityEngine.EventSystems;

namespace ThirdParty.Extensions
{
    public static class PointerEventDataExtensions
    {
        public static Vector2 ScreenPositionToLocalPointInRectangle (this PointerEventData pointerEventData, RectTransform rect)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                rect,
                pointerEventData.position,
                null,
                out var position);

            return position;
        }

        public static Vector2 ScreenDeltaToLocalPointInRectangle (this PointerEventData pointerEventData, RectTransform rect)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                rect,
                pointerEventData.position,
                null,
                out var current);

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                rect,
                pointerEventData.position - pointerEventData.delta,
                null,
                out var previous);

            return current - previous;
        }

        public static Vector2 ScreenPositionToLocalPointForRectangle (this PointerEventData pointerEventData, RectTransform rect)
        {
            var parent = rect.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                parent,
                pointerEventData.position,
                null,
                out var position);

            return position;
        }

        public static Vector2 ScreenPressDeltaToLocalPointForRectangle (
            this PointerEventData pointerEventData,
            RectTransform rect)
        {
            var parent = rect.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                parent,
                pointerEventData.position,
                null,
                out var current);

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                parent,
                pointerEventData.pressPosition,
                null,
                out var previous);

            return current - previous;
        }

        public static Vector2 ScreenDeltaToLocalPointForRectangle (this PointerEventData pointerEventData, RectTransform rect)
        {
            var parent = rect.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                parent,
                pointerEventData.position,
                null,
                out var current);

            RectTransformUtility.ScreenPointToLocalPointInRectangle (
                parent,
                pointerEventData.position - pointerEventData.delta,
                null,
                out var previous);

            return current - previous;
        }
    }
}