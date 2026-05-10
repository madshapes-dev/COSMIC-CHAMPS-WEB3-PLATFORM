using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace ThirdParty.Extensions.Attributes
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        #if UNITY_EDITOR
        [CustomPropertyDrawer (typeof (ArrayElementTitleAttribute))]
        public class ArrayElementTitleDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight (
                SerializedProperty property,
                GUIContent label)
            {
                return EditorGUI.GetPropertyHeight (property, label, true);
            }

            protected virtual ArrayElementTitleAttribute Attribute => (ArrayElementTitleAttribute) attribute;

            private SerializedProperty titleNameProp;

            public override void OnGUI (
                Rect position,
                SerializedProperty property,
                GUIContent label)
            {
                var fullPathName = property.propertyPath + "." + Attribute.Varname;
                titleNameProp = property.serializedObject.FindProperty (fullPathName);
                var newLabel = GetTitle ();
                if (string.IsNullOrEmpty (newLabel))
                    newLabel = label.text;
                EditorGUI.PropertyField (position, property, new GUIContent (newLabel, label.tooltip), true);
            }

            private string GetTitle ()
            {
                switch (titleNameProp.propertyType)
                {
                    case SerializedPropertyType.Generic:
                        break;
                    case SerializedPropertyType.Integer:
                        return titleNameProp.intValue.ToString ();
                    case SerializedPropertyType.Boolean:
                        return titleNameProp.boolValue.ToString ();
                    case SerializedPropertyType.Float:
                        return titleNameProp.floatValue.ToString ();
                    case SerializedPropertyType.String:
                        return titleNameProp.stringValue;
                    case SerializedPropertyType.Color:
                        return titleNameProp.colorValue.ToString ();
                    case SerializedPropertyType.ObjectReference:
                        return titleNameProp.objectReferenceValue == null ? null : titleNameProp.objectReferenceValue.ToString ();
                    case SerializedPropertyType.LayerMask:
                        break;
                    case SerializedPropertyType.Enum:
                        return titleNameProp.enumNames[titleNameProp.enumValueIndex];
                    case SerializedPropertyType.Vector2:
                        return titleNameProp.vector2Value.ToString ();
                    case SerializedPropertyType.Vector3:
                        return titleNameProp.vector3Value.ToString ();
                    case SerializedPropertyType.Vector4:
                        return titleNameProp.vector4Value.ToString ();
                    case SerializedPropertyType.Rect:
                        break;
                    case SerializedPropertyType.ArraySize:
                        break;
                    case SerializedPropertyType.Character:
                        break;
                    case SerializedPropertyType.AnimationCurve:
                        break;
                    case SerializedPropertyType.Bounds:
                        break;
                    case SerializedPropertyType.Gradient:
                        break;
                    case SerializedPropertyType.Quaternion:
                        break;
                }

                return "";
            }
        }
        #endif

        public string Varname;

        public ArrayElementTitleAttribute (string ElementTitleVar)
        {
            Varname = ElementTitleVar;
        }
    }
}