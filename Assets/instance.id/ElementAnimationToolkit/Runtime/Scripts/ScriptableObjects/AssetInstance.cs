using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;

#if ID_LOGGER
using instance.id.Logging;
#endif

namespace instance.id.EATK
{
    /// <summary>
    /// A Generic class for creating singleton assets.
    /// </summary>
    /// <typeparam name="T">The Type of the class you wish to make a Singleton for (likely the same class you're extending with).</typeparam>
    [Serializable]
    public abstract class AssetInstance<T> : ScriptableObject where T : AssetInstance<T>
    {
        private const string assetFolder = "Assets";
        public virtual LoadType loadType { get; set; } = LoadType.Resources;
        public enum SearchFilter { All, Assets, Packages }
        public enum LoadType { EditorOnly, Resources }

        private static T _Instance;

        /// <summary>
        /// Singleton asset instance of the provided type T.
        /// </summary>
        public static T I
        {
            get
            {
                if (_Instance != null) return _Instance;
                if (!((_Instance = LoadAssetOfType<T>()) is null)) return _Instance;
                string typeName = typeof(T).Name;

                Debug.Log(typeName + " Asset was initialised, its location is not fixed.");
                T instance = CreateInstance<T>();

                var lt = instance.loadType;

                _Instance = CreateInstanceInProject(typeName, instance.ResourcesLocation, instance);

                string niceName = _Instance.NicifiedTypeName;

#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(niceName))
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_Instance), $"{niceName}.asset");
#endif

                return _Instance;
            }
        }

        static string[] GetSearchDirs(SearchFilter searchAssets)
        {
            string[] searchDirs;
            switch (searchAssets)
            {
                case SearchFilter.All:
                    searchDirs = new[] {"Assets", "Packages"};
                    break;
                case SearchFilter.Assets:
                    searchDirs = new[] {"Assets"};
                    break;
                case SearchFilter.Packages:
                    searchDirs = new[] {"Packages"};
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchAssets), searchAssets, null);
            }

            return searchDirs;
        }

        /// <summary>
        /// Creates a ScriptableObject Asset in the specified location
        /// </summary>
        /// <param name="fileName">the file name for the Asset</param>
        /// <param name="folder">the folder to create the asset in</param>
        /// <param name="instance">Optional already-created instance of the asset (it will be saved to the project folder)</param>
        /// <typeparam name="T">The Type of asset</typeparam>
        /// <returns>the newly created Asset instance</returns>
        public static T CreateInstanceInProject(string fileName, string folder, T instance = null)
        {
            if (!fileName.Contains("."))
                fileName += ".asset";

            var basePath = folder.StartsWith(assetFolder)
                ? Application.dataPath.Substring(0, Application.dataPath.Length - assetFolder.Length)
                : Application.dataPath;

            if (instance.loadType == LoadType.Resources)
            {
                if (folder.EndsWith(Path.PathSeparator.ToString())) folder = folder.Substring(0, folder.Length - 1);
                if (!folder.EndsWith("Resources")) folder = Path.Combine(folder, "Resources");
            }

            string assetPath = Path.Combine(basePath, folder);

            if (!Directory.Exists(assetPath))
                Directory.CreateDirectory(assetPath);

            var path = Path.Combine(folder, fileName);
            if (instance == null) instance = CreateInstance<T>();

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
#endif
            return instance;
        }

        public static TSO LoadAssetOfType<TSO>(string query = null) where TSO : ScriptableObject
        {
#if UNITY_EDITOR
            if (!TryGetGUIDs(out var guids, typeof(TSO), query))
                return null;
#endif
            TSO anyCandidate = null;
#if UNITY_EDITOR
            foreach (string guid in guids)
            {
                var candidate = AssetDatabase.LoadAssetAtPath<TSO>(AssetDatabase.GUIDToAssetPath(guid));
                if (candidate == null)
                    continue;

                if (candidate.name == query)
                    return candidate;
                anyCandidate = candidate;
            }
#endif

            return anyCandidate;
        }

        public static List<TAssets> FindAssets<TAssets>(string query = null) where TAssets : UnityObject
        {
            List<TAssets> results = new List<TAssets>();
            var queryString = query != null ? $"a:all t:{typeof(TAssets).Name} {query}" : $"a:all t:{typeof(TAssets).Name}";

#if UNITY_EDITOR
            results = AssetDatabase.FindAssets(queryString)
                .Select(guid => AssetDatabase.LoadAssetAtPath<TAssets>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
#endif
            if (results.Count == 0) Debug.Log($"No Assets found of type: {typeof(TAssets)}");
            return results;
        }

#if UNITY_EDITOR
        private static bool TryGetGUIDs(out string[] guids, Type type, string query = null)
        {
            guids = AssetDatabase.FindAssets(query == null ? $"t:{type.FullName}" : $"t:{type.FullName} {query}");
            if (guids.Length == 0)
            {
                guids = AssetDatabase.FindAssets(query == null ? $"t:{type.Name}" : $"t:{type.Name} {query}");
                if (guids.Length == 0)
                    return false;
            }

            return true;
        }
#endif

        /// <summary>
        /// The name used when creating the asset instance.
        /// </summary>
        protected virtual string NicifiedTypeName => null;

        /// <summary>
        /// The location where the asset instance is created.
        /// </summary>
        protected virtual string ResourcesLocation => "Assets";

        protected void HardSave()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
            AssetDatabase.Refresh();
#endif
        }

        protected void SaveAsset()
        {
#if UNITY_EDITOR
            Debug.Log($"Saving Asset: {this.name}");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
#endif
        }

        // internal enum Priority { Info, Warning, Error}

        // --| Logging Methods --------------------------------------
#if ID_LOGGER
            [Conditional("ID_LOGGER")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden] [HideInCallstack] [StackTraceHidden]
        internal void Log(string message/*, Level level = default*/)
        {
            var str = $"<b><color=#3597D7>[EditorDebug] [{typeof(T).Name}] </color></b> <color=#EDE0CE>{message}</color>";
            Debug.Log(str);

    }
#endif
    }
}
