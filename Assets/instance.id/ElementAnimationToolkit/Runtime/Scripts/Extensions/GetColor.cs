// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/UIElementsAnimation           --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEngine;

namespace instance.id.EATK.Extensions
{
    public static class GetColor
    {
        /// <summary>
        /// Converts a hexadecimal color string to a Unity RGBA Color value
        /// </summary>
        /// <example><code>var color = GetColor.FromHex("#CCCCCC");</code></example>
        /// <param name="color">The string parameter must be a hexadecimal string beginning with #</param>
        /// <returns>Returns a Unity Color parameter converted from hexadecimal</returns>
        /// <remarks>The string parameter must be a hexadecimal string beginning with #</remarks>
        public static Color FromHex(this string color)
        {
            if (!color.StartsWith("#")) Debug.LogWarning("The FromHex() function must be used on a hexadecimal string beginning with #");
            ColorUtility.TryParseHtmlString(color, out var outColor);
            return outColor;
        }
    }
}
