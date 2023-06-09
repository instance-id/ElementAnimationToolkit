#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace instance.id.EATK.Extensions
{
    public class IMGuiObject : IMGUIContainer
    {
        public VisualElement imGuiContainerParent;
        public IMGUIContainer imGUIContainer;
        public SerializedObject serializedObject;
        public SerializedProperty serializedProperty;
        public Action hasChanged;
        private int currentSize;
        public string labelText;

        public IMGuiObject(SerializedObject serializedObject, SerializedProperty serializedProperty, string[] elementClasses = default, string labelText = default, bool useParent = false)
        {
            this.AddToClassList("imguiObject");
            this.labelText = labelText;
            this.serializedObject = serializedObject;
            this.serializedProperty = serializedProperty;
            imGuiContainerParent = new VisualElement();
            imGUIContainer = new IMGUIContainer(() =>
            {
                try
                {
                    DoDrawDefaultIMGUIProperty(this.serializedProperty);
                }
#pragma warning disable CS0168
                catch (InvalidOperationException e) { }
#pragma warning restore CS0168
            }).ToUSS(this.serializedProperty.name, "collectionContainer", "imguiContainer");
            
            if (elementClasses != default)
                foreach (var elementClass in elementClasses)
                {
                    if (useParent) imGuiContainerParent.AddToClassList(elementClass);
                    else imGUIContainer.AddToClassList(elementClass);
                }
            
            if (useParent)
            {
                imGUIContainer.SetParent(imGuiContainerParent);
                Add(imGuiContainerParent);
                imGuiContainerParent.Bind(this.serializedObject);
            }
            else
            {
                imGUIContainer.Bind(this.serializedObject);
                Add(imGUIContainer);
            }
            
            this.Bind(this.serializedObject);
        }

        private void DoDrawDefaultIMGUIProperty(SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            var isArray = property.IsReallyArray();
            bool wasExpanded = false;
            if (isArray) wasExpanded = property.isExpanded;

            if (labelText != default) EditorGUILayout.PropertyField(property, new GUIContent(labelText), true);
            else EditorGUILayout.PropertyField(property, true);

            if (isArray)
            {
                if (property.arraySize != currentSize)
                {
                    currentSize = property.arraySize;
                    hasChanged?.Invoke();
                }
                if (property.isExpanded != wasExpanded) hasChanged?.Invoke();
            }

            // if (property.type.Contains("SerializedDictionary")) 
            // property.FindPropertyRelative("isCustomEditor").boolValue = true;

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
