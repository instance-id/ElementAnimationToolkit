using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using instance.id;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK
{
    [Serializable]
    public class EATKStyleAsset : AssetInstance<EATKStyleAsset>
    {
        private const string asmdefName = "id.instance.elementanimationtoolkit";
        [SerializeField] private string assetPath;
        [SerializeField] public bool useAnimatedFoldouts;
        private const string imageAssetTerms = "a:all t:ImageAsset";
        private void OnEnable()
        {
            assetPath = CheckAssetPath();
            styleSheets = styleSheets?.Where(styleSheet => styleSheet != null).ToList();
            layouts = layouts?.Where(layout => layout != null).ToList();
            imageAssets = imageAssets?.Where(image => image != null).ToList();
        }

        private void OnValidate() => ReloadLookups();

        // --| StyleSheet and Layout --------------------------------------------------------------
        // --|-------------------------------------------------------------------------------------
        [SerializeField] public List<StyleSheet> styleSheets = new List<StyleSheet>();
        [SerializeField] public List<VisualTreeAsset> layouts = new List<VisualTreeAsset>();
        [SerializeField] public List<ImageAsset> imageAssets = new List<ImageAsset>();

        [SerializeField, HideInInspector] private SerializedDictionary<string, StyleSheet> styleSheetLookup
            = new SerializedDictionary<string, StyleSheet>();

        [SerializeField, HideInInspector] private SerializedDictionary<string, VisualTreeAsset> layoutLookup
            = new SerializedDictionary<string, VisualTreeAsset>();

        [SerializeField, HideInInspector] private SerializedDictionary<string, ImageAsset> imageAssetLookup
            = new SerializedDictionary<string, ImageAsset>();

        public StyleSheet GetStyleSheet(string styleName)
        {
            StyleSheet sheet;
            if ((sheet = styleSheetLookup.TryGetVal(styleName)) == true) return sheet;
            ReloadLookups();

            var foundSheet = styleSheetLookup.TryGetVal(styleName) ?? FindAssets<StyleSheet>($"{styleName}Style").FirstOrDefault();

            if (foundSheet != null && !styleSheets.Contains(foundSheet)) 
                styleSheets.Add(sheet = foundSheet);

            ReloadLookups(true);
            if (sheet != null)
            {
                var sName = sheet.name.Replace("Style", "");
                if (!styleSheetLookup.ContainsKey(sName))
                    styleSheetLookup.Add(sheet.name.Replace("Style", ""), sheet);

                return sheet;
            }
            else
            {
                Debug.Log($"Could not locate StyleSheet for: {styleName}");
            }

            return sheet;
        }

        public VisualTreeAsset GetLayout(string layoutName)
        {
            var layout = layoutLookup.TryGetVal(layoutName);
            if (layout != null) return layout;
            ReloadLookups();

            return layoutLookup.TryGetVal(layoutName) ??
                   layouts.FirstOrDefault(l => l.name == layoutName);
        }

        public ImageAsset GetImage(string imageName)
        {
            ImageAsset imageAsset;
            if ((imageAsset = imageAssetLookup.TryGetVal(imageName)) != null)
                return imageAsset;

            Debug.Log("ImageAsset not found: Searching..");

#if UNITY_EDITOR
            imageAssets = AssetDatabase.FindAssets(imageAssetTerms)
                .Select(guid => AssetDatabase.LoadAssetAtPath<ImageAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
#endif
            for (int i = 0; i < imageAssets.Count; i++) imageAssetLookup.TryAdd(imageAssets[i].name, imageAssets[i]);
            return imageAssetLookup.TryGetVal(imageName);
        }

        private void ReloadLookups(bool force = false)
        {
            assetPath = CheckAssetPath();
            styleSheets = styleSheets?.Where(styleSheet => styleSheet != null).ToList();
            layouts = layouts?.Where(layout => layout != null).ToList();

            imageAssets = imageAssets?.Where(image => image != null).ToList();

            if (styleSheets == null || styleSheets.Count == 0 || force) LocateStyleSheets();
            if (styleSheetLookup == null || styleSheetLookup.Count == 0 || force)
            {
                styleSheetLookup = new SerializedDictionary<string, StyleSheet>();
                foreach (var styleSheet in styleSheets)
                    if (!styleSheetLookup.ContainsKey(styleSheet.name))
                        styleSheetLookup.TryAdd(styleSheet.name.Replace("Style", ""), styleSheet);
            }

            if (layouts == null || layouts.Count == 0 || force) LocateLayouts();
            if (layoutLookup == null || layoutLookup.Count == 0 || force)
            {
                layoutLookup = new SerializedDictionary<string, VisualTreeAsset>();
                foreach (var layout in layouts)
                    if (!layoutLookup.ContainsKey(layout.name))
                        layoutLookup.TryAdd(layout.name, layout);
            }

            styleSheets = styleSheets?.Where(styleSheet => styleSheet != null).ToList();
            layouts = layouts?.Where(layout => layout != null).ToList();
            imageAssets = imageAssets?.Where(image => image != null).ToList();
        }

        internal void LocateStyleSheets()
        {
#if UNITY_EDITOR
            var ourPath = assetPath;
            styleSheets.Clear();
            foreach (var sheet in AssetDatabase.FindAssets("a:all t:StyleSheet Style")
                .Select(guid =>
                {
                    string p;
                    return (p = AssetDatabase.GUIDToAssetPath(guid)).StartsWith(ourPath)
                        ? AssetDatabase.LoadAssetAtPath<StyleSheet>(p)
                        : null;
                }))
                if (sheet != null && !styleSheets.Contains(sheet) && sheet.name != "inlineStyle")
                    styleSheets.Add(sheet);
#endif
        }

        internal void LocateLayouts()
        {
#if UNITY_EDITOR
            var ourPath = assetPath;
            layouts.Clear();
            foreach (var layout in AssetDatabase.FindAssets("a:all t:VisualTreeAsset")
                .Select(guid =>
                {
                    string p;
                    return (p = AssetDatabase.GUIDToAssetPath(guid)).StartsWith(ourPath)
                        ? AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(p)
                        : null;
                }))
                if (layout != null && !layouts.Contains(layout))
                    layouts.Add(layout);
#endif
        }

        public static string CheckAssetPath()
        {
            string assetPath = String.Empty;
            string assetAssembly = String.Empty;
#if UNITY_EDITOR
            assetAssembly = AssetDatabase.FindAssets($"a:all t:asmdef {asmdefName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();

#endif
            if (assetAssembly != null)
            {
                var dirParent = Path.GetDirectoryName(Path.GetDirectoryName(assetAssembly));
                if (!string.IsNullOrWhiteSpace(dirParent))
                {
                    assetPath = dirParent.StartsWith("Packages")
                        ? dirParent.Replace("Packages", "Assets")
                        : dirParent;
                }
                else
                {
                    Debug.Log("AssemblyDefinition file could not be located");
                    throw new Exception("AssemblyDefinition file could not be located");
                }
            }

            return assetPath;
        }

        #region Asset Initial Settings
        // --| Asset Initial Settings -------------------------------
        // --|-------------------------------------------------------
        protected override string ResourcesLocation => "Assets/instance.id/instance.id.ProStream/AssetFiles";
        public override LoadType loadType => LoadType.EditorOnly;
        #endregion

#if UNITY_EDITOR
        public void UpdateStyles()
        {
            ReloadLookups(true);
        }
#endif
    }
}
