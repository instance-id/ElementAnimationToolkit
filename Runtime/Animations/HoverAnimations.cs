// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    public static class HoverAnimations
    {
        #region Hover Animations

        // --------------------------------------------------- @HoverColor
        // ---------------------------------------------------------------
        /// <summary>
        /// Adds forecolor hover capability that will not be lost like CSS:hover when programatically setting background color
        /// </summary>
        /// <example>
        /// <code>
        /// var originalColor = ColorUtil.FromHex("#BABABA");
        /// var hoverColor = ColorUtil.FromHex("#2F569C");
        ///
        /// label.HoverColor(originalColor, hoverColor);
        /// </code>
        /// </example>
        /// <param name="target">The element in which this function will be applied</param>
        /// <param name="original">The original color of the element being changed. Can be obtained and passed via 'visualElement.style.backgroundColor.value'</param>
        /// <param name="hoverColor">The color to fade to when element is hovered</param>
        /// <param name="condition">Create a condition to pass to this function. Example: bool Condition(VisualElement sRow) => selectedRow == packageListRow;</param>
        /// <param name="conditionElement">The element in which the optional condition will be evaluated. Ex. in the example of 'bool Condition(VisualElement sRow) => selectedRow == packageListRow;', the conditionalElement would be 'VisualElement selectedRow'</param>
        /// <param name="animate">Whether to animate the transition of the background color</param>
        /// <param name="unregister">Whether this command is issued to register or unregister this event</param>
        public static void HoverColor<T>(this T target, StyleColor original, Color hoverColor, Func<T, bool> condition = null,
        T conditionElement = null, bool animate = false, bool unregister = false) where T : VisualElement
        {
            ValueAnimation<StyleValues> mouseOver = new ValueAnimation<StyleValues>();
            ValueAnimation<StyleValues> mouseOut = new ValueAnimation<StyleValues>();

            target.RegisterCallback<MouseOverEvent>(evt =>
            {
                var tar = (VisualElement)evt.target;
                if (animate)
                {
                    mouseOver = tar.AnimateColor(original.value, hoverColor, 250);
                    mouseOver.KeepAlive();
                }

                if (condition != null && condition(conditionElement)) return;
                if (mouseOut.isRunning) mouseOut.Stop();
                if (animate) tar.schedule.Execute(() => { mouseOver.Start(); }).StartingIn(50);
                else tar.style.color = hoverColor;
                evt.StopPropagation();
            });

            target.RegisterCallback<MouseOutEvent>(evt =>
            {
                var tar = (VisualElement)evt.target;
                if (animate)
                {
                    mouseOut = tar.AnimateColor(hoverColor, original.value, 250);
                    mouseOut.KeepAlive();
                }

                if (condition != null && condition(conditionElement)) return;
                if (mouseOver.isRunning) mouseOver.Stop();
                if (animate) tar.schedule.Execute(() => { mouseOut.Start(); }).StartingIn(50);
                else tar.style.color = original;
                evt.StopPropagation();
            });

            if (!unregister) return;
            {
                target.UnregisterCallback<MouseOverEvent>(evt =>
                {
                    var tar = (VisualElement)evt.target;

                    if (condition != null && condition(conditionElement)) return;
                    if (mouseOut.isRunning) mouseOut.Stop();

                    if (animate) tar.schedule.Execute(() => { mouseOver.Start(); }).StartingIn(50);
                    else tar.style.color = hoverColor;
                    evt.StopPropagation();
                });
                target.UnregisterCallback<MouseOutEvent>(evt =>
                {
                    var tar = (VisualElement)evt.target;

                    if (condition != null && condition(conditionElement)) return;
                    if (mouseOver.isRunning) mouseOver.Stop();

                    if (animate) tar.schedule.Execute(() => { mouseOut.Start(); }).StartingIn(50);
                    else tar.style.color = original;
                    evt.StopPropagation();
                });
            }
        }

        // ---------------------------------------------- @HoverBackground
        // ---------------------------------------------------------------
        /// <summary>
        /// Adds background hover capability that will not be lost like CSS:hover when programatically setting background color
        /// </summary>
        /// <example>
        /// <code>
        /// var originalColor = ColorUtil.FromHex("#BABABA");
        /// var hoverBGColor = ColorUtil.FromHex("#2F569C");
        ///
        /// label.HoverBackground(originalColor, hoverBGColor);
        /// </code>
        /// </example>>
        /// <param name="target">The element in which this function will be applied</param>
        /// <param name="original">The original color of the element being changed. Can be obtained and passed via 'visualElement.style.backgroundColor.value'</param>
        /// <param name="hoverColor">The color to fade to when element is hovered</param>
        /// <param name="condition">Create a condition to pass to this function. Example: bool Condition(VisualElement sRow) => selectedRow == packageListRow;</param>
        /// <param name="conditionElement">The element in which the optional condition will be evaluated. Ex. in the example of 'bool Condition(VisualElement sRow) => selectedRow == packageListRow;', the conditionalElement would be 'VisualElement selectedRow'</param>
        /// <param name="animate">Whether to animate the transition of the background color</param>
        public static void HoverBackground(this VisualElement target, StyleColor original, Color hoverColor, Func<VisualElement, bool> condition = null,
        VisualElement conditionElement = null, bool animate = false)
        {
            var mouseOver = new ValueAnimation<StyleValues>();
            var mouseOut = new ValueAnimation<StyleValues>();

            if (animate)
            {
                mouseOver = target.AnimateBackgroundColor(original.value, hoverColor, 250);
                mouseOver.KeepAlive();

                mouseOut = target.AnimateBackgroundColor(hoverColor, original.value, 250);
                mouseOut.KeepAlive();
            }

            target.RegisterCallback<MouseOverEvent>(evt =>
            {
                if (condition != null && condition(conditionElement)) return;
                if (mouseOut.isRunning) mouseOut.Stop();
                if (animate) target.schedule.Execute(() => { mouseOver.Start(); }).StartingIn(50);
                else target.style.backgroundColor = hoverColor;
                evt.StopPropagation();
            });
            target.RegisterCallback<MouseOutEvent>(evt =>
            {
                if (condition != null && condition(conditionElement)) return;
                if (mouseOver.isRunning) mouseOver.Stop();
                if (animate) target.schedule.Execute(() => { mouseOut.Start(); }).StartingIn(50);
                else target.style.backgroundColor = original;
                evt.StopPropagation();
            });
        }

        // -------------------------------------------------- @HoverBorder
        // ---------------------------------------------------------------
        /// <summary>
        /// Adds background hover capability that will not be lost like CSS:hover when programatically setting background color
        ///
        /// ** To help combat your element shifting position slightly when a border is applied on hover,
        /// it is a good idea to add a border to your element before hand and just set color to 'initial'
        /// so that it is transparent, then keep 'addBorder' parameter as false.
        /// </summary>
        /// <param name="target">The element in which this function will be applied</param>
        /// <param name="original">The original color of the element being changed. Can be obtained and passed via 'visualElement.style.backgroundColor.value'</param>
        /// <param name="hoverColor">The color in which the borders should be when the mouse is hovering over the target</param>
        /// <param name="addBorder">Adds a border if the element does not have one already</param>
        /// <param name="borderStartEndWidth">The width in which the borders should be when displaying</param>
        public static void HoverBorder(this VisualElement target, Color original, Color hoverColor, bool addBorder = false, Vector2 borderStartEndWidth = default)
        {
            if (borderStartEndWidth == default)
                borderStartEndWidth = new Vector2(1, 0);

            target.RegisterCallback<MouseOverEvent>(evt =>
            {
                if (addBorder)
                {
                    target.style.borderBottomWidth = borderStartEndWidth.x;
                    target.style.borderLeftWidth = borderStartEndWidth.x;
                    target.style.borderRightWidth = borderStartEndWidth.x;
                    target.style.borderTopWidth = borderStartEndWidth.x;
                }

                target.style.borderBottomColor = hoverColor;
                target.style.borderLeftColor = hoverColor;
                target.style.borderRightColor = hoverColor;
                target.style.borderTopColor = hoverColor;


                evt.StopPropagation();
            });
            target.RegisterCallback<MouseOutEvent>(evt =>
            {
                if (addBorder)
                {
                    target.style.borderBottomWidth = borderStartEndWidth.y;
                    target.style.borderLeftWidth = borderStartEndWidth.y;
                    target.style.borderRightWidth = borderStartEndWidth.y;
                    target.style.borderTopWidth = borderStartEndWidth.y;
                }


                target.style.borderBottomColor = original;
                target.style.borderLeftColor = original;
                target.style.borderRightColor = original;
                target.style.borderTopColor = original;
                evt.StopPropagation();
            });
        }

        // -------------------------------------------------- @HoverBorder
        // ---------------------------------------------------------------
        /// <summary>
        /// Adds background hover capability that will not be lost like CSS:hover when programatically setting background color
        /// 
        /// ** To help combat your element shifting position slightly when a border is applied on hover,
        /// it is a good idea to add a border to your element before hand and just set color to 'initial'
        /// so that it is transparent, then keep 'addBorder' parameter as false.
        /// </summary>
        /// <param name="target">The element in which this function will be applied</param>
        /// <param name="pulseStartColor">The first color</param>
        /// <param name="pulseEndColor">The second color</param>
        /// <param name="original">The original color of the element being changed. Can be obtained and passed via 'visualElement.style.backgroundColor.value'</param>
        /// <param name="addBorder">Adds a border if the element does not have one already</param>
        /// <param name="borderStartEndWidth">The width in which the borders should be when displaying</param>
        /// <param name="colorDuration">The duration of each colors cycle</param>
        /// <param name="includeChildren">Register child elements for the hover event</param>
        /// <param name="stopPropagation">Stop event from propagating further </param>
        /// <returns>Tuple of type (EventCallback&lt;MouseOverEvent&gt; mouseOverEvent, EventCallback&lt;MouseOutEvent&gt; mouseOutEvent) which can be used for manually invoking or unregistering the events when no longer needed</returns>
        /// <example>
        ///<code>
        /// var myHoverEvents = myVisualElement.HoverBorderPulse("#2F569C".FromHex(), "#D2A00C".FromHex(), includeChildren: true);
        /// ...
        /// myVisualElement.UnregisterCallback(myHoverEvents.mouseOverEvent, true);
        /// myVisualElement.UnregisterCallback(myHoverEvents.mouseOutEvent, true);
        /// </code>
        /// </example>
        public static AnimatedItems<MouseOverEvent,MouseOutEvent>
            HoverBorderPulse(
            this VisualElement target,
            Color pulseStartColor,
            Color pulseEndColor,
            Color original = default,
            bool addBorder = false,
            Vector2 borderStartEndWidth = default,
            int colorDuration = 1000,
            bool includeChildren = true,
            bool stopPropagation = true)
        {
            if (borderStartEndWidth == default)
                borderStartEndWidth = new Vector2(1, 0);

            var pulseIn = new ValueAnimation<StyleValues>();
            var pulseOut = new ValueAnimation<StyleValues>();
            IVisualElementScheduledItem repeatedAnim = null;
            var doHover = false;

            EventCallback<MouseOverEvent> mouseOverEvent = evt =>
            {
                repeatedAnim = null;
                doHover = true;
                if (addBorder)
                {
                    target.style.borderBottomWidth = borderStartEndWidth.x;
                    target.style.borderLeftWidth = borderStartEndWidth.x;
                    target.style.borderRightWidth = borderStartEndWidth.x;
                    target.style.borderTopWidth = borderStartEndWidth.x;
                }

                // -- Pulse color will fade original => desired color   --
                // -- via the AnimateTo local function. Once completed  --
                // -- the AnimateFrom function is called animating back --
                // -- to the original color. This is then repeated for  --
                // -- as long as the mouse is hovered over the target   --
                void PulseIn(in ValueAnimation<StyleValues> pulse)
                {
                    if (pulse.isRunning) pulse.Stop();
                    pulseIn = target.AnimateBorderColor(pulseStartColor, pulseEndColor, colorDuration, () => PulseOut(pulseIn));
                }

                void PulseOut(in ValueAnimation<StyleValues> pulse)
                {
                    if (pulse.isRunning) pulse.Stop();
                    pulseOut = target.AnimateBorderColor(pulseEndColor, pulseStartColor, colorDuration);
                }

                if (pulseIn.isRunning) pulseIn.Stop();
                if (pulseOut.isRunning) pulseOut.Stop();
                repeatedAnim = target.schedule.Execute(() => PulseIn(pulseOut)).StartingIn(0).Every(colorDuration * 2 + 20).Until(() => !doHover);

                if (stopPropagation) evt.StopPropagation();
            };

            EventCallback<MouseOutEvent> mouseOutEvent = evt =>
            {
                doHover = false;
                if (pulseIn.isRunning) pulseIn?.Stop();
                if (pulseOut.isRunning) pulseOut?.Stop();
                if (repeatedAnim.isActive)
                {
                    repeatedAnim.Pause();
                }

                if (addBorder)
                {
                    target.style.borderBottomWidth = borderStartEndWidth.y;
                    target.style.borderLeftWidth = borderStartEndWidth.y;
                    target.style.borderRightWidth = borderStartEndWidth.y;
                    target.style.borderTopWidth = borderStartEndWidth.y;
                }

                if (pulseIn.isRunning) pulseIn.Stop();
                if (pulseOut.isRunning) pulseOut.Stop();

                target.style.borderBottomColor = original;
                target.style.borderLeftColor = original;
                target.style.borderRightColor = original;
                target.style.borderTopColor = original;
                if (stopPropagation) evt.StopPropagation();
            };

            target.RegisterCallback(mouseOverEvent, includeChildren);
            target.RegisterCallback(mouseOutEvent, includeChildren);

            return new AnimatedItems<MouseOverEvent,MouseOutEvent>(target)
            {
                AnimatedItemList = new List<ValueAnimation<StyleValues>> { pulseIn, pulseOut }, 
                EventCallbacks = (mouseOverEvent, mouseOutEvent) 
            };
        }

        #region Unregister

        public static void HoverBorderPulseUnregister(this VisualElement target, Color pulseStartColor, Color pulseEndColor, Color original = default, bool addBorder = false,
        Vector2 borderStartEndWidth = default, int colorDuration = 1000, bool includeChildren = true, bool stopPropagation = true)
        {
            if (borderStartEndWidth == default)
                borderStartEndWidth = new Vector2(1, 0);

            var pulseIn = new ValueAnimation<StyleValues>();
            var pulseOut = new ValueAnimation<StyleValues>();
            IVisualElementScheduledItem repeatedAnim = null;
            var doHover = false;

            target.UnregisterCallback<MouseOverEvent>(evt =>
            {
                repeatedAnim = null;
                doHover = true;
                if (addBorder)
                {
                    target.style.borderBottomWidth = borderStartEndWidth.x;
                    target.style.borderLeftWidth = borderStartEndWidth.x;
                    target.style.borderRightWidth = borderStartEndWidth.x;
                    target.style.borderTopWidth = borderStartEndWidth.x;
                }

                // -- Pulse color will fade original => desired color   --
                // -- via the AnimateTo local function. Once completed  --
                // -- the AnimateFrom function is called animating back --
                // -- to the original color. This is then repeated for  --
                // -- as long as the mouse is hovered over the target   --
                void PulseIn(in ValueAnimation<StyleValues> pulse)
                {
                    if (pulse.isRunning) pulse.Stop();
                    pulseIn = target.AnimateBorderColor(pulseStartColor, pulseEndColor, colorDuration, () => PulseOut(pulseIn));
                }

                void PulseOut(in ValueAnimation<StyleValues> pulse)
                {
                    if (pulse.isRunning) pulse.Stop();
                    pulseOut = target.AnimateBorderColor(pulseEndColor, pulseStartColor, colorDuration);
                }

                if (pulseIn.isRunning) pulseIn.Stop();
                if (pulseOut.isRunning) pulseOut.Stop();
                repeatedAnim = target.schedule.Execute(() => PulseIn(pulseOut)).StartingIn(0).Every(colorDuration * 2 + 20).Until(() => !doHover);

                if (stopPropagation) evt.StopPropagation();
            }, includeChildren);
            target.UnregisterCallback<MouseOutEvent>(evt =>
            {
                doHover = false;
                if (pulseIn.isRunning) pulseIn?.Stop();
                if (pulseOut.isRunning) pulseOut?.Stop();
                if (repeatedAnim.isActive)
                {
                    repeatedAnim.Pause();
                }

                if (addBorder)
                {
                    target.style.borderBottomWidth = borderStartEndWidth.y;
                    target.style.borderLeftWidth = borderStartEndWidth.y;
                    target.style.borderRightWidth = borderStartEndWidth.y;
                    target.style.borderTopWidth = borderStartEndWidth.y;
                }

                if (pulseIn.isRunning) pulseIn.Stop();
                if (pulseOut.isRunning) pulseOut.Stop();


                target.style.borderBottomColor = original;
                target.style.borderLeftColor = original;
                target.style.borderRightColor = original;
                target.style.borderTopColor = original;

                if (stopPropagation) evt.StopPropagation();
            }, includeChildren);
        }

        #endregion

        // -------------------------------------------------------- @HoverWidth
        // -- Animate the width of target element to desired value on hover  --
        // --------------------------------------------------------------------
        public static AnimatedItems HoverWidth(this VisualElement target, float initialWidth = 0f, float desiredWidth = 100f, int duration = 1000, Action hoverCallback = null,
        Action leaveCallback = null,
        bool afterAnimation = false,
        bool includeChildren = true,
        bool stopPropagation = true)
        {
            initialWidth = initialWidth == 0f ? target.resolvedStyle.width : initialWidth;
            var enterAnim = new ValueAnimation<StyleValues>();
            var leaveAnim = new ValueAnimation<StyleValues>();

            enterAnim = afterAnimation
                ? target.AnimateWidth(initialWidth, desiredWidth, duration, callback: hoverCallback)
                : target.AnimateWidth(initialWidth, desiredWidth, duration);
            enterAnim.KeepAlive();

            leaveAnim = target.AnimateWidth(desiredWidth, initialWidth, duration, easing: Easy.EaseInOutQuint);
            leaveAnim.KeepAlive();

            void MouseEnter()
            {
                enterAnim.Start();
            } // @formatter:off
            void MouseLeave() { leaveAnim.Start(); } // @formatter:on

            EventCallback<MouseOverEvent> mouseOverEvent = evt => { 
                if (enterAnim.isRunning) enterAnim.Stop();
                if (leaveAnim.isRunning) leaveAnim.Stop();
                MouseEnter();
                if (!afterAnimation) hoverCallback?.Invoke();
                if (stopPropagation) evt.StopPropagation(); 
            };
            
            EventCallback<MouseOutEvent> mouseOutEvent = evt =>
            {
                if (enterAnim.isRunning) enterAnim.Stop();
                if (leaveAnim.isRunning) leaveAnim.Stop();
                MouseLeave();
                if (stopPropagation) evt.StopPropagation();
                target.schedule.Execute(leaveCallback).StartingIn(duration);
            };

            target.RegisterCallback(mouseOverEvent, includeChildren);
            target.RegisterCallback(mouseOutEvent, includeChildren);

            return new AnimatedItems<MouseOverEvent,MouseOutEvent>(target)
            {
                AnimatedItemList = new List<ValueAnimation<StyleValues>> { enterAnim, leaveAnim }, 
                EventCallbacks =  (mouseOverEvent, mouseOutEvent)
            };
        }

        // ------------------------------------------------------- @HoverHeight
        // -- Animate the Height of target element to desired value on hover --
        // --------------------------------------------------------------------
        public static AnimatedItems HoverHeight(this VisualElement target, float initialHeight = 0f, float desiredHeight = 100f, int duration = 1000, Action hoverCallback = null,
        Action leaveCallback = null,
        bool afterAnimation = false)
        {
            initialHeight = initialHeight == 0f ? target.resolvedStyle.height : initialHeight;
            var enterAnim = new ValueAnimation<StyleValues>();
            var leaveAnim = new ValueAnimation<StyleValues>();

            void MouseEnter()
            {
                enterAnim = afterAnimation
                    ? target.AnimateHeight(initialHeight, desiredHeight, duration, hoverCallback)
                    : target.AnimateHeight(initialHeight, desiredHeight, duration);
            } // @formatter:off
            void MouseLeave() { leaveAnim = target.AnimateHeight(desiredHeight, initialHeight, duration); } // @formatter:on

            EventCallback<MouseOverEvent> mouseOverEvent = evt =>
            {
                if (enterAnim.isRunning) enterAnim.Stop();
                if (leaveAnim.isRunning) leaveAnim.Stop();
                MouseEnter();
                if (!afterAnimation) hoverCallback?.Invoke();
                evt.StopPropagation();
            };
            
            EventCallback<MouseOutEvent> mouseOutEvent = evt =>
            {
                if (enterAnim.isRunning) enterAnim.Stop();
                if (leaveAnim.isRunning) leaveAnim.Stop();
                MouseLeave();
                evt.StopPropagation();
                target.schedule.Execute(leaveCallback).StartingIn(duration);
            };

            target.RegisterCallback(mouseOverEvent);
            target.RegisterCallback(mouseOutEvent);
            
            return new AnimatedItems<MouseOverEvent,MouseOutEvent>(target)
            {
                AnimatedItemList = new List<ValueAnimation<StyleValues>> { enterAnim, leaveAnim }, 
                EventCallbacks = (mouseOverEvent, mouseOutEvent) 
            };
        }

        // ------------------------------------------------- @HoverToolTip
        // ---------------------------------------------------------------
        /// <summary>
        /// Display tooltip within desired label
        /// </summary>
        /// <param name="target">The Element in which the cursor will hover</param>
        /// <param name="callback">The function to send the tooltip to the external label</param>
        public static void HoverToolTip(this VisualElement target, Action callback)
        {
            target.RegisterCallback<MouseOverEvent>(evt =>
            {
                callback?.Invoke();
                evt.StopPropagation();
            });
        }

        #endregion
    }
}
#endif
