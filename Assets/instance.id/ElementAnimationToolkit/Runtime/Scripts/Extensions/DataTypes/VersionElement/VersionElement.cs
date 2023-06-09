using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace instance.id.EATK.Extensions
{
    /// <summary>
    /// Visual Element to display a semantic version number with MAJOR.MINOR.PATCH format consisting of a row of three text input fields separated by dots.
    /// </summary>
    [Serializable]
    public class VersionElement : VisualElement
    {
        private Label label;
        private TextField majorField;
        private TextField minorField;
        private TextField patchField;

        private Version _version = new Version();
        private Vector3 internalValue;

        public Version Value
        {
            set => _version = value;
            get => _version;
        }

        public VersionElement(string label = default)
        {
            // Initialize TextFields with labels
            majorField = new TextField("Major");
            minorField = new TextField("Minor");
            patchField = new TextField("Patch");

            this.label = new Label(label).ToUSS("versionElementLabel");
            this.label.style.alignSelf = new StyleEnum<Align>(Align.Center);

            new VisualElement().CreateRow(out var versionElementFieldContainer).ToUSS(nameof(versionElementFieldContainer), "containerRow").AddAll(new VisualElement[]
            {
                majorField, minorField, patchField
            });
            versionElementFieldContainer.style.paddingRight = 0;

            new VisualElement().CreateRow(out var versionElementContainer).ToUSS(nameof(versionElementContainer), "containerRow").AddAll(new VisualElement[]
            {
                this.label,
                versionElementFieldContainer
            });
            versionElementContainer.style.paddingRight = 0;
            versionElementContainer.style.justifyContent = new StyleEnum<Justify>(Justify.FlexEnd);

            // Add TextFields to the VisualElement
            Add(versionElementContainer);

            // Register value changed events
            majorField.RegisterValueChangedCallback(OnValueChanged);
            minorField.RegisterValueChangedCallback(OnValueChanged);
            patchField.RegisterValueChangedCallback(OnValueChanged);

            versionElementFieldContainer.Query<Label>().ForEach(x => x.style.minWidth = 35f);
            versionElementFieldContainer.Query<TextElement>().ForEach(x => x.style.minWidth = 25f);
        }
        
        private void OnValueChanged(ChangeEvent<string> evt)
        {
            int major, minor, patch;

            // Try parsing the input values. If parsing fails, default to zero.
            int.TryParse(majorField.value, out major);
            int.TryParse(minorField.value, out minor);
            int.TryParse(patchField.value, out patch);

            value = new Version(
                major, minor, patch
            );
        }

        public Version value
        {
            get
            {
                return Value = new Version(
                    int.Parse(majorField.value),
                    int.Parse(minorField.value),
                    int.Parse(patchField.value)
                );
            }
            set
            {
                if (value == null) return;
                var previousValue = Value;
                Value = value;
                majorField.value = Value.Major.ToString();
                minorField.value = Value.Minor.ToString();
                patchField.value = Value.Build.ToString();

                using (ChangeEvent<Version> pooled = ChangeEvent<Version>.GetPooled(previousValue, value))
                {
                    pooled.target = (IEventHandler)this;
                    this.SendEvent((EventBase)pooled);
                }
            }
        }
    }

}
