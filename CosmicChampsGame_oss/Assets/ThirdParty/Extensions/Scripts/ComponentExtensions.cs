using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThirdParty.Extensions
{
    public static class ComponentExtensions
    {
        public static void SetParentVisible (this Component component, bool visible)
        {
            component
                .transform
                .parent
                .SetVisible (visible);
        }

        public static void SetVisible (this Component component, bool visible)
        {
            component
                .gameObject
                .SetActive (visible);
        }

        public static void SetInvisible (this Component component, bool invisible)
        {
            component
                .gameObject
                .SetActive (!invisible);
        }

        public static bool IsVisible (this Component component)
        {
            return component
                .gameObject
                .activeSelf;
        }

        public static T SetParent<T> (this T component, Component parent) where T : Component
        {
            component
                .transform
                .SetParent (parent.transform, false);

            return component;
        }

        public static T SetParent<T> (this T component, Component parent, int index) where T : Component
        {
            component
                .SetParent (parent.transform)
                .SetSiblingIndex (index);

            return component;
        }

        public static T SetSiblingIndex<T> (this T component, int index) where T : Component
        {
            component
                .transform
                .SetSiblingIndex (index);

            return component;
        }

        public static T SetAsLastSibling<T> (this T component) where T : Component
        {
            if (component == null)
                return component;

            var parent = component
                .transform
                .parent;

            if (parent == null)
                return component;

            component
                .transform
                .SetSiblingIndex (parent.childCount - 1);

            return component;
        }

        public static RectTransform GetRectTransform<T> (this T component) where T : Component
        {
            var rectTransform = component.transform as RectTransform;
            if (rectTransform == null)
                throw new InvalidOperationException ("Component doesn't contain RectTransform component");

            return rectTransform;
        }

        public static void DestroyAndReset<T> (ref T component) where T : Component
        {
            if (component == null)
                return;

            Object.Destroy (component);
            component = null;
        }
    }
}