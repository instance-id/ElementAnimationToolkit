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
            element.parent.schedule.Execute(action).StartingIn(delayMs);
        }

        /// <summary>
        /// Shorter extension in which to specify when an action should be started, and then proceed start it after specified amount of time.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <param name="element">The target element in which to register the callback</param>
        /// <param name="delayMs">The amount of time in milliseconds that should be waited until the action begins</param>
        public static void ExecuteIn(this Action action, VisualElement element, long delayMs = 0)
        {
            element.parent.schedule.Execute(action).StartingIn(delayMs);
        }
    }
}
#endif
