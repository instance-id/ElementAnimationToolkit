using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    /// <summary>
    /// Contains the objects needed to generate editor window elements 
    /// </summary>
    public class ContainerData
    {
        public ContainerStyle containerType;
        public VisualElement element;
        public string containerName;
        public int containerId;
        public int order;
    }

    public struct ContainerDatas
    {
        public ContainerStyle containerType;
        public VisualElement element;
        public string containerName;
        public int containerId;
        public int order;
    }
}
