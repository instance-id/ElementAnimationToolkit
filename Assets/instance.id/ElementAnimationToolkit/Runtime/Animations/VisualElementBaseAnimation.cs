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
// ReSharper disable ConvertIfStatementToNullCoalescingAssignment

namespace instance.id.EATK
{
    public static class VisualElementBaseAnimation
    {
        #region Generic Animation
        public static ValueAnimation<StyleValues> Animate<T>(this VisualElement target, AnimValueStore<T> valueStore,
            Func<float, float> easing = null, bool reverse = false, int durationOverride = default, int delayOverride = default, Action callback = default) where T : struct
        {
            ValueAnimation<StyleValues> anim = valueStore.Animation ?? new ValueAnimation<StyleValues>();

            StyleValues from;
            StyleValues to;

            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var valueCallback = reverse ? valueStore.reverseCallback : valueStore.callback;

            // @formatter:off
            switch (valueStore.element)
            {
                case AnimateElement.Height:                        from = new StyleValues {height = (float) Convert.ChangeType(start, typeof(float))}; to = new StyleValues {height = (float) Convert.ChangeType(end, typeof(float))}; break;
                case AnimateElement.Width:                         from = new StyleValues {width = (float) Convert.ChangeType(start, typeof(float))}; to = new StyleValues {width = (float) Convert.ChangeType(end, typeof(float))}; break;
                case AnimateElement.Opacity:                       from = new StyleValues {opacity = (float) Convert.ChangeType(start, typeof(float))}; to = new StyleValues {opacity = (float) Convert.ChangeType(end, typeof(float))}; break;
                case AnimateElement.Color:                         from = new StyleValues {color = (Color) Convert.ChangeType(start, typeof(Color))}; to = new StyleValues {color = (Color) Convert.ChangeType(end, typeof(Color))}; break;
                case AnimateElement.BackgroundColor:               from = new StyleValues {backgroundColor = (Color) Convert.ChangeType(start, typeof(Color))}; to = new StyleValues {backgroundColor = (Color) Convert.ChangeType(end, typeof(Color))}; break;                
                case AnimateElement.BorderColor:                   from = new StyleValues {borderColor = (Color) Convert.ChangeType(start, typeof(Color))}; to = new StyleValues {borderColor = (Color) Convert.ChangeType(end, typeof(Color))}; break;                
                case AnimateElement.UnityBackgroundImageTintColor: from = new StyleValues {unityBackgroundImageTintColor = (Color) Convert.ChangeType(start, typeof(Color))}; to = new StyleValues {unityBackgroundImageTintColor = (Color) Convert.ChangeType(end, typeof(Color))}; break;
            } // @formatter:on

            if (easing == default) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(from, to, durationOverride != default ? durationOverride : valueStore.duration)
                    .Ease(easing)
                    .OnCompleted(callback ?? valueCallback);
            }).StartingIn(delayOverride != default ? delayOverride : valueStore.delay);
            return valueStore.Animation = anim;
        }

        public static ValueAnimation<StyleValues> Animate<TValue, TStyle>(this VisualElement target, AnimValueStore<TValue, TStyle> valueStore,
            Func<float, float> easing = null, bool reverse = false, int durationOverride = default, int delayOverride = default, Action callback = default)
            where TValue : struct
            where TStyle : struct
        {
            ValueAnimation<StyleValues> anim = valueStore.Animation ?? new ValueAnimation<StyleValues>();
            StyleValues from;
            StyleValues to;

            var i = valueStore.initial;
            var e = valueStore.final;

            // @formatter:off
            switch (valueStore.element)
            {
                case AnimateElement.Height:                        
                    from = new StyleValues {height = (float) Convert.ChangeType(valueStore.initial, typeof(float))};                          
                    to =  new StyleValues {height = (float) Convert.ChangeType(valueStore.final, typeof(StyleLength))}; break;
                case AnimateElement.Width:                         
                    from = new StyleValues {width = (float) Convert.ChangeType(i, typeof(float))};                           
                    to = new StyleValues {width = e.ToString().TryParseFloat() }; break;
                case AnimateElement.Opacity:                       
                    from = new StyleValues {opacity = (float) Convert.ChangeType(valueStore.initial, typeof(float))};                         
                    to = new StyleValues {opacity = (float) Convert.ChangeType(valueStore.final, typeof(StyleLength))}; break;
                case AnimateElement.Color:                         
                    from = new StyleValues {color = (Color) Convert.ChangeType(valueStore.initial, typeof(Color))};                           
                    to = new StyleValues {color = (Color) Convert.ChangeType(valueStore.final, typeof(StyleColor))}; break;
                case AnimateElement.BackgroundColor:               
                    from = new StyleValues {backgroundColor = (Color) Convert.ChangeType(valueStore.initial, typeof(Color))};                 
                    to = new StyleValues {backgroundColor = (Color) Convert.ChangeType(valueStore.final, typeof(StyleColor))}; break;                
                case AnimateElement.BorderColor:                   
                    from = new StyleValues {borderColor = (Color) Convert.ChangeType(valueStore.initial, typeof(Color))};                     
                    to = new StyleValues {borderColor = (Color) Convert.ChangeType(valueStore.final, typeof(StyleColor))}; break;                
                case AnimateElement.UnityBackgroundImageTintColor: 
                    from = new StyleValues {unityBackgroundImageTintColor = (Color) Convert.ChangeType(valueStore.initial, typeof(Color))};   
                    to = new StyleValues {unityBackgroundImageTintColor = (Color) Convert.ChangeType(valueStore.final, typeof(StyleColor))}; break;
            } // @formatter:on

            var start = reverse ? to : from;
            var end = reverse ? from : to;
            var valueCallback = reverse ? valueStore.reverseCallback : valueStore.callback;

            Debug.Log($"start: {start.width} end: {end.width}");

            if (easing == default) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(start, end, durationOverride != default ? durationOverride : valueStore.duration)
                    .Ease(easing)
                    .OnCompleted(callback ?? valueCallback);
            }).StartingIn(delayOverride != default ? delayOverride : valueStore.delay);
            return valueStore.Animation = anim;
        }
        #endregion

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

        public static ValueAnimation<StyleValues> AnimateBackgroundColor(this VisualElement target, AnimValueStore<Color> valueStore, Func<float, float> easing = null, bool reverse = false)
        {
            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {backgroundColor = start}, new StyleValues {backgroundColor = end}, valueStore.duration)
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

        public static ValueAnimation<StyleValues> AnimateBorderColor(this VisualElement target, AnimColorStore colorStore,
            Func<float, float> easing = null, bool reverse = false)
        {
            var start = reverse ? colorStore.final : colorStore.initial;
            var end = reverse ? colorStore.initial : colorStore.final;
            var callback = reverse ? colorStore.reverseCallback : colorStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {borderColor = start}, new StyleValues {borderColor = end}, colorStore.duration)
                .Ease(easing)
                .OnCompleted(callback);
        }

        public static ValueAnimation<StyleValues> AnimateBorderColor(this VisualElement target, AnimValueStore<Color> colorStore,
            Func<float, float> easing = null, bool reverse = false)
        {
            var start = reverse ? colorStore.final : colorStore.initial;
            var end = reverse ? colorStore.initial : colorStore.final;
            var callback = reverse ? colorStore.reverseCallback : colorStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {borderColor = start}, new StyleValues {borderColor = end}, colorStore.duration)
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
        /// <param name="colorStore">Contains all the necessary values in a single element</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
        /// <param name="reverse">Play the animation in reverse order</param>
        public static ValueAnimation<StyleValues> AnimateColor(this VisualElement target, AnimColorStore colorStore,
            Func<float, float> easing = null, bool reverse = false)
        {
            var start = reverse ? colorStore.final : colorStore.initial;
            var end = reverse ? colorStore.initial : colorStore.final;
            var callback = reverse ? colorStore.reverseCallback : colorStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {color = start}, new StyleValues {color = end}, colorStore.duration)
                .Ease(easing)
                .OnCompleted(callback);
        }

        public static ValueAnimation<StyleValues> AnimateColor(this VisualElement target, AnimValueStore<Color> colorStore,
            Func<float, float> easing = null, bool reverse = false)
        {
            var start = reverse ? colorStore.final : colorStore.initial;
            var end = reverse ? colorStore.initial : colorStore.final;
            var callback = reverse ? colorStore.reverseCallback : colorStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {color = start}, new StyleValues {color = end}, colorStore.duration)
                .Ease(easing)
                .OnCompleted(callback);
        }

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

        public static ValueAnimation<StyleValues> AnimateOpacity(this VisualElement target,
            AnimValueStore valueStore,
            Func<float, float> easing = null,
            bool reanimate = false,
            bool reverse = false)
        {
            if (!reanimate)
            {
                if (target.style.opacity == valueStore.final) return null;
            }

            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {opacity = start}, new StyleValues {opacity = end}, valueStore.duration)
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
        /// <param name="durationMs">The length of time in which the animation will occur (in milliseconds)</param>
        /// <param name="delayMs">Delay the animation from starting for X amount of time (in milliseconds)</param>
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

        public static ValueAnimation<StyleValues> AnimateOpacityDelayed(this VisualElement target, AnimValueStore valueStore, Func<float, float> easing = null, bool reverse = false)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();

            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {opacity = start}, new StyleValues {opacity = end}, valueStore.duration)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(valueStore.delay);

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
                callback ??= () => target.SetDisplay(false);
            }

            easing ??= Easy.EaseInOutQuint;
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

        public static ValueAnimation<StyleValues> AnimateWidth(this VisualElement target, AnimValueStore valueStore, Func<float, float> easing = null, bool reverse = false)
        {
            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(new StyleValues {width = start}, new StyleValues {width = end}, valueStore.duration)
                .Ease(easing)
                .OnCompleted(callback);
        }

        /// <summary>
        /// Animate the width of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startWidth">Initial width of element</param>
        /// <param name="endWidth">The desired end result width which to animate</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="delayMs">The length of time in which to wait before initially starting the animation</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
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

        public static ValueAnimation<StyleValues> AnimateWidthDelayed(this VisualElement target, AnimValueStore valueStore, Func<float, float> easing = null, bool reverse = false)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();
            if (easing == null) easing = Easy.EaseInOutQuint;

            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {width = start}, new StyleValues {width = end}, valueStore.duration)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(valueStore.delay);
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

        public static ValueAnimation<StyleValues> AnimateHeight(this VisualElement target, AnimValueStore valueStore,
            Func<float, float> easing = null, bool reverse = false, int durationOverride = default, int delayOverride = 0)
        {
            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();

            if (easing == null) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {height = start}, new StyleValues {height = end}, durationOverride != default ? durationOverride : valueStore.duration)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(delayOverride != default ? delayOverride : valueStore.delay);
            return anim;
        }

        /// <summary>
        /// Animate the height of target element to desired value
        /// </summary>
        /// <param name="target">VisualElement to animate</param>
        /// <param name="startHeight">Initial height of element</param>
        /// <param name="endHeight">Desired ending height after animation</param>
        /// <param name="durationMs">The length of time in which the animation will occur(in milliseconds)</param>
        /// <param name="delayMs">The length of time in which to wait before initially starting the animation</param>
        /// <param name="callback">Function that can be called when the animation is completed</param>
        /// <param name="easing">Controls the animation timing curve mathematically</param>
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

        public static ValueAnimation<StyleValues> AnimateHeightDelayed(this VisualElement target, AnimValueStore valueStore,
            Func<float, float> easing = null, bool reverse = false)
        {
            ValueAnimation<StyleValues> anim = new ValueAnimation<StyleValues>();

            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            if (easing == null) easing = Easy.EaseInOutQuint;
            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(new StyleValues {height = start}, new StyleValues {height = end}, valueStore.duration)
                    .Ease(easing)
                    .OnCompleted(callback);
            }).StartingIn(valueStore.delay);
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
        public static ValueAnimation<float> AnimateValue(
            this VisualElement target,
            float startValue,
            float endValue,
            int durationMs,
            Action<VisualElement, float> valueChange = null,
            Action callback = null,
            Func<float, float> easing = null)
        {
            if (easing == null) easing = Easy.EaseInOutQuint;
            return target.experimental.animation
                .Start(startValue, endValue, durationMs, valueChange)
                .Ease(easing)
                .OnCompleted(callback);
        }
        
        public static ValueAnimation<float> AnimateValue(
            this VisualElement target,
            AnimValueStore valueStore,
            bool reverse = false,
            Action<VisualElement, float> valueChange = null)
        {
            ValueAnimation<float> anim = new ValueAnimation<float>();

            var start = reverse ? valueStore.final : valueStore.initial;
            var end = reverse ? valueStore.initial : valueStore.final;
            var callback = reverse ? valueStore.reverseCallback : valueStore.callback;

            if (valueStore.easing == null) valueStore.easing = Easy.EaseInOutQuint;

            target.parent.schedule.Execute(() =>
            {
                anim = target.experimental.animation
                    .Start(start, end, valueStore.duration, valueChange)
                    .Ease(valueStore.easing)
                    .OnCompleted(callback);
            }).StartingIn(valueStore.delay);
            return anim;
        }

        public static ValueAnimation<float> AnimateSlider(
            this Slider target, 
            float startValue, 
            float endValue, 
            int durationMs, 
            Action<VisualElement, float> valueChange = null,
            Action callback = null,
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
