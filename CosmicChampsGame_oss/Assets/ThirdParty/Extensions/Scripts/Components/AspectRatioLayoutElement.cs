using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThirdParty.Extensions.Components
{
    public class AspectRatioLayoutElement : LayoutElement
    {
        #if UNITY_EDITOR
        [CustomEditor (typeof (AspectRatioLayoutElement), true)]
        [CanEditMultipleObjects]
        public sealed class AspectRatioLayoutElementEditor : Editor
        {
            private SerializedProperty _ignoreLayout;
            private SerializedProperty _minWidth;
            private SerializedProperty _minHeight;
            private SerializedProperty _preferredWidth;
            private SerializedProperty _preferredHeight;
            private SerializedProperty _flexibleWidth;
            private SerializedProperty _flexibleHeight;
            private SerializedProperty _layoutPriority;
            private SerializedProperty _aspectMode;
            private SerializedProperty _aspectRatio;            

            private void OnEnable ()
            {
                _ignoreLayout = serializedObject.FindProperty("m_IgnoreLayout");
                _minWidth = serializedObject.FindProperty("m_MinWidth");
                _minHeight = serializedObject.FindProperty("m_MinHeight");
                _preferredWidth = serializedObject.FindProperty("m_PreferredWidth");
                _preferredHeight = serializedObject.FindProperty("m_PreferredHeight");
                _flexibleWidth = serializedObject.FindProperty("m_FlexibleWidth");
                _flexibleHeight = serializedObject.FindProperty("m_FlexibleHeight");
                _layoutPriority = serializedObject.FindProperty("m_LayoutPriority");
                _aspectMode = serializedObject.FindProperty ("_aspectMode");
                _aspectRatio = serializedObject.FindProperty ("_aspectRatio");
            }

            public override void OnInspectorGUI ()
            {
                serializedObject.Update ();

                EditorGUILayout.PropertyField (_ignoreLayout);

                if (!_ignoreLayout.boolValue)
                {
                    EditorGUILayout.Space ();

                    EditorGUILayout.PropertyField (_aspectMode);
                    EditorGUILayout.PropertyField (_aspectRatio);
                    
                    EditorGUILayout.Space ();

                    LayoutElementField (_minWidth, 0);
                    LayoutElementField (_minHeight, 0);
                    LayoutElementField (_preferredWidth, t => t.rect.width);
                    LayoutElementField (_preferredHeight, t => t.rect.height);
                    LayoutElementField (_flexibleWidth, 1);
                    LayoutElementField (_flexibleHeight, 1);
                }

                EditorGUILayout.PropertyField (_layoutPriority);

                serializedObject.ApplyModifiedProperties ();
            }

            private void LayoutElementField (SerializedProperty property, float defaultValue)
            {
                LayoutElementField (property, _ => defaultValue);
            }

            private void LayoutElementField (SerializedProperty property, System.Func<RectTransform, float> defaultValue)
            {
                var position = EditorGUILayout.GetControlRect ();

                // Label
                var label = EditorGUI.BeginProperty (position, null, property);

                // Rects
                var fieldPosition = EditorGUI.PrefixLabel (position, label);

                var toggleRect = fieldPosition;
                toggleRect.width = 16;

                var floatFieldRect = fieldPosition;
                floatFieldRect.xMin += 16;

                // Checkbox
                EditorGUI.BeginChangeCheck ();
                var enabled = EditorGUI.ToggleLeft (toggleRect, GUIContent.none, property.floatValue >= 0);
                if (EditorGUI.EndChangeCheck ())
                {
                    property.floatValue = (enabled ? defaultValue ((target as LayoutElement).transform as RectTransform) : -1);
                }

                if (!property.hasMultipleDifferentValues && property.floatValue >= 0)
                {
                    EditorGUIUtility.labelWidth = 4; // Small invisible label area for drag zone functionality
                    EditorGUI.BeginChangeCheck ();
                    var newValue = EditorGUI.FloatField (floatFieldRect, new GUIContent (" "), property.floatValue);
                    if (EditorGUI.EndChangeCheck ())
                    {
                        property.floatValue = Mathf.Max (0, newValue);
                    }

                    EditorGUIUtility.labelWidth = 0;
                }

                EditorGUI.EndProperty ();
            }
        }
        #endif

        private enum AspectMode
        {
            None,
            WidthControlsHeight,
            HeightControlsWidth,
        }

        [SerializeField]
        private AspectMode _aspectMode;

        [SerializeField]
        private float _aspectRatio;

        private RectTransform RectTransform => transform as RectTransform;

        public override float minWidth
        {
            get => _aspectMode == AspectMode.HeightControlsWidth
                ? RectTransform.rect.height / _aspectRatio
                : base.minWidth;
            set => base.minWidth = value;
        }

        public override float minHeight
        {
            get => _aspectMode == AspectMode.WidthControlsHeight
                ? RectTransform.rect.width * _aspectRatio
                : base.minHeight;
            set => base.minHeight = value;
        }

        public override float preferredWidth
        {
            get => _aspectMode == AspectMode.HeightControlsWidth
                ? RectTransform.rect.height / _aspectRatio
                : base.preferredWidth;
            set => base.preferredWidth = value;
        }

        public override float preferredHeight
        {
            get => _aspectMode == AspectMode.WidthControlsHeight
                ? RectTransform.rect.width * _aspectRatio
                : base.preferredHeight;
            set => base.preferredHeight = value;
        }
    }
}