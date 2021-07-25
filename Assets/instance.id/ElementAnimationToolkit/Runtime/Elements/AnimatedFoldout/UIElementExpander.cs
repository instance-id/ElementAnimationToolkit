// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using instance.id.EATK;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    public class Expander : VisualElement
    {
        ValueAnimation<StyleValues> foldoutAnimation;
        private readonly Toggle expandToggle;
        private readonly VisualElement expandContainerItems;
        public VisualElement shownItem;
        [CanBeNull] public Action expandTrigger;
        private int tmpAnimTime = 0;

        public bool startExpanded { get; set; } = false;
        private bool firstStart = false;
        private bool isAnimating;

        public bool IsAnimating
        {
            get => isAnimating;
            set => AnimationStatus?.Invoke(isAnimating = value);
        }

        public Action<bool> AnimationStatus;

        private int m_AnimationTime = 500;

        public int animationTime
        {
            get => firstStart ? tmpAnimTime : m_AnimationTime;
            [UsedImplicitly] set => m_AnimationTime = value;
        }

        public Expander()
        {
            expandToggle = new Toggle {style = {display = DisplayStyle.None}, name = "ExpandToggle"};
            expandToggle.RegisterValueChangedCallback(ExpandContainerValueChanges);

            expandContainerItems = new VisualElement
            {
                name = "expandContainer",
                style = {overflow = Overflow.Hidden}
            };
            expandContainerItems.AddToClassList("expandContainer");
            expandContainerItems.Add(shownItem);

            Add(expandToggle);
            Add(expandContainerItems);
            Add(shownItem);

            expandContainerItems.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Trigger expansion or collapse of the expansion group
        /// </summary>
        public void Activate()
        {
            expandToggle.value = !expandToggle.value;
        }

        /// <summary>
        /// Manually trigger expansion or collapse of the expansion group via parameter
        /// </summary>
        /// <param name="value">Expands on true, collapses on false</param>
        public void Activate(bool value)
        {
            // IsAnimating = true;
            expandToggle.value = value;
        }

        /// <summary>
        /// Add elements to expansion group. Elements that are added will be displayed when Expander is expanded, and hidden when collapsed.
        /// </summary>
        /// <param name="element">A single element or container which will be added to the expansion group</param>
        public void AddToExpansionGroup(VisualElement element)
        {
            expandContainerItems.Add(element);
        }

        public void TriggerExpanderResize(ChangeEvent<bool> eventValue)
        {
            ExpandContainerValueChanges(eventValue);
        }

        /// <summary>
        /// Trigger the expansion container to resize. Needed when child elements change size.
        /// </summary>
        /// <param name="eventValue">Default is true, which will make the container resize. If false, container will animate closed</param>
        public void TriggerExpanderResize(bool eventValue = true)
        {
            ExpandContainerValueChanges(eventValue);
        }

        /// <summary>
        /// Manually trigger the OnGeometryChangedEvent event.
        /// </summary>
        /// <param name="eventValue">GeometryChangedEvent</param>
        public void TriggerGeometryChange(GeometryChangedEvent eventValue)
        {
            OnGeometryChangedEvent(eventValue);
        }

        private void AnimationComplete()
        {
            IsAnimating = false;
        }

        private void ExpandContainerValueChanges(ChangeEvent<bool> evt)
        {
            if (style.display == DisplayStyle.None) style.display = DisplayStyle.Flex;
            if (expandContainerItems.style.display == DisplayStyle.None) expandContainerItems.style.display = DisplayStyle.Flex;

            if (foldoutAnimation != null)
            {
                foldoutAnimation.Recycle();
                foldoutAnimation = null;
            }

            if (evt.newValue)
            {
                expandContainerItems.style.height = StyleKeyword.Auto;
                expandContainerItems.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            }
            else
            {
                void HideContents()
                {
                    AnimationComplete();
                    expandContainerItems.style.display = DisplayStyle.None;
                }

                IsAnimating = true;
                foldoutAnimation =
                    expandContainerItems.experimental.animation.Start(new StyleValues
                        {
                            height = expandContainerItems.layout.height
                        }, new StyleValues {height = 0}, animationTime)
                        .Ease(Easy.EaseInOutQuint)
                        .OnCompleted(HideContents);
                foldoutAnimation.KeepAlive();
            }
        }

        private void ExpandContainerValueChanges(bool evt)
        {
            if (style.display == DisplayStyle.None) style.display = DisplayStyle.Flex;
            if (expandContainerItems.style.display == DisplayStyle.None) expandContainerItems.style.display = DisplayStyle.Flex;

            if (foldoutAnimation != null)
            {
                foldoutAnimation.Recycle();
                foldoutAnimation = null;
            }

            if (evt)
            {
                expandContainerItems.style.height = StyleKeyword.Auto;
                expandContainerItems.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            }
            else
            {
                IsAnimating = true;
                foldoutAnimation =
                    expandContainerItems.experimental.animation
                        .Start(new StyleValues {height = expandContainerItems.layout.height}, new StyleValues {height = 0}, firstStart ? tmpAnimTime : m_AnimationTime)
                        .Ease(Easy.EaseInOutQuint)
                        .OnCompleted(AnimationComplete);
                foldoutAnimation.KeepAlive();
            }
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            IsAnimating = true;
            foldoutAnimation =
                expandContainerItems.experimental.animation
                    .Start(new StyleValues {height = evt.oldRect.height}, new StyleValues {height = evt.newRect.height}, firstStart ? tmpAnimTime : m_AnimationTime)
                    .Ease(Easy.EaseInOutQuint)
                    .OnCompleted(AnimationComplete);

            expandContainerItems.style.height = evt.oldRect.height;

            foldoutAnimation.KeepAlive();
            expandContainerItems.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }
    }
}
