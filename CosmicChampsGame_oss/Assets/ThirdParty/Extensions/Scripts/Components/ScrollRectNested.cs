using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace ThirdParty.Extensions.Components
{
    public class ScrollRectNested : ScrollRect
    {
        #if UNITY_EDITOR
        [CustomEditor (typeof (ScrollRectNested))]
        public class ScrollRectNestedEditor : ScrollRectEditor
        {
            private readonly GUIContent parentScrollRectGUIContent = new("Parent ScrollRect");
            private SerializedProperty parentScrollRectProp;

            protected override void OnEnable ()
            {
                base.OnEnable ();
                parentScrollRectProp = serializedObject.FindProperty ("parentScrollRect");
            }

            public override void OnInspectorGUI ()
            {
                base.OnInspectorGUI ();
                serializedObject.Update ();
                EditorGUILayout.PropertyField (parentScrollRectProp, parentScrollRectGUIContent);
                serializedObject.ApplyModifiedProperties ();
            }
        }
        #endif
        
        [Header ("Additional Fields")]
        [SerializeField]
        private ScrollRect _parentScrollRect;

        private bool routeToParent;

        public override void OnInitializePotentialDrag (PointerEventData eventData)
        {
            // Always route initialize potential drag event to parent
            if (_parentScrollRect != null)
            {
                ((IInitializePotentialDragHandler) _parentScrollRect).OnInitializePotentialDrag (eventData);
            }

            base.OnInitializePotentialDrag (eventData);
        }

        public override void OnDrag (PointerEventData eventData)
        {
            if (routeToParent)
            {
                if (_parentScrollRect != null)
                {
                    ((IDragHandler) _parentScrollRect).OnDrag (eventData);
                }
            } else
            {
                base.OnDrag (eventData);
            }
        }

        public override void OnBeginDrag (PointerEventData eventData)
        {
            if (!horizontal && Math.Abs (eventData.delta.x) > Math.Abs (eventData.delta.y))
            {
                routeToParent = true;
            } else if (!vertical && Math.Abs (eventData.delta.x) < Math.Abs (eventData.delta.y))
            {
                routeToParent = true;
            } else
            {
                routeToParent = false;
            }

            if (routeToParent)
            {
                if (_parentScrollRect != null)
                {
                    ((IBeginDragHandler) _parentScrollRect).OnBeginDrag (eventData);
                }
            } else
            {
                base.OnBeginDrag (eventData);
            }
        }

        public override void OnEndDrag (PointerEventData eventData)
        {
            if (routeToParent)
            {
                if (_parentScrollRect != null)
                {
                    ((IEndDragHandler) _parentScrollRect).OnEndDrag (eventData);
                }
            } else
            {
                base.OnEndDrag (eventData);
            }

            routeToParent = false;
        }

        public void SetParentScrollRect (ScrollRect scrollRect) => _parentScrollRect = scrollRect;
    }
}