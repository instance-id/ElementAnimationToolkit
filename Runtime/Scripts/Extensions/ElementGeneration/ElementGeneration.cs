using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace instance.id.EATK.Extensions
{
    public static class ElementGeneration
    {
        /// <summary>
        /// Generate UIElements Editor Window 
        /// </summary>
        /// <param name="classData"></param>
        /// <param name="labelMinWidth"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static VisualElement GenerateElements<T>(this T classData, float labelMinWidth, float marginLeft = 0)
        {
            new VisualElement().Create(out var generatedElements).ToUSS(nameof(generatedElements));
            try
            {
                Dictionary<(int, string), VisualElement> containers = new Dictionary<(int, string), VisualElement>();
                Dictionary<Type, VisualElement> containerTypes = new Dictionary<Type, VisualElement>();
                Dictionary<int, string> uniqueContainers = new Dictionary<int, string>();

                var attributes = classData.GetEditorAttributes<EditorFieldAttribute>();


                foreach (var v in attributes.fieldDatas)
                {
                    var data = v.Value.attributeData.container;
                    
                    if (uniqueContainers.ContainsKey(data.containerId)) continue;
                    uniqueContainers.Add(data.containerId, data.containerName);
                    if (containers.ContainsKey((data.containerId, data.containerName))) continue;
                    
                    switch (data.containerType)
                    {
                        case ContainerStyle.AnimatedFoldout:
                            containers.TryAddValue((data.containerId, data.containerName), CreateAnimatedFoldout(data));
                            containerTypes.TryAddValue(typeof(AnimatedFoldout), containers[(data.containerId, data.containerName)]);
                            break;
                        case ContainerStyle.Foldout:
                            containers.TryAddValue((data.containerId, data.containerName), CreateFoldout(data));
                            containerTypes.TryAddValue(typeof(Foldout), containers[(data.containerId, data.containerName)]);
                            break;
                        case ContainerStyle.Box:
                            containers.TryAddValue((data.containerId, data.containerName), CreateBox(data));
                            containerTypes.TryAddValue(typeof(Box), containers[(data.containerId, data.containerName)]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                foreach (var v in attributes.fieldDatas)
                {
                    var data = v.Value.attributeData.container;
                    var chosenContainer = uniqueContainers.TryGetVal(data.containerId);
                    
                    var parent = containers.FirstOrDefault(x => x.Key == (data.containerId, chosenContainer));
                    DetermineType(v.Value, classData)
                        .CreateWithLabel(out v.Value.attributeData.element, labelText: v.Value.attributeData.description, labelMinWidth: labelMinWidth)
                        .ToUSS($"{v.Value.name}Field")
                        .SetParent(parent.Value);
                }

                containers.ForEach(x => { generatedElements.Add(x.Value); });
                containerTypes.ForEach(x =>
                {
                    if (x.Key == typeof(AnimatedFoldout))
                        generatedElements.Query<AnimatedFoldout>()
                            .Descendents<VisualElement>("expandContainer").ToList().ForEach(z => z.style.marginLeft = marginLeft);
                });
            } // @formatter:off
            catch (Exception e) { Console.WriteLine(e); throw; }
            return generatedElements;  // @formatter:on
        }

        // --| Create Container Elements -----------------------
        private static VisualElement CreateAnimatedFoldout(ContainerData data) =>
            new AnimatedFoldout { text = data.containerName, value = false }.Create(out data.element).ToUSS(data.containerName);

        private static VisualElement CreateFoldout(ContainerData data) =>
            new Foldout { text = data.containerName, value = false }.Create(out data.element).ToUSS(data.containerName);

        private static VisualElement CreateBox(ContainerData data) =>
            new Box { name = data.containerName }.Create(out data.element).ToUSS(data.containerName);

        // --| Determine Container Type ------------------------
        private static VisualElement DetermineType(FieldData<EditorFieldAttribute> fieldData, object classData)
        {
            var value = fieldData.fieldInfo.GetValue(classData);
            switch (fieldData.fieldInfo.FieldType)
            {
                case Type t when t == typeof(long):
                    return new LongField { value = (long)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<LongField, ChangeEvent<long>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(int):
                    return new IntegerField { value = (int)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<IntegerField, ChangeEvent<int>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(bool):
                    return new Toggle { value = (bool)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<Toggle, ChangeEvent<bool>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(float):
                    return new FloatField { value = (float)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<FloatField, ChangeEvent<float>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(double):
                    return new DoubleField { value = (double)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<DoubleField, ChangeEvent<double>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(string):
                    return new TextField { value = (string)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<TextField, ChangeEvent<string>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Vector2):
                    return new Vector2Field { value = (Vector2)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<Vector2Field, ChangeEvent<Vector2>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Vector3):
                    return new Vector3Field { value = (Vector3)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<Vector3Field, ChangeEvent<Vector3>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Vector4):
                    return new Vector4Field { value = (Vector4)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<Vector4Field, ChangeEvent<Vector4>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t == typeof(Color):
                    return new ColorField { value = (Color)value, style = { flexGrow = 1 } }
                        .RegisterValueCallback<ColorField, ChangeEvent<Color>>(evt => fieldData.fieldInfo.SetValue(classData, evt.newValue));
                case Type t when t.IsEnum:
                    var e = new EnumField() { style = { flexGrow = 1 } };
                    e.Init((System.Enum)fieldData.fieldInfo.GetValue(classData));
                    return e;
            }

            if (fieldData.fieldInfo.FieldType.IsArray)
                Debug.LogError("Array types have not yet been implemented " + fieldData.fieldInfo.FieldType + " " + fieldData.fieldInfo.Name);
            else if (typeof(System.Collections.IList).IsAssignableFrom(fieldData.fieldInfo.FieldType))
                Debug.LogError("List types have not yet been implemented " + fieldData.fieldInfo.FieldType + " " + fieldData.fieldInfo.Name);

            return new VisualElement { name = "__BLANK", style = { display = DisplayStyle.None } }.ToUSS("__BLANK");
        }
    }
}
