using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    /// <summary>
    /// Contains the objects needed to generate editor window elements 
    /// </summary>
    public class ContainerData
    {
        public ContainerType containerType;
        public VisualElement element;
        public string containerClass;
        public string containerName;
        public int containerId;
        public bool foldoutOpen;
        public int order;
    }

    public struct ContainerDatas
    {
        public ContainerType containerType;
        public VisualElement element;
        public string containerClass;
        public string containerName;
        public int containerId;
        public bool closed;
        public int order;
    }
}
