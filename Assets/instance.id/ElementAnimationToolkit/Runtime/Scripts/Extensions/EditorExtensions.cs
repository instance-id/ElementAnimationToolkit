// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    public static class EditorExtensions
    {
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
                additionalMenus?.ForEach(x => // @formatter:off
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
    }
}
