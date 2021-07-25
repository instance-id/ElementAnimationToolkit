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

        // --------------------------------------------------- Constructor
        // ---------------------------------------------------------------
        public AnimatedItems(VisualElement ele)
        {
            element = ele;
            elementName = ele.name;
            animatedItemList = new List<ValueAnimation<StyleValues>>();
        }

        public void CopyToArray()
        {
            animatedItemArray = animatedItemList.ToArray();
        }

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

    }
}
