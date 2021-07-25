// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------
// -- Note: EATK is still currently being developed - API subject to change  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using instance.id.EATK.Extensions;
// using instance.id.Extensions;
// using instance.id.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    public static class AnimationSequences
    {
        #region AnimateCharacterSequence

        private static bool doDebug = false;

        // ------------------------------------- @AnimateCharacterSequence
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the height of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="color1">Initial height of element</param>
        /// <param name="color2">Desired ending height after animation</param>
        /// <param name="cascadeMs">The length of time in which each subsequent animation in the sequence will be offset (in milliseconds)</param>
        /// <param name="durationMS">The amount of time in which the animation will complete from start to finish (in milliseconds)</param>
        /// <param name="reverse">Animation will play in reverse, from right to left</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="flexBasis">Set a custom flex basis</param>
        public static List<ValueAnimation<StyleValues>> AnimCharacterSequence(this VisualElement target, Color color1, Color color2, int cascadeMs, int durationMS,
        bool reverse = false,
        Action callback = null,
        float flexBasis = default)
        {
            var animatedValues = new List<ValueAnimation<StyleValues>>();
            VisualElementStyleStore styleData = new VisualElementStyleStore();
            var textString = "";

            Label labelRef = new Label();


            if (target is AnimatedLabel label)
            {
                labelRef = label.GetLabel();
                textString = label.text;
                styleData = label.CreateStyleData();
                styleData.SourceName = target.name;
            }

            new VisualElement().Create(out var animatedContainer).ToUSS(nameof(animatedContainer), "animated-label-container");
            animatedContainer.style.height = new StyleLength(StyleKeyword.Auto);

            List<Label> labelList = new List<Label>();
            for (int i = 0; i < textString.Length; i++)
            {
                Label animLabel = new Label();

                if (!doDebug) animLabel = styleData.FromStyleData(new Label(textString[i].ToString()));
                else if (i == 0) animLabel = styleData.FromStyleData(new Label(textString[i].ToString()));

                if (!flexBasis.Equals(default)) animLabel.style.flexBasis = new Length(flexBasis);
                animLabel.SetMargin();
                animLabel.SetPadding();
                labelList.Add(animLabel);

                animatedContainer.Add(labelList[i]);
            }

            // -- Hide original label so that the animated label ---------
            // -- can show in it's place. Set values for animation -------
            labelRef.style.display = DisplayStyle.None;
            int cascade = cascadeMs;
            var count = labelList.Count;
            var current = 1;

            // -- Starts the animation -----------------------------------
            target.schedule.Execute(AnimationPhase1).StartingIn(0);

            // --  Phase 1 : Initials animation --------------------------
            void AnimationPhase1()
            {
                // -- Adds newly created labels to the target parent and -
                target.Add(animatedContainer);

                labelList.ForEachR(l =>
                {
                    if (current == count)
                        l.schedule.Execute(() => animatedValues.Add(
                            l.AnimateColor(color1, color2, durationMS, AnimationPhase2))).StartingIn(cascade);

                    else
                        l.schedule.Execute(() => animatedValues.Add(
                            l.AnimateColor(color1, color2, durationMS))).StartingIn(cascade);

                    cascade += cascadeMs;
                    current++;
                }, reverse);
            }

            // -- Phase 2: of animation sequence, called by onComplete ---
            // -- of Phase one of animation sequence ---------------------
            void AnimationPhase2()
            {
                current = 1;
                cascade = cascadeMs;

                labelList.ForEachR(l =>
                {
                    if (current == count)
                        l.schedule.Execute(() => animatedValues.Add(
                            l.AnimateColor(color2, color1, durationMS, AnimationComplete))).StartingIn(cascade);

                    else
                        l.schedule.Execute(() => animatedValues.Add(
                            l.AnimateColor(color2, color1, durationMS))).StartingIn(cascade);

                    cascade += cascadeMs;
                    current++;
                }, reverse);
            }

            // -- Called when all animations have completed --------------
            // -- Performs cleanup actions back to original --------------
            void AnimationComplete()
            {
                animatedContainer.RemoveFromHierarchy();
                labelRef.style.display = DisplayStyle.Flex;
                callback?.Invoke();
            }

            return animatedValues;
        }

        #endregion


        #region Text Fade Sequence

        // ------------------------------------------------------------ @TextFadeSequence
        // -- After sequence completes, defaultText is displayed until the next ---------
        // -- animation is player. If no default text is desired, change text -----------
        // -- to a blank string: Ex. string defaultText = "";  --------------------------
        // ------------------------------------------------------------------------------
        private static float fadeInTime = 1000f;
        private static float displayTime = 3000f;
        private static float fadeOutTime = 1000f;


        public static void AnimFadeInSequence(this Label element, string newText = default, Color textColor = default, Color textDefaultColor = default, float fadeIn = 0f,
        float display = 0f,
        float fadeOut = 0f,
        Func<float, float> easing = null)
        {
            DoFadeInSequence(element, newText, textColor, textDefaultColor, fadeIn, display, fadeOut, element.text, easing);
        }

        public static void AnimFadeInSequence(this VisualElement element, Color textColor = default, Color textDefaultColor = default, float fadeIn = 0f,
        float display = 0f,
        float fadeOut = 0f,
        Func<float, float> easing = null)
        {
            DoFadeInSequence(element, default, textColor, textDefaultColor, fadeIn, display, fadeOut, easing: easing);
        }

        private static void DoFadeInSequence(this VisualElement element, string newText = default, Color textColor = default, Color textDefaultColor = default, float fadeIn = 0f,
        float display = 0f,
        float fadeOut = 0f,
        string originalText = default,
        Func<float, float> easing = null)
        {
            var animFadeIn = new ValueAnimation<StyleValues>();
            var animWaiter = new ValueAnimation<StyleValues>();
            var animFadeOut = new ValueAnimation<StyleValues>();
            // -- If no time parameters are passed in, use declared defaults ------------
            fadeInTime = fadeIn == 0f ? fadeInTime : fadeIn;
            displayTime = display == 0f ? displayTime : display;
            fadeOutTime = fadeOut == 0f ? fadeOutTime : fadeOut;

            // -- If an animation is already running, interrupt it ----------------------
            if (animFadeIn.isRunning) animFadeIn.Stop();
            if (animWaiter.isRunning) animWaiter.Stop();
            if (animFadeOut.isRunning) animFadeOut.Stop();

            // -- If the element we are working with is null, return --------------------
            if (element is null) return;

            // -- Begin FadeInSequence: Initial call is a standard function but ---------
            // -- subsequent calls are local functions called by the animations ---------
            // -- onCompleted callback which will allow them to play one by one ---------
            element.schedule.Execute(() => animFadeOut = DoFadeOut(element, fadeOutTime, FadeIn)).StartingIn(0);

            // -- FadeIn : Callback 1 ---------------------
            void FadeIn()
            {
                if (textColor != default) element.style.color = textColor;
                if (element is Label label) label.text = newText;
                DoFadeIn(element, fadeInTime, Display, easing);
            }

            // -- Display : Callback 2 --------------------
            void Display()
            {
                DoDisplayMessage(element, displayTime, FadeOut, easing);
            }

            // -- FadeOut : Callback 3 --------------------
            void FadeOut()
            {
                animFadeOut = originalText != default ? DoFadeOut(element, fadeOutTime, ClearText, easing) : DoFadeOut(element, fadeOutTime, easing: easing);
            }

            // -- ClearText : Callback 4 is triggered if a the animated element is ------
            // -- a label and new text was passed as a parameter in which to animate ----
            void ClearText()
            {
                if (textDefaultColor != default) element.style.color = textDefaultColor;
                if (element is Label label) label.text = originalText;
                animFadeIn = DoFadeIn(element, fadeInTime);
            }
        }

        // ------------------------------------------------------ DoFadeIn
        // -- DoFadeIn ---------------------------------------------------
        private static ValueAnimation<StyleValues> DoFadeIn(VisualElement elementToFadeIn, float durationMs, Action callback = null, Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.OutQuad;
            return elementToFadeIn.experimental.animation
                .Start(new StyleValues { opacity = 0.Zero() }, new StyleValues { opacity = 1 }, (int)durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        // ---------------------------------------------- DoDisplayMessage
        // -- DoDisplayMessage -------------------------------------------
        private static ValueAnimation<StyleValues> DoDisplayMessage(VisualElement elementToDisplay, float durationMs, Action callback = null, Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.OutQuad;
            return elementToDisplay.experimental.animation
                .Start(new StyleValues { opacity = 1 }, new StyleValues { opacity = 1 }, (int)durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        // ----------------------------------------------------- DoFadeOut
        // -- DoFadeOut --------------------------------------------------
        private static ValueAnimation<StyleValues> DoFadeOut(VisualElement elementToFadeOut, float durationMs, Action callback = null, Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.OutQuad;
            return elementToFadeOut.experimental.animation
                .Start(new StyleValues { opacity = 1 }, new StyleValues { opacity = 0.Zero() }, (int)durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        #endregion
    }
}
#endif
