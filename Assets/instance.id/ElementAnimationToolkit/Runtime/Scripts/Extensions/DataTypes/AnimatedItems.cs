// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ProjectSetup         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    public class AnimatedItems  
    {
        private string elementName;
        private VisualElement element;
        private List<ValueAnimation<StyleValues>> animatedItemList;
        private ValueAnimation<StyleValues>[] animatedItemArray;

        // -- Default to allowing the animation to run, but allows for manually   
        // -- setting false to prevent animation from running until set back to true
        private bool allowRun = true;

        // --------------------------------------------------- Constructor
        // ---------------------------------------------------------------
        public AnimatedItems(VisualElement ele)
        {
            element = ele;
            elementName = ele.name;
            animatedItemList = new List<ValueAnimation<StyleValues>>();
        }

        public void CopyToArray() => animatedItemArray = animatedItemList.ToArray();

        // ---------------------------------------------------- Properties
        // ---------------------------------------------------------------
        public string ElementName
        {
            get => elementName;
            set => elementName = value;
        }

        public VisualElement Element
        {
            get => element;
            set => element = value;
        }

        public List<ValueAnimation<StyleValues>> AnimatedItemList
        {
            get => animatedItemList;
            set => animatedItemList = value;
        }

        public ValueAnimation<StyleValues>[] AnimatedItemArray
        {
            get => animatedItemArray;
            set => animatedItemArray = value;
        }
        
        public bool AllowRun
        {
            get => allowRun;
            set => allowRun = value;
        }
    }
    
    public class AnimatedItems<TEventType> : AnimatedItems
    {
        private EventCallback<TEventType> eventCallback;
        public EventCallback<TEventType> EventCallback
        {
            get => eventCallback;
            set => eventCallback = value;
        }
        
        private List<EventCallback<TEventType>> eventCallbacks;
        public List<EventCallback<TEventType>> EventCallbacks
        {
            get => eventCallbacks;
            set => eventCallbacks = value;
        }

        public AnimatedItems(VisualElement ele) : base(ele) { }
    }

    public class AnimatedItems<TEventType, T2EventType> : AnimatedItems
    {
        private (EventCallback<TEventType> mouseOverEvent, EventCallback<T2EventType> mouseOutEvent) eventCallbacks;
        public (EventCallback<TEventType> mouseOverEvent, EventCallback<T2EventType> mouseOutEvent) EventCallbacks
        {
            get => eventCallbacks;
            set => eventCallbacks = value;
        }

        public AnimatedItems(VisualElement ele) : base(ele) { }
    }
}
