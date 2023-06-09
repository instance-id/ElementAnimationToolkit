#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace instance.id.EATK
{
    // [CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
    class SerializedDictionaryDrawer : PropertyDrawer
    {
        /// <summary>
        /// One property drawer instance used for many different properties. 
        /// So keep a dictionary of drawers
        /// </summary>
        private Dictionary<int, ReorderableList> listDrawers = new Dictionary<int, ReorderableList>();

        private bool duplicateKeyMode;
        private (int, int) duplicateIndices;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded && listDrawers.TryGetValue(getHash(property), out var listDrawer))
                height += listDrawer.GetHeight();
            return height;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isEditorWindow = property.FindPropertyRelative("isCustomEditor").boolValue;

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                var targetObject = getTargetObject(property);
                if (targetObject == null) return;

                if (!listDrawers.TryGetValue(getHash(property), out var listDrawer))
                    listDrawers.Add(getHash(property), listDrawer = createListDrawer(property, targetObject, isEditorWindow));

                position.y += EditorGUIUtility.singleLineHeight;

                GUI.enabled = !duplicateKeyMode;
                listDrawer.DoList(position);

                if (duplicateKeyMode)
                {
                    GUI.enabled = true;
                    position.y += listDrawer.GetHeight() - EditorGUIUtility.singleLineHeight;
                    position.height = EditorGUIUtility.singleLineHeight;
                    position.xMax -= 80f;
                    EditorGUI.HelpBox(position, $"There are duplicate keys, please ensure they are unique!", MessageType.Error);
                }
            }
        }

        private ReorderableList createListDrawer(SerializedProperty property, object targetObject, bool isEditor = false)
        {
            var serializedDataProp = property.FindPropertyRelative(nameof(SerializedDictionary<int, int>.serializedData));
            var listDrawer = new ReorderableList(property.serializedObject, serializedDataProp, true, true, true, true);
            listDrawer.drawHeaderCallback += (rect) =>
            {
                var keyRect = rect;
                keyRect.x = rect.x + 15f;
                keyRect.width = rect.width / 2f - 4f;

                var valueRect = rect;
                valueRect.x = rect.width / 2f + 15f;
                valueRect.width = rect.width / 2f - 4f;

                GUI.Label(keyRect, "Keys");
                GUI.Label(valueRect, "Values");
            };
            listDrawer.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty kvpProp = serializedDataProp.GetArrayElementAtIndex(index);
                SerializedProperty keyProp = kvpProp.FindPropertyRelative(nameof(SerializedKeyValuePair<int, int>.Key));
                SerializedProperty valProp = kvpProp.FindPropertyRelative(nameof(SerializedKeyValuePair<int, int>.Value));

                GUIContent keyContent = new GUIContent(keyProp.hasVisibleChildren ? keyProp.displayName : string.Empty, keyProp.tooltip);
                GUIContent valContent = new GUIContent(valProp.hasVisibleChildren ? valProp.displayName : string.Empty, valProp.tooltip);

                var keyRect = rect;
                keyRect.y = rect.y + 1f;
                keyRect.x = rect.x - 10f;
                keyRect.width = rect.width / 2f - 18f;

                var valueRect = rect;
                valueRect.y = rect.y + 1f;

                if (isEditor)
                {
                    valueRect.x = rect.width / 2f - 8f;
                }
                else
                {
                    valueRect.x = rect.width / 2f + 8f;
                }

                if (valProp.type == "bool")
                    valueRect.x += 18f;

                valueRect.width = rect.width / 2f + 28f;

                //They have a foldout, move a bit
                if (keyProp.hasChildren)
                {
                    keyRect.x += 14f;
                    keyRect.width -= 14f;
                }

                if (valProp.hasChildren)
                {
                    valueRect.x += 18f;
                    valueRect.width -= 18f;
                }

                bool isDuplicate = duplicateKeyMode &&
                                   (duplicateIndices.Item1 == index ||
                                    duplicateIndices.Item2 == index);

                GUI.enabled = !duplicateKeyMode || isDuplicate;
                if (isDuplicate) GUI.color = Color.yellow;

                EditorGUIUtility.labelWidth = keyRect.width / 4f;
                EditorGUI.PropertyField(keyRect, keyProp, keyContent, true);

                GUI.enabled = !duplicateKeyMode;
                GUI.color = Color.white;

                EditorGUIUtility.labelWidth = valueRect.width / 4f;
                EditorGUI.PropertyField(valueRect, valProp, valContent, true);
            };
            listDrawer.onAddDropdownCallback += (Rect btn, ReorderableList l) =>
            {
                SerializedProperty editorAddKeyProp = property.FindPropertyRelative(nameof(SerializedDictionary<int, int>.editorAddKey)),
                    editorAddValProp = property.FindPropertyRelative(nameof(SerializedDictionary<int, int>.editorAddValue));

                PopupWindow.Show(btn, new AddElementDrawer(
                    editorAddKeyProp, editorAddValProp,
                    targetObject));
            };
            listDrawer.elementHeightCallback += (int index) =>
            {
                SerializedProperty kvpProp = serializedDataProp.GetArrayElementAtIndex(index);
                SerializedProperty keyProp = kvpProp.FindPropertyRelative(nameof(SerializedKeyValuePair<int, int>.Key));
                SerializedProperty valProp = kvpProp.FindPropertyRelative(nameof(SerializedKeyValuePair<int, int>.Value));

                return Mathf.Max(EditorGUI.GetPropertyHeight(keyProp), EditorGUI.GetPropertyHeight(valProp)) + EditorGUIUtility.standardVerticalSpacing;
            };
            return listDrawer;
        }


        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null) return null;

            var enm = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }

            return enm.Current;
        }

        //This should be a stable hash for each unique property on each object
        private int getHash(SerializedProperty prop)
        {
            int r = prop.propertyPath.GetHashCode();
            var p = getTargetObject(prop);
            if (p != null)
                r += p.GetHashCode();
            return r;
        }

        //based on https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs#L214
        private object getTargetObject(SerializedProperty property)
        {
            if (property == null) return null;

            var path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    int startBracketIndex = element.LastIndexOf('['),
                        endBracketIndex = element.LastIndexOf(']');

                    var elementName = element.Substring(0, startBracketIndex);
                    var index = int.Parse(element.Substring(startBracketIndex + 1, element.Length - endBracketIndex));

                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }
    }

    class AddElementDrawer : PopupWindowContent
    {
        private readonly SerializedProperty keyProp, valProp;
        private object targetObject;
        private readonly Vector2 minWindowSize;

        public AddElementDrawer(SerializedProperty keyProp, SerializedProperty valProp, object targetObject)
        {
            this.targetObject = targetObject;
            this.keyProp = keyProp;
            this.valProp = valProp;

            keyProp.isExpanded = valProp.isExpanded = true;

            minWindowSize = new Vector2(350,
                EditorGUI.GetPropertyHeight(keyProp) +
                EditorGUI.GetPropertyHeight(valProp) +
                EditorGUIUtility.singleLineHeight * 2);
        }

        //todo: better size
        public override Vector2 GetWindowSize() => Vector2.Max(minWindowSize, new Vector2(minWindowSize.x,
            EditorGUI.GetPropertyHeight(keyProp) +
            EditorGUI.GetPropertyHeight(valProp) +
            EditorGUIUtility.singleLineHeight * 2));

        public override void OnGUI(Rect rect)
        {
            EditorGUIUtility.labelWidth = 75f;
            GUI.SetNextControlName("Key");
            EditorGUILayout.PropertyField(keyProp, new GUIContent("Key", keyProp.tooltip), true);

            GUI.SetNextControlName("Value");
            EditorGUILayout.PropertyField(valProp, new GUIContent("Value", valProp.tooltip), true);

            //Important, we need to apply the propertyfields to the actual object before reading it out!
            keyProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            object key = ReflectionUtils.GetPrivate<object>(targetObject, nameof(SerializedDictionary<int, int>.editorAddKey)),
                value = ReflectionUtils.GetPrivate<object>(targetObject, nameof(SerializedDictionary<int, int>.editorAddValue));

            bool isDuplicateKey = key != null && ReflectionUtils.CallPublic<bool>(targetObject, nameof(SerializedDictionary<int, int>.ContainsKey), key);

            EditorGUILayout.Space();
            GUI.enabled = !isDuplicateKey;

            if (isDuplicateKey)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;

            if (GUILayout.Button("Add") ||
                (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)) //add clicked or enter pressed
            {
                ReflectionUtils.CallPublic(targetObject, nameof(SerializedDictionary<int, int>.Add), key, value);

                editorWindow.Close();
            }

            GUI.enabled = true;

            if (firstOnGUI)
            {
                firstOnGUI = false;
                GUI.FocusControl("Key");
            }
        }

        private bool firstOnGUI;

        public override void OnOpen()
        {
            firstOnGUI = true;
        }

        public override void OnClose() { }
    }

    internal static class ReflectionUtils
    {
        internal static T GetPrivate<T>(object obj, string name) => (T) obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        internal static T CallPublic<T>(object obj, string name, params object[] args) => (T) obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.Instance).Invoke(obj, args);
        internal static void CallPublic(object obj, string name, params object[] args) => obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.Instance).Invoke(obj, args);

        internal static string DumpObjectProps(SerializedProperty prop)
        {
            var sb = new StringBuilder();
            sb.Append($"{prop.name}:\n");
            prop.Next(true);

            do
            {
                for (int i = 0; i < prop.depth; i++)
                    sb.Append('\t');
                sb.Append($"{prop.name} {prop.isArray} ({prop.type})\n");
            } while (prop.Next(true));

            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Type GetCallersType()
        {
            StackTrace stackTrace = new StackTrace(1, false);
            return stackTrace.GetFrame(1).GetMethod().DeclaringType;
        }
    }
}

#endif
