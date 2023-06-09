#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;
using UnityObject = UnityEngine.Object;
// ReSharper disable RedundantExplicitArrayCreation

// ReSharper disable ConvertTypeCheckPatternToNullCheck
// ReSharper disable MergeIntoPattern

namespace instance.id.EATK.Extensions
{
    public static class ElementGeneration
    {
        /// <summary>
        /// Generate UIElements Editor Window 
        /// </summary>
        /// <param name="classData"></param>
        /// <param name="labelMinWidth"></param>
        /// <param name="marginLeft"></param>
        /// <param name="animated"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static VisualElement GenerateElements<T>(this T classData, float labelMinWidth, float marginLeft = 0, bool animated = true, bool performanceTimer = false) where T : UnityObject
        {
            Timer timer = new Timer("GenerateElements", typeof(ElementGeneration), true);
            timer.Start("TotalTime");

            var serializedObject = new SerializedObject(classData as UnityObject);

            bool isEditorWindow = false;
            var callerType = Helper.GetCallersType();
            if (typeof(EditorWindow).IsSubclassOf(callerType) || typeof(EditorWindow).IsAssignableFrom(callerType))
                isEditorWindow = true;

            new VisualElement().Create(out var generatedElements).ToUSS(nameof(generatedElements));
            try
            {
                Dictionary<(int, string), VisualElement> containers = new Dictionary<(int, string), VisualElement>();
                Dictionary<Type, VisualElement> containerTypes = new Dictionary<Type, VisualElement>();
                Dictionary<int, string> uniqueContainers = new Dictionary<int, string>();

                var attributes = classData.GetEditorAttributes<EditorFieldAttribute>();

                timer.Toggle("CreateFoldouts");
                foreach (var v in attributes.fieldDatas)
                {
                    var data = v.Value.attributeData.container;
                    var containerName = data.containerClass;

                    if (uniqueContainers.ContainsKey(data.containerId)) continue;
                    uniqueContainers.Add(data.containerId, containerName);
                    if (containers.ContainsKey((data.containerId, containerName))) continue;

                    switch (data.containerType)
                    {
                        case ContainerType.AnimatedFoldout:
                            containers.TryAdd((data.containerId, containerName), CreateAnimatedFoldout(data, animated));
                            containerTypes.TryAdd(typeof(AnimatedFoldout), containers[(data.containerId, containerName)]);
                            break;
                        case ContainerType.Foldout:
                            containers.TryAdd((data.containerId, containerName), CreateFoldout(data));
                            containerTypes.TryAdd(typeof(Foldout), containers[(data.containerId, containerName)]);
                            break;
                        case ContainerType.Box:
                            containers.TryAdd((data.containerId, containerName), CreateBox(data));
                            containerTypes.TryAdd(typeof(Box), containers[(data.containerId, containerName)]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                timer.Toggle("CreateFoldouts");

                timer.Toggle("EditorFields");

                string tmpContainerName = "editorField";
                foreach (var v in attributes.fieldDatas)
                {
                    var data = v.Value.attributeData.container;
                    if (data is {containerClass: { }} && !string.IsNullOrEmpty(data.containerClass))
                        tmpContainerName = data.containerClass;

                    var chosenContainer = uniqueContainers.TryGetVal(data.containerId);
                    var elementClasses = new[]
                    {
                        $"{CleanName(v.Value.name)}Field",
                        $"{tmpContainerName}Field",
                        "editorField"
                    };

                    var parent =
                        Enumerable.FirstOrDefault(containers, x => x.Key == (data.containerId, chosenContainer));

                    var field = DetermineType(v.Value, classData, serializedObject);

                    if (field is PropertyField)
                        elementClasses = elementClasses.Concat(new[] {"propertyField"}).ToArray();

                    void Resize(VisualElement fieldElement)
                    {
                        var animFoldOutParent = fieldElement.GetFirstAncestorOfType<Expander>();
                        if (animFoldOutParent == null) return;

                        animFoldOutParent.TriggerExpanderResize();
                    }

                    if (field is IMGUIContainer)
                    {
                        field.Create(out v.Value.attributeData.element,
                            name: CleanName(v.Value.name)
                        ).ToUSS($"{CleanName(v.Value.name)}Container").SetParent(parent.Value);
                        ((IMGuiObject)field).hasChanged -= () => Resize(field);
                        ((IMGuiObject)field).hasChanged += () => Resize(field);
                    }
                    else
                    {
                        field.CreateWithLabel(out v.Value.attributeData.element,
                            name: CleanName(v.Value.name),
                            labelText: v.Value.attributeData.description,
                            labelMinWidth: labelMinWidth,
                            elementClasses: elementClasses
                        ).ToUSS($"{CleanName(v.Value.name)}Container").SetParent(parent.Value);
                    }
                } // @formatter:off
                timer.Toggle("EditorFields");

                timer.Toggle("Queries");
                containers.forEach(x => { generatedElements.Add(x.Value); });
                containerTypes.forEach(x =>
                {
                    if (x.Key == typeof(AnimatedFoldout)){
                        generatedElements.Query<AnimatedFoldout>()
                            .Descendents<VisualElement>("expandContainer").ToList().ForEach(
                                z => z.style.marginLeft = marginLeft
                            );
                    }
                });
     
                generatedElements.Query<PropertyField>().ToList().ForEach(pf =>
                {
                    pf.style.flexShrink = 0;
                });
                    
                generatedElements.Query<VisualElement>(className:"expandToggleContainerHeader").ForEach( v =>
                {
                    v.style.marginLeft = isEditorWindow ? 0 : 0;
                });
                timer.Toggle("Queries");
            } 
            catch (Exception e) { UnityEngine.Debug.Log(e.ToString()); throw; }
            finally
            {
                timer.Stop("TotalTime");
                
                if (performanceTimer)
                    timer.PrintAll();
            }
            
            generatedElements.Bind(serializedObject);
            return generatedElements;  // @formatter:on
        }

        // --| Create Container Elements -----------------------
        private static VisualElement CreateAnimatedFoldout(ContainerData data, bool animated = true) =>
            new AnimatedFoldout {text = data.containerName, value = data.foldoutOpen, disableAnimation = !animated}.Create(out data.element).ToUSS(data.containerClass);

        private static VisualElement CreateFoldout(ContainerData data) =>
            new Foldout {text = data.containerName, value = data.foldoutOpen}.Create(out data.element).ToUSS(data.containerClass);

        private static VisualElement CreateBox(ContainerData data) =>
            new Box {name = data.containerName}.Create(out data.element).ToUSS(data.containerClass);

        // --| Determine Container Type ------------------------
        private static VisualElement DetermineType(FieldData<EditorFieldAttribute> fieldData, object classData, SerializedObject serializedObject)
        {
            var nameArray = new string[] {"ScriptableObjectCollectionItem"};
            var ruleCollectionName = "IRuleEngine";

            var interfaceCache = TypeCache.GetTypesDerivedFrom<object>().Where(x => x.IsInterface && x.Name == ruleCollectionName).ToList();
            var typeList = TypeCache.GetTypesDerivedFrom<UnityObject>().ToList();
            var typeCollection = typeList.Where(x => nameArray.Contains(x.Name)).ToList();

            var ruleType = typeList.FirstOrDefault(x => x.Name == ruleCollectionName);

            // var serializedObject = new SerializedObject(classData as UnityObject);
            var property = serializedObject.FindProperty(fieldData.fieldInfo.Name);

            var value = fieldData.fieldInfo.GetValue(classData);
            var fType = fieldData.fieldInfo.FieldType;
            switch (fType)
            {
                // --| Custom Types ---------------------------------
                case Type t when interfaceCache.Contains(t):
                    return new PropertyField(property, " ") {bindingPath = property.propertyPath}
                        .Create(out var ruleCollectionProperty)
                        .ToUSS(nameof(ruleCollectionProperty));

                case Type t when typeCollection.Contains(t):
                case Type a when a == typeCollection.First():
                case Type b when typeCollection.First().IsSubclassOf(b):
                case Type c when typeCollection.First().IsAssignableFrom(c):
                    return new IMGuiObject(serializedObject, property, new string[] {"imguiSOCollection"});

                // --| Default Types --------------------------------
                case Type t when t == typeof(long):
                    return new LongField {value = (long)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<LongField, ChangeEvent<long>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(int):
                    return new IntegerField {value = (int)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<IntegerField, ChangeEvent<int>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(bool):
                    return new Toggle {value = (bool)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<Toggle, ChangeEvent<bool>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(float):
                    return new FloatField {value = (float)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<FloatField, ChangeEvent<float>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(double):
                    return new DoubleField {value = (double)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<DoubleField, ChangeEvent<double>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(string):
                    return new TextField {value = (string)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<TextField, ChangeEvent<string>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Vector2):
                    return new Vector2Field {value = (Vector2)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<Vector2Field, ChangeEvent<Vector2>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Vector3):
                    return new Vector3Field {value = (Vector3)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<Vector3Field, ChangeEvent<Vector3>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Vector4):
                    return new Vector4Field {value = (Vector4)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<Vector4Field, ChangeEvent<Vector4>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Color):
                    return new ColorField {value = (Color)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<ColorField, ChangeEvent<Color>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(ScriptableObject):
                case Type b when typeof(ScriptableObject).IsSubclassOf(b):
                case Type c when typeof(ScriptableObject).IsAssignableFrom(c):
                    return new PropertyField(property, " ") {name = fieldData.fieldInfo.Name, style = {flexGrow = 1, flexShrink = 0}, label = " ", bindingPath = property.propertyPath}
                        .RegisterValueChangeCallback<PropertyField, ChangeEvent<SerializedPropertyChangeEvent>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(UnityObject):
                case Type b when typeof(UnityObject).IsSubclassOf(b):
                case Type c when typeof(UnityObject).IsAssignableFrom(c):
                    Type ty = fieldData.fieldInfo.FieldType;
                    return new ObjectField {value = (UnityObject)value, objectType = ty, style = {flexGrow = 1}}
                        .RegisterValueCallback<ObjectField, ChangeEvent<UnityObject>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t.IsEnum:
                    var e = value.CreateEnumField(
                        onChange: val => fieldData.fieldInfo.SetValue(classData, val),
                        defaultValue: fieldData.fieldInfo.GetValue(classData)
                    );
                    e.style.flexGrow = 1;
                    return e;
                case Type t when t == typeof(Version):
                    return new VersionElement {value = (Version)value, style = {flexGrow = 1}}
                        .RegisterValueCallback<VersionElement, ChangeEvent<Version>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                
            }

            if (fieldData.fieldInfo.FieldType.IsArray)
                // return new PropertyField(property, " ") {name = fieldData.fieldInfo.Name, style = {flexGrow = 1, flexShrink = 0}, label = " ", bindingPath = property.propertyPath}
                return new IMGuiObject(serializedObject, property, new string[] {"imguiArrayElement"}).RegisterValueChangeCallback<IMGuiObject, ChangeEvent<SerializedPropertyChangeEvent>>(
                    evt =>
                    {
                        Debug.Log($"{((VisualElement)evt.target).name} {evt.newValue}");
                        fieldData.fieldInfo.SetValue(classData, evt.newValue);
                    });

            if (typeof(System.Collections.IList).IsAssignableFrom(fieldData.fieldInfo.FieldType))
                return new IMGuiObject(serializedObject, property, new string[] {"imguiListElement"}).RegisterValueChangeCallback<IMGuiObject, ChangeEvent<SerializedPropertyChangeEvent>>(
                    evt =>
                    {
                        Debug.Log($"{((VisualElement)evt.target).name} {evt.newValue}");
                        fieldData.fieldInfo.SetValue(classData, evt.newValue);
                    });

            //     .RegisterValueChangeCallback<IMGuiObject, ChangeEvent<SerializedPropertyChangeEvent>>(
            // evt =>
            // {
            //     Debug.Log($"{((VisualElement)evt.target).name} {evt.newValue}");
            //     fieldData.fieldInfo.SetValue(classData, evt.newValue);
            // });
            // return new PropertyField(property, " ") {name = fieldData.fieldInfo.Name, style = {flexGrow = 1, flexShrink = 0}, label = " ", bindingPath = property.propertyPath}

            return new PropertyField(property, " ") {name = fieldData.fieldInfo.Name, style = {flexGrow = 1, flexShrink = 0}, label = " ", bindingPath = property.propertyPath}
                                           .RegisterValueChangeCallback<PropertyField, ChangeEvent<SerializedPropertyChangeEvent>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                // new VisualElement {name = "__BLANK", style = {display = DisplayStyle.None}}.ToUSS("__BLANK");
        }

        // --| Helper Methods ---------------------------------------
        // --| ------------------------------------------------------
        [UsedImplicitly] private static T CastObject<T>(object input, T type) => (T)input;
        [UsedImplicitly] private static T ConvertObject<T>(object input) => (T)Convert.ChangeType(input, typeof(T));

        private static class Helper
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static Type GetCallersType()
            {
                StackTrace stackTrace = new StackTrace(1, false);
                return stackTrace.GetFrame(1).GetMethod().DeclaringType;
            }
        }

        // --| Clean Up field Name - Remove <>k__BackingFieldField -------
        private static string CleanName(string itemName)
        {
            itemName = itemName.Replace("<", "").Replace(">", "");
            if (itemName.EndsWith("k__BackingField")) itemName = itemName.Substring(0, itemName.Length - 15);
            return itemName;
        }
    }
}
#endif
