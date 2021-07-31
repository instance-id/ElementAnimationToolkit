// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/Extensions                    --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


using UnityEditor;
using UnityEditor.UIElements;


namespace instance.id.EATK.Extensions
{
    public static class VisualElementExtension
    {
#pragma warning disable 67
        public static event Action onChangeProperty;
#pragma warning restore 67

        // --|---------------------------------- Style variables
        private static StyleEnum<FlexDirection> directionRow = new StyleEnum<FlexDirection>(FlexDirection.Row);
        private static StyleEnum<FlexDirection> directionCol = new StyleEnum<FlexDirection>(FlexDirection.Column);

        #region General VisualElement

        public enum ContainerType
        {
            Row,
            Column
        }

        // @formatter:off ------------------------------------------------- Hierarchy Modifications
        // ----------------------------------------------------------------------------------------

        #region Hierarchical Modifications 
#if UNITY_EDITOR
        // --|---------------------------------- Custom elements
        public static VisualElement CreateEnumField<T>(this T enumField, Action<T> onChange = null, string label = null)  // @formatter:on
        {
            var e = new EnumField(label, Convert<Enum>(enumField));
            if (onChange != null) e.RegisterValueChangedCallback(change => { onChange(Convert<T>(change.newValue)); });
            return e;
        }
#endif
        public static TextField CreateTextField(this TextField textField, EventCallback<ChangeEvent<string>> evt = null, string label = null)
        {
            textField = new TextField { label = label };
            if (evt != null) textField.RegisterCallback(evt);
            return textField;
        }

        // --|--------------------------------------- Containers
        // --|--------------------------------------------------
        /// <summary>
        /// Creates a row styled VisualElement and return it as an out variable which can be chained to .ToUSS()
        /// <example><code>new VisualElement().CreateRow(out var myElement);</code></example>
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="variable">Returns the element as an out variable to allow the user of the nameof() function in chained methods</param>
        /// <param name="name">If a name is specifically passed as a parameter, it will be used, otherwise the target variable name is used</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T CreateRow<T>(this T element, out T variable, string name = null) where T : VisualElement
        {
            if (name != null) element.name = name;
            element.style.flexDirection = directionRow;
            return variable = element;
        }

        /// <summary>
        /// Creates a column styled VisualElement and return it as an out variable which can be chained to .ToUSS()
        /// <example><code>new VisualElement().CreateColumn(out var myElement);</code></example>
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="variable">Returns the element as an out variable to allow the user of the nameof() function in chained methods</param>
        /// <param name="name">If a name is specifically passed as a parameter, it will be used, otherwise the target variable name is used</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T CreateColumn<T>(this T element, out T variable, string name = null) where T : VisualElement
        {
            if (name != null) element.name = name;
            element.style.flexDirection = directionCol;
            return variable = element;
        }

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

        public static T Create<T>(this T element, string name = null) where T : VisualElement
        {
            if (name != null) element.name = name;
            return element;
        }

        /// <summary>
        /// Creates the VisualElement and return it as an out variable which can be chained to .ToUSS()
        /// <example><code>new VisualElement().Create(out var myElement);</code></example>
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="variable">Returns the element as an out variable to allow the user of the nameof() function in chained methods</param>
        /// <param name="name">If a name is specifically passed as a parameter, it will be used, otherwise the target variable name is used</param>
        /// <param name="containerType">Whether this element should be a row or column</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static VisualElement CreateWithLabel<T>(this T element, out VisualElement variable, string name = null, ContainerType containerType = ContainerType.Row, string labelText = default) where T : VisualElement
        {
            StyleEnum<FlexDirection> direction = containerType == ContainerType.Row
                ? new StyleEnum<FlexDirection>(FlexDirection.Row)
                : new StyleEnum<FlexDirection>(FlexDirection.Column);

            if (name != null) element.name = name;

            new VisualElement { style = { flexDirection = direction } }.Create(out var elementContainer).ToUSS($"{element.name}Container", (containerType == ContainerType.Row ? "containerRow" : "containerColumn"), labelText);
            new Label { text = labelText }.Create().ToUSS($"{element.name}Label").SetParent(elementContainer);

            element.SetParent(elementContainer);
            return variable = elementContainer;
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
                    element.AddToClassList(elementClassNames[i]);
            }
            else if (element.name != null) element.AddToClassList(element.name);
            else element.name = $"{element.parent.name}sChild";

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


