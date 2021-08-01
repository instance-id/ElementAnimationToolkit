// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace instance.id.EATK
{
    [Serializable]
    public class ObjectElementData
    {
        [SerializeField] public string objectName;
        [SerializeField] public Object objectReference;
        [SerializeField] public Type objectType;
        [SerializeField] public Dictionary<string, VisualElement> elementData = new Dictionary<string, VisualElement>();

        public ObjectElementData(Object obj)
        {
            objectReference = obj;
            objectName = obj.name;
            objectType = obj.GetType();
        }
    }
}
