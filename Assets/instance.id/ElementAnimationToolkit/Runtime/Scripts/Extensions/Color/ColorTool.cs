using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace instance.id.EATK.Extensions
{
    public static class ColorTool
    {
        public static Dictionary<string, Color> KnownColors { get; private set; }

        static ColorTool() => Init();

        /// <summary>
        /// Initialize the list of known colors so that we don't have to use reflection all the time.
        /// </summary>
        public static void Init()
        {
            Type t = typeof(Color);
            Type c = typeof(Colors);
            
            KnownColors = new Dictionary<string, Color>();

            /* Find all public static properties and attempt to get a color out of them */
            foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                /* Make sure the property can return a color type */
                if (prop.PropertyType == t)
                {
                    string name = prop.Name.ToLowerInvariant();
                    Color color = (Color) prop.GetValue(null, null);
                    KnownColors.Add(name, color);
                }
            }
            
            var type = typeof(Colors);
            var fields = type.GetFields().ToDictionary(f => f.Name, f => (Color) f.GetValue(f));

            foreach (var color in fields) KnownColors.Add(color.Key, color.Value);
        }
        /// <summary>
        /// Allows you to convert either a known name or hexidecimal representation into a potential color object.
        /// </summary>
        /// <param name="colorString"></param>
        /// <returns></returns>
        public static Color ReadColor(string colorString)
        {
            colorString = colorString.ToLowerInvariant();

            if (KnownColors.ContainsKey(colorString)) return KnownColors[colorString];
            
            else if (colorString.Contains("#"))
            {
                int r, g, b, a = 0;
                var style = NumberStyles.HexNumber;

                colorString = colorString.Replace("#", "");

                switch (colorString.Length)
                {
                    case 3:
                        /* rgb */
                        r = int.Parse(colorString.Substring(0, 1) + colorString.Substring(0, 1), style);
                        g = int.Parse(colorString.Substring(1, 1) + colorString.Substring(1, 1), style);
                        b = int.Parse(colorString.Substring(2, 1) + colorString.Substring(2, 1), style);
                        return new Color(r, g, b);
                    case 4:
                        /* rgba */
                        r = int.Parse(colorString.Substring(0, 1) + colorString.Substring(0, 1), style);
                        g = int.Parse(colorString.Substring(1, 1) + colorString.Substring(1, 1), style);
                        b = int.Parse(colorString.Substring(2, 1) + colorString.Substring(2, 1), style);
                        a = int.Parse(colorString.Substring(3, 1) + colorString.Substring(2, 1), style);
                        return new Color(r, g, b, a);
                    case 6:
                        /* rrggbb */
                        r = int.Parse(colorString.Substring(0, 2), style);
                        g = int.Parse(colorString.Substring(2, 2), style);
                        b = int.Parse(colorString.Substring(4, 2), style);
                        return new Color(r, g, b);
                    case 8:
                        /* rrggbbaa */
                        r = int.Parse(colorString.Substring(0, 2), style);
                        g = int.Parse(colorString.Substring(2, 2), style);
                        b = int.Parse(colorString.Substring(4, 2), style);
                        a = int.Parse(colorString.Substring(6, 2), style);
                        return new Color(r, g, b, a);
                    default:
                        throw new ArgumentException("Invalid hex string.");
                }
            }

            throw new ArgumentException("Invalid color string.");
        }
        
        /// <summary>
        /// Lua does not support overloads, so this utility class provides a special global function rgb().
        /// </summary>
        public static Color ReadRGB(byte r, byte g, byte b)
        {
            return new Color(r, g, b);
        }


        /// <summary>
        /// Lua does not support overloads, so this utility class provides a special global function rgba().
        /// </summary>
        public static Color ReadRGBA(byte r, byte g, byte b, byte alpha)
        {
            return new Color(r, g, b, alpha);
        }
    }
}
