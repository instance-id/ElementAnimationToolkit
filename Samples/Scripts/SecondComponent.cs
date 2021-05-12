// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Examples
{
    [ExecuteInEditMode]
    [Serializable]
    public class SecondComponent : MonoBehaviour
    {
        public Action<string> highlight;
        public List<VisualElement> inspectorElements = new List<VisualElement>();

        public string characterName;
        public string location;

        public int health;
        public int mana;
        public int lives;

        public List<WeaponBase> availableWeapons = new List<WeaponBase>();

        public GameObject playerModel;
        public bool isNPC;
        public Vector3 spawnLocation;

        public bool showHat;

        public Vector4 containerBorders = new Vector4();
        public Vector4 headerBorders = new Vector4();

        public void HighLightComponent(string componentTarget)
        {
            highlight.Invoke(componentTarget);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var terms = "t:WeaponBase";
            availableWeapons = AssetDatabase.FindAssets(terms)
                .Select(guid => AssetDatabase.LoadAssetAtPath<WeaponBase>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
        }
#endif
    }
}
