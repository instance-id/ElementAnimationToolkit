using System;
using System.Collections.Generic;
using System.Linq;
using instance.id.EATK.Examples;
using instance.id.EATK.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    [CustomEditor(typeof(ExampleComponent))]
    public class ExampleComponentEditor : Editor
    {
        private VisualElement root;
        private ExampleComponent exampleComponent;
        [UsedImplicitly] private Type editorType;
        [SerializeField] public StyleSheet styleSheet;

        private List<AnimatedLabel> labelData = new List<AnimatedLabel>();

        private AnimatedFoldout characterDetailsFoldout;
        private AnimatedFoldout characterVitalsFoldout;
        private AnimatedFoldout characterWeaponListFoldout;

        private void OnEnable()
        {
            exampleComponent = target as ExampleComponent;
            if (!(exampleComponent is null))
                exampleComponent.highlight += HighLightComponent;

            editorType = GetType();
            StylesheetSetup();
        }

        private StyleSheet StylesheetSetup()
        {
            if (styleSheet == null) styleSheet = editorType.GetStyleSheet();
            return styleSheet;
        }

        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();

            root = new VisualElement();
            root.styleSheets.Add(StylesheetSetup());
            root.ToUSS(nameof(root));

            // root.RegisterCallback<GeometryChangedEvent>(DeferredExecution);

            var exampleInspectorContainer = new VisualElement();
            exampleInspectorContainer.NameAsUSS(nameof(exampleInspectorContainer));

            var playerDetailsRowContainer = new VisualElement();
            playerDetailsRowContainer.NameAsUSS(nameof(playerDetailsRowContainer));

            var detailsHolder = new VisualElement();
            detailsHolder.NameAsUSS(nameof(detailsHolder));
            var playerDetailsLabel = new AnimatedLabel {text = "Player Details"};
            playerDetailsLabel.NameAsUSS(nameof(playerDetailsLabel));
            detailsHolder.Add(playerDetailsLabel);
            playerDetailsRowContainer.Add(detailsHolder);
            labelData.Add(playerDetailsLabel);

            playerDetailsLabel.HoverBorderPulse("#2F569C".FromHex(), "#D2A00C".FromHex(), default, true, new Vector2(1, 1));

            // -- Character Details -----------------------
            characterDetailsFoldout = new AnimatedFoldout {text = "Character Details", value = false};
            characterDetailsFoldout.NameAsUSS(nameof(characterDetailsFoldout));
            characterDetailsFoldout.expander.animationTime = 500;
            characterDetailsFoldout.contentContainer.usageHints = UsageHints.GroupTransform;

            var characterDetailsRow = new VisualElement();
            characterDetailsRow.NameAsUSS(nameof(characterDetailsRow));

            var nameContainer = new VisualElement();
            nameContainer.NameAsUSS(nameof(nameContainer));

            var nameHolder = new VisualElement();
            nameHolder.NameAsUSS(nameof(nameHolder));
            var nameLabel = new AnimatedLabel {text = "Player Name"};
            nameLabel.NameAsUSS(nameof(nameLabel));
            nameHolder.Add(nameLabel);
            labelData.Add(nameLabel);

            nameContainer.Add(nameHolder);

            var exampleCharacterName = new TextField {bindingPath = serializedObject.FindProperty("characterName").propertyPath};
            exampleCharacterName.NameAsUSS(nameof(exampleCharacterName));
            nameContainer.Add(exampleCharacterName);

            var locationContainer = new VisualElement();
            locationContainer.NameAsUSS(nameof(locationContainer));

            var locationHolder = new VisualElement();
            locationHolder.NameAsUSS(nameof(locationHolder));
            var locationLabel = new AnimatedLabel {text = "Player Location"};
            locationLabel.NameAsUSS(nameof(locationLabel));
            locationHolder.Add(locationLabel);
            labelData.Add(locationLabel);

            locationContainer.Add(locationHolder);

            var exampleLocation = new TextField {bindingPath = serializedObject.FindProperty("location").propertyPath};
            exampleLocation.NameAsUSS(nameof(exampleLocation));
            locationContainer.Add(exampleLocation);

            characterDetailsRow.Add(nameContainer);
            characterDetailsRow.Add(locationContainer);


            characterDetailsFoldout.Add(characterDetailsRow);

            // -- Character Vitals -------------------------
            characterVitalsFoldout = new AnimatedFoldout {text = "Character Vitals", value = false};
            characterVitalsFoldout.NameAsUSS(nameof(characterVitalsFoldout));
            characterVitalsFoldout.expander.animationTime = 500;
            characterVitalsFoldout.contentContainer.usageHints = UsageHints.GroupTransform;

            // -----------
            var healthContainer = new VisualElement();
            healthContainer.NameAsUSS(nameof(healthContainer));

            var healthLabel = new AnimatedLabel {text = "Max Health"};
            healthLabel.NameAsUSS(nameof(healthLabel));
            labelData.Add(healthLabel);
            healthContainer.Add(healthLabel);

            var exampleHealth = new IntegerField {bindingPath = serializedObject.FindProperty("health").propertyPath};
            exampleHealth.NameAsUSS(nameof(exampleHealth));
            healthContainer.Add(exampleHealth);

            // ----------
            var manaContainer = new VisualElement();
            manaContainer.NameAsUSS(nameof(manaContainer));

            var manaLabel = new AnimatedLabel {text = "Max Mana"};
            manaLabel.NameAsUSS(nameof(manaLabel));
            labelData.Add(manaLabel);
            manaContainer.Add(manaLabel);

            var exampleMana = new IntegerField {bindingPath = serializedObject.FindProperty("mana").propertyPath};
            exampleMana.NameAsUSS(nameof(exampleMana));
            manaContainer.Add(exampleMana);

            // -------------
            var livesContainer = new VisualElement();
            livesContainer.NameAsUSS(nameof(livesContainer));


            var livesLabel = new AnimatedLabel {text = "Resurrections"};
            livesLabel.NameAsUSS(nameof(livesLabel));
            labelData.Add(livesLabel);
            livesContainer.Add(livesLabel);

            var exampleLives = new IntegerField {bindingPath = serializedObject.FindProperty("lives").propertyPath};
            exampleLives.NameAsUSS(nameof(exampleLives));
            livesContainer.Add(exampleLives);

            characterVitalsFoldout.Add(healthContainer);
            characterVitalsFoldout.Add(manaContainer);
            characterVitalsFoldout.Add(livesContainer);

            // -- Character Weapons -----------------------
            characterWeaponListFoldout = new AnimatedFoldout {text = "Character Weapons", value = false};
            characterWeaponListFoldout.NameAsUSS(nameof(characterWeaponListFoldout));
            characterWeaponListFoldout.expander.animationTime = 500;
            characterWeaponListFoldout.contentContainer.usageHints = UsageHints.GroupTransform;

            var exampleWeaponList = new PropertyField {bindingPath = serializedObject.FindProperty("availableWeapons").propertyPath, label = "Available Weapons"};
            exampleWeaponList.NameAsUSS(nameof(exampleWeaponList));

            characterWeaponListFoldout.Add(exampleWeaponList);

            // -- Character Model -------------------------
            var examplePlayerModel = new ObjectField {bindingPath = serializedObject.FindProperty("playerModel").propertyPath, label = "Player Model"};
            examplePlayerModel.NameAsUSS(nameof(examplePlayerModel));

            // -- Character Info --------------------------
            var exampleIsNPC = new Toggle {bindingPath = serializedObject.FindProperty("isNPC").propertyPath, label = "Is NPC?"};
            exampleIsNPC.NameAsUSS(nameof(exampleIsNPC));

            var exampleSpawnLocation = new Vector3Field {bindingPath = serializedObject.FindProperty("spawnLocation").propertyPath, label = "Spawn Location"};
            exampleSpawnLocation.NameAsUSS(nameof(exampleSpawnLocation));


            exampleInspectorContainer.Add(playerDetailsRowContainer);
            exampleInspectorContainer.Add(characterDetailsFoldout);
            exampleInspectorContainer.Add(characterVitalsFoldout);
            exampleInspectorContainer.Add(characterWeaponListFoldout);
            exampleInspectorContainer.Add(examplePlayerModel);
            exampleInspectorContainer.Add(exampleIsNPC);
            exampleInspectorContainer.Add(exampleSpawnLocation);
            root.Add(exampleInspectorContainer);

            return root;
        }

        private void DeferredExecution(GeometryChangedEvent evt)
        {
            root.UnregisterCallback<GeometryChangedEvent>(DeferredExecution);
            // base.LoadingCompleted(root, target);

            SetupHighlighter();

            bool doExpand = false;

            // @formatter:off
            void First() { if (doExpand) root.schedule.Execute(() => characterDetailsFoldout.value = true).StartingIn(0); }
            void Second() { if (doExpand) root.schedule.Execute(() => characterVitalsFoldout.value = true).StartingIn(0); }
            void Third() { if (doExpand) root.schedule.Execute(() => characterWeaponListFoldout.value = true).StartingIn(0); /*DoLabels();*/ } // @formatter:on

            var charDetailLabel = characterDetailsFoldout.Query<Label>().First();
            var charVitalsLabel = characterVitalsFoldout.Query<Label>().First();
            var charWeaponLabel = characterWeaponListFoldout.Query<Label>().First();

            root.schedule.Execute(() => charWeaponLabel.AnimateColor(default, "#ff8000".FromHex(), 1000, Third)).StartingIn(300);
            root.schedule.Execute(() => charVitalsLabel.AnimateColor(default, "#ff8000".FromHex(), 1000, Second)).StartingIn(600);
            root.schedule.Execute(() => charDetailLabel.AnimateColor(default, "#ff8000".FromHex(), 1000, First)).StartingIn(900);

      
            // void DoLabels()
            // {
            //     var cascade = 100;
            //     labelData.ForEach(l =>
            //     {
            //         l.schedule.Execute(() =>
            //         {
            //             l.AnimCharacterSequence(
            //                 "#BABABA".FromHex(),
            //                 "#2F569C".FromHex(),
            //                 50,
            //                 150);
            //         }).StartingIn(cascade += 300);
            //     });
            // }
        }

        private VisualElement headerContainer;
        private VisualElement containerElement;
        private IVisualElementScheduledItem headerHighlighter;
        private IVisualElementScheduledItem containerHighlighter;

        private int color1Duration = 500;
        private  int color2Duration = 500;
        // private int durationBuffer = 20;

        private void SetupHighlighter()
        {
            inspectorElements = exampleComponent.inspectorElements;
            // -- Highlighter Setup --------------------------------------

            // -- EditorElement ------------------------------------------
            TypeCache.GetTypesDerivedFrom(typeof(VisualElement))
                .ToList()
                .FirstOrDefault(x => x.Name == "EditorElement")
                .GetFirstAncestorOfType<VisualElement>(root, out containerElement);

            containerElement.styleSheets.Add(editorType.GetStyleSheet("HighlighterStyle"));

            if (containerElement != null)
            {
                containerElement.AddToClassList("addHighlightBorder");
                inspectorElements.TryAddValue(containerElement);

                void Cleanup()
                {
                    headerContainer.SetBorderColor();
                }

                containerHighlighter = containerElement.AnimBorderPulse(
                    color1: ColorUtil.FromHex("#3A82E7"),
                    color2: ColorUtil.FromHex("#7F3B3A"),
                    original: default,
                    color1DurationMs: color1Duration,
                    color2DurationMs: color2Duration,
                    callback: Cleanup,
                    borderSelection: exampleComponent.containerBorders);
                containerHighlighter.Pause();
            }

            var headerElement = containerElement
                .Children()
                .FirstOrDefault(x => x.name.Contains("(Script)Header"));
            inspectorElements.TryAddValue(headerElement);

            // -- Header Setup -------------------------------------------
            if (headerElement != null)
            {
                new VisualElement()
                    .Create(out headerContainer)
                    .ToUSS(nameof(headerContainer))
                    .SetParent(containerElement, 0);

                headerElement.SetParent(headerContainer);
                headerContainer.AddToClassList("addHighlightBorder");

                void Cleanup()
                {
                    headerContainer.SetBorderColor();
                }

                headerHighlighter = headerContainer.AnimBorderPulse(
                    color1: ColorUtil.FromHex("#3A82E7"),
                    color2: ColorUtil.FromHex("#7F3B3A"),
                    original: default,
                    color1DurationMs: color1Duration,
                    color2DurationMs: color2Duration,
                    callback: Cleanup,
                    borderSelection: exampleComponent.headerBorders);
                headerHighlighter.Pause();
            }
        }

        private List<VisualElement> inspectorElements;

        private void HighLightComponent(string componentTarget)
        {
            switch (componentTarget)
            {
                case string a when a == "container":
                    HighlightContainer();
                    break;
                case string a when a == "header":
                    HighlightHeader();
                    break;
            }
        }

        private void ResetColors()
        {
        }

        private void HighlightContainer()
        {
            if (containerElement != null)
            {
                if (!containerHighlighter.isActive)
                {
                    containerHighlighter.ExecuteLater(0);
                }
                else
                {
                    containerHighlighter.Pause();
                    containerElement.SetBorderColorAction()
                        .ExecuteIn(containerElement, 200);
                }
            }
            else Debug.Log("ContainerElement not found");
        }

        private void HighlightHeader()
        {
            if (headerContainer != null)
            {
                if (!headerHighlighter.isActive)
                {
                    headerHighlighter.ExecuteLater(0);
                }
                else
                {
                    headerHighlighter.Pause();
                    headerContainer
                        .SetBorderColorAction()
                        .ExecuteIn(headerContainer, 200);
                }
            }
            else Debug.Log("HeaderElement not found");
        }
    }
}