        public static T RegisterValueCallback<T, E>(this T element, EventCallback<E> changeEvent) where T : VisualElement where E : EventBase<E>, new()
        {
            ((VisualElement)element).RegisterCallback(changeEvent);
            return element;
        }

        /// <summary>
        /// Convert an object to another type
        /// </summary>
        /// <param name="value">The object in which to convert</param>
        /// <typeparam name="T">VisualElement</typeparam>
        static T Convert<T>(object value)
        {
            return (T)System.Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Add multiple child elements to a parent VisualElement
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="elements">The elements to add to this VisualElement as child elements</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T AddAll<T>(this T element, params VisualElement[] elements) where T : VisualElement
        {
            for (var i = 0; i < elements.Length; i++) element.Add(elements[i]);
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
            for (var i = 0; i < elements.Length; i++) element.Add(elements[i]);
            return element;
        }

        /// <summary>
        /// Add multiple child elements to a parent VisualElement
        /// </summary>
        /// <param name="element">The target element to perform this action upon</param>
        /// <param name="parent">The elements to add to this VisualElement as child elements</param>
        /// <param name="index">If index parameter is included, this element will be added at that index to the parent</param>
        /// <typeparam name="T">Must be of Type VisualElement</typeparam>
        /// <typeparam name="V">Must also be of Type VisualElement</typeparam>
        public static V SetParent<T, V>(this T element, V parent, int index = -1) where V : VisualElement
        {
            if (index != -1) parent.Insert(index, element as VisualElement);
            else parent.Add(element as VisualElement);

            return parent;
        }

        #endregion

        #endregion

        public static VisualElement GetFirstAncestorWithClass(this VisualElement element, string className)
        {
            if (element == null)
                return null;

            if (element.ClassListContains(className))
                return element;

            return element.parent.GetFirstAncestorWithClass(className);
        }

        // --------------------------------------- Locating parent objects
        // ---------------------------------------------------------------

        public static VisualElement GetFirstAncestorOfType<T>(VisualElement element)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Traverses up the hierarchy to find all of the parent instances of type T.
        /// </summary>
        /// <param name="element">Current element to search parents.</param>
        /// <typeparam name="T">Type which you want to find</typeparam>
        /// <returns>Collection of T instances found.</returns>
        public static IEnumerable<T> GetParentsOfType<T>(this T element, T type = null) where T : VisualElement
        {
            Debug.Log($"Type {typeof(T).Name}");

            var result = new List<T>();

            var parent = element;
            while (parent != null)
            {
                if (parent is T selected)
                    result.Add(selected);

                parent = (T)parent.parent;
            }

            return result;
        }

        /// <summary>
        /// Gets the sibling index.
        /// Use this to return the sibling index of the VisualElement.
        /// If a VisualElement shares a parent with other VisualElement and are on the same level (i.e. they share the same direct parent),
        /// these VisualElements are known as siblings. The sibling index shows where each VisualElement sits in this sibling hierarchy.
        /// Similar to <see href="https://docs.unity3d.com/ScriptReference/Transform.GetSiblingIndex.html"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetSiblingIndex(this VisualElement element)
        {
            return element.parent?.IndexOf(element) ?? 0;
        }

        /// <summary>
        /// Traverses up the hierarchy to find first parent instance of type T.
        /// </summary>
        /// <param name="element">Current element to search parent.</param>
        /// <typeparam name="T">Type which you want to find</typeparam>
        /// <returns>T instance found</returns>
        public static T GetFirstParentOfType<T>(this VisualElement element) where T : VisualElement
        {
            return GetParentsOfType<T>((T)element).FirstOrDefault();
        }

        public static T GetFirstAncestorOfType<T>(this VisualElement element, Type elementType) where T : VisualElement
        {
            for (VisualElement parent = element.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent.GetType() == elementType)
                    return parent as T;
            }

            return default(T);
        }

        public static T GetFirstAncestorOfType<T>(this Type elementType, VisualElement element) where T : VisualElement
        {
            for (VisualElement parent = element.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent.GetType() == elementType)
                    return parent as T;
            }

            return default(T);
        }

        public static T GetFirstAncestorOfType<T>(this Type elementType, VisualElement element, out VisualElement outElement) where T : VisualElement
        {
            VisualElement outVe = new VisualElement();
            if (elementType == null || element == null)
            {
                var elementTypeBool = elementType == null;
                var elementBool = element == null;
                Debug.LogWarning($"Element passed was null: elementType {elementTypeBool} : element {elementBool}");
                outElement = outVe;
                return default(T);
            }

            for (VisualElement parent = element.hierarchy.parent; parent != null; parent = parent.hierarchy.parent)
            {
                if (parent.GetType() == elementType)
                    outVe = parent as T;
            }

            outElement = outVe;
            return default(T);
        }

