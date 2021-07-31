using System;
using System.Globalization;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    public static class ColorUtil
    {
        public static string EnsureHex(this string color)
        {
            if (color.StartsWith("#")) color = color.Replace("#", "");
            if (Int32.TryParse(color, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _))
            {
                if (!color.StartsWith("#")) color = $"#{color}";
            }
            else
            {
                Debug.LogWarning($"Color string: {color} no proper hexadecimal");
                return "";
            }

            return color;
        }
        
        /// <summary>
        /// Converts a hexadecimal color string to a Unity RGBA Color value
        /// </summary>
        /// <example><code>var color = ColorUtil.FromHex("#CCCCCC");</code></example>
        /// <param name="color">The string parameter must be a hexadecimal string, can beginning with #</param>
        /// <returns>Returns a Unity Color parameter converted from a HTML/hexadecimal color string</returns>
        /// <remarks>The string parameter must be a hexadecimal string beginning with #</remarks>
        public static Color FromHex(this string color)
        {
            if (color.StartsWith("#")) color = color.Replace("#", "");
            if (Int32.TryParse(color, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _))
                if (!color.StartsWith("#"))
                    color = $"#{color}";

            ColorUtility.TryParseHtmlString(color, out var outColor);
            return outColor;
        }
    }
}
