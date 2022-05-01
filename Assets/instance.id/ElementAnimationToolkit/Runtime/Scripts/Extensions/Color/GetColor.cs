// using System;
// using System.Globalization;
// using UnityEngine;
//
// #if !USING_EATK
// namespace instance.id.EATK.Extensions
// {
// //     public static class GetColor
// //     {
// //         /// <summary>
// //         /// Converts a hexadecimal color string to a Unity RGBA Color value
// //         /// </summary>
// //         /// <example><code>var color = GetColor.FromHex("#CCCCCC");</code></example>
// //         /// <param name="color">The string parameter must be a hexadecimal string, can beginning with #</param>
// //         /// <returns>Returns a Unity Color parameter converted from a HTML/hexadecimal color string</returns>
// //         /// <remarks>The string parameter must be a hexadecimal string beginning with #</remarks>
// //         public static Color FromHex(this string color)
// //         {
// //             if (color.StartsWith("#")) color = color.Replace("#", "");
// //             if (Int32.TryParse(color, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _))
// //                 if (!color.StartsWith("#"))
// //                     color = $"#{color}";
// //
// //             ColorUtility.TryParseHtmlString(color, out var outColor);
// //             return outColor;
// //         }
// //     }
// // }
//
// #else 
// namespace instance.id.Extensions
// {
//     internal static class GetColor
//     {
//         /// <summary>
//         /// Converts a hexadecimal color string to a Unity RGBA Color value
//         /// </summary>
//         /// <example><code>var color = GetColor.FromHex("#CCCCCC");</code></example>
//         /// <param name="color">The string parameter must be a hexadecimal string, can beginning with #</param>
//         /// <returns>Returns a Unity Color parameter converted from a HTML/hexadecimal color string</returns>
//         /// <remarks>The string parameter must be a hexadecimal string beginning with #</remarks>
//         public static Color FromHex(this string color)
//         {
//             if (color.StartsWith("#")) color = color.Replace("#", "");
//             if (Int32.TryParse(color, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _))
//                 if (!color.StartsWith("#"))
//                     color = $"#{color}";
//
//             ColorUtility.TryParseHtmlString(color, out var outColor);
//             return outColor;
//         }
//     }
// }
// #endif
