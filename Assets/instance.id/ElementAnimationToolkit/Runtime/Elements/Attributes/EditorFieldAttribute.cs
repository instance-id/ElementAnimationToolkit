using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    /// <summary>
    /// Attribute providing easier creation of custom internal editor windows
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Field)]
    public class EditorFieldAttribute : PropertyAttribute, IEquatable<EditorFieldAttribute>
    {
        public string description;
        public string toolTip;
        public VisualElement element;
        public ContainerData container;

        /// <summary>
        /// Attribute providing easier creation of custom internal editor windows
        /// </summary>
        public EditorFieldAttribute() { }

        /// <summary>
        /// Attribute providing easier creation of custom internal editor windows
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="ToolTip"></param>
        /// <param name="ContainerId"></param>
        /// <param name="Order"></param>
        /// <param name="ContainerType"></param>
        /// <param name="ContainerName"></param>
        public EditorFieldAttribute(string Description = default, ContainerStyle ContainerType = default, string ContainerName = default,
        int ContainerId = -1,
        int Order = -1,
        string ToolTip = default)
        {
            if (Description == default)
            {
                description = "Field Description Missing!";
                toolTip = "Please fill out the field description in the EditorField attribute!";
            }
            else
            {
                description = Description;
                toolTip = default ? Description : ToolTip;
            }

            container = new ContainerData
            {
                order = Order,
                containerId = ContainerId,
                containerType = ContainerType,
                containerName = ContainerName
            };
        }
 
        #region Equality Methods

        /// <inheritdoc />
        public bool Equals(EditorFieldAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && description == other.description && toolTip == other.toolTip && Equals(element, other.element);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EditorFieldAttribute)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (description != null ? description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (toolTip != null ? toolTip.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (element != null ? element.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
