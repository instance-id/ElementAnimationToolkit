// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/UIElementsAnimation           --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    public static class VEExtensions
    {
        #region General VisualElement

        // @formatter:off ------------------------------------------------- Hierarchy Modifications
        // ----------------------------------------------------------------------------------------
        #region Hierarchical Modifications
        // @formatter:on
        /// <summary>
        /// Creates the VisualElement and return it as an out variable which can be chained to .ToUSS()
        /// <example><code>new VisualElement().Create(out var myElement);</code></example>
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="variable">Returns the element as an out variable to allow the user of the nameof() function in chained methods</param>
        /// <param name="name">If a name is specifically passed as a parameter, it will be used, otherwise the target variable name is used</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T Create<T>(this T element, out T variable, string name = null) where T : VisualElement
        {
            if (name != null) element.name = name;
            return variable = element;
        }

        /// <summary>
        /// Is elementName is passed, sets the target elements name and USS class to elementName.
        /// If no parameter is passed, the element USS class is set to the targets current name.
        /// If neither a parameter is passed, and the target element has no name, the type is used as both name and USS class.
        /// <remarks>(Make sure to either name the initial element, or pass a name)</remarks>
        ///
        /// <code>new VisualElement().Create(out var myElement).ToUSS(nameof(myElement));</code>
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="elementClassNames">The name to be assigned to this element.
        /// If multiple strings are passed and the element is not yet named, the first string parameter will be used as the the element name</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T ToUSS<T>(this T element, params string[] elementClassNames) where T : VisualElement
        {
            if (elementClassNames != null && element.name != null)
            {
                element.name = elementClassNames[0];
                for (var i = 0; i < elementClassNames.Length; i++)
                {
                    element.AddToClassList(elementClassNames[i]);
                }
            }
            else if (element.name != null) element.AddToClassList(element.name);
            else
            {
                var tmpName = "";
                if (element is VisualElement visualElement) tmpName = $"{visualElement.parent.name}sChild";
                element.name = tmpName;
            }

            return element;
        }

        /// <summary>
        /// Sets the target elements variable name as it's Element name and USS class
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="name">If a name is specifically passed as a parameter, it will be used, otherwise the target variable name is used</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static void NameAsUSS<T>(this T element, string name = null) where T : VisualElement
        {
            if (!string.IsNullOrEmpty(name))
                element.name = name;
            element.AddToClassList(element.name);
        }

        /// <summary>
        /// Add multiple child elements to a parent VisualElement
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="elements">The elements to add to this VisualElement as child elements</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T Add<T>(this T element, VisualElement[] elements) where T : VisualElement
        {
            for (var i = 0; i < elements.Length; i++)
            {
                element.Add(elements[i]);
            }

            return element;
        }

        /// <summary>
        /// Add multiple child elements to a parent VisualElement
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="elements">The elements to add to this VisualElement as child elements</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T SetParent<T>(this T element, VisualElement[] elements) where T : VisualElement
        {
            for (var i = 0; i < elements.Length; i++)
            {
                element.Add(elements[i]);
            }

            return element;
        }

        /// <summary>
        /// Add multiple child elements to a parent VisualElement
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="parent">The elements to add to this VisualElement as child elements</param>
        /// <typeparam name="T">VisualElement</typeparam>
        /// <typeparam name="V"></typeparam>
        public static V SetParent<T, V>(this T element, V parent) where V : VisualElement
        {
            parent.Add(element as VisualElement);
            return parent;
        }

        #endregion

        #endregion

        // @formatter:off ------------------------------------------------------- Actions / Helpers
        // ----------------------------------------------------------------------------------------
        #region Actions / Helpers
        // @formatter:on
        /// <summary>
        /// Opens a web address in an external browser
        /// </summary>
        /// <param name="element">The target element to become the clickable link</param>
        /// <param name="url">The web address in which to open in external browser</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T OpenURL<T>(this T element, string url) where T : VisualElement
        {
            element.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    Application.OpenURL(url);
                    evt.StopPropagation();
                }
            });

            return element;
        }

        /// <summary>
        /// Automatically set the value of a Toggle after N milliseconds of the cursor leaving the bounds of (this) target object
        /// </summary>
        /// <param name="element">The target element to become the clickable link</param>
        /// <param name="closeTarget">The target toggle in which to set its value within N milliseconds of the mouse leaving this  elements bounds  </param>
        /// <param name="autoToggleValue">The value in which to set the Toggle after toggleTimer completes</param>
        /// <param name="toggleTimer">The amount of time in which to wait before the toggles value is changed.
        /// Default: 1000ms</param>
        /// <param name="interruptible">Whether the automatic toggle should be interrupted if the cursor is placed back into the bounds of the target element</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T AutoToggleAfter<T>(this T element, Toggle closeTarget, long toggleTimer = 1000, bool interruptible = false, bool autoToggleValue = false)
            where T : VisualElement
        {
            bool interrupter = false;
            IVisualElementScheduledItem menuCloser = element.schedule.Execute(() =>
            {
                if (interrupter == false && closeTarget.value) closeTarget.value = autoToggleValue;
            });

            element.RegisterCallback<MouseOverEvent>(evt =>
            {
                if (!interruptible) return;

                interrupter = true;
                menuCloser?.Pause();
                evt.StopPropagation();
            });
            element.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                interrupter = false;

                if (closeTarget.value)
                    menuCloser.ExecuteLater(toggleTimer);
                evt.StopPropagation();
            });

            return element;
        }

        #endregion

        #region Text Related

        public static void SelectRangeDelayed(this TextField textField, int cursorIndex, int selectionIndex)
        {
            textField.schedule.Execute(() =>
            {
                // textField.Q("unity-text-input").Focus();
                textField.SelectRange(cursorIndex, selectionIndex);
            });
        }

        /// <summary>
        /// Creates a privacy string in which characters at either the beginning or end are shown as asterisks ( * )
        /// Ex. 0123456 -> **2345*
        /// </summary>
        /// <param name="exposedString">The string in which to apply the privacy mask</param>
        /// <param name="prefixNum">The number of characters are the beginning of the string to mask</param>
        /// <param name="suffixNum">The number of characters are the end of the string to mask. Defaults to 4 suffix characters.
        /// Ex. 0123456789 -> 012345****</param>
        /// <returns>Returns a Tuple value in which Item1 is the original string, Item2 is the privacy masked string</returns>
        public static (string, string) ApplyPrivacyMask(this string exposedString, int prefixNum = 0, int suffixNum = 4)
        {
            if (exposedString.Length <= 0) return ("", "");
            var tmpString = new List<string>();
            for (var i = 0; i < exposedString.Length; i++)
            {
                tmpString.Add((i < prefixNum || i > suffixNum) ? exposedString[i].ToString() : "*");
            }

            return (exposedString, tmpString.Aggregate((i, j) => i + j));
        }

        #endregion


        #region Style Related

        // ---------------------------------------------------------------------------------------- Style Changes

        #region Style Changes

        // ------------------------------------------------ Value adjustments
        /// <summary>
        /// Sets a float value to 0.0001 instead of true 0 due to issues with opacity bugging out.
        /// </summary>
        /// <param name="num">The target number in which to adjust value</param>
        public static float Zero(this int num)
        {
            var tmpNum = (float) num;
            tmpNum = 0.0001f;
            return tmpNum;
        }

        /// <summary>
        /// Sets a float value to 0.0001 instead of true 0 due to issues with opacity bugging out.
        /// </summary>
        /// <param name="num">The target number in which to adjust value</param>
        public static float Zero(this float num)
        {
            num = 0.0001f;
            return num;
        }

        // ------------------------------------------------ Set Values
        /// <summary>
        /// Set primary the color of the element
        /// </summary>
        /// <param name="element">The target element to apply color</param>
        /// <param name="color">The color to apply to the primary element</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static bool SetColor<T>(this T element, Color color = default) where T : VisualElement
        {
            if (element.style.color != color) element.style.color = color;
            return true;
        }

        /// <summary>
        /// Set background the color of the element
        /// </summary>
        /// <param name="element">The target element to apply background color</param>
        /// <param name="color">The color to apply to the border</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static bool SetBackgroundColor<T>(this T element, Color color = default) where T : VisualElement
        {
            if (element.style.backgroundColor != color) element.style.backgroundColor = color;
            return true;
        }

        /// <summary>
        /// Adds a border to all sides of the VisualElement
        /// </summary>
        /// <param name="element">The target element to add a border</param>
        /// <param name="borderThickness">The value in which to set the border thickness</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static void SetBorderWidth<T>(this T element, float borderThickness = 0) where T : VisualElement
        {
            element.style.borderBottomWidth = borderThickness;
            element.style.borderLeftWidth = borderThickness;
            element.style.borderRightWidth = borderThickness;
            element.style.borderTopWidth = borderThickness;
        }

        /// <summary>
        /// Sets the margin to all sides of the VisualElement
        /// </summary>
        /// <param name="element">The target element to add a border</param>
        /// <param name="margin">The value in which to set the border thickness</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static void SetMargin<T>(this T element, int margin = 0) where T : VisualElement
        {
            element.style.marginBottom = margin;
            element.style.marginLeft = margin;
            element.style.marginRight = margin;
            element.style.marginTop = margin;
        }

        /// <summary>
        /// Adds a border to all sides of the VisualElement
        /// </summary>
        /// <param name="element">The target element to add a border</param>
        /// <param name="padding">The value in which to set the border thickness</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static void SetPadding<T>(this T element, int padding = 0) where T : VisualElement
        {
            element.style.paddingBottom = padding;
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            element.style.paddingTop = padding;
        }

        /// <summary>
        /// Adds a border to all sides of the VisualElement
        /// </summary>
        /// <param name="element">The target element to add a border</param>
        /// <param name="color">The color to apply to the border</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static bool SetBorderColor<T>(this T element, Color color = default) where T : VisualElement
        {
            if (element.style.borderBottomColor != color) element.style.borderBottomColor = color;
            if (element.style.borderLeftColor != color) element.style.borderLeftColor = color;
            if (element.style.borderRightColor != color) element.style.borderRightColor = color;
            if (element.style.borderTopColor != color) element.style.borderTopColor = color;
            return true;
        }

        /// <summary>
        /// Adds a border to all sides of the VisualElement
        /// </summary>
        /// <param name="element">The target element to add a border</param>
        /// <param name="opacity">The value in which to set the border thickness</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static void SetOpacity<T>(this T element, float opacity = 0) where T : VisualElement
        {
            if (opacity == 0) opacity = opacity.Zero();
            element.style.opacity = opacity;
        }

        #endregion

        // ---------------------------------------------------------------------------------------- Reusable Style Data
        // -- Create Reusable Style Data ---------------------------------
        public static VisualElementStyleStore CreateStyleData<T>(this T source) where T : VisualElement
        {
            return new VisualElementStyleStore
            {
                Width = new StyleLength(StyleKeyword.Auto),
                Height = new StyleLength(StyleKeyword.Auto),
                // maxWidth = source.resolvedStyle.maxWidth.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.resolvedStyle.maxWidth.value,
                // maxHeight = source.resolvedStyle.maxHeight.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.resolvedStyle.maxHeight.value,
                // minWidth = source.resolvedStyle.minWidth.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.resolvedStyle.minWidth.value,
                // minHeight = source.resolvedStyle.minHeight.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.resolvedStyle.minHeight.value,
                // flexBasis = source.resolvedStyle.flexBasis.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.resolvedStyle.flexBasis.value,
                FlexGrow = source.resolvedStyle.flexGrow,
                FlexShrink = source.resolvedStyle.flexShrink,
                FlexDirection = source.style.flexDirection.value.ToStyleInt(),
                FlexWrap = source.style.flexWrap.value.ToStyleInt(),
                // left = source.resolvedStyle.left,
                // top = source.resolvedStyle.top,
                // right = source.resolvedStyle.right,
                // bottom = source.resolvedStyle.bottom,
                MarginLeft = source.resolvedStyle.marginLeft,
                MarginTop = source.resolvedStyle.marginTop,
                MarginRight = source.resolvedStyle.marginRight,
                MarginBottom = source.resolvedStyle.marginBottom,
                PaddingLeft = source.resolvedStyle.paddingLeft,
                PaddingTop = source.resolvedStyle.paddingTop,
                PaddingRight = source.resolvedStyle.paddingRight,
                PaddingBottom = source.resolvedStyle.paddingBottom,
                // position = source.resolvedStyle.position,
                AlignSelf = source.resolvedStyle.alignSelf.ToStyleInt(),
                UnityTextAlign = source.resolvedStyle.unityTextAlign.ToStyleInt(),
                UnityFontStyleAndWeight = source.style.unityFontStyleAndWeight.value.ToStyleInt(),
                FontSize = source.resolvedStyle.fontSize,
                WhiteSpace = source.style.whiteSpace.value.ToStyleInt(),
                Color = source.resolvedStyle.color,
                BackgroundColor = source.resolvedStyle.backgroundColor,
                UnityFont = source.resolvedStyle.unityFont,
                UnityBackgroundScaleMode = source.style.unityBackgroundScaleMode.value.ToStyleInt(),
                UnityBackgroundImageTintColor = source.resolvedStyle.unityBackgroundImageTintColor,
                AlignItems = source.style.alignItems.value.ToStyleInt(),
                AlignContent = source.style.alignContent.value.ToStyleInt(),
                JustifyContent = source.style.justifyContent.value.ToStyleInt(),
                BorderLeftColor = source.resolvedStyle.borderLeftColor,
                BorderRightColor = source.resolvedStyle.borderRightColor,
                BorderTopColor = source.resolvedStyle.borderTopColor,
                BorderBottomColor = source.resolvedStyle.borderBottomColor,
                BorderLeftWidth = source.resolvedStyle.borderLeftWidth,
                BorderRightWidth = source.resolvedStyle.borderRightWidth,
                BorderTopWidth = source.resolvedStyle.borderTopWidth,
                BorderBottomWidth = source.resolvedStyle.borderBottomWidth,
                BorderTopLeftRadius = source.resolvedStyle.borderTopLeftRadius,
                BorderTopRightRadius = source.resolvedStyle.borderTopRightRadius,
                BorderBottomLeftRadius = source.resolvedStyle.borderBottomLeftRadius,
                BorderBottomRightRadius = source.resolvedStyle.borderBottomRightRadius,
                // unitySliceLeft = source.resolvedStyle.unitySliceLeft,
                // unitySliceTop = source.resolvedStyle.unitySliceTop,
                // unitySliceRight = source.resolvedStyle.unitySliceRight,
                // unitySliceBottom = source.resolvedStyle.unitySliceBottom,
                Opacity = source.resolvedStyle.opacity,
                Visibility = source.style.visibility.value.ToStyleInt(),
                Display = source.style.display.value.ToStyleInt()
            };
        }

        // -- Apply Style data to element from StyleStore ----------------
        public static T FromStyleData<T>(this VisualElementStyleStore source, T target, bool buildTable = false) where T : VisualElement
        {
            var styleValues = new Dictionary<string, object>
            {
                {nameof(target.style.width), target.style.width = source.Width},
                {nameof(target.style.height), target.style.height = source.Height},
                {
                    nameof(target.style.maxWidth), target.style.maxWidth =
                        (source.MaxWidth.value == 0 || source.MaxWidth == StyleKeyword.Null) ? new StyleLength(StyleKeyword.Auto) : source.MaxWidth.value
                },
                {nameof(target.style.maxHeight), target.style.maxHeight = source.MaxHeight.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.MaxHeight.value},
                {nameof(target.style.minWidth), target.style.minWidth = source.MinWidth.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.MinWidth.value},
                {nameof(target.style.minHeight), target.style.minHeight = source.MinHeight.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.MinHeight.value},
                {nameof(target.style.flexBasis), target.style.flexBasis = source.FlexBasis.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.FlexBasis.value},
                {nameof(target.style.flexGrow), target.style.flexGrow = source.FlexGrow},
                {nameof(target.style.flexShrink), target.style.flexShrink = source.FlexShrink},
                {nameof(target.style.flexDirection), target.style.flexDirection = (FlexDirection) source.FlexDirection.value},
                {nameof(target.style.flexWrap), target.style.flexWrap = (Wrap) source.FlexWrap.value},
                {nameof(target.style.marginLeft), target.style.marginLeft = source.MarginLeft},
                {nameof(target.style.marginTop), target.style.marginTop = source.MarginTop},
                {nameof(target.style.marginRight), target.style.marginRight = source.MarginRight},
                {nameof(target.style.marginBottom), target.style.marginBottom = source.MarginBottom},
                {nameof(target.style.paddingLeft), target.style.paddingLeft = source.PaddingLeft},
                {nameof(target.style.paddingTop), target.style.paddingTop = source.PaddingTop},
                {nameof(target.style.paddingRight), target.style.paddingRight = source.PaddingRight},
                {nameof(target.style.paddingBottom), target.style.paddingBottom = source.PaddingBottom},
                {nameof(target.style.alignSelf), target.style.alignSelf = (Align) source.AlignSelf.value},
                {nameof(target.style.unityTextAlign), target.style.unityTextAlign = (TextAnchor) source.UnityTextAlign.value},
                {nameof(target.style.unityFontStyleAndWeight), target.style.unityFontStyleAndWeight = (FontStyle) source.UnityFontStyleAndWeight.value},
                {nameof(target.style.fontSize), target.style.fontSize = source.FontSize},
                {nameof(target.style.whiteSpace), target.style.whiteSpace = (WhiteSpace) source.WhiteSpace.value},
                {nameof(target.style.color), target.style.color = source.Color},
                {nameof(target.style.backgroundColor), target.style.backgroundColor = source.BackgroundColor},
                {nameof(target.style.unityFont), target.style.unityFont = source.UnityFont},
                {nameof(target.style.unityBackgroundScaleMode), target.style.unityBackgroundScaleMode = (ScaleMode) source.UnityBackgroundScaleMode.value},
                {nameof(target.style.unityBackgroundImageTintColor), target.style.unityBackgroundImageTintColor = source.UnityBackgroundImageTintColor},
                {nameof(target.style.alignItems), target.style.alignItems = (Align) source.AlignItems.value},
                {nameof(target.style.alignContent), target.style.alignContent = (Align) source.AlignContent.value},
                {nameof(target.style.justifyContent), target.style.justifyContent = (Justify) source.JustifyContent.value},
                {nameof(target.style.borderLeftColor), target.style.borderLeftColor = source.BorderLeftColor},
                {nameof(target.style.borderRightColor), target.style.borderRightColor = source.BorderRightColor},
                {nameof(target.style.borderTopColor), target.style.borderTopColor = source.BorderTopColor},
                {nameof(target.style.borderBottomColor), target.style.borderBottomColor = source.BorderBottomColor},
                {nameof(target.style.borderLeftWidth), target.style.borderLeftWidth = source.BorderLeftWidth},
                {nameof(target.style.borderRightWidth), target.style.borderRightWidth = source.BorderRightWidth},
                {nameof(target.style.borderTopWidth), target.style.borderTopWidth = source.BorderTopWidth},
                {nameof(target.style.borderBottomWidth), target.style.borderBottomWidth = source.BorderBottomWidth},
                {nameof(target.style.borderTopLeftRadius), target.style.borderTopLeftRadius = source.BorderTopLeftRadius},
                {nameof(target.style.borderTopRightRadius), target.style.borderTopRightRadius = source.BorderTopRightRadius},
                {nameof(target.style.borderBottomLeftRadius), target.style.borderBottomLeftRadius = source.BorderBottomLeftRadius},
                {nameof(target.style.borderBottomRightRadius), target.style.borderBottomRightRadius = source.BorderBottomRightRadius},
                {nameof(target.style.opacity), target.style.opacity = source.Opacity.value},
                {nameof(target.style.visibility), target.style.visibility = (Visibility) source.Visibility.value},
                {nameof(target.style.display), target.style.display = (DisplayStyle) source.Display.value}
            };
            //styleValues.Add(nameof(target.style. ),target.style.left = source.left);
            //styleValues.Add(nameof(target.style. ),target.style.top = source.top);
            //styleValues.Add(nameof(target.style. ),target.style.right = source.right);
            //styleValues.Add(nameof(target.style. ),target.style.bottom = source.bottom);
            //styleValues.Add(nameof(target.style. ),target.style.position = source.position);
            //styleValues.Add(nameof(target.style. ),target.style.unitySliceLeft = source.unitySliceLeft);
            //styleValues.Add(nameof(target.style. ),target.style.unitySliceTop = source.unitySliceTop);
            //styleValues.Add(nameof(target.style. ),target.style.unitySliceRight = source.unitySliceRight);
            //styleValues.Add(nameof(target.style. ),target.style.unitySliceBottom = source.unitySliceBottom);

            if (!buildTable) return target;

            var table = new TableBuilder();
            table.WithHeader("Target", "Name", "Type", "Value");
            styleValues.ForEach(x => { table.WithRow(source.SourceName, x.Key, x.Value.GetType().Name, x.Value.ToString()); });

            table.ToConsole();
            return target;
        }

        //--------------------------------------------------------------------------------------------
        public static T CopyElement<T>(this T source, T target) where T : VisualElement
        {
            var styleValues = new List<object>();
            styleValues.Add(target.style.width = source.resolvedStyle.width == 0
                ? new StyleLength(StyleKeyword.Auto)
                : source.resolvedStyle.width);

            styleValues.Add(target.style.height = source.resolvedStyle.height == 0
                ? new StyleLength(StyleKeyword.Auto)
                : source.resolvedStyle.height);

            styleValues.Add(target.style.maxWidth = source.resolvedStyle.maxWidth.value == 0
                ? new StyleLength(StyleKeyword.Auto)
                : source.resolvedStyle.maxWidth.value);

            styleValues.Add(target.style.maxHeight = source.resolvedStyle.maxHeight.value == 0
                ? new StyleLength(StyleKeyword.Auto)
                : source.resolvedStyle.maxHeight.value);

            styleValues.Add(target.style.minWidth = source.resolvedStyle.minWidth.value == 0
                ? new StyleLength(StyleKeyword.Auto)
                : source.resolvedStyle.minWidth.value);

            styleValues.Add(target.style.minHeight = source.resolvedStyle.minHeight.value == 0
                ? new StyleLength(StyleKeyword.Auto)
                : source.resolvedStyle.minHeight.value);

            // styleValues.Add(target.style.flexBasis = source.resolvedStyle.flexBasis.keyword == StyleKeyword.Auto
            //     ? new StyleLength(StyleKeyword.Auto)
            //     : source.resolvedStyle.flexBasis.keyword == StyleKeyword.Undefined
            //         ? new StyleLength(StyleKeyword.Auto)
            //         : source.resolvedStyle.flexBasis.keyword);
            //
            // Debug.Log($"target flexBasis {target.style.flexBasis.value}");
            // Debug.Log($"source style flexBasis {source.style.flexBasis.value}");
            // Debug.Log($"source resolvedStyle flexBasis {source.resolvedStyle.flexBasis.value}");

            styleValues.Add(target.style.flexGrow = source.resolvedStyle.flexGrow);
            styleValues.Add(target.style.flexShrink = source.resolvedStyle.flexShrink);
            styleValues.Add(target.style.flexDirection = source.resolvedStyle.flexDirection);
            styleValues.Add(target.style.flexWrap = source.resolvedStyle.flexWrap);
            // target.style.left = source.resolvedStyle.left;
            // target.style.top = source.resolvedStyle.top;
            // target.style.right = source.resolvedStyle.right;
            // target.style.bottom = source.resolvedStyle.bottom;
            styleValues.Add(target.style.marginLeft = source.resolvedStyle.marginLeft);
            styleValues.Add(target.style.marginTop = source.resolvedStyle.marginTop);
            styleValues.Add(target.style.marginRight = source.resolvedStyle.marginRight);
            styleValues.Add(target.style.marginBottom = source.resolvedStyle.marginBottom);
            styleValues.Add(target.style.paddingLeft = source.resolvedStyle.paddingLeft);
            styleValues.Add(target.style.paddingTop = source.resolvedStyle.paddingTop);
            styleValues.Add(target.style.paddingRight = source.resolvedStyle.paddingRight);
            styleValues.Add(target.style.paddingBottom = source.resolvedStyle.paddingBottom);
            // target.style.position = source.resolvedStyle.position;
            styleValues.Add(target.style.alignSelf = source.resolvedStyle.alignSelf);
            styleValues.Add(target.style.unityTextAlign = source.resolvedStyle.unityTextAlign);
            styleValues.Add(target.style.unityFontStyleAndWeight = source.resolvedStyle.unityFontStyleAndWeight);
            styleValues.Add(target.style.fontSize = source.resolvedStyle.fontSize);
            styleValues.Add(target.style.whiteSpace = source.resolvedStyle.whiteSpace);
            styleValues.Add(target.style.color = source.resolvedStyle.color);
            styleValues.Add(target.style.backgroundColor = source.resolvedStyle.backgroundColor);
            styleValues.Add(target.style.unityFont = source.resolvedStyle.unityFont);
            styleValues.Add(target.style.unityBackgroundScaleMode = source.resolvedStyle.unityBackgroundScaleMode);
            styleValues.Add(target.style.unityBackgroundImageTintColor = source.resolvedStyle.unityBackgroundImageTintColor);
            styleValues.Add(target.style.alignItems = source.resolvedStyle.alignItems);
            styleValues.Add(target.style.alignContent = source.resolvedStyle.alignContent);
            styleValues.Add(target.style.justifyContent = source.resolvedStyle.justifyContent);
            styleValues.Add(target.style.borderLeftColor = source.resolvedStyle.borderLeftColor);
            styleValues.Add(target.style.borderRightColor = source.resolvedStyle.borderRightColor);
            styleValues.Add(target.style.borderTopColor = source.resolvedStyle.borderTopColor);
            styleValues.Add(target.style.borderBottomColor = source.resolvedStyle.borderBottomColor);
            styleValues.Add(target.style.borderLeftWidth = source.resolvedStyle.borderLeftWidth);
            styleValues.Add(target.style.borderRightWidth = source.resolvedStyle.borderRightWidth);
            styleValues.Add(target.style.borderTopWidth = source.resolvedStyle.borderTopWidth);
            styleValues.Add(target.style.borderBottomWidth = source.resolvedStyle.borderBottomWidth);
            styleValues.Add(target.style.borderTopLeftRadius = source.resolvedStyle.borderTopLeftRadius);
            styleValues.Add(target.style.borderTopRightRadius = source.resolvedStyle.borderTopRightRadius);
            styleValues.Add(target.style.borderBottomLeftRadius = source.resolvedStyle.borderBottomLeftRadius);
            styleValues.Add(target.style.borderBottomRightRadius = source.resolvedStyle.borderBottomRightRadius);
            // styleValues.Add(target.style.unitySliceLeft = source.resolvedStyle.unitySliceLeft);
            // styleValues.Add(target.style.unitySliceTop = source.resolvedStyle.unitySliceTop);
            // styleValues.Add(target.style.unitySliceRight = source.resolvedStyle.unitySliceRight);
            // styleValues.Add(target.style.unitySliceBottom = source.resolvedStyle.unitySliceBottom);
            styleValues.Add(target.style.opacity = source.resolvedStyle.opacity);
            styleValues.Add(target.style.visibility = source.resolvedStyle.visibility);
            styleValues.Add(target.style.display = source.resolvedStyle.display);

            var table = new TableBuilder();
            table.WithHeader("Name", "Value");

            styleValues.ForEach(x =>
            {
                if (x is IResolvedStyle style)
                {
                    table.WithRow(x.GetType().Name, style.GetType().Name);
                }
            });

            table.ToConsole();


            return target;
        }

        #endregion
    }
}
