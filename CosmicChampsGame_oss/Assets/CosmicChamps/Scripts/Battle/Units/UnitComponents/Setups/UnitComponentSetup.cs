using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CosmicChamps.Battle.Units.UnitComponents.Abstract;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Pool;
#endif

namespace CosmicChamps.Battle.Units.UnitComponents.Setups
{
    [Serializable]
    public abstract class UnitComponentSetup<T> where T : IUnitComponent<T>
    {
        #if UNITY_EDITOR
        public abstract class PropertyDrawer : UnityEditor.PropertyDrawer
        {
            private static Dictionary<Type, (Type[], (string, string)[])> _typesCache = new();

            private string[] _choices;

            private void ProcessInterfaceForConstants (Type type, List<(string, string)> constants)
            {
                foreach (var @interface in type.GetInterfaces ())
                {
                    ProcessInterfaceForConstants (@interface, constants);
                }

                constants.AddRange (
                    type
                        .GetFields (BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .Where (fi => fi.IsLiteral && !fi.IsInitOnly)
                        .Select (x => (x.Name, (string)x.GetRawConstantValue ())));
            }

            private int GetSubPropertiesCount (SerializedProperty property)
            {
                var count = 0;
                var enumerator = property.Copy ().GetEnumerator ();

                while (enumerator.MoveNext ())
                {
                    if (enumerator.Current is not SerializedProperty subProperty || subProperty.name == nameof (_type))
                        continue;

                    count += subProperty.hasVisibleChildren ? GetSubPropertiesCount (subProperty) : 1;
                }

                return count;
            }

            public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty (position, label, property);

                var componentType = typeof (T);
                var typeProperty = property.FindPropertyRelative (nameof (_type));

                if (!_typesCache.TryGetValue (componentType, out (Type[] types, (string name, string value)[] ids) typeData))
                {
                    var assembly = Assembly.GetAssembly (componentType);
                    var types = assembly
                        .GetTypes ()
                        .Where (x => x.IsClass && !x.IsAbstract && x.GetInterfaces ().Contains (componentType))
                        .ToArray ();

                    var ids = ListPool<(string, string)>.Get ();
                    ProcessInterfaceForConstants (componentType, ids);

                    _typesCache.Add (componentType, typeData = (types, ids.ToArray ()));

                    ListPool<(string, string)>.Release (ids);
                }

                if (_choices == null)
                    _choices = new string[typeData.types.Length + 1];
                else
                    Array.Resize (ref _choices, typeData.types.Length + 1);

                _choices[0] = "None";
                for (var i = 0; i < typeData.types.Length; i++)
                {
                    _choices[i + 1] = typeData.types[i].Name;
                }

                var currentIndex = string.IsNullOrEmpty (typeProperty.stringValue)
                    ? 0
                    : Array.FindIndex (typeData.types, x => x.FullName == typeProperty.stringValue) + 1;
                if (currentIndex < 0)
                    currentIndex = 0;

                position.height = EditorGUIUtility.singleLineHeight;
                var selectedIndex = EditorGUI.Popup (
                    position,
                    property.displayName.Replace ("setup", string.Empty, StringComparison.OrdinalIgnoreCase),
                    currentIndex,
                    _choices);

                if (selectedIndex < 0)
                    selectedIndex = 0;

                string managedReferenceFieldTypename;
                if (selectedIndex == 0)
                {
                    managedReferenceFieldTypename = typeProperty.stringValue = string.Empty;
                } else
                {
                    var certainType = typeData.types[selectedIndex - 1];
                    typeProperty.stringValue = certainType.FullName;
                    managedReferenceFieldTypename =
                        certainType.Assembly.GetName ().Name + " " + certainType.FullName.Replace ("+", "/");
                }

                var prototypeProperty = property.FindPropertyRelative (nameof (_prototype));
                typeProperty.stringValue = selectedIndex == 0 ? string.Empty : typeData.types[selectedIndex - 1].FullName;
                if (currentIndex == 0 && prototypeProperty.managedReferenceValue != null)
                    prototypeProperty.managedReferenceValue = null;

                if (currentIndex > 0 && prototypeProperty.managedReferenceFullTypename != managedReferenceFieldTypename)
                    prototypeProperty.managedReferenceValue = (T)Activator.CreateInstance (typeData.types[currentIndex - 1]);

                if (prototypeProperty.managedReferenceValue != null)
                {
                    EditorGUI.indentLevel++;

                    var enumerator = prototypeProperty.Copy ().GetEnumerator ();
                    while (enumerator.MoveNext ())
                    {
                        if (enumerator.Current is not SerializedProperty subProperty || subProperty.name == nameof (_type))
                            continue;

                        position.y += position.height;
                        position.height = EditorGUI.GetPropertyHeight (subProperty, true);

                        EditorGUI.PropertyField (position, subProperty, true);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.EndProperty ();
            }

            public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
            {
                var prototypeProperty = property.FindPropertyRelative (nameof (_prototype));
                var height = EditorGUIUtility.singleLineHeight;
                if (prototypeProperty.managedReferenceValue != null)
                    height += EditorGUIUtility.singleLineHeight * GetSubPropertiesCount (prototypeProperty);

                return height;
            }
        }
        #endif

        [SerializeField]
        private string _type;

        [SerializeReference]
        private T _prototype;

        public T Prototype => _prototype;
    }
}