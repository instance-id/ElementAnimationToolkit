// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/UIElementsAnimation           --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    public static class VisualElementUtils
    {
        public static StyleSheet GetStyleSheet(this Type type, string name = default, bool debug = false)
        {
            var sheetName = name == default ? type.Name : name;
            if (debug) Debug.Log($"{type.GetScriptPath(getFolder: true) + $"/Style/{sheetName}.uss"}");
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(type.GetScriptPath(getFolder: true) + $"/Style/{sheetName}.uss");
        }

        public static Texture2D GetImage(this Type type, string name)
        {
            Texture2D image;
            image = AssetDatabase.LoadAssetAtPath<Texture2D>(type.GetScriptPath(getFolder: true) + $"/Images/{name}");
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
    }
}
