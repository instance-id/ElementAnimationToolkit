// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/Extensions                    --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace instance.id.EATK.Extensions
{
    public static class AssetFileExtensions
    {
        private static string searchStr = "a:all t:";
        public static StyleSheet GetStyleSheet(this Type type, string name = default, bool suffix = false, bool debug = false)
        {
            var sheetName = name == default && !suffix ? type.Name : suffix ? $"{type.Name}{name}" : name;
            var str = $"{searchStr}StyleSheet {sheetName}";
            if (debug) Debug.Log($"ScriptDir: {type.GetScriptPath()} {type.GetScriptPath() + $"/Style/{sheetName}Style.uss"}");

            var sheet =  AssetDatabase.LoadAssetAtPath<StyleSheet>(type.GetScriptPath() + $"/Style/{sheetName}.uss");
            if (sheet == null) sheet =  AssetDatabase.FindAssets(str)
                .Select(guid => AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList().FirstOrDefault();
            return sheet;
        }

        public static StyleSheet GetLocalStyleSheet(this Type type, string name = default, bool suffix = false, bool debug = false, [CallerFilePath] string sourceFilePath = "")
        {
            var sheetName = name == default && !suffix ? type.Name : suffix ? $"{type.Name}{name}" : name;
            var path = new Uri(Application.dataPath).MakeRelativeUri(new Uri(sourceFilePath)).ToString();

            if (debug) Debug.Log($"ScriptDir: {path} {path + $"/Style/{sheetName}.uss"}");
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(path + $"/Style/{sheetName}.uss");
        }

        public static StyleSheet GetStyleSheet(this string path, string name, bool debug = false)
        {
            var sheetName = name;
            if (debug) Debug.Log($"ScriptDir: {path} {path + $"/Style/{sheetName}.uss"}");
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(path + $"/Style/{sheetName}.uss");
        }

        public static Texture2D GetImage(this Type type, string name)
        {
            Texture2D image;
            image = AssetDatabase.LoadAssetAtPath<Texture2D>(type.SearchScriptPath(getFolder: true) + $"/Images/{name}");
            if (image == null) Debug.Log($"Cannot locate : {name} {image}");
            return image;
        }

        public static Font GetFont(string fontName)
        {
            var searchTerm = $"t:Font {fontName}";
            return AssetDatabase
                .FindAssets(searchTerm)
                .Select(guid => AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault();
        }

        public static T GetObjectOfType<T>(this Type type, string name) where T : Object
        {
            var searchTerm = $"t:{type.Name} {name}";
            return AssetDatabase
                .FindAssets(searchTerm)
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault();
        }
        
        public static string GetScriptPath(this object obj, [CallerFilePath] string sourceFilePath = "")
        {
            return new Uri(Application.dataPath).MakeRelativeUri(new Uri(sourceFilePath)).ToString();
        }
        
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
        /// Locate the local path of a script file
        /// </summary>
        /// <param name="type"></param>
        /// <param name="getName"></param>
        /// <param name="getFolder"></param>
        /// <returns></returns>
        public static string SearchScriptPath(this Type type, bool getName = false, bool getFolder = false)
        {
            string guidValue = "";
            var guidArray = AssetDatabase.FindAssets($"t:Script {type.Name}");
            if (guidArray.Length > 1)
            {
                foreach (var guid in guidArray)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var folderPath = Directory.GetParent(assetPath);
                    var filePath = Path.ChangeExtension(assetPath, null);
                    var filename = Path.GetFileNameWithoutExtension(assetPath);

                    if (filename != type.Name) continue;
                    if (getFolder) return folderPath.ToString();
                    if (getName) return filePath;

                    guidValue = guid;
                    break;
                }
            }
            else if (guidArray.Length == 1)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guidArray[0]);
                var folderPath = Directory.GetParent(assetPath);
                var filePath = Path.ChangeExtension(assetPath, null);
                var filename = Path.GetFileNameWithoutExtension(assetPath);

                if (filename != type.Name) return AssetDatabase.GUIDToAssetPath(guidValue);

                if (getFolder) return folderPath.ToString();
                if (getName) return filePath;
                guidValue = guidArray[0];
            }
            else
            {
                Debug.LogErrorFormat("Unable to locate {0}", type.Name);
                return null;
            }


            return AssetDatabase.GUIDToAssetPath(guidValue);
        }
        
      // -- UIElement extension that lets you jump straight ------------
        // -- to the line of code in which this method is used -----------
        /// <summary>
        /// From an elements context menu, jump straight to a specific line of code in your IDE
        /// (if supported by your specific IDE) by either a string of text, or by keyword in a comment beginning with @
        /// </summary>
        ///
        /// <param name="element">The element in which to add the "Jump to Code" context menu</param>
        /// <param name="type">The Type is used to locate the script/file in which you wish to jump to.
        /// By default the Type is the type of the script in which JumpToCode is called.
        /// If Type is passed as a parameter, this Type is used instead to locate the script/file.</param>
        /// <param name="locator">The line of code in which to jump to is found by locating the "locator" string. This is where in the code in which to jump</param>
        /// <param name="externalKeyword"></param>
        /// <param name="path">The directory location in which the code file exists. Notes, when variable is omitted, this path is found automatically via reflection</param>
        public static void JumpToCode(this VisualElement element, string locator = default, bool externalKeyword = false, string menuItemLabel = default,
            string menuItemUSSLabel = default,
            Type type = null, [CallerFilePath] string path = "", bool jumpUSS = false, bool jumpAnimation = false, Dictionary<string, Action> additionalMenus = null,
            List<JumpTarget> jumpTargets = null)
        {
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                Type GetIncomingType()
                {
                    if (type != null) return type;
                    // -- Get Type name via script name, as they should be the same -----
                    var typeName = Path.GetFileNameWithoutExtension(path);

                    // -- Get type from TypeCache API: Much faster than reflection ------
                    var types = TypeCache.GetTypesDerivedFrom<object>();
                    type = types.FirstOrDefault(x => x.Name == typeName);
                    if (type != null) return type;

                    Debug.LogError("Error: Could not get proper type of desired file in which to jump.");

                    // -- If it's not found, try a last ditch (probably failed) effort --
                    if (type == null) type = Type.GetType($"instance.id.{typeName}");
                    if (type == null) type = Type.GetType($"instance.id.Extensions.{typeName}");
                    // -- If *still* not found, admit defeat -----------------------------
                    if (type == null) Debug.LogError($"Could not locate Type: {typeName}");
                    return type;
                }

                jumpTargets?.ForEach(t =>
                {
                    switch (t.JumpType)
                    {
                        case JumpType.Element:
                            evt.menu.AppendAction(
                                t.MenuTitle = t.MenuTitle != default ? t.MenuTitle : $"Jump To {t.JumpType.ToString()}",
                                CallCodeJump, a => DropdownMenuAction.Status.Normal,
                                (GetIncomingType(), t).ToTuple());
                            break;
                        case JumpType.Animation:
                            evt.menu.AppendAction(
                                t.MenuTitle = t.MenuTitle != default ? t.MenuTitle : $"Jump To {t.JumpType.ToString()}",
                                CallCodeJump, a => DropdownMenuAction.Status.Normal,
                                (GetIncomingType(), t).ToTuple());
                            break;
                        case JumpType.USS:
                            evt.menu.AppendAction(
                                t.MenuTitle = t.MenuTitle != default ? t.MenuTitle : $"Jump To {t.JumpType.ToString()}",
                                CallUSSJump, a => DropdownMenuAction.Status.Normal,
                                (GetIncomingType(), t).ToTuple());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

                if (locator != null)
                {
                    evt.menu.AppendAction(
                        menuItemLabel = menuItemLabel != default ? menuItemLabel : "Jump To Code",
                        CallStringCodeJump, a => DropdownMenuAction.Status.Normal,
                        (GetIncomingType(), locator).ToTuple());
                }

                // -------------------------------------------------------
                // -- Dynamically added context menus per element --------
                additionalMenus?.forEach(x => // @formatter:off
                {
                    evt.menu.AppendAction(x.Key, CallActionMenu,
                        a => DropdownMenuAction.Status.Normal, x.Value);
                }); // @formatter:on

                void CallActionMenu(DropdownMenuAction dropdownMenuAction)
                {
                    if (dropdownMenuAction.userData is Action action) action.Invoke();
                }

                // -------------------------------------------------------
                // -- Jump to editor code menu ---------------------------
                void CallCodeJump(DropdownMenuAction dropdownMenuAction)
                {
                    var uData = dropdownMenuAction.userData as Tuple<Type, JumpTarget>;
                    uData.Item1.GetScriptFile(uData.Item2, out var lineNum).ToIDE(lineNum);
                }

                // -------------------------------------------------------
                // -- Jump to editor code menu ---------------------------
                void CallStringCodeJump(DropdownMenuAction dropdownMenuAction)
                {
                    var uData = dropdownMenuAction.userData as Tuple<Type, string>;
                    uData.Item1.GetScriptFile(uData.Item2, out var lineNum).ToIDE(lineNum);
                }

                // -------------------------------------------------------
                // -- Jump to USS menu item ------------------------------
                void CallUSSJump(DropdownMenuAction dropdownMenuAction)
                {
                    var uData = dropdownMenuAction.userData as Tuple<Type, JumpTarget>;
                    uData.Item1.GetStylesheetFile(uData.Item2, out var lineNum).ToIDE(lineNum);
                }

                evt.StopPropagation();
            }));
        }

        // -- Locate file from provided path, then use the provided string as the search criteria
        // -- in which to locate the desired line number and then return said line number
        public static int LocateLineNumber(this string filePath, string locator)
        {
            string line;
            var counter = 1;
            var locatedLine = 1;

            var file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(locator)) locatedLine = counter;
                counter++;
            }

            file.Close();
            return locatedLine;
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
