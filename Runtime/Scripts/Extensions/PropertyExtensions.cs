#if UNITY_EDITOR
using UnityEditor;

namespace instance.id.EATK.Extensions
{
    public static class PropertyExtensions
    {
        public static bool IsReallyArray(this SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }
    }
}
#endif
