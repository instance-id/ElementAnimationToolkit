#if UNITY_EDITOR
using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;


namespace instance.id.EATK.Extensions
{
    public static class UIElementsEditorHelper
    {
        const string animFold = "AnimatedFoldout";
        [UsedImplicitly] private static string imGUIPropNeedsRelayout;
#if !UNITY_2020_2_OR_NEWER
        private static ReorderableList reorderableList;
#endif

        public static string FillDefaultInspector(VisualElement container, SerializedObject serializedObject, bool hideScript, bool children = false)
        {
            var showChildObjects = children;

            var propPathArray = new[] {"m_Material.m_BelongsToCategories", "m_Material.m_CollidesWithCategories"};

            var property = serializedObject.GetIterator();
            if (property.NextVisible(true)) // Expand first child.
            {
                do
                {
                    showChildObjects = propPathArray.Contains(property.propertyPath) || children;
                    // if (showChildObjects && property.propertyPath != "m_Material.m_BelongsToCategories") continue;

                    if (property.propertyPath == "m_Script" && hideScript) continue;
                    var propPath = property.propertyPath;
                    var propertyField = new PropertyField(property) {name = "PropertyField:" + propPath};
                    propertyField.style.marginBottom = new StyleLength(2f);
                    propertyField.RegisterCallback<GeometryChangedEvent>(DeferredExecution);

                    switch (propPath)
                    {
                        case "m_Script" when serializedObject.targetObject != null:
                            propertyField.visible = true;
                            propertyField.SetEnabled(true);
                            break;
                        case "m_SerializedVersion" when serializedObject.targetObject != null:
                            propertyField.visible = false;
                            break;
                        default:
                            if (property.IsReallyArray() && serializedObject.targetObject != null)
                            {
                                var copiedProperty = property.Copy(); // @formatter:off
#if UNITY_2020_2_OR_NEWER
                                var imDefaultProperty = new IMGUIContainer(() =>
                                {
                                    DoDrawDefaultIMGUIProperty(serializedObject, copiedProperty);
                                }) {name = propPath, style = { marginBottom = new StyleLength(2f)}};
#else
                                reorderableList = new ReorderableList(serializedObject, copiedProperty)
                                {
                                    drawHeaderCallback = DrawHeaderCallback,
                                    drawElementCallback = DrawElementCallback,
                                    elementHeightCallback = ElementHeightCallback,
                                    onAddCallback = OnAddCallback
                                }; // @formatter:on

                                reorderableList.elementHeightCallback = ElementHeightCallback;

                                var imDefaultProperty = new IMGUIContainer(() =>
                                {
                                    serializedObject.Update();
                                    reorderableList.DoLayoutList();
                                    serializedObject.ApplyModifiedProperties();
                                }) {name = propPath};
#endif
                                imDefaultProperty.style.flexShrink = 0f;
                                imDefaultProperty.RegisterCallback((ChangeEvent<bool> evt) => RecomputeSize(imDefaultProperty));
                                container.Add(imDefaultProperty);
                            }
                            else container.Add(propertyField);

                            break; // @formatter:on
                    }
                } while (property.NextVisible(showChildObjects));
            }

            foreach (var foldoutList in container.Query<Foldout>().ToList())
            {
                Debug.Log($"Found Foldout: {foldoutList.name}");
                foldoutList.RegisterValueChangedCallback(e =>
                {
                    if (!(e.target is Foldout fd)) return;
                    var path = fd.bindingPath;
                    var c = container.Q<IMGUIContainer>(path);
                    RecomputeSize(c);
                });
            }

            serializedObject.ApplyModifiedProperties();
            container.Bind(serializedObject);
            return imGUIPropNeedsRelayout;
        }

        private static void DeferredExecution(GeometryChangedEvent evt)
        {
            ((VisualElement) evt.target).UnregisterCallback<GeometryChangedEvent>(DeferredExecution);

            var ve = ((VisualElement) evt.target);
            var toggles = ve.parent.Query<Foldout>().ToList();
            toggles.ForEach(
                toggle => toggle.RegisterCallback(
                    (ChangeEvent<bool> e) =>
                    {
                        var container = (Foldout) e.target;
                        imGUIPropNeedsRelayout = container.bindingPath;

                        var animFoldType = TypeCache.GetTypesDerivedFrom<VisualElement>()
                            .FirstOrDefault(x => x.Name == animFold);
                        if (animFoldType == null) return;

                        var animFoldOutParent = container.GetFirstAncestorOfType<VisualElement>(animFoldType);
                        if (animFoldOutParent == null) return;

                        var expander = animFoldOutParent.Query<VisualElement>("expander").First();
                        expander.InvokeMethod("TriggerExpanderResize", true);
                    }));
        }

        #region IMGUI
        #region 2020.2_OR_LESS
#if !UNITY_2020_2_OR_NEWER
        private static void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Header Settings");
        }

        private static void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            var elementName = element.FindPropertyRelative("m_Name");
            var elementTitle = string.IsNullOrEmpty(elementName.stringValue)
                ? "Add New Settings"
                : $"Settings: {elementName.stringValue}";

            EditorGUI.PropertyField(new Rect(
                    rect.x += 10, rect.y,
                    Screen.width * .8f,
                    EditorGUIUtility.singleLineHeight),
                element,
                new GUIContent(elementTitle), true);
        }

        private static float ElementHeightCallback(int index)
        {
            var propertyHeight =
                EditorGUI.GetPropertyHeight(reorderableList.serializedProperty.GetArrayElementAtIndex(index), true);

            var spacing = EditorGUIUtility.singleLineHeight / 2;
            return propertyHeight + spacing;
        }

        private static void OnAddCallback(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
        }
#endif
        #endregion

        public static string CheckNeedsResize(VisualElement root, string result)
        {
            Debug.Log($"Recomputing Size. CheckNeedsResize");
            imGUIPropNeedsRelayout = result;
            if (!string.IsNullOrEmpty(imGUIPropNeedsRelayout))
            {
                var container = root.Q<IMGUIContainer>(name: imGUIPropNeedsRelayout);
                RecomputeSize(container);
                result = imGUIPropNeedsRelayout = string.Empty;
            }

            return result;
        }

        public static void RecomputeSize(VisualElement container)
        {
            if (container == null) return;
            Debug.Log($"Recomputing Size. {container.name}");
            var parent = container.parent;
            container.RemoveFromHierarchy();
            parent.Add(container);
        }

        private static void DoDrawDefaultIMGUIProperty(SerializedObject serializedObj, SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();
            serializedObj.Update();
            var wasExpanded = property.isExpanded;
            EditorGUILayout.PropertyField(property, true);
            if (property.isExpanded != wasExpanded) imGUIPropNeedsRelayout = property.propertyPath;
            serializedObj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
        
        public static void UpdateRecompute(VisualElement root)
        {
            if (string.IsNullOrEmpty(imGUIPropNeedsRelayout)) return;

            var container = root.Q<VisualElement>(name: imGUIPropNeedsRelayout);
            if (container == null) return;
            imGUIPropNeedsRelayout = string.Empty;

            var animFoldType = TypeCache.GetTypesDerivedFrom<VisualElement>()
                .FirstOrDefault(x => x.Name == animFold);
            if (animFoldType == null) return;

            var animFoldOutParent = container.GetFirstAncestorOfType<VisualElement>(animFoldType);
            if (animFoldOutParent == null) return;

            var expander = animFoldOutParent.Query<VisualElement>("expander").First();
            expander.InvokeMethod("TriggerExpanderResize", true);
        }
        #endregion
    }
}
#endif
