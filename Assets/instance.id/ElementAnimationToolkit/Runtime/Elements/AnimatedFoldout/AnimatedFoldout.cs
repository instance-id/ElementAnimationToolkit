// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using instance.id.EATK.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK
{
    /// <summary>
    ///   <para>Collapsable section of UI.</para>
    /// </summary>
    public class AnimatedFoldout : BindableElement, INotifyValueChanged<bool>
    {
        internal static readonly string ussFoldoutDepthClassName = "unity-foldout--depth-";
        internal static readonly int ussFoldoutMaxDepth = 4;
        private Toggle m_Toggle;
        public Expander expander;
        private VisualElement m_Container;
        private VisualElement headerBorderLine;
        private VisualElement resolvableElement;
        private AnimValueStore<float> headerBorderValues;

        private bool m_Value;
        private bool isAnimating;
        public bool toggleAsHeader = true;
        public bool startExpand = false;
        public bool disableAnimation = false;

        public Action stateChange = () => { };

        public Action OnComplete
        {
            get => expander.onComplete;
            set => expander.onComplete = value;
        }

        public int initialHeaderBorderWidth = default;
        private int initialHeaderWidth;
        public static readonly string toggleName = "ExpandToggle";

        /// <summary>
        ///   <para>USS class name of elements of this type.</para>
        /// </summary>
        public static readonly string ussClassName = "unity-foldout";

        /// <summary>
        ///   <para>USS class name of toggle elements in elements of this type.</para>
        /// </summary>
        public static readonly string toggleUssClassName = ussClassName + "__toggle";

        /// <summary>
        ///   <para>USS class name of content element in a AnimatedFoldout.</para>
        /// </summary>
        public static readonly string contentUssClassName = ussClassName + "__content";

        /// <summary>
        ///   <para>USS class name of expander element in a AnimatedFoldout.</para>
        /// </summary>
        public static readonly string expanderUssClassName = ussClassName + "__expander";

        private string toggleLabelUssClass;

        public string ToggleLabelUssClass
        {
            get => toggleLabelUssClass;
            set
            {
                toggleLabelUssClass = value;
                m_Toggle.Query<Label>().First().AddToClassList(toggleLabelUssClass);
            }
        }

        /// <summary>
        /// The container element that holds the AnimatedFoldout's child objects
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        public string text
        {
            get => m_Toggle.text;
            set => m_Toggle.text = value;
        }

        /// <summary>
        ///   <para>Contains the collapse state. True if the AnimatedFoldout is open and the contents are visible. False if it's collapsed.</para>
        /// </summary>
        public bool value
        {
            get => m_Value;
            set
            {
                if (m_Value == value) return;
                using (ChangeEvent<bool> pooled = ChangeEvent<bool>.GetPooled(m_Value, value))
                {
                    pooled.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(pooled);
                }

                stateChange.Invoke();
            }
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            m_Value = newValue;
            value = newValue;
            expander.startExpanded = newValue;
        }

        public AnimatedFoldout() => BuildFoldout();
        public AnimatedFoldout(bool startExpanded = false) => BuildFoldout(startExpanded: startExpanded);

        public AnimatedFoldout(VisualElement headerElement, FlexDirection direction = FlexDirection.Column, Align alignSelf = default) =>
            BuildFoldout(headerElement, direction, alignSelf);

        public AnimatedFoldout(VisualElement headerElement, bool replaceHeader, FlexDirection direction = FlexDirection.Column, Align alignSelf = default, VisualElement resolvableElement = default) =>
            BuildFoldout(headerElement, direction, alignSelf, replaceHeader: replaceHeader, resolvable: resolvableElement, headerBorderWidth: 0);

        private void BuildFoldout(
            VisualElement headerElement = default,
            FlexDirection direction = FlexDirection.Column,
            Align alignSelf = default,
            int headerBorderWidth = default,
            bool startExpanded = false,
            bool replaceHeader = default,
            VisualElement resolvable = default)
        {
            startExpand = startExpanded;
            initialHeaderBorderWidth = headerBorderWidth;
            resolvableElement = resolvable;
            m_Value = false;
            AddToClassList(ussClassName);
            new Expander().Create(out expander).ToUSS(nameof(expander));

            new VisualElement().Create(out headerBorderLine).ToUSS(nameof(headerBorderLine));
            new Toggle {value = false, name = toggleName}.Create(out m_Toggle, name: toggleName).ToUSS(toggleUssClassName, "expandToggle")
                .RegisterValueChangedCallback(evt =>
                {
                    if (evt.currentTarget != evt.target) return;
                    value = evt.newValue;
                    expander.Activate(value);
                    if (toggleAsHeader)
                        ToggleHeaderAnimation(value);

                    evt.StopPropagation();
                });

            m_Toggle.name = toggleName;
            hierarchy.Add(m_Toggle);
            if (toggleAsHeader) hierarchy.Add(headerBorderLine);

            var checkmarkParent = m_Toggle.Query("unity-checkmark").First().parent;
            checkmarkParent.name = toggleAsHeader ? "ExpandToggleContainerHeader" : "ExpandToggleContainer";
            checkmarkParent.AddToClassList(toggleAsHeader ? "expandToggleContainerHeader" : "expandToggleContainer");

            if (headerElement != default)
            {
                if (direction == FlexDirection.Row)
                {
                    checkmarkParent.Add(headerElement);
                    m_Toggle.contentContainer.Add(headerElement);
                    checkmarkParent.style.flexGrow = 1;
                }
                else
                {
                    checkmarkParent.style.flexGrow = 0;
                    if (alignSelf != default) m_Toggle.style.alignSelf = alignSelf;
                }
            }

            new VisualElement().Create(out m_Container, "unity-content").ToUSS(contentUssClassName);
            expander.AddToExpansionGroup(m_Container);
            hierarchy.Add(expander);

            expander.AddToClassList("variables");
            expander.RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);

            RegisterCallback(new EventCallback<AttachToPanelEvent>(OnAttachToPanel));
            RegisterCallback<GeometryChangedEvent>(evt => DeferredExecution(evt, headerElement));
            if (startExpand)
                SetValueWithoutNotify(true);
        }

        private void DeferredExecution(GeometryChangedEvent evt, VisualElement headerElement)
        {
            UnregisterCallback<GeometryChangedEvent>(e => DeferredExecution(e, headerElement));
            headerBorderLine.style.maxWidth = new StyleLength(Length.Percent(100));
            GetStyles();

            var label = this.Query<Label>().First();
            label.AddToClassList("animatedFoldoutLabel");
            this.Query("unity-checkmark").First().style.alignSelf = Align.Center;
            var foContainer = this.Query<VisualElement>("unity-foldout__content").First();

            foContainer.style.flexShrink = 0;
            foContainer.style.flexGrow = 1;

            foContainer.contentContainer.Children().forEach(x => x.AddToClassList("expandContainerItem"));

            if (disableAnimation) expander.animationTime = 0;
            if (headerElement != default) headerElement.BringToFront();
        }

        // --| Get StyleSheet -------------------------------------------------
        // --| ----------------------------------------------------------------
        private void GetStyles()
        {
            // ReSharper disable once Unity.UnknownResource
            var sheet = Resources.Load<StyleSheet>("Style/AnimatedFoldoutStyle");
            if (sheet != null) styleSheets.Add(sheet);
            else Debug.Log("StyleSheet 'Style/AnimatedFoldoutStyle' not found");
        }

        // --| Header Animation -----------------------------------------------
        // --| ----------------------------------------------------------------
        private void ToggleHeaderAnimation(bool val)
        {
            float percent = default;
            if (resolvableElement != null)
            {
                percent = resolvableElement.resolvedStyle.width;
            }
            else percent = (headerBorderLine.parent.resolvedStyle.width * 0.9999f);

            if (headerBorderValues != null)
            {
                headerBorderValues.final = percent;
                if (val)
                {
                    headerBorderLine.AddToClassList("headerBorderLineExpanded");
                    headerBorderLine.Animate(headerBorderValues).OnCompleted(() =>
                    {
                        headerBorderLine.style.width = new StyleLength(StyleKeyword.Auto);
                    });
                }
                else
                {
                    headerBorderLine.RemoveFromClassList("headerBorderLineExpanded");
                    headerBorderLine.Animate(headerBorderValues, reverse: true);
                }
            }
        }

        // --| Manually Expand/Collapse -------------------
        // --|---------------------------------------------
        public void Expand() => value = !value;
        public void Expand(bool expandValue, bool animate = true)
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>
            {
#endif
                expander.Animate = animate;
                m_Toggle.value = expandValue;
#if UNITY_EDITOR
            };
#endif
        }

        /// <summary> Manually trigger expansion or collapse of the expansion group via parameter </summary>
        /// <param name="rootElement">The root visual element of the binding object</param>
        public void ActivateOnStart(VisualElement rootElement) =>
            rootElement.RegisterCallback<GeometryChangedEvent>(evt => DoActivationOnStart(evt, rootElement));

        // -- Callback will call this once the editor is ready, but just before displaying, so there is no visual "pop"
        private void DoActivationOnStart(GeometryChangedEvent evt, VisualElement visualElement)
        {
            visualElement.UnregisterCallback<GeometryChangedEvent>(e => DoActivationOnStart(evt, visualElement));
            visualElement.schedule.Execute(() => { value = true; }).StartingIn(0);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            int num = 0;
            for (int index = 0; index <= ussFoldoutMaxDepth; ++index)
                RemoveFromClassList(ussFoldoutDepthClassName + index);
            RemoveFromClassList(ussFoldoutDepthClassName + "max");
            if (parent != null)
            {
                for (VisualElement getParent = parent; getParent != null; getParent = getParent.parent)
                {
                    if (getParent.GetType() == typeof(AnimatedFoldout))
                        ++num;
                }
            }

            if (num > ussFoldoutMaxDepth)
                AddToClassList(ussFoldoutDepthClassName + "max");
            else
                AddToClassList(ussFoldoutDepthClassName + num);
        }

        // --| Custom Event Handlers --------------------------------
        // --| ------------------------------------------------------
        static readonly CustomStyleProperty<string> k_headerBorderWidth = new CustomStyleProperty<string>("--headerBorderWidth");
        private bool widthIsSet;
        void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            if (widthIsSet) return;
            if (initialHeaderBorderWidth != default)
            {
                widthIsSet = true;
                var width = headerBorderLine.parent.resolvedStyle.width;
                headerBorderValues = new AnimValueStore<float>(AnimateElement.Width, initialHeaderBorderWidth, width, 500);
                return;
            }

            if (e.customStyle.TryGetValue(k_headerBorderWidth, out var headerWidth))
            {
                initialHeaderWidth = int.Parse(headerWidth.Replace("px", ""));
                headerBorderLine.style.width = initialHeaderWidth;

                var width = headerBorderLine.parent.resolvedStyle.width;
                headerBorderValues = new AnimValueStore<float>(AnimateElement.Width, initialHeaderWidth, width, 500);
            }
        }

        public VisualElement SetLabelClass(string ussClass) => this;

        private void SetPickingMode(bool animating)
        {
            m_Toggle.style.opacity = 1;
            m_Toggle.SetEnabled(!animating);
        }

        /// <summary>
        ///   <para>Instantiates an AnimatedFoldout using the data read from a UXML file.</para>
        /// </summary>
        public new class UxmlFactory : UxmlFactory<AnimatedFoldout, UxmlTraits> { }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_Text;
            private UxmlBoolAttributeDescription m_Value;

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (!(ve is AnimatedFoldout foldout))
                    return;
                foldout.text = m_Text.GetValueFromBag(bag, cc);
                foldout.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
            }

            public UxmlTraits()
            {
                UxmlStringAttributeDescription attributeDescription1 = new UxmlStringAttributeDescription();
                attributeDescription1.name = "text";
                m_Text = attributeDescription1;
                UxmlBoolAttributeDescription attributeDescription2 = new UxmlBoolAttributeDescription();
                attributeDescription2.name = "value";
                attributeDescription2.defaultValue = true;
                m_Value = attributeDescription2;
                // ISSUE: explicit constructor call
            }
        }
    }
}
#endif
