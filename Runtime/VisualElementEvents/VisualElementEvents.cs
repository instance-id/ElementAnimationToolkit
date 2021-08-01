// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using instance.id.EATK.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK
{
    public static class VisualElementEvents
    {
        // -- Register Callback with element return ------------
        /// <summary>
        /// RegisterCallback on element, as well as children, and return the element
        /// </summary>
        /// <param name="element">The target element in which to register the callback</param>
        /// <param name="callback">The callback in which to register</param>
        /// <param name="includeChildren">Register child elements in addition to the target element</param>
        /// <typeparam name="TEventType">The event callback type in which to register</typeparam>
        /// <returns>The target element</returns>
        public static VisualElement RegisterCallback<TEventType>(this VisualElement element, EventCallback<TEventType> callback, bool includeChildren, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType : EventBase<TEventType>, new()
        {
            element.RegisterCallback(callback);
            if (!includeChildren) return element;
            var children = element.Query<VisualElement>().Descendents<VisualElement>().ToList();
            children.ForEach(x => x.RegisterCallback(callback));
            return element;
        }

        /// <summary>
        /// RegisterCallback on element, as well as children, and return the element
        /// </summary>
        /// <param name="element">The target element in which to register the callback</param>
        /// <param name="callback">The callback in which to register</param>
        /// <param name="includeChildren">Register child elements in addition to the target element</param>
        /// <typeparam name="TEventType">The event callback type in which to register</typeparam>
        /// <returns>The target element</returns>
        public static void UnregisterCallback<TEventType>(this VisualElement element, EventCallback<TEventType> callback, bool includeChildren, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType : EventBase<TEventType>, new()
        {
            element.UnregisterCallback(callback);
            if (!includeChildren) return;
            var children = element.Query<VisualElement>().Descendents<VisualElement>().ToList();
            children.ForEach(x => x.UnregisterCallback(callback));
        }

        public static void MouseDownBackgroundColor<T>(this T element, Color downColor, Color upColor = default) where T : VisualElement
        {
            var originalColor = element.resolvedStyle.backgroundColor;
            
            if (typeof(T) == typeof(Button))
                (element as Button).clickable.clickedWithEventInfo += (evt) =>
                {
                    if ((VisualElement)evt.currentTarget == element)
                        (element as Button).SetBackgroundColor(downColor);
                };

            else
                element.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button == 0) 
                        element.SetBackgroundColor(downColor);
                });

            element.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.button == 0) 
                    element.ExecuteIn(() => element.SetBackgroundColor(originalColor), 100);
            });
        }

        public static void MouseDownImageColor<T>(this T element, Color downColor, Color upColor = default) where T : VisualElement
        {
            var originalColor = element.resolvedStyle.backgroundColor;

            if (typeof(T) == typeof(Button))
                (element as Button).clickable.clickedWithEventInfo += (evt) =>
                {
                    if ((VisualElement)evt.currentTarget == element)
                        (element as Button).SetBackgroundImageColor(downColor);
                };

            else
                element.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button == 0) 
                        element.SetBackgroundImageColor(downColor);
                });

            element.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.button == 0) 
                    element.ExecuteIn(() => element.SetBackgroundImageColor(originalColor), 100);
            });
        }
    }
}
#endif
