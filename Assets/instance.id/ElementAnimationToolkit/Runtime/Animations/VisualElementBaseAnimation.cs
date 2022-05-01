// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using instance.id.EATK.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    public static class VisualElementBaseAnimation
    {
        #region Base Animation Extensions
        // --------------------------------------- @AnimateBackgroundColor
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the background color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateBackgroundColor(this VisualElement target, Color startColor, Color endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {backgroundColor = startColor}, new StyleValues {backgroundColor = endColor}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        /// <summary>
        /// Animate the background color of target element to desired value after delay
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in milliseconds in which the animation will occur</param>
        /// <param name="delayMs">The length of time in milliseconds to wait before starting animation</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateBackgroundColorDelayed(this VisualElement target, Color startColor, Color endColor, int durationMs = 500, int delayMs = 0, Action callback = null,
        Func<float, float> easing = null)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();

            if (easing == null) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {backgroundColor = startColor}, new StyleValues {backgroundColor = endColor}, durationMs)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(delayMs);
            return anim;
        }

        /// <summary>
        /// Animate the background color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element as hexadecimal string (ex. "A4B5C6")</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateBackgroundColor(this VisualElement target, string startColor, string endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null) =>
            AnimateBackgroundColor(target, startColor.EnsureHex().FromHex(), endColor.EnsureHex().FromHex(), durationMs, callback, easing);

        // ------------------------------------------- @AnimateBorderColor
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the border color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateBorderColor(this VisualElement target, Color startColor, Color endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {borderColor = startColor}, new StyleValues {borderColor = endColor}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        /// <summary>
        /// Animate the background color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element as hexadecimal string (ex. "A4B5C6")</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateBorderColor(this VisualElement target, string startColor, string endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null) =>
            AnimateBorderColor(target, startColor.EnsureHex().FromHex(), endColor.EnsureHex().FromHex(), durationMs, callback, easing);

        // ------------------------------------------------- @AnimateColor
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateColor(this VisualElement target, Color startColor, Color endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {color = startColor}, new StyleValues {color = endColor}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        /// <summary>
        /// Animate the background color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element as hexadecimal string (ex. "A4B5C6")</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateColor(this VisualElement target, string startColor, string endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null) =>
            AnimateColor(target, startColor.EnsureHex().FromHex(), endColor.EnsureHex().FromHex(), durationMs, callback, easing);

        // ---------------------------------------- @AnimateImageTintColor
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the image tint color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateImageTintColor(this VisualElement target, Color startColor, Color endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {unityBackgroundImageTintColor = startColor}, new StyleValues {unityBackgroundImageTintColor = endColor}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        /// <summary>
        /// Animate the background color of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startColor">Initial color of element as hexadecimal string (ex. "A4B5C6")</param>
        /// <param name="endColor">The desired end result color which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateImageTintColor(this VisualElement target, string startColor, string endColor, int durationMs, Action callback = null,
        Func<float, float> easing = null) =>
            AnimateImageTintColor(target, startColor.EnsureHex().FromHex(), endColor.EnsureHex().FromHex(), durationMs, callback, easing);

        // ----------------------------------------------- @AnimateOpacity
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the opacity of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startOpacity">Initial opacity of element</param>
        /// <param name="endOpacity">The desired end result opacity which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        /// <param name="reanimate">If enabled, animation can trigger from start value regardless of current opacity value</param>
        public static ValueAnimation<StyleValues> AnimateOpacity(this VisualElement target, float startOpacity, float endOpacity, int durationMs, Action callback = null,
        Func<float, float> easing = null, bool reanimate = false)
        {
            if (!reanimate)
            {
                if (target.style.opacity == endOpacity) return null;
            }

            if (startOpacity == 0) startOpacity = 0.Zero();
            if (endOpacity == 0) endOpacity = 0.Zero();

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {opacity = startOpacity}, new StyleValues {opacity = endOpacity}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        // ----------------------------------------------- @AnimateOpacity
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the opacity of target element to desired value after waiting for specified delay
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startOpacity">Initial opacity of element</param>
        /// <param name="endOpacity">The desired end result opacity which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateOpacityDelayed(this VisualElement target, float startOpacity, float endOpacity, int durationMs, int delayMs, Action callback = null,
        Func<float, float> easing = null)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();
            if (startOpacity == 0) startOpacity = 0.Zero();
            if (endOpacity == 0) endOpacity = 0.Zero();

            if (easing == null) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {opacity = startOpacity}, new StyleValues {opacity = endOpacity}, durationMs)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(delayMs);

            return anim;
        }

        // ----------------------------------------------- @AnimateOpacity
        // ---------------------------------------------------------------
        /// <summary>
        /// Enable Display of an element and animate it's opacity
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="setDisplay">Bool value representing the desired display state of the element</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateDisplay(this VisualElement target, bool setDisplay, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            switch (setDisplay)
            {
                case true when target.GetDisplay():
                case false when !target.GetDisplay():
                    return default;
            }

            float startOpacity;
            float endOpacity;

            if (setDisplay)
            {
                startOpacity = 0.Zero();
                endOpacity = 1;
                target.SetOpacity(startOpacity);
                target.SetDisplay(true);
            }
            else
            {
                startOpacity = 1;
                endOpacity = 0.Zero();
                target.SetOpacity(startOpacity);
                if (callback == null) 
                    callback = () => target.SetDisplay(false);
            }

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {opacity = startOpacity}, new StyleValues {opacity = endOpacity}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        // ------------------------------------------------- @AnimateWidth
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the width of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startWidth">Initial width of element</param>
        /// <param name="endWidth">The desired end result width which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateWidth(this VisualElement target, float startWidth, float endWidth, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {width = startWidth}, new StyleValues {width = endWidth}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        public static ValueAnimation<StyleValues> AnimateWidthDelayed(this VisualElement target, float startWidth, float endWidth, int durationMs, int delayMs, Action callback = null,
        Func<float, float> easing = null)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();
            if (easing == null) easing = Easy.EaseInOutQuint;

            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {width = startWidth}, new StyleValues {width = endWidth}, durationMs)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(delayMs);
            return anim;
        }

        // ------------------------------------------------ @AnimateHeight
        // ---------------------------------------------------------------
        /// <summary>
        /// Animate the height of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startHeight">Initial height of element</param>
        /// <param name="endHeight">Desired ending height after animation</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        public static ValueAnimation<StyleValues> AnimateHeight(this VisualElement target, float startHeight, float endHeight, int durationMs, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {height = startHeight}, new StyleValues {height = endHeight}, durationMs)
                .Ease(easing)
                .OnCompleted(callback);
        }

        public static ValueAnimation<StyleValues> AnimateHeightDelayed(this VisualElement target, float startHeight, float endHeight, int durationMs, int delayMs, Action callback = null,
        Func<float, float> easing = null)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();
            if (easing == null) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {height = startHeight}, new StyleValues {height = endHeight}, durationMs)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(delayMs);
            return anim;
        }

        /// <summary>
        /// Animate an arbitrary float value
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="durationMs"></param>
        /// <param name="valueChange"></param>
        /// <param name="callback"></param>
        /// <param name="easing"></param>
        /// <returns></returns>
        public static ValueAnimation<float> AnimateValue(this VisualElement target, float startValue, float endValue, int durationMs, Action<VisualElement, float> valueChange = null, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(startValue, endValue, durationMs, valueChange)
                .Ease(easing)
                .OnCompleted(callback);
        }

        public static ValueAnimation<float> AnimateSlider(this Slider target, float startValue, float endValue, int durationMs, Action<VisualElement, float> valueChange = null, Action callback = null,
        Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(startValue, endValue, durationMs, valueChange)
                .Ease(easing)
                .OnCompleted(callback);
        }
        #endregion
    }
}
#endif
