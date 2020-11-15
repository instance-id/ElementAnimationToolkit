// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK.Extensions
{
    public static class ContinuousAnimations
    {
        public static bool doBorderPulse;

        // -------------------------------------------------- @HoverBorder
        // ---------------------------------------------------------------
        /// <summary>
        /// Pulse the  border of an element  between two colors
        ///
        /// ** To help combat your element shifting position slightly when a border is applied on hover,
        /// it is a good idea to add a border to your element before hand and just set color to 'initial'
        /// so that it is transparent, then keep 'addBorder' parameter as false.
        /// </summary>
        /// <param name="target">The element in which this function will be applied</param>
        /// <param name="pulseStartColor">Color 1 of 2 in which to pulse between</param>
        /// <param name="pulseEndColor">Color 2 of 2 in which to pulse between</param>
        /// <param name="original">The original color of the element being changed. Can be obtained and passed via 'visualElement.style.backgroundColor.value'</param>
        /// <param name="startColorDurationMs">The amount of time it takes in milliseconds to complete the first color animation</param>
        /// <param name="endColorDurationMs">The amount of time it takes in milliseconds to complete the second color animation</param>
        /// <param name="addBorder">Adds a border if the element does not have one already</param>
        /// <param name="borderStartEndWidth">The width in which the borders should be when displaying</param>
        public static IVisualElementScheduledItem AnimBorderPulse(this VisualElement target, Color pulseStartColor, Color pulseEndColor, Color original = default,
            int startColorDurationMs = 1000,
            int endColorDurationMs = 1000,
            bool addBorder = false,
            Vector2 borderStartEndWidth = default, Action callback = null)
        {
            if (borderStartEndWidth == default)
                borderStartEndWidth = new Vector2(1, 0);

            var pulseIn = new ValueAnimation<StyleValues>();
            var pulseOut = new ValueAnimation<StyleValues>();
            IVisualElementScheduledItem repeatedAnim = null;

            doBorderPulse = true;
            if (addBorder)
            {
                target.SetBorderWidth(borderStartEndWidth.x);
            }

            void DoCleanup()
            {
                if (pulseOut.isRunning) pulseOut.Stop();
                if (pulseIn.isRunning) pulseIn.Stop();
                target.SetBorderColor();
                callback?.Invoke();
            }

            // -- Pulse color will fade original => desired color   --
            // -- via the AnimateTo local function. Once completed  --
            // -- the AnimateFrom function is called animating back --
            // -- to the original color. This is then repeated for  --
            // -- as long as the mouse is hovered over the target   --
            void PulseIn(IVisualElementScheduledItem repeated)
            {
                if (!repeated.isActive) { DoCleanup(); return; }
                if (pulseOut.isRunning) pulseOut.Stop();
                callback?.Invoke();
                pulseIn = target.AnimateBorderColor(pulseStartColor, pulseEndColor, startColorDurationMs, () => PulseOut(repeated));
            }

            void PulseOut(IVisualElementScheduledItem repeated)
            {
                if (!repeated.isActive) { DoCleanup(); return; }
                if (pulseIn.isRunning) pulseIn.Stop();
                callback?.Invoke();
                pulseOut = target.AnimateBorderColor(pulseEndColor, pulseStartColor, endColorDurationMs);
            }

            var recurring = startColorDurationMs + endColorDurationMs + 20;
            repeatedAnim = target.schedule.Execute(() => PulseIn(repeatedAnim)).StartingIn(0).Every(recurring).Until(() => !doBorderPulse);
            return repeatedAnim;
        }
    }
}