        public static List<T> GetChildElementsOfType<T>(this Type elementType, VisualElement element, out List<VisualElement> outElement) where T : List<VisualElement>
        {
            List<VisualElement> outVe = new List<VisualElement>();
            if (elementType == null || element == null)
            {
                var elementTypeBool = elementType == null;
                var elementBool = element == null;
                Debug.LogWarning($"Element passed was null: elementType {elementTypeBool} : element {elementBool}");
                outElement = outVe;
                return default(List<T>);
            }

            var elementItem = element.Query(null, "unity-inspector-editors-list").First();
            outVe = elementItem.Query()
                .Children<VisualElement>()
                .Where(x => x.GetType() == elementType)
                .ToList();

            outElement = outVe;
            return default(List<T>);
        }


        // @formatter:off ------------------------------------------------------- Actions / Helpers
        // ----------------------------------------------------------------------------------------

        #region Actions / Helpers

        // @formatter:on

        /// <summary>
        /// Registers a field for any possible change event type
        /// </summary>
        // EventCallback<ChangeEvent<int>> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
        public static void RegisterAnyChangeEvent(this VisualElement field, Action callback)
        {
            field.RegisterCallback<ChangeEvent<int>>(e => callback());
            field.RegisterCallback((ChangeEvent<bool> e) => callback());
            field.RegisterCallback((ChangeEvent<float> e) => callback());
            field.RegisterCallback((ChangeEvent<string> e) => callback());
            field.RegisterCallback((ChangeEvent<Color> e) => callback());
            field.RegisterCallback((ChangeEvent<UnityEngine.Object> e) => callback());
            field.RegisterCallback((ChangeEvent<Enum> e) => callback());
            field.RegisterCallback((ChangeEvent<Vector2> e) => callback());
            field.RegisterCallback((ChangeEvent<Vector3> e) => callback());
            field.RegisterCallback((ChangeEvent<Vector4> e) => callback());
            field.RegisterCallback((ChangeEvent<Rect> e) => callback());
            field.RegisterCallback((ChangeEvent<AnimationCurve> e) => callback());
            field.RegisterCallback((ChangeEvent<Bounds> e) => callback());
            field.RegisterCallback((ChangeEvent<Gradient> e) => callback());
            field.RegisterCallback((ChangeEvent<Quaternion> e) => callback());
            field.RegisterCallback((ChangeEvent<Vector2Int> e) => callback());
            field.RegisterCallback((ChangeEvent<Vector3Int> e) => callback());
            field.RegisterCallback((ChangeEvent<RectInt> e) => callback());
            field.RegisterCallback((ChangeEvent<BoundsInt> e) => callback());
        }

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
        /// <param name="element">The target element in which the bounds are used to activate the toggle countdown</param>
        /// <param name="toggleTarget">The target toggle in which to set its value within N milliseconds of the mouse leaving this  elements bounds  </param>
        /// <param name="autoToggleValue">The value in which to set the Toggle after toggleTimer completes</param>
        /// <param name="toggleTimer">The amount of time in which to wait before the toggles value is changed.
        /// Default: 1000ms</param>
        /// <param name="interruptible">Whether the automatic toggle should be interrupted if the cursor is placed back into the bounds of the target element</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T AutoToggleAfter<T>(this T element, Toggle toggleTarget, long toggleTimer = 1000, bool interruptible = false, bool autoToggleValue = false)
            where T : VisualElement
        {
            bool interrupter = false;
            IVisualElementScheduledItem menuCloser = element.schedule.Execute(() =>
            {
                if (interrupter == false && toggleTarget.value) toggleTarget.value = autoToggleValue;
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

                if (toggleTarget.value)
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
            var tmpNum = (float)num;
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

        public static Action SetBorderColorAction<T>(this T element, Color color = default) where T : VisualElement
        {
            void DoSetBorderColor()
            {
                element.style.borderBottomColor = color;
                element.style.borderLeftColor = color;
                element.style.borderRightColor = color;
                element.style.borderTopColor = color;
            }

            return DoSetBorderColor;
        }

        
        // --------------------------------- Base Style Elements
        /// <summary>
        /// Set <see cref="Label"/> element text value 
        /// </summary>
        /// <param name="element">The element in which to set the text</param>
        /// <param name="value">String value in which ot set the text</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T SetText<T>(this T element, string value) where T : Label
        {
            element.text = value;
            return element;
        }
        
        public static T SetDisplay<T>(this T element, bool value) where T : VisualElement
        {
            element.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            return element;
        }
        
        /// <summary>
        /// Get element current display value
        /// </summary>
        /// <param name="element">The element in which to get display status</param>
        /// <typeparam name="T">VisualElement</typeparam>
        /// <returns>Bool value of element display status</returns>
        public static bool GetDisplay<T>(this T element) where T : VisualElement
        {
            return element.resolvedStyle.display == DisplayStyle.Flex ;
        }
        
        /// <summary>
        /// Adds a border to all sides of the VisualElement
        /// </summary>
        /// <param name="element">The target element to add a border</param>
        /// <param name="opacity">The value in which to set the border thickness</param>
        /// <typeparam name="T">VisualElement</typeparam>
        public static T SetOpacity<T>(this T element, float opacity = 0) where T : VisualElement
        {
            if (opacity == 0) opacity = opacity.Zero();
            element.style.opacity = opacity;
            return element;
        }

        #endregion


        public static void SetStyleValue<T>(this IStyle style, string propertyName, T value)
        {
            typeof(IStyle).GetProperty(propertyName)?.SetValue(style, value);
        }

        public static T Set<T>(this T v,
        string name = null,
        string _class = null,
        FlexDirection? flexDirection = null,
        Justify? justifyContent = null,
        Align? alignItems = null,
        string background_image = null,
        float? flexGrow = null,
        float? maxHeight = null,
        float? maxWidth = null,
        float? height = null,
        float? width = null,
        Color? color = null,
        ScaleMode? unityBackgroundScaleMode = null,
        DisplayStyle? display = null) where T : VisualElement
        {
            if (name != null)
                v.name = name;
            if (_class != null)
                v.AddToClassList(_class);
            if (flexDirection.HasValue)
                v.style.flexDirection = new StyleEnum<FlexDirection>(flexDirection.Value);
            if (alignItems.HasValue)
                v.style.alignItems = new StyleEnum<Align>(alignItems.Value);
            if (flexGrow.HasValue)
                v.style.flexGrow = new StyleFloat(flexGrow.Value);
#if UNITY_EDITOR
            if (background_image != null)
                v.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(background_image));
#endif
            if (maxHeight.HasValue)
                v.style.maxHeight = maxHeight.Value;
            if (maxWidth.HasValue)
                v.style.maxWidth = maxWidth.Value;
            if (height.HasValue)
                v.style.height = height.Value;
            if (width.HasValue)
                v.style.width = width.Value;
            if (justifyContent.HasValue)
                v.style.justifyContent = new StyleEnum<Justify>(justifyContent.Value);
            if (color.HasValue)
                v.style.color = new StyleColor(color.Value);
            if (unityBackgroundScaleMode.HasValue)
                v.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(unityBackgroundScaleMode.Value);
            if (display.HasValue)
                v.style.display = display.Value;
            return v;
        }

        public static T Set<T>(this T v, string text = null) where T : TextElement
        {
            if (text != null)
                v.text = text;
            return v;
        }

        public static T AssignTo<T>(this T v, out T reference) where T : VisualElement
        {
            reference = v;
            return v;
        }

        public static VisualElement AddRange(this VisualElement v, params VisualElement[] elements)
        {
            foreach (var el in elements)
                v.Add(el);
            return v;
        }

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
        public static T FromStyleData<T>(this VisualElementStyleStore source, T target) where T : VisualElement
        {
            var styleValues = new Dictionary<string, object>
            {
                { nameof(target.style.width), target.style.width = source.Width },
                { nameof(target.style.height), target.style.height = source.Height },
                {
                    nameof(target.style.maxWidth), target.style.maxWidth =
                        (source.MaxWidth.value == 0 || source.MaxWidth == StyleKeyword.Null) ? new StyleLength(StyleKeyword.Auto) : source.MaxWidth.value
                },
                { nameof(target.style.maxHeight), target.style.maxHeight = source.MaxHeight.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.MaxHeight.value },
                { nameof(target.style.minWidth), target.style.minWidth = source.MinWidth.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.MinWidth.value },
                { nameof(target.style.minHeight), target.style.minHeight = source.MinHeight.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.MinHeight.value },
                { nameof(target.style.flexBasis), target.style.flexBasis = source.FlexBasis.value == 0 ? new StyleLength(StyleKeyword.Auto) : source.FlexBasis.value },
                { nameof(target.style.flexGrow), target.style.flexGrow = source.FlexGrow },
                { nameof(target.style.flexShrink), target.style.flexShrink = source.FlexShrink },
                { nameof(target.style.flexDirection), target.style.flexDirection = (FlexDirection)source.FlexDirection.value },
                { nameof(target.style.flexWrap), target.style.flexWrap = (Wrap)source.FlexWrap.value },
                { nameof(target.style.marginLeft), target.style.marginLeft = source.MarginLeft },
                { nameof(target.style.marginTop), target.style.marginTop = source.MarginTop },
                { nameof(target.style.marginRight), target.style.marginRight = source.MarginRight },
                { nameof(target.style.marginBottom), target.style.marginBottom = source.MarginBottom },
                { nameof(target.style.paddingLeft), target.style.paddingLeft = source.PaddingLeft },
                { nameof(target.style.paddingTop), target.style.paddingTop = source.PaddingTop },
                { nameof(target.style.paddingRight), target.style.paddingRight = source.PaddingRight },
                { nameof(target.style.paddingBottom), target.style.paddingBottom = source.PaddingBottom },
                { nameof(target.style.alignSelf), target.style.alignSelf = (Align)source.AlignSelf.value },
                { nameof(target.style.unityTextAlign), target.style.unityTextAlign = (TextAnchor)source.UnityTextAlign.value },
                { nameof(target.style.unityFontStyleAndWeight), target.style.unityFontStyleAndWeight = (FontStyle)source.UnityFontStyleAndWeight.value },
                { nameof(target.style.fontSize), target.style.fontSize = source.FontSize },
                { nameof(target.style.whiteSpace), target.style.whiteSpace = (WhiteSpace)source.WhiteSpace.value },
                { nameof(target.style.color), target.style.color = source.Color },
                { nameof(target.style.backgroundColor), target.style.backgroundColor = source.BackgroundColor },
                { nameof(target.style.unityFont), target.style.unityFont = source.UnityFont },
                { nameof(target.style.unityBackgroundScaleMode), target.style.unityBackgroundScaleMode = (ScaleMode)source.UnityBackgroundScaleMode.value },
                { nameof(target.style.unityBackgroundImageTintColor), target.style.unityBackgroundImageTintColor = source.UnityBackgroundImageTintColor },
                { nameof(target.style.alignItems), target.style.alignItems = (Align)source.AlignItems.value },
                { nameof(target.style.alignContent), target.style.alignContent = (Align)source.AlignContent.value },
                { nameof(target.style.justifyContent), target.style.justifyContent = (Justify)source.JustifyContent.value },
                { nameof(target.style.borderLeftColor), target.style.borderLeftColor = source.BorderLeftColor },
                { nameof(target.style.borderRightColor), target.style.borderRightColor = source.BorderRightColor },
                { nameof(target.style.borderTopColor), target.style.borderTopColor = source.BorderTopColor },
                { nameof(target.style.borderBottomColor), target.style.borderBottomColor = source.BorderBottomColor },
                { nameof(target.style.borderLeftWidth), target.style.borderLeftWidth = source.BorderLeftWidth },
                { nameof(target.style.borderRightWidth), target.style.borderRightWidth = source.BorderRightWidth },
                { nameof(target.style.borderTopWidth), target.style.borderTopWidth = source.BorderTopWidth },
                { nameof(target.style.borderBottomWidth), target.style.borderBottomWidth = source.BorderBottomWidth },
                { nameof(target.style.borderTopLeftRadius), target.style.borderTopLeftRadius = source.BorderTopLeftRadius },
                { nameof(target.style.borderTopRightRadius), target.style.borderTopRightRadius = source.BorderTopRightRadius },
                { nameof(target.style.borderBottomLeftRadius), target.style.borderBottomLeftRadius = source.BorderBottomLeftRadius },
                { nameof(target.style.borderBottomRightRadius), target.style.borderBottomRightRadius = source.BorderBottomRightRadius },
                { nameof(target.style.opacity), target.style.opacity = source.Opacity.value },
                { nameof(target.style.visibility), target.style.visibility = (Visibility)source.Visibility.value },
                { nameof(target.style.display), target.style.display = (DisplayStyle)source.Display.value }
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

            return target;
        }

        #endregion
    }
}
#endif
