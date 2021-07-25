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
        public static void ExecuteIn(this VisualElement element, Action action, long delayMs = 0)
        {
            element.schedule.Execute(action).StartingIn(delayMs);
        }

        public static void ExecuteIn(this Action action, VisualElement element,  long delayMs = 0)
        {
            element.schedule.Execute(action).StartingIn(delayMs);
        }
    }
}
#endif
