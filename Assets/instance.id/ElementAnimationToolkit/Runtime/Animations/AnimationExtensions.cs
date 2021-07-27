// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;

namespace instance.id.EATK
{
    public static class AnimationExtensions
    {
        /// <summary>
        /// Shorter extension in which to specify when an action should be started, and then proceed start it after specified amount of time.
        /// </summary>
        /// <param name="element">The target element in which to register the callback</param>
        /// <param name="action">The action to perform</param>
        /// <param name="delayMs">The amount of time in milliseconds that should be waited until the action begins</param>
        public static void ExecuteIn(this VisualElement element, Action action, long delayMs = 0)
        {
            element.schedule.Execute(action).StartingIn(delayMs);
        }

        /// <summary>
        /// Shorter extension in which to specify when an action should be started, and then proceed start it after specified amount of time.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="element">The target element in which to register the callback</param>
        /// <param name="delayMs">The amount of time in milliseconds that should be waited until the action begins</param>
        public static void ExecuteIn(this Action action, VisualElement element, long delayMs = 0)
        {
            element.schedule.Execute(action).StartingIn(delayMs);
        }

        // -- Register Callback with element return ------------
        /// <summary>
        /// RegisterCallback on element, as well as children, and return the element
        /// </summary>
        /// <param name="element">The target element in which to register the callback</param>
        /// <param name="callback">The callback in which to register</param>
        /// <param name="includeChildren">Register child elements in addition to the target element</param>
        /// <typeparam name="TEventType">The event callback type in which to register</typeparam>
        /// <returns>The target element</returns>
        public static VisualElement RegisterCallback<TEventType>(this VisualElement element, EventCallback<TEventType> callback, bool includeChildren)
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
        public static void UnregisterCallback<TEventType>(this VisualElement element, EventCallback<TEventType> callback, bool includeChildren)
            where TEventType : EventBase<TEventType>, new()
        {
            element.UnregisterCallback(callback);
            if (!includeChildren) return;
            var children = element.Query<VisualElement>().Descendents<VisualElement>().ToList();
            children.ForEach(x => x.UnregisterCallback(callback));
        }
    }
}
#endif
