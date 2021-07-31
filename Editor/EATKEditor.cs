// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------
// -- Note: EATK is still currently being developed - API subject to change  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using instance.id.EATK.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace instance.id.EATK
{
    [Serializable]
    public class EATKEditor : EditorWindow
    {
        // -- Element Objects -------------------
        private static EditorWindow window;
        private VisualElement root;
        private ScrollView elementScroller;
        private Toggle menuInfoToggle;
        private Label headerActionLabel;
        private Label headerJumpToCodeLabel;
        private AnimatedLabel instanceidLabel;
        private AnimatedLabel foldoutLabel1;
        private AnimatedLabel foldoutLabel2;
        private Button imageAnimationButton;
        private AnimatedFoldout imageAnimationAnimatedFoldout;
        [SerializeField] public StyleSheet styleSheet;

        // -- General Data ----------------------
        private bool isDebug = false;
        private bool isIntro;

        // -- Icon/Images -----------------------
        private Texture2D animatedIcon;
        private Font monoFont;

        // -- Element Colors --------------------
        // -- Generic / Base colors ----
        [UsedImplicitly] private Color originalColor;
        private Color pulseStartColor;
        private Color pulseEndColor;

        // -- Element Collections ---------------
        private readonly List<Button> buttonList = new List<Button>();
        private readonly List<Label> labelList = new List<Label>();
        private readonly List<VisualElement> rowList = new List<VisualElement>();
        private readonly Dictionary<string, float> labelWidthDictionary = new Dictionary<string, float>();
        private readonly Dictionary<string, float> labelHeightDictionary = new Dictionary<string, float>();
        private readonly Dictionary<string, VisualElement> buttonContainerDictionary = new Dictionary<string, VisualElement>();

        // -- Animation Types -------------------
        private ValueAnimation<StyleValues> widthAnimateFrom = new ValueAnimation<StyleValues>();
        private ValueAnimation<StyleValues> widthAnimateTo = new ValueAnimation<StyleValues>();
        private ValueAnimation<StyleValues> heightAnimateFrom = new ValueAnimation<StyleValues>();
        private ValueAnimation<StyleValues> heightAnimateTo = new ValueAnimation<StyleValues>();

        // -- Code lookup types -----------------
        [UsedImplicitly] private Type editorType;
        [UsedImplicitly] private Type animType;
        [UsedImplicitly] private Type ussType;

        // -- Sets whether or not to play intro animation ------
        // -- Primarily for debug and iteration ----------------
        private bool doIntro = true;

        [MenuItem("Tools/instance.id/Element Animation Toolkit")]
        private static void ShowWindow()
        {
            window = GetWindow<EATKEditor>();
            window.titleContent = new GUIContent("Element Animation Toolkit");
            window.minSize = new Vector2(530, 275);
            window.maxSize = new Vector2(530, 275);
        }

        private void OnEnable()
        {
            Assignments();
            BuildUIInterface();
        }

        private void Assignments()
        {
            editorType = GetType();
            animType = typeof(VisualElementBaseAnimation);
            ussType = typeof(StyleSheet);

            if (monoFont == null)
                monoFont = AssetFileExtensions.GetFont("SourceCodePro-Medium");

            StylesheetSetup();
            animatedIcon = (Texture2D) EditorGUIUtility.IconContent("ScriptableObject Icon").image;

            // -- Color Assignments -----------------------
            originalColor = ColorUtil.FromHex("#BABABA");
            pulseStartColor = ColorUtil.FromHex("#7F3B3A");
            pulseEndColor = ColorUtil.FromHex("#607FAE");
        }

        private StyleSheet StylesheetSetup()
        {
            if (styleSheet == null) styleSheet = editorType.GetStyleSheet();
            return styleSheet;
        }

        private void BuildUIInterface()
        {
            root = rootVisualElement;
            root.RegisterCallback<GeometryChangedEvent>(DeferredExecutionTasks);

            root.style.unityFont = monoFont;

            if (styleSheet != null) root.styleSheets.Add(StylesheetSetup());
            else Debug.Log($"StyleSheet for {editorType.Name} not found");

            // -- Toolbar -----------------------------------------------
            var toolbar = new Toolbar();
            toolbar.NameAsUSS(nameof(toolbar));

            var mainMenu = new ToolbarMenu {text = "Menu"};
            mainMenu.JumpToCode("mainMenu.JumpToCode");
            mainMenu.NameAsUSS(nameof(mainMenu));
            toolbar.Add(mainMenu);

            // -- Adds the menu items to the mainMenu --------------------
            BuildMainMenuItems(mainMenu);

            // -- Builds the remaining toolbar items ---------------------
            BuildToolbarButtons(toolbar);

            // -- Element Box --------------------------------------------
            var elementBox = new Box();
            elementBox.NameAsUSS(nameof(elementBox));

            // -- Element ScrollView -------------------------------------
            elementScroller = new ScrollView();
            elementScroller.NameAsUSS(nameof(elementScroller));

            // -- Header Elements ----------------------------------------
            // -- Header Row ---------------------------------------------
            var headerRowContainer = new VisualElement();
            headerRowContainer.NameAsUSS(nameof(headerRowContainer));

            headerActionLabel = new Label {text = "Action"};
            headerActionLabel.NameAsUSS(nameof(headerActionLabel));

            var headerDetailsLabel = new Label {text = "Details"};
            headerDetailsLabel.NameAsUSS(nameof(headerDetailsLabel));

            var headerSpacer = new VisualElement();
            headerSpacer.NameAsUSS(nameof(headerSpacer));

            headerJumpToCodeLabel = new Label {text = "Jump to Code"};
            headerJumpToCodeLabel.NameAsUSS(nameof(headerJumpToCodeLabel));

            headerRowContainer.Add(headerActionLabel);
            headerRowContainer.Add(headerDetailsLabel);
            headerRowContainer.Add(headerSpacer);
            headerRowContainer.Add(headerJumpToCodeLabel);
            elementScroller.Add(headerRowContainer);

            // ----------------------------------- Build Animated Elements
            // -- ImageAnimation -----------------------------------------
            BuildImageAnimation();

            // -- Fade Text Animation ------------------------------------
            BuildFadeTextAnimation();

            // -- Color Fade Animation -----------------------------------
            BuildFadeColorAnimation();

            // -- Width Change Animation ---------------------------------
            BuildWidthChangeAnimation();

            // -- Height Change Animation --------------------------------
            BuildHeightChangeAnimation();

            // -- Add all elements to main window ---------
            elementBox.Add(elementScroller);
            root.Add(toolbar);
            root.Add(elementBox);

            buttonContainerDictionary.ForEach(x =>
            {
                x.Value.AddToClassList("rightEdgePadding");
                x.Value.JumpToCode("ButtonCascadeAnimation", true, "Jump To Animation",
                    additionalMenus: new Dictionary<string, Action> {{"Replay Animation", ButtonCascadeAnimation}});
            });
        }

        #region Build Menu Items

        private void BuildMainMenuItems(ToolbarMenu mainMenu)
        {
            mainMenu.menu.AppendAction(
                "Replay Animations",
                a => { PlayIntroAnimationSequence(true); },
                a => DropdownMenuAction.Status.Normal);

            mainMenu.menu.AppendAction(
                "Animated Intro Code",
                a =>
                {
                    editorType
                        .GetScriptFile("IntroAnimation", out var lineNum, true)
                        .ToIDE(lineNum);
                },
                a => DropdownMenuAction.Status.Normal);
        }

        // -------------------------------------------------------- @ToolbarInfoMenu
        // -------------------------------------------------------------------------
        private void BuildToolbarButtons(Toolbar toolbar)
        {
            // -- Expanding spacer used to separate toolbar items                 --
            // ReSharper disable once EntityNameCapturedOnly.Local
            new ToolbarSpacer {flex = true}.Create(out var toolbarSpacer).ToUSS(nameof(toolbarSpacer)).SetParent(toolbar);

            new VisualElement().Create(out var menuInfoParent)
                .ToUSS(nameof(menuInfoParent))
                .SetParent(toolbar);

            // -- Toolbar info container -- @EditorMenuInfo
            new VisualElement().Create(out var menuInfoContainer)
                .ToUSS(nameof(menuInfoContainer)).SetParent(menuInfoParent)
                .JumpToCode("ToolbarInfoMenu", true);

            // -- Create the Discord menu item and make it a clickable link       --
            // -- Set the initial opacity to 0 so that it can fade in when opened --
            new AnimatedLabel {text = "Github", style = {opacity = 0.Zero()}}
                .Create(out var discordLabel).ToUSS(nameof(discordLabel)).SetParent(menuInfoContainer)
                .OpenURL("https://github.com/instance-id/ElementAnimationToolkit");

            // -- Create the Documentation menu item and make it a clickable link --
            // -- Set the initial opacity to 0 so that it can fade in when opened --
            new AnimatedLabel {text = "Documentation", style = {opacity = 0.Zero()}}
                .Create(out var docsLabel).ToUSS(nameof(docsLabel)).SetParent(menuInfoContainer)
                .OpenURL("https://docs.instance.id/elementanimationtoolkit/");

            // ----------------------------------------------------- @MenuInfoToggle
            // -- The below Toggle lets us utilize it's "ValueChanged" callback,  --
            // -- which emits a signal when the value is set, (even when it is not -
            // -- being displayed) which is then used to hide/unhide and animate  --
            // -- menuInfoContainer, which contains the Discord and Docs links    --
            new Toggle {style = {display = DisplayStyle.None}}
                .Create(out menuInfoToggle).ToUSS(nameof(menuInfoToggle)).SetParent(toolbar);

            // -- Registration of the "ValueChange" callback, so that when        --
            // -- menuInfoToggle's value is set to true/false, it calls the       --
            // -- functions contained within the lambda                           --
            menuInfoToggle.RegisterValueChangedCallback(evt =>
            {
                // -- Pass necessary elements and values to animation function    --
                HeaderInfoMenuAnimation(evt.newValue, menuInfoContainer, discordLabel, docsLabel);
            });

            // -- Automatically close menu after duration (set by toggleTimer)    --
            // -- after the mouse leaves the menu area. If the mouse re-enters    --
            // -- the menu area, this blocks the menu from closing until the      --
            // -- mouse leaves again, resetting the timer                         --
            menuInfoParent.AutoToggleAfter(menuInfoToggle, 900, true);

            // -------------------------------------------------------- @ToolBarInfo
            // -- Info button toggles the above menuInfoToggle and kicks off the  --
            // -- above functionality based on if the new value is true or false  --
            new ToolbarButton(() => menuInfoToggle.value = !menuInfoToggle.value) {text = "Information"}
                .Create(out var toolbarInfo).ToUSS(nameof(toolbarInfo)).SetParent(menuInfoParent);

            // -- Register the HoverBorderPulse callback                          --
            toolbarInfo.HoverBorderPulse(
                pulseStartColor: ColorUtil.FromHex("#7F3B3A"),
                pulseEndColor: ColorUtil.FromHex("#2F569C"),
                colorDuration: 500);
        }

        // ------------------------------------------ @InstanceIdContainer
        // ---------------------------------------------------------------
        private VisualElement BuildInstanceIdContainer()
        {
            var instanceIdContainer = new VisualElement();
            instanceIdContainer.NameAsUSS(nameof(instanceIdContainer));
            instanceIdContainer.AddToClassList("rightEdgePadding");

            instanceidLabel = new AnimatedLabel {text = "instance.id"};
            instanceidLabel.NameAsUSS(nameof(instanceidLabel));
            instanceidLabel.JumpToCode(
                jumpTargets: new List<JumpTarget>
                {
                    new JumpTarget {Locator = "_IntroAnimateIdLabel", MenuTitle = "Jump to Animation", JumpType = JumpType.Animation},
                    new JumpTarget {Locator = "_InstanceIdContainer", MenuTitle = "Jump to Element", JumpType = JumpType.Element},
                    new JumpTarget {Locator = "_InstanceIdLabel", MenuTitle = "Jump to USS", JumpType = JumpType.USS},
                },
                additionalMenus: new Dictionary<string, Action> {{"Replay Animation", AnimateIdLabel}});

            instanceIdContainer.Add(instanceidLabel);
            return instanceIdContainer;
        }

        #endregion

        // @formatter:off -------------------------- @EditorImageAnimation
        // ---------------------------------------------------------------
        #region ImageAnimation
        private void BuildImageAnimation() // @formatter:on
        {
            // -- Image Color Animation ----------------------------------
            new VisualElement().Create(out var imageAnimationContainer).ToUSS(nameof(imageAnimationContainer));
            imageAnimationContainer.JumpToCode("HoverBorderPulse", true);
            rowList.Add(imageAnimationContainer);

            new VisualElement().Create(out var imageAnimationLabelColumn).ToUSS(nameof(imageAnimationLabelColumn));

            // -- @AnimatedFoldout ---------------------------------------
            // -- The AnimatedFoldout is a custom element type I created
            // -- that works just like a normal foldout, but the action
            // -- of showing/hiding the contents is animated, of course!
            new AnimatedFoldout {text = "Click me, I open!", value = false}.Create(out imageAnimationAnimatedFoldout).ToUSS(nameof(imageAnimationAnimatedFoldout));
            imageAnimationAnimatedFoldout.JumpToCode("AnimatedFoldout", true, jumpUSS: true,
                additionalMenus: new Dictionary<string, Action> {{"Replay Animation", FoldoutAnimation}});
            imageAnimationAnimatedFoldout.RegisterValueChangedCallback(evt =>
            {
                imageAnimationAnimatedFoldout.text = evt.newValue
                    ? "Animated Foldout"
                    : "Click me, I open!";
            });

            new VisualElement().Create(out var foldoutLabelContainer).ToUSS(nameof(foldoutLabelContainer));
            new AnimatedLabel {text = "Right click for source."}.Create(out foldoutLabel1).ToUSS(nameof(foldoutLabel1));
            new AnimatedLabel {text = "Editor code or USS styling!"}.Create(out foldoutLabel2).ToUSS(nameof(foldoutLabel2));

            foldoutLabelContainer.Add(foldoutLabel1);
            foldoutLabelContainer.Add(foldoutLabel2);

            imageAnimationAnimatedFoldout.Add(foldoutLabelContainer);
            // --- End Animated Foldout ---------

            new Label {text = "Click Icon to animate icon image color!"}.Create(out var imageAnimationLabel).ToUSS(nameof(imageAnimationLabel));
            labelList.Add(imageAnimationLabel);

            new Button(() => ImageAnimation(imageAnimationButton)) {style = {backgroundImage = animatedIcon}}
                .Create(out imageAnimationButton)
                .ToUSS(nameof(imageAnimationButton));
            buttonList.Add(imageAnimationButton);

            new VisualElement().Create(out var imageAnimationSpacer).ToUSS(nameof(imageAnimationSpacer));

            // -- Nested Info Column -------------------------------------
            new VisualElement().Create(out var nestedInfoColumn).ToUSS(nameof(nestedInfoColumn));

            // -- Button Container -------------------
            new VisualElement().Create(out var imageAnimationButtonContainer).ToUSS(nameof(imageAnimationButtonContainer));
            imageAnimationButtonContainer.JumpToCode("imageAnimationButtonContainer.JumpToCode",
                additionalMenus: new Dictionary<string, Action> {{"Replay Animation", ButtonCascadeAnimation}});

            new Button(() => editorType.GetScriptFile("EditorImageAnimation", out var lineNum).ToIDE(lineNum))
                    {text = "Editor", tooltip = "Click to jump to the editor code for this element"}
                .Create(out var imageAnimationEditorButton)
                .ToUSS(nameof(imageAnimationEditorButton));

            new Button(() => editorType.GetScriptFile("AnimImageAnimation", out var lineNum).ToIDE(lineNum))
                    {text = "Anim", tooltip = "Click to jump to the code for this animation"}
                .Create(out var imageAnimationAnimButton)
                .ToUSS(nameof(imageAnimationAnimButton));

            new Button(() => editorType.GetStylesheetFile("ImageAnimation", out var lineNum).ToIDE(lineNum))
                    {text = "USS", tooltip = "Click to jump to the USS style code for this element"}
                .Create(out var imageAnimationUSSButton)
                .ToUSS(nameof(imageAnimationUSSButton));

            // -- Add Buttons to Button Container ----
            imageAnimationButtonContainer.AddAll(new VisualElement[] {imageAnimationEditorButton, imageAnimationAnimButton, imageAnimationUSSButton});
            buttonContainerDictionary.Add(imageAnimationButtonContainer.name, imageAnimationButtonContainer);

            imageAnimationLabelColumn.AddAll(new VisualElement[] {imageAnimationAnimatedFoldout, imageAnimationLabel});

            // -- Image Animation Info Column --------
            nestedInfoColumn.AddAll(new[] {BuildInstanceIdContainer(), imageAnimationButtonContainer});

            // -- Add elements to Row Container ------
            var items = new[] {imageAnimationButton, imageAnimationLabelColumn, imageAnimationSpacer, nestedInfoColumn};
            imageAnimationContainer.AddAll(items);
            elementScroller.Add(imageAnimationContainer);
        }

        #endregion

        // @formatter:off ----------------------- @EditorFadeTextAnimation
        // ---------------------------------------------------------------
        #region FadeTextAnimation
        private void BuildFadeTextAnimation() // @formatter:on
        {
            // -- Text Fade Animation ------------------------------------
            var fadeTextAnimationContainer = new VisualElement();
            fadeTextAnimationContainer.NameAsUSS(nameof(fadeTextAnimationContainer));
            rowList.Add(fadeTextAnimationContainer);

            var fadeTextAnimationLabel = new Label("Click button to make me fade!");
            fadeTextAnimationLabel.NameAsUSS(nameof(fadeTextAnimationLabel));

            var fadeTextAnimButton = new Button(() => FadeTextAnimation(fadeTextAnimationLabel)) {text = "Fade Out/In"};
            fadeTextAnimButton.NameAsUSS(nameof(fadeTextAnimButton));
            buttonList.Add(fadeTextAnimButton);
            labelList.Add(fadeTextAnimationLabel);

            var fadeTextAnimationSpacer = new VisualElement();
            fadeTextAnimationSpacer.NameAsUSS(nameof(fadeTextAnimationSpacer));

            // -- Button Container -------------------
            new VisualElement().Create(out var fadeTextAnimationButtonContainer).ToUSS(nameof(fadeTextAnimationButtonContainer));

            var fadeTextAnimationEditorButton = new Button(() =>
                    editorType.GetScriptFile("EditorFadeTextAnimation", out var lineNum).ToIDE(lineNum))
                {text = "Editor", tooltip = "Click to jump to the editor code for this element"};
            fadeTextAnimationEditorButton.NameAsUSS(nameof(fadeTextAnimationEditorButton));

            var fadeTextAnimationAnimButton = new Button(() =>
                    editorType.GetScriptFile("AnimFadeTextAnimation", out var lineNum).ToIDE(lineNum))
                {text = "Anim", tooltip = "Click to jump to the code for this animation"};
            fadeTextAnimationAnimButton.NameAsUSS(nameof(fadeTextAnimationAnimButton));

            var fadeTextAnimationUSSButton = new Button(() =>
                    editorType.GetStylesheetFile("FadeTextAnimation", out var lineNum).ToIDE(lineNum))
                {text = "USS", tooltip = "Click to jump to the USS style code for this element"};
            fadeTextAnimationUSSButton.NameAsUSS(nameof(fadeTextAnimationUSSButton));

            // -- Add Buttons to Button Container ----
            fadeTextAnimationButtonContainer
                .AddAll(new VisualElement[] {fadeTextAnimationEditorButton, fadeTextAnimationAnimButton, fadeTextAnimationUSSButton});
            buttonContainerDictionary.Add(fadeTextAnimationButtonContainer.name, fadeTextAnimationButtonContainer);

            // -- Add elements to Row Container ------
            var ftaItems = new[] {fadeTextAnimButton, fadeTextAnimationLabel, fadeTextAnimationSpacer, fadeTextAnimationButtonContainer};
            fadeTextAnimationContainer.AddAll(ftaItems).SetParent(elementScroller);
        }

        #endregion

        // @formatter:off ---------------------- @EditorFadeColorAnimation
        // ---------------------------------------------------------------
        #region FadeColorAnimation
        private void BuildFadeColorAnimation() // @formatter:on
        {
            // -- Color Fade Animation -----------------------------------
            var fadeColorAnimationContainer = new VisualElement();
            fadeColorAnimationContainer.NameAsUSS(nameof(fadeColorAnimationContainer));
            rowList.Add(fadeColorAnimationContainer);

            var fadeColorAnimationLabel = new Label("Click button to make my colors change!");
            fadeColorAnimationLabel.NameAsUSS(nameof(fadeColorAnimationLabel));
            labelList.Add(fadeColorAnimationLabel);

            var fadeColorAnimationButton = new Button(() => FadeColorAnimation(fadeColorAnimationLabel)) {text = "Fade Color"};
            fadeColorAnimationButton.NameAsUSS(nameof(fadeColorAnimationButton));
            buttonList.Add(fadeColorAnimationButton);

            var fadeColorAnimationSpacer = new VisualElement();
            fadeColorAnimationSpacer.NameAsUSS(nameof(fadeColorAnimationSpacer));

            // -- Button Container -------------------
            var fadeColorButtonContainer = new VisualElement();
            fadeColorButtonContainer.NameAsUSS(nameof(fadeColorButtonContainer));

            var fadeColorAnimationEditorButton = new Button(() =>
                editorType.GetScriptFile("EditorFadeColorAnimation", out var lineNum, true).ToIDE(lineNum))
            {
                text = "Editor",
                tooltip = "Click to jump to the editor code for this element"
            };
            fadeColorAnimationEditorButton.NameAsUSS(nameof(fadeColorAnimationEditorButton));

            var fadeColorAnimationAnimButton = new Button(() =>
                editorType.GetScriptFile("AnimFadeColorAnimation", out var lineNum, true).ToIDE(lineNum))
            {
                text = "Anim",
                tooltip = "Click to jump to the code for this animation"
            };
            fadeColorAnimationAnimButton.NameAsUSS(nameof(fadeColorAnimationAnimButton));

            var fadeColorAnimationUSSButton = new Button(() =>
                editorType.GetStylesheetFile("FadeColorAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "USS",
                tooltip = "Click to jump to the USS style code for this element"
            };
            fadeColorAnimationUSSButton.NameAsUSS(nameof(fadeColorAnimationUSSButton));

            // -- Add Buttons to Button Container ----
            fadeColorButtonContainer.Add(fadeColorAnimationEditorButton);
            fadeColorButtonContainer.Add(fadeColorAnimationAnimButton);
            fadeColorButtonContainer.Add(fadeColorAnimationUSSButton);
            buttonContainerDictionary.Add(fadeColorButtonContainer.name, fadeColorButtonContainer);

            // -- Add elements to Row Container ------
            fadeColorAnimationContainer.Add(fadeColorAnimationButton);
            fadeColorAnimationContainer.Add(fadeColorAnimationLabel);
            fadeColorAnimationContainer.Add(fadeColorAnimationSpacer);
            fadeColorAnimationContainer.Add(fadeColorButtonContainer);
            elementScroller.Add(fadeColorAnimationContainer);
        }

        #endregion

        // @formatter:off -------------------- @EditorWidthChangeAnimation
        // ---------------------------------------------------------------
        #region WidthChangeAnimation
        private void BuildWidthChangeAnimation() // @formatter:on
        {
            // -- Width Change Animation ---------------------------------
            var widthChangeAnimationContainer = new VisualElement();
            widthChangeAnimationContainer.NameAsUSS(nameof(widthChangeAnimationContainer));
            rowList.Add(widthChangeAnimationContainer);

            var widthChangeAnimationLabel = new Label("Click button to change my width!");
            widthChangeAnimationLabel.NameAsUSS(nameof(widthChangeAnimationLabel));
            labelList.Add(widthChangeAnimationLabel);

            var widthChangeAnimationButton = new Button(() => WidthAnimation(widthChangeAnimationLabel)) {text = "Width"};
            widthChangeAnimationButton.NameAsUSS(nameof(widthChangeAnimationButton));
            buttonList.Add(widthChangeAnimationButton);

            var widthChangeAnimationSpacer = new VisualElement();
            widthChangeAnimationSpacer.NameAsUSS(nameof(widthChangeAnimationSpacer));

            // -- Button Container -------------------
            var widthChangeAnimationButtonContainer = new VisualElement();
            widthChangeAnimationButtonContainer.NameAsUSS(nameof(widthChangeAnimationButtonContainer));

            var widthChangeAnimationEditorButton = new Button(() =>
                editorType.GetScriptFile("EditorWidthChangeAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "Editor",
                tooltip = "Click to jump to the editor code for this element"
            };
            widthChangeAnimationEditorButton.NameAsUSS(nameof(widthChangeAnimationEditorButton));

            var widthChangeAnimationAnimButton = new Button(() =>
                editorType.GetScriptFile("AnimWidthChangeAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "Anim",
                tooltip = "Click to jump to the code for this animation"
            };
            widthChangeAnimationAnimButton.NameAsUSS(nameof(widthChangeAnimationAnimButton));

            var widthChangeAnimationUSSButton = new Button(() =>
                editorType.GetStylesheetFile("WidthChangeAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "USS",
                tooltip = "Click to jump to the USS style code for this element"
            };
            widthChangeAnimationUSSButton.NameAsUSS(nameof(widthChangeAnimationUSSButton));

            // -- Add Buttons to Button Container ----
            widthChangeAnimationButtonContainer.Add(widthChangeAnimationEditorButton);
            widthChangeAnimationButtonContainer.Add(widthChangeAnimationAnimButton);
            widthChangeAnimationButtonContainer.Add(widthChangeAnimationUSSButton);
            buttonContainerDictionary.Add(widthChangeAnimationButtonContainer.name, widthChangeAnimationButtonContainer);

            // -- Add elements to Row Container ------
            widthChangeAnimationContainer.Add(widthChangeAnimationButton);
            widthChangeAnimationContainer.Add(widthChangeAnimationLabel);
            widthChangeAnimationContainer.Add(widthChangeAnimationSpacer);
            widthChangeAnimationContainer.Add(widthChangeAnimationButtonContainer);
            elementScroller.Add(widthChangeAnimationContainer);
        }

        #endregion

        // @formatter:off ------------------- @EditorHeightChangeAnimation
        // ---------------------------------------------------------------
        #region HeightChangeAnimation
        private void BuildHeightChangeAnimation() // @formatter:on
        {
            // -- Height Change Animation --------------------------------
            var heightChangeAnimationContainer = new VisualElement();
            heightChangeAnimationContainer.NameAsUSS(nameof(heightChangeAnimationContainer));
            rowList.Add(heightChangeAnimationContainer);

            var heightChangeAnimationLabel = new Label("Click button to change my height!");
            heightChangeAnimationLabel.NameAsUSS(nameof(heightChangeAnimationLabel));
            labelList.Add(heightChangeAnimationLabel);

            var heightChangeAnimationButton = new Button(() => HeightAnimation(heightChangeAnimationLabel)) {text = "Height"};
            heightChangeAnimationButton.NameAsUSS(nameof(heightChangeAnimationButton));
            buttonList.Add(heightChangeAnimationButton);

            var heightChangeAnimationSpacer = new VisualElement();
            heightChangeAnimationSpacer.NameAsUSS(nameof(heightChangeAnimationSpacer));

            // -- Button Container -------------------
            var heightChangeAnimationButtonContainer = new VisualElement();
            heightChangeAnimationButtonContainer.NameAsUSS(nameof(heightChangeAnimationButtonContainer));

            var heightChangeAnimationEditorButton = new Button(() =>
                editorType.GetScriptFile("EditorHeightChangeAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "Editor",
                tooltip = "Click to jump to the editor code for this element"
            };
            heightChangeAnimationEditorButton.NameAsUSS(nameof(heightChangeAnimationEditorButton));

            var heightChangeAnimationAnimButton = new Button(() =>
                editorType.GetScriptFile("AnimHeightChangeAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "Anim",
                tooltip = "Click to jump to the code for this animation"
            };
            heightChangeAnimationAnimButton.NameAsUSS(nameof(heightChangeAnimationAnimButton));

            var heightChangeAnimationUSSButton = new Button(() =>
                editorType.GetStylesheetFile("HeightChangeAnimation", out var lineNum).ToIDE(lineNum))
            {
                text = "USS",
                tooltip = "Click to jump to the USS style code for this element"
            };
            heightChangeAnimationUSSButton.NameAsUSS(nameof(heightChangeAnimationUSSButton));

            // -- Add Buttons to Button Container ----
            heightChangeAnimationButtonContainer.Add(heightChangeAnimationEditorButton);
            heightChangeAnimationButtonContainer.Add(heightChangeAnimationAnimButton);
            heightChangeAnimationButtonContainer.Add(heightChangeAnimationUSSButton);
            buttonContainerDictionary.Add(heightChangeAnimationButtonContainer.name, heightChangeAnimationButtonContainer);

            // -- Add elements to Row Container ------
            heightChangeAnimationContainer.Add(heightChangeAnimationButton);
            heightChangeAnimationContainer.Add(heightChangeAnimationLabel);
            heightChangeAnimationContainer.Add(heightChangeAnimationSpacer);
            heightChangeAnimationContainer.Add(heightChangeAnimationButtonContainer);
            elementScroller.Add(heightChangeAnimationContainer);
        }

        #endregion

        // --------------------------------------- @DeferredExecutionTasks
        /// <summary>
        /// Count the characters in each buttons text and return the
        /// </summary>
        /// <param name="evt">The GeometryChangedEvent parameters</param>
        private void DeferredExecutionTasks(GeometryChangedEvent evt)
        {
            root.UnregisterCallback<GeometryChangedEvent>(DeferredExecutionTasks);

            // ------------------------------------ Label Width Adjustment
            // Count the characters in each buttons text and return the --
            // highest int. Multiply the int by the approximate with of --
            // each character in pixels + add slight padding and then   --
            // apply that value to each buttons width to become uniform --
            // -----------------------------------------------------------
            var actionColumnWidth = buttonList
                .Select(x => x.text)
                .Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;
            var adjustedActionColumnWidth = actionColumnWidth * 5.5f + 20f;

            buttonList.ForEach(b =>
            {
                if (b.name != "imageAnimationButton")
                    b.style.width = adjustedActionColumnWidth;
                else
                {
                    b.style.width = adjustedActionColumnWidth - 1;
                    b.style.height = b.style.width;
                }
            });
            headerActionLabel.style.width = adjustedActionColumnWidth + 5;

            var codeHeaderWidth = buttonContainerDictionary
                .Select(x => x.Value.resolvedStyle.width)
                .Aggregate(0, (max, cur) => max > (int) cur ? max : (int) cur);
            headerJumpToCodeLabel.style.width = codeHeaderWidth + 15;

            // -- Get the actual width of some elements and save        --
            // -- to dictionary for use elsewhere                       --
            labelList.ForEach(l =>
            {
                labelWidthDictionary.Add(l.name, l.resolvedStyle.width);
                labelHeightDictionary.Add(l.name, l.resolvedStyle.height);
            });

            rowList.ForEach(r =>
            {
                // @HoverBorderPulse
                r.HoverBorderPulse(
                    pulseStartColor,
                    pulseEndColor);
            });

            PlayIntroAnimationSequence();
        }

        // @formatter:off -------------------------------- @IntroAnimation
        // -- Executes each function after another to demonstrate       --
        // -- a few of the different possibilities of animations        --
        // ---------------------------------------------------------------
        private void PlayIntroAnimationSequence(bool forceIntro = false)
        {
            if (!forceIntro && !doIntro) return;
            // -- Lets methods know that these particular animations    --
            // -- are occurring for the intro sequence in case there    --
            // -- are conditional actions that occur during the intro   --
            isIntro = true;

            // -- Callbacks for each main animations so that they can   --
            // -- be sequentially animated with a cascading delay       --
            void ImageAnim()  { ImageAnimation(buttonList.FirstOrDefault(l => l.name == "imageAnimationButton"), Easy.InSine); }
            void TextAnim()   { FadeTextAnimation(labelList.FirstOrDefault(l => l.name == "fadeTextAnimationLabel"), Easy.InSine); }
            void FadeAnim()   { FadeColorAnimation(labelList.FirstOrDefault(l => l.name == "fadeColorAnimationLabel"), Easy.InSine); }
            void WidthAnim()  { WidthAnimation(labelList.FirstOrDefault(l => l.name == "widthChangeAnimationLabel"), Easy.InSine); }
            void HeightAnim() { HeightAnimation(labelList.FirstOrDefault(l => l.name == "heightChangeAnimationLabel"), Easy.InSine); }

            var cascade = 400;
            root.schedule.Execute(ImageAnim).StartingIn(cascade);
            root.schedule.Execute(TextAnim).StartingIn(cascade += 400);
            root.schedule.Execute(FadeAnim).StartingIn(cascade += 400);
            root.schedule.Execute(WidthAnim).StartingIn(cascade += 400);
            root.schedule.Execute(HeightAnim).StartingIn(cascade += 400); // @formatter:on

            // -- Play the AnimatedFoldout Animation ---------------------
            FoldoutAnimation();

            // -- Animates the toolbar "info" menu -----------------------
            ToolBarInfoAnimation();

            // -- Animates the "instance.id" text label ------------------
            AnimateIdLabel();

            // -- Animates the Jump To Code button group -----------------
            ButtonCascadeAnimation();

            // -- Sets the condition that the intro is completed, so    --
            // -- those that need to know can appropriately change      --
            // -- their behaviour                                       --
            root.schedule.Execute(() =>
            {
                isIntro = false;
                if (isDebug) Debug.Log($"Intro Complete: {!isIntro}");
            }).StartingIn(5000);
        }

        #region Animations

        #region Intro Animations

        // -- @IntroFoldoutAnimation -------------------------------------
        private void FoldoutAnimation()
        {
            // -- AnimatedFoldout values ------------------
            var openDelayMs = 500;
            var closeDelayMs = 4500;
            var animatedColor = ColorUtil.FromHex("#2F569C");

            // -- Animated Label values -------------------
            var cascadeMs = 50;
            var durationMs = 100;
            var label1Delay = 200;
            var label2Delay = 200;

            // -- Open and close the animated foldout menu ---------------
            // -- The AnimatedFoldout opens and closes when the element --
            // -- value is set to true/false, so we set it manually     --
            imageAnimationAnimatedFoldout.schedule.Execute(() => imageAnimationAnimatedFoldout.value = true).StartingIn(openDelayMs);
            imageAnimationAnimatedFoldout.schedule.Execute(() => imageAnimationAnimatedFoldout.value = false).StartingIn(closeDelayMs);

            // -- Animate the labels within the foldout ------------------
            foldoutLabel1.schedule.Execute(() =>
            {
                foldoutLabel1.AnimCharacterSequence(
                    color1: originalColor,
                    color2: animatedColor,
                    cascadeMs: cascadeMs,
                    durationMS: durationMs);
            }).StartingIn(label1Delay);

            foldoutLabel2.schedule.Execute(() =>
            {
                foldoutLabel2.AnimCharacterSequence(
                    color1: originalColor,
                    color2: animatedColor,
                    cascadeMs: cascadeMs,
                    durationMS: durationMs);
            }).StartingIn(label2Delay);
        } // @formatter:on

        private void ToolBarInfoAnimation()
        {
            menuInfoToggle.schedule.Execute(() => menuInfoToggle.value = true).StartingIn(500);
            menuInfoToggle.schedule.Execute(() => menuInfoToggle.value = false).StartingIn(4500);
        }

        // -- @IntroAnimateIdLabel ---------------------------------------
        private void AnimateIdLabel()
        {
            var cascadeMs = 175;
            var durationMs = 400;

            instanceidLabel.AnimCharacterSequence(
                color1: originalColor,
                color2: ColorUtil.FromHex("#2F569C"),
                cascadeMs: cascadeMs,
                durationMS: durationMs);
        }

        // --------------------------------------- @ButtonCascadeAnimation
        // -- By iterating over a dictionary of VisualElements(multiple --
        // -- rows, each with multiple buttons) and incrementing the    --
        // -- delay time after each animation, the cascade effect seen  --
        // -- in the intro animation is achieved                        --
        // ---------------------------------------------------------------
        private void ButtonCascadeAnimation() // @formatter:on
        {
            // -- Image colors ------------------
            var buttonColor = ColorUtil.FromHex("#676767");
            var endButtonColor = ColorUtil.FromHex("#2F569C");
            const int durationInMs = 500;
            const int delayInMs = 1000;

            // -- Begin the animated sequence after a short delay --------
            root.schedule.Execute(ButtonCascade).StartingIn(delayInMs + 500);

            // -- The first animation fades buttons to desired color -----
            void ButtonCascade()
            {
                var loopTime = 0;
                var iteration = 0;
                var totalCount = buttonContainerDictionary.Count;

                // -- Iterate over the dictionary of row elements --------
                buttonContainerDictionary.ForEach(x =>
                {
                    // -- Then query for each button within the row ------
                    // -- Set default button cascade delay and counters --
                    var buttons = x.Value.Query<Button>().ToList();
                    var buttonCascade = 100;
                    var buttonCount = buttons.Count;
                    var buttonIteration = 0;

                    // -- Loop through each button in this row -----------
                    buttons.ForEach(b =>
                    {
                        // ReSharper disable once AccessToModifiedClosure

                        // -- Check if this is the last button/row, if  --
                        // -- so, call the reversal callback, if not,   --
                        // -- perform animation on next button in row   --
                        if (totalCount == iteration + 1 && buttonCount == buttonIteration + 1)
                            b.schedule.Execute(() => // ReSharper disable once AccessToModifiedClosure
                                b.AnimateBackgroundColor(buttonColor, endButtonColor, durationInMs, ButtonCascadeReversal)).StartingIn(buttonCascade + loopTime);
                        else
                            b.schedule.Execute(() => // ReSharper disable once AccessToModifiedClosure
                                b.AnimateBackgroundColor(buttonColor, endButtonColor, durationInMs)).StartingIn(buttonCascade + loopTime);

                        // -- Increment cascade delay after each button --
                        buttonCascade += buttonCascade;
                        buttonIteration++;
                    });
                    // -- Slightly increment the delay for the next row --
                    // -- and increment the current row count ------------
                    loopTime += 100;
                    iteration++;
                });
            }

            // -- The second animation fades color back to the original --
            // -- The steps are the same as the first animation, minus  --
            // -- there is no callback at the end when completed ---------
            void ButtonCascadeReversal()
            {
                // -- Start reversal animation after desired delay -------
                root.schedule.Execute(StartReturnLoop).StartingIn(delayInMs);

                void StartReturnLoop()
                {
                    var loopTime = 0;
                    buttonContainerDictionary.ForEach(x =>
                    {
                        var buttons = x.Value.Query<Button>().ToList();
                        var buttonCascade = 100;
                        buttons.ForEach(b =>
                        {
                            b.schedule.Execute(() => // ReSharper disable once AccessToModifiedClosure
                                b.AnimateBackgroundColor(endButtonColor, buttonColor, durationInMs)).StartingIn(buttonCascade + loopTime);
                            buttonCascade += buttonCascade;
                        });
                        loopTime += 100;
                    });
                }
            }
        }

        #endregion


        #region Header Animation

        // -- @HeaderInfoMenuAnimation -----------------------------------
        private void HeaderInfoMenuAnimation(bool evt, VisualElement menuInfoContainer, AnimatedLabel discordLabel, AnimatedLabel docsLabel)
        {
            // -- Define animation values -------
            const float hidden = 0f;
            const float visible = 1f;
            const int smallWidth = 1;
            const float fullWidth = 160;
            const int durationMs = 500;
            const int inDurationMs = 300;
            const int outDurationMs = 150;
            var hoverLabelColor = ColorUtil.FromHex("#2F569C");
            var originalInfoBgColor = ColorUtil.FromHex("#303030");
            var toolbarHoverColor = ColorUtil.FromHex("#2F569C");

            // -- Local Func which is used as a callback for AnimateWidth()  --
            // -- so that when the large => small animation completes,       --
            // -- it hides the container completely from view                --
            void HideDisplay()
            {
                menuInfoContainer.style.display = DisplayStyle.None;
            }

            // -- Local Func which Registers the MouseOver callbacks for     --
            // -- the labels once the animations are completed               --
            void RegisterHoverCallback()
            {
                discordLabel.HoverColor(originalColor, toolbarHoverColor, animate: true);
                docsLabel.HoverColor(originalColor, toolbarHoverColor, animate: true);
            }

            void UnregisterHoverCallback()
            {
                discordLabel.HoverColor(originalColor, toolbarHoverColor, animate: true, unregister: true);
                docsLabel.HoverColor(originalColor, toolbarHoverColor, animate: true, unregister: true);
            }

            // -- If the value was set to true, display the container and    --
            // -- animate it from smallWidth => fullWidth over durationMs    --
            // -- and fade in the text opacity for the labels from 0 => 1    --
            if (evt)
            {
                menuInfoContainer.style.display = DisplayStyle.Flex;

                // -- Fade In Labels ------------------------------------------
                // -- Labels start with 0 opacity and then appear after      --
                // -- 250 milliseconds, with slightly staggered time         --
                menuInfoContainer.schedule.Execute(() =>
                {
                    discordLabel.SetOpacity();
                    discordLabel.AnimateOpacity(
                        startOpacity: hidden,
                        endOpacity: visible,
                        durationMs: inDurationMs,
                        easing: Easy.InQuad, callback: RegisterHoverCallback);
                }).StartingIn(300);

                menuInfoContainer.schedule.Execute(() =>
                {
                    docsLabel.SetOpacity();
                    docsLabel.AnimateOpacity(
                        startOpacity: hidden,
                        endOpacity: visible,
                        durationMs: inDurationMs,
                        easing: Easy.InQuad);
                }).StartingIn(250);

                // -- Fade In Background Container Color ----------------------
                menuInfoContainer.schedule.Execute(() =>
                {
                    var menupg = VisualElementBaseAnimation.AnimateBackgroundColor(menuInfoContainer, startColor: originalInfoBgColor,
                        endColor: ColorUtil.FromHex("#212121"),
                        durationMs: 600);
                    menupg.Start();
                }).StartingIn(0);

                menuInfoContainer.AnimateWidth(
                    startWidth: smallWidth,
                    endWidth: fullWidth,
                    durationMs: durationMs,
                    easing: Easy.EaseInOutQuint);

                // -- Only do character animation sequence if initial intro  --
                if (!isIntro) return;

                menuInfoContainer.schedule.Execute(() =>
                {
                    discordLabel.AnimCharacterSequence(
                        color1: originalColor,
                        color2: hoverLabelColor,
                        cascadeMs: 50,
                        durationMS: 100,
                        reverse: true);
                }).StartingIn(1500);

                menuInfoContainer.schedule.Execute(() =>
                {
                    docsLabel.AnimCharacterSequence(
                        color1: originalColor,
                        color2: hoverLabelColor,
                        cascadeMs: 50,
                        durationMS: 100,
                        reverse: true);
                }).StartingIn(1000);
            }

            // -- If set to false, reverse the animation and then hide the   --
            // -- container completely from view after the animation via     --
            // -- HideDisplay() Local Function callback above                --
            else
            {
                // -- Discord Label disappears first by fading opacity       --
                // -- from 1 to 0 over duration time
                menuInfoContainer.schedule.Execute(() =>
                {
                    discordLabel.AnimateOpacity(
                        startOpacity: visible,
                        endOpacity: hidden,
                        durationMs: outDurationMs,
                        easing: Easy.OutExpo, callback: UnregisterHoverCallback);
                }).StartingIn(35);

                // -- Followed by the docs label, which fades 50ms after     --
                // -- the discord label                                      --
                menuInfoContainer.schedule.Execute(() =>
                {
                    docsLabel.AnimateOpacity(
                        startOpacity: visible,
                        endOpacity: hidden,
                        durationMs: outDurationMs,
                        easing: Easy.OutExpo);
                }).StartingIn(0);

                menuInfoContainer.schedule.Execute(() =>
                {
                    VisualElementBaseAnimation.AnimateBackgroundColor(menuInfoContainer, ColorUtil.FromHex("#212121"),
                        originalInfoBgColor,
                        400);
                }).StartingIn(0);

                menuInfoContainer.AnimateWidth(
                    startWidth: fullWidth,
                    endWidth: smallWidth,
                    durationMs: durationMs,
                    callback: HideDisplay,
                    easing: Easy.EaseInOutQuint);
            }
        }

        #endregion

        // ------------------------------------------- @AnimImageAnimation
        // -- Perform the height change animation ------------------------
        private void ImageAnimation(VisualElement imageButton, Func<float, float> easing = null) // @formatter:on
        {
            // -- Image colors ------------------
            var originalImageColor = ColorUtil.FromHex("#000000");
            var targetImageColor = ColorUtil.FromHex("#607FAE");
            const int durationInMs = 1000;
            const int delayInMs = 1000;

            imageButton.AnimateImageTintColor(
                originalImageColor,
                targetImageColor,
                durationInMs,
                ReturnImageToOriginal,
                easing: easing);

            void ReturnImageToOriginal()
            {
                imageButton.schedule
                    .Execute(() => // @formatter:off
                    {
                        widthAnimateTo = imageButton.AnimateImageTintColor(
                            targetImageColor,
                            originalImageColor,
                            durationInMs,
                            easing: easing);
                    })  // @formatter:on
                    .StartingIn(delayInMs);
            }
        }

        // ---------------------------------------- @AnimFadeTextAnimation
        // -- Perform the text fade animation ----------------------------
        private void FadeTextAnimation(Label fadeTextAnimationLabel, Func<float, float> easing = null)
        {
            const int fadeInTime = 500;
            const float displayTime = 2000f;
            const int fadeOutTime = 500;

            const string newText = "then back to the original!";
            var originalTextColor = ColorUtil.FromHex("#BABABA");
            var animatedTextColor = ColorUtil.FromHex("#607FAE");

            fadeTextAnimationLabel.AnimFadeInSequence(
                newText,
                animatedTextColor,
                originalTextColor,
                fadeInTime,
                displayTime,
                fadeOutTime,
                easing);
        }

        // --------------------------------------- @AnimFadeColorAnimation
        // -- Perform the color fade animations --------------------------
        private void FadeColorAnimation(VisualElement fadeColorAnimationLabel, Func<float, float> easing = null)
        {
            // -- Background colors -------------
            var originalBackgroundColor = new StyleColor(StyleKeyword.Initial).value;
            var targetBackgroundColor = ColorUtil.FromHex("#607FAE");
            // -- Text colors -------------------
            var originalTextColor = ColorUtil.FromHex("#BABABA");
            var targetTextColor = ColorUtil.FromHex("#000000");
            // -- Duration values ---------------
            const int backgroundDurationMs = 500;
            const int textDurationMs = 500;
            const int delayDuration = 2500;

            fadeColorAnimationLabel.SetBackgroundColor(originalBackgroundColor);
            fadeColorAnimationLabel.AnimateBackgroundColor(
                originalBackgroundColor,
                targetBackgroundColor,
                backgroundDurationMs,
                FadeBackgroundToOriginal,
                easing: easing);

            fadeColorAnimationLabel.SetColor(originalBackgroundColor);
            fadeColorAnimationLabel.AnimateColor(
                originalTextColor,
                targetTextColor,
                textDurationMs,
                FadeTextToOriginal,
                easing: easing);

            // -- Callback executed after first animation completes but --
            // -- with a scheduled delay in order to hold the animated  --
            // -- values for a duration ----------------------------------
            void FadeBackgroundToOriginal()
            {
                fadeColorAnimationLabel.schedule
                    .Execute(() => fadeColorAnimationLabel.AnimateBackgroundColor(
                        targetBackgroundColor,
                        originalBackgroundColor,
                        backgroundDurationMs,
                        easing: easing))
                    .StartingIn(delayDuration);
            }

            void FadeTextToOriginal()
            {
                fadeColorAnimationLabel.schedule
                    .Execute(() => fadeColorAnimationLabel.AnimateColor(
                        targetTextColor,
                        originalTextColor,
                        textDurationMs,
                        easing: easing))
                    .StartingIn(delayDuration);
            }
        }

        // ------------------------------------- @AnimWidthChangeAnimation
        // -- Perform the width change animation -------------------------
        private void WidthAnimation(VisualElement widthChangeAnimationLabel, Func<float, float> easing = null)
        {
            var currentWidth = labelWidthDictionary[widthChangeAnimationLabel.name];
            const float desiredWidth = 250f;
            const int durationInMs = 1000;
            const int delayInMs = 1000;

            widthChangeAnimationLabel.SetBorderColor(ColorUtil.FromHex("#6B7DFF"));
            widthAnimateFrom = widthChangeAnimationLabel.AnimateWidth(
                currentWidth,
                desiredWidth,
                durationInMs,
                ReturnWidthToOriginal,
                easing);

            void ReturnWidthToOriginal()
            {
                widthChangeAnimationLabel.schedule
                    .Execute(() => // @formatter:off
                    {
                        void RemoveBorders(){ widthChangeAnimationLabel.SetBorderColor(); }
                        widthAnimateTo = widthChangeAnimationLabel.AnimateWidth(
                            desiredWidth,
                            currentWidth,
                            durationInMs,
                            RemoveBorders,
                             easing);
                     })  // @formatter:on
                    .StartingIn(delayInMs);
            }
        }

        // ------------------------------------ @AnimHeightChangeAnimation
        // -- Perform the height change animation ------------------------
        private void HeightAnimation(VisualElement heightChangeAnimationLabel, Func<float, float> easing = null)
        {
            // -- Animation setup values ------------------
            var parent = heightChangeAnimationLabel.parent;
            var buttonContainer = parent.Q("heightChangeAnimationButtonContainer");
            var button = parent.Q<Button>();

            var currentHeight = labelHeightDictionary[heightChangeAnimationLabel.name];
            const float desiredHeight = 50f;
            const int durationInMs = 1000;
            const long delayInMs = 1000;

            // -- Not necessarily needed, they just help --
            // -- make the demo cleaner while animating  --
            heightChangeAnimationLabel.SetBorderColor(ColorUtil.FromHex("#6B7DFF"));
            button.style.alignSelf = Align.FlexStart;
            buttonContainer.style.alignSelf = Align.FlexStart;
            buttonContainer.style.paddingTop = 1;

            // -- Begin animation -------------------------
            heightAnimateFrom = VisualElementBaseAnimation.AnimateHeight(heightChangeAnimationLabel, currentHeight,
                desiredHeight,
                durationInMs,
                ReturnHeightToOriginal);

            // -- Complete animation ----------------------
            void ReturnHeightToOriginal()
            {
                heightChangeAnimationLabel.schedule
                    .Execute(() => // @formatter:off
                    {
                        void RemoveBorders(){
                            heightChangeAnimationLabel.SetBorderColor();
                            button.style.alignSelf = Align.Center;
                            buttonContainer.style.alignSelf = Align.Center;
                            buttonContainer.style.paddingTop = 0;
                        }

                        heightAnimateTo = heightChangeAnimationLabel.AnimateHeight(
                            desiredHeight,
                            currentHeight,
                            durationInMs,
                            RemoveBorders,
                            easing: easing);
                    }) // @formatter:on
                    .StartingIn(delayInMs);
            }
        }

        #endregion

        // -- Manually repaint the editor window while an animation is running due to an editor bug. ------------------------------
        // -- Without this, animations can be jittery if the mouse is not moving over the window while the animation is playing ---
        private void Update()
        {
            if (widthAnimateFrom.isRunning || widthAnimateTo.isRunning || heightAnimateFrom.isRunning || heightAnimateTo.isRunning)
                Repaint();
        }
    }
}
