// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace instance.id.EATK.Extensions
{
    public static class AssetDatabaseExtensions
    {
        /// <summary>
        /// Locates the requested script file by type and searches the file for the locator string. If found, the line number is captured.
        /// Then returns the script file as Type of Object and the line number as an out parameter.
        /// </summary>
        /// <param name="type">The Type of the file which initially called the JumpToCode method</param>
        /// <param name="jumpTarget">Class containing data needed to determine the jump type and location<see cref="JumpTarget"/></param>
        /// <param name="lineNum">The line number of the locator string, if found.</param>
        /// <param name="externalLocator">Deprecated: Used to mark if the locator is on a different line than the calling method</param>
        /// <returns>The located script file from the AssetDatabase as Type of Object</returns>
        public static Object GetScriptFile(this Type type, JumpTarget jumpTarget, out int lineNum, bool externalLocator = false)
        {
            var fullPath = "";
            var scriptFile = new Object();
            var guidArray = AssetDatabase.FindAssets($"t:Script {type.Name}");
            if (guidArray.Length == 1)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guidArray[0]);
                scriptFile = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                fullPath = Path.GetFullPath(assetPath);
            }

            if (jumpTarget.Locator.StartsWith("_")) jumpTarget.Locator = jumpTarget.Locator.Replace("_", "@");
            else if (externalLocator || !jumpTarget.Locator.Contains("."))
                jumpTarget.Locator = $"@{jumpTarget.Locator}";

            lineNum = fullPath.LocateLineNumber(jumpTarget.Locator);
            return scriptFile;
        }

        /// <summary>
        /// Locates the requested script file by type and searches the file for the locator string. If found, the line number is captured.
        /// Then returns the script file as Type of Object and the line number as an out parameter.
        /// </summary>
        /// <param name="type">The Type of the file which initially called the JumpToCode method</param>
        /// <param name="locator">The string which is used to locate the desired line number in which to jump</param>
        /// <param name="lineNum">The line number of the locator string, if found.</param>
        /// <param name="externalLocator">Deprecated: Used to mark if the locator is on a different line than the calling method</param>
        /// <returns>The located script file from the AssetDatabase as Type of Object</returns>
        public static Object GetScriptFile(this Type type, string locator, out int lineNum, bool externalLocator = false)
        {
            var fullPath = "";
            var scriptFile = new Object();
            var guidArray = AssetDatabase.FindAssets($"t:Script {type.Name}");
            if (guidArray.Length == 1)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guidArray[0]);
                scriptFile = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                fullPath = Path.GetFullPath(assetPath);
            }

            if (locator.StartsWith("_")) locator = locator.Replace("_", "@");
            else if (externalLocator || !locator.Contains("."))
                locator = $"@{locator}";

            lineNum = fullPath.LocateLineNumber(locator);
            return scriptFile;
        }

        /// <summary>
        /// Locates the requested Stylesheet file searches the file for the locator string. If found, the line number is captured.
        /// Then returns the script file as Type of Object and the line number as an out parameter.
        /// </summary>
        /// <param name="type">The Type of the file which initially called the JumpToCode method</param>
        /// <param name="jumpTarget">Class containing data needed to determine the jump type and location<see cref="JumpTarget"/></param>
        /// <param name="lineNum">The line number of the locator string, if found.</param>
        /// <param name="externalLocator">Deprecated: Used to mark if the locator is on a different line than the calling method</param>
        /// <returns>The located script file from the AssetDatabase as Type of Object</returns>
        public static Object GetStylesheetFile(this Type type, JumpTarget jumpTarget, out int lineNum, bool externalLocator = false)
        {
            var fullPath = "";
            var scriptFile = new Object();
            var guidArray = AssetDatabase.FindAssets($"t:Stylesheet {type.Name}");
            if (guidArray.Length == 1)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guidArray[0]);
                scriptFile = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                fullPath = Path.GetFullPath(assetPath);
            }
            else Debug.Log($"Stylesheet: {type.Name} not found.");

            if (jumpTarget.Locator.StartsWith("_")) jumpTarget.Locator = jumpTarget.Locator.Replace("_", "@");
            else if (externalLocator || !jumpTarget.Locator.Contains("."))
                jumpTarget.Locator = $"@{jumpTarget.Locator}";

            lineNum = fullPath.LocateLineNumber(jumpTarget.Locator);
            return scriptFile;
        }

        /// <summary>
        /// Locates the requested Stylesheet file searches the file for the locator string. If found, the line number is captured.
        /// Then returns the script file as Type of Object and the line number as an out parameter.
        /// </summary>
        /// <param name="type">The Type of the file which initially called the JumpToCode method</param>
        /// <param name="locator">The string which is used to locate the desired line number in which to jump</param>
        /// <param name="lineNum">The line number of the locator string, if found.</param>
        /// <param name="externalLocator">Deprecated: Used to mark if the locator is on a different line than the calling method</param>
        /// <returns>The located script file from the AssetDatabase as Type of Object</returns>
        public static Object GetStylesheetFile(this Type type, string locator, out int lineNum, bool externalLocator = false)
        {
            var fullPath = "";
            var scriptFile = new Object();
            var guidArray = AssetDatabase.FindAssets($"t:Stylesheet {type.Name}");
            if (guidArray.Length == 1)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guidArray[0]);
                scriptFile = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                fullPath = Path.GetFullPath(assetPath);
            }
            else Debug.Log($"Stylesheet: {type.Name} not found.");

            if (locator.StartsWith("_")) locator = locator.Replace("_", "@");
            else if (externalLocator || !locator.Contains("."))
                locator = $"@{locator}";

            lineNum = fullPath.LocateLineNumber(locator);
            return scriptFile;
        }

        /// <summary>
        /// Opens a supported IDE to the appropriate file and places the caret at the desired line number
        /// </summary>
        /// <param name="asset">The script/file in which to open, of Type Object</param>
        /// <param name="lineNumber">The desired line number to place the caret</param>
        /// <returns></returns>
        public static bool ToIDE(this Object asset, int lineNumber = -1)
        {
            return AssetDatabase.OpenAsset(asset, lineNumber);
        }
    }
}
#endif
