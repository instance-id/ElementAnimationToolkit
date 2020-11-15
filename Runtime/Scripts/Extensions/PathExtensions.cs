// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/UIElementsAnimation           --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace instance.id.EATK.Extensions
{
    public static class PathExtensions
    {
        public static string GetScriptPath(this Type type, bool getName = false, bool getFolder = false)
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
    }
}
#endif
