// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using instance.id.EATK.Extensions;

namespace instance.id.EATK
{
    public static class ContinuousAnimations
    {
        public static bool debug = false;

        // -------------------------------------------------- @HoverBorder
        // ---------------------------------------------------------------
        /// <summary>
        /// Pulse the  border of an element  between two colors
        ///
        /// ** To help combat your element shifting position slightly when a border is applied on hover,
        /// it is a good idea to add a border to your element before hand and just set color to 'initial'
        /// so that it is transparent, then keep 'addBorder' parameter as false.
        /// </summary>
        /// <param name="element">The element in which this function will be applied</param>
        /// <param name="color1">Color 1 in which to pulse between</param>
        /// <param name="color2">Color 2 in which to pulse between</param>
        /// <param name="original">The original color of the element being changed. Can be obtained and passed via 'visualElement.style.backgroundColor.value'</param>
        /// <param name="color1DurationMs">The amount of time it takes in milliseconds to complete the first color animation</param>
        /// <param name="color2DurationMs">The amount of time it takes in milliseconds to complete the second color animation</param>
        /// <param name="addBorder">Adds a border if the element does not have one already</param>
        /// <param name="borderStartEndWidth">The width in which the borders should be when displaying</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="borderSelection">The parameters of the Vector4(1-4) represent which borders should have their colors changed: 1(x) = left, 2(y) = top, 3(z) = right, 4(w) = bottom.
        /// If only the top and bottom borders are desired to pulse, you would pass new Vector4(0, 1, 0, 1)</param>
        public static IVisualElementScheduledItem AnimBorderPulse(this VisualElement element, Color color1, Color color2, Color original = default,
        int color1DurationMs = 1000,
        int color2DurationMs = 1000,
        bool addBorder = false,
        Vector2 borderStartEndWidth = default,
        Action callback = null,
        Vector4 borderSelection = default,
        IVisualElementScheduledItem repeatedAnim = default
        )
        {
            if (borderStartEndWidth == default)
                borderStartEndWidth = new Vector2(1, 0);

            bool doBorderPulse;
            var pulseIn = new ValueAnimation<StyleValues>();
            var pulseOut = new ValueAnimation<StyleValues>();
            pulseIn.autoRecycle = true;
            pulseOut.autoRecycle = true;
            pulseIn.KeepAlive();
            pulseOut.KeepAlive();

            doBorderPulse = true;
            if (addBorder) element.SetBorderWidth(borderStartEndWidth.x);

            var borderValues = new Vector4();
            var paddingValues = new Vector4();

            void SetBorderValues()
            {
                if (borderSelection.x == 0)
                {
                    element.style.paddingLeft = paddingValues.x + borderValues.x;
                    element.style.borderLeftWidth = 0;
                }

                if (borderSelection.y == 0)
                {
                    element.style.paddingTop = paddingValues.y + borderValues.y;
                    element.style.borderTopWidth = 0;
                }

                if (borderSelection.z == 0)
                {
                    element.style.paddingRight = paddingValues.z + borderValues.z;
                    element.style.borderRightWidth = 0;
                }

                if (borderSelection.w == 0)
                {
                    element.style.paddingBottom = paddingValues.w + borderValues.w;
                    element.style.borderBottomWidth = 0;
                }
            }

            void ReplaceBorderValues()
            {
                if (borderSelection.x == 0)
                {
                    element.style.paddingLeft = paddingValues.x;
                    element.style.borderLeftWidth = borderValues.x;
                }

                if (borderSelection.y == 0)
                {
                    element.style.paddingTop = paddingValues.y;
                    element.style.borderTopWidth = borderValues.y;
                }

                if (borderSelection.z == 0)
                {
                    element.style.paddingRight = paddingValues.z;
                    element.style.borderRightWidth = borderValues.z;
                }

                if (borderSelection.w == 0)
                {
                    element.style.paddingBottom = paddingValues.w;
                    element.style.borderBottomWidth = borderValues.w;
                }
            }

            if (borderSelection != default)
            {
                borderValues = new Vector4(
                    element.style.borderLeftWidth.value,
                    element.style.borderTopWidth.value,
                    element.style.borderRightWidth.value,
                    element.style.borderBottomWidth.value);

                paddingValues = new Vector4(
                    element.resolvedStyle.paddingLeft,
                    element.resolvedStyle.paddingTop,
                    element.resolvedStyle.paddingRight,
                    element.resolvedStyle.paddingBottom);
                SetBorderValues();
            }

            void DoCleanup()
            {
                if (pulseOut.isRunning) pulseOut.Stop();
                if (pulseIn.isRunning) pulseIn.Stop();

                element.SetBorderColor();
                if (addBorder) element.SetBorderWidth(borderStartEndWidth.y);
                if (borderSelection != default) ReplaceBorderValues();
                callback?.Invoke();
            } // @formatter:off
            
            // -- Pulse color will fade original => desired color   --
            // -- via the AnimateTo local function. Once completed  --
            // -- the AnimateFrom function is called animating back --
            // -- to the original color. This is then repeated for  --
            // -- as long as the mouse is hovered over the target   --
            void PulseIn(IVisualElementScheduledItem repeated)
            {
                if (!repeated.isActive) { DoCleanup(); return; }
                if (pulseOut.isRunning) pulseOut.Stop();
                pulseIn = element.AnimateBorderColor(
                    color1,
                    color2,
                    color1DurationMs,
                    () => PulseOut(repeated)).KeepAlive();
            }

            void PulseOut(IVisualElementScheduledItem repeated)
            {
                if (!repeated.isActive) { DoCleanup(); return; }
                if (pulseIn.isRunning) pulseIn.Stop();
                pulseOut = element.AnimateBorderColor(
                    color2,
                    color1,
                    color2DurationMs, () => { if(!repeated.isActive) DoCleanup();}).KeepAlive();
            } // @formatter:on

            var recurring = color1DurationMs + color2DurationMs + 20;

            repeatedAnim = element.schedule
                .Execute(() => { PulseIn(repeatedAnim); })
                .StartingIn(0)
                .Every(recurring)
                .Until(() => !doBorderPulse);

            return repeatedAnim;
        }
    }
}
#endif
