#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using instance.id.EATK.Extensions;
using UnityEngine;

namespace instance.id.EATK
{
    [Serializable]
    public class idEditorWindow : EditorWindow
    {
        private const string styleName = nameof(idEditorWindow);
        private Type styleType;

        public bool includeHeader;
        public virtual string headerText => "";
        public VisualElement root;

        private VisualElement internalRoot;
        private StyleSheet stylesheet;
        private List<StyleSheet> stylesheets;
        EATKStyleAsset styleAsset;

        public ContextualMenuManipulator headerManipulator;

        private void Awake() => Assignments();

        private void Assignments()
        {
            styleAsset = EATKStyleAsset.I;
            styleType = typeof(StyleSheet);
            stylesheet = styleAsset.GetStyleSheet(styleName);
        }

        public void AddStyles(string[] styleNames)
        {
            foreach (var styleStr in styleNames)
                root.styleSheets.Add(styleAsset.GetStyleSheet(styleStr));
        }

        protected virtual void Enable() { }
        private void OnEnable()
        {
            Assignments();
            root = new VisualElement();
            internalRoot = rootVisualElement;
            internalRoot.styleSheets.Add(stylesheet);
            internalRoot.AddToClassList("inspectorRoot");
            
            var styleArray = new[] {"rootContainer", "noTopMargin", "containerRow"};
            new VisualElement().CreateColumn(out root).ToUSS("idEditorWindowRootContainer", styleArray).AddAll(new VisualElement[]
            {
                new VisualElement().BuildHeader(out var rootHeader, headerText: headerText, className: "rootHeader").ToUSS(nameof(rootHeader), "noTopMargin"),
            }).SetParent(internalRoot);
            
            
            root.styleSheets.Add(stylesheet ??= this.FindAssets<StyleSheet>(styleName).FirstOrDefault());
            Enable();
            
            rootHeader.AddManipulator(headerManipulator);
        }
    }
}
#endif
