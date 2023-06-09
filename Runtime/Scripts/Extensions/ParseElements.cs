 using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VisualElement = UnityEngine.UIElements.VisualElement;
#if UNITY_EDITOR
 
namespace instance.id.EATK.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class UIParser
    {
        /// <param name="node"></param>
        /// <returns></returns>
        [UsedImplicitly]
        public static VisualElement CreateVisualElement(XmlNode node)
        {
            if (node == null)
                throw new Exception("Node is null");

            // Debug.Log($"Looking for type: {node.Name.Replace("ui:", "")}");
            Type elementType = Array.Find(typeof(VisualElement).Assembly.GetTypes(), x =>
                x.Name.Equals(node.Name.Replace("ui:", "")));
            if (elementType == null)
            {
                if (!node.Name.Replace("ui:", "").Equals("Style"))
                {
                    Debug.Log($"Couldn't find type \"{node.Name.Replace("ui:", "")}\"\nSkipping...");
                }

                return null;
            }

            VisualElement element = (VisualElement)typeof(VisualElement).GetMethod("CreateUnityType")
                ?.MakeGenericMethod(elementType)
                .Invoke(null, new object[] { new object[] { } });
            // Debug.Log("Creating element..");

            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                {
                    // Debug.Log("Parsing Attribute");

                    var property = elementType.GetProperty(MapCssName(attr.Name));

                    if (attr.Name.Equals("class"))
                    {
                        //Debug.Log("Class found with value: " + attr.Value);
                        element?.AddToClassList(attr.Value);
                    }

                    if (property == null)
                    {
                        if (!attr.Name.Equals("class"))
                        {
                            Debug.Log($"Skipping attribute \"{attr.Name}\"");
                        }

                        // throw new Exception($"No property found with name \"{attr.Name}\"");
                        continue;
                    }

                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(element, Enum.Parse(property.PropertyType, attr.Value));
                        continue;
                    }

                    switch (property.PropertyType.Name)
                    {
                        case "Boolean":
                            // Debug.Log($"Parsing Boolean: {attr.Value}");
                            property.SetValue(element, bool.Parse(attr.Value));
                            break;
                        case "String":
                            // Debug.Log($"Parsing String: {attr.Value}");
                            property.SetValue(element, attr.Value);
                            break;
                        case "IStyle":
                            // Debug.Log("Parsing IStyle");
                            element = ParseStyle(attr.Value, element);
                            break;
                        default:
                            throw new Exception($"Found no matching type to {property.PropertyType.FullName}");
                    }
                }


            VisualElement resultElement = element;

            foreach (XmlNode child in node.ChildNodes)
            {
                var parsedChild = CreateVisualElement(child);
            }

            return resultElement;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="element">WARNING: Make sure this type is or inherits <see cref="UnityEngine.UIElements.VisualElement" /></param>
        public static VisualElement ParseStyle(string data, VisualElement element)
        {
            var list = data.Replace("&apos;", "\"").Split(';').ToList();
            foreach (string entry in list)
            {
                if (entry != " ")
                {
                    element = ParseEntry(entry, element);
                }
            }

            return element;
        }

        public static string MapCssName(string cssTag) =>
            (cssTag[0] == '-' ? char.ToLower(cssTag[1]) : char.ToLower(cssTag[0])) + cssTag.Split('-')
                .Select(s => s != "" ? char.ToUpper(s[0]) + s.Substring(1) : "")
                .Aggregate("", (s, s1) => s + s1).Substring(1);

        public static PropertyInfo MapProperty(string propertyStr)
        {
            string propertyName = MapCssName(propertyStr);
            if (propertyName == "unityFontStyle") propertyName = "unityFontStyleAndWeight";
            if (propertyName == "textAlign") propertyName = "unityTextAlign";
            return typeof(IStyle).GetProperty(propertyName);
        }

        public static object ParseProperty(PropertyInfo property, string value)
        {
            if (property.PropertyType.GenericTypeArguments.Length > 0)
            {
                var result = typeof(UIParser)
                    .GetMethod("ToStyleEnum",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    ?.MakeGenericMethod(property.PropertyType.GenericTypeArguments[0])
                    .Invoke(null, new object[] { value });

                return result;
            }

            switch (property.PropertyType.Name)
            {
                case "StyleFloat":
                    return ToStyleFloat(value);
                case "StyleInt":
                    return ToStyleInt(value);
                case "StyleLength":
                    return ToStyleLength(value);
                case "StyleFont":
                    return ToStyleFont(value);
                case "StyleCursor":
                    return ToStyleCursor(value);
                case "StyleColor":
                    return ToStyleColor(value);
                case "StyleBackground":
                    return ToStyleBackground(value);
                default:
                    Debug.Log("Unhandled type in USS parser");
                    break;
            }

            return null;
        }

        public static VisualElement ParseEntry(string entry, VisualElement element)
        {
            if (element?.name is null)
            {
                Debug.Log($"Failed to parse entry, element was null");
                return null;
            }

            var entrySplit = entry.Split(':');
            entrySplit[0] = entrySplit[0].Replace(" ", "");
            if (entrySplit[0] == "") return element;

            var property = MapProperty(entrySplit[0]);
            if (property == null)
            {
                Debug.Log($"Failed to find property \"{MapCssName(entrySplit[0])}\"");
                // Debug.Log($"Type of style: \"{typeof(element.style).FullName}\"");
            }

            try
            {
                if (property != null)
                {
                    var value = ParseProperty(property, entrySplit[1]);
                    if (value == null)
                    {
                        Debug.Log($"Failed to parse entry {property.Name}\n\"{entry}\"");
                    }
                    else
                    {
                        property.SetValue(element.style, value);
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log($"Failed to set property. Skipping {property?.Name}\n\"{entry}\"");
            }

            return element;
        }

        public static StyleFloat ToStyleFloat(string str) => new StyleFloat(float.Parse(str.Replace("px", "")));

        public static StyleInt ToStyleInt(string str) => new StyleInt(int.Parse(str.Replace("px", "")));

        public static StyleInt ToStyleInt<T>(this T str) where T : struct, IConvertible
        {
            StyleEnum<T> newOne = str;
            return new StyleInt(newOne.value.ToInt32(CultureInfo.CurrentCulture));
        }

        public static StyleLength ToStyleLength(string str)
        {
            str = str.Replace(" ", "");
            if (str.Contains("auto"))
                return new StyleLength(StyleKeyword.Auto);

            bool isPercent = str.Contains("%");
            LengthUnit unit;
            if (isPercent)
                unit = LengthUnit.Percent;
            else
                unit = LengthUnit.Pixel;

            // Debug.Log($"StyleInt from: \"{str}\"");
            // Debug.Log($"Units: \"{unit}\"");

            return new StyleLength(new Length(float.Parse(str.Replace("%", "").Replace("px", "")), unit));
        }

        public static StyleFont ToStyleFont(string _) => new StyleFont(StyleKeyword.Null);

        public static StyleBackground ToStyleBackground(string str)
        {
            string path = str.Replace(" ", "");

            if (path.StartsWith("url(\"") || path.StartsWith("url('"))
                path = path.Remove(0, 5);
            if (path.EndsWith("\")") || path.EndsWith("')"))
                path = path.Remove(path.Length - 2, 2);

            try
            {
                Sprite asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (asset == null)
                {
                    Debug.Log($"Can't find USS background image asset: {path}");
                    return new StyleBackground(StyleKeyword.Null);
                }
                else
                {
                    return new StyleBackground(asset.texture);
                }
            }
            catch (Exception e)
            {
                // FAIL TO GET TEXTURE
                Debug.Log($"Exception when parsing background texture\n{e}");
                return new StyleBackground(StyleKeyword.Null);
            }
        }

        public static StyleCursor ToStyleCursor(string _) => new StyleCursor(StyleKeyword.Null);

        public static StyleColor ToStyleColor(string str)
        {
            var color = new StyleColor();

            var strTmp = str;
            if (strTmp.StartsWith("#"))
                return color.value = str.FromHex();

            if (str[0] == ' ')
                str = str.Substring(1);

            var numbers = new List<string>();
            string start = str.Substring(str.IndexOf('(') + 1);
            while (true)
            {
                if (start.IndexOf(',') >= 0)
                {
                    numbers.Add(start.Substring(0, start.IndexOf(',')));
                    start = start.Substring(start.IndexOf(',') + 2);
                }
                else
                {
                    numbers.Add(start.Substring(0, start.IndexOf(')')));
                    break;
                }
            }

            switch (str.Substring(0, str.IndexOf('(')))
            {
                case "rgb":
                    color.value = new Color(
                        float.Parse(numbers[0]) / 255.0f,
                        float.Parse(numbers[1]) / 255.0f,
                        float.Parse(numbers[2]) / 255.0f);
                    break;
                case "rgba":
                    color.value = new Color(
                        float.Parse(numbers[0]) / 255.0f,
                        float.Parse(numbers[1]) / 255.0f,
                        float.Parse(numbers[2]) / 255.0f,
                        float.Parse(numbers[3]));
                    break;
                case "#":
                    color.value = str.FromHex();
                    break;
            }

            return color;
        }

        public static StyleEnum<T> ToStyleEnum<T>(string str) where T : struct, IConvertible => new StyleEnum<T>
            { value = (T)Enum.Parse(typeof(T), MapCssName(str.Replace(" ", "")), true) };

        public static float TryParseFloat(this string str)
        {
            float.TryParse(str, out float result);
            return result;
        }
    }
}

#endif
