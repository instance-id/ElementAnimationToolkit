using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace instance.id.EATK.Extensions
{
    // Hopefully these features will eventually be in the default TextField.
    public class MaskedInputField : TextField
    {
        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public const string UssClassName = "unity-masked-text-field";

        /// <summary>
        /// USS class name of placeholder elements of this type.
        /// </summary>
        public const string PlaceholderUssClassName = UssClassName + "__placeholder";

        StyleSheet k_StylePath;
        private Type editorType;

        readonly Label m_PlaceholderLabel;
        private VisualElement maskedInput;
        public TextField maskedText;
        private Font monoFont;

        public int prefixNum = 2;
        public int suffixNum = 9;
        public bool usePrivacyMask = false;

        /// <summary>
        /// Notify external subscribers that value of text property changed.
        /// </summary>
        public Action<string> OnValueChangedHandler;

        public MaskedInputField()
        {
            if (monoFont == null)
                monoFont = VisualElementUtils.GetFont("SourceCodePro-Medium");
            if (monoFont == null) Debug.Log($"Font not found");

            AddToClassList(UssClassName);
            editorType = GetType();
            styleSheets.Add(StylesheetSetup());
            style.unityFont = monoFont;

            var textInputBase = this.Q("unity-text-input");
            textInputBase.NameAsUSS(nameof(textInputBase));
            textInputBase.style.display = DisplayStyle.Flex;
            textInputBase.style.color = new StyleColor(StyleKeyword.Initial);
            textInputBase.SetBorderWidth(2);

            maskedText = new TextField
            {
                style = {display = DisplayStyle.Flex, position = Position.Absolute, unityFont = monoFont},
                value = "",
                pickingMode = PickingMode.Ignore,
            };
            maskedInput = maskedText.Q("unity-text-input");
            maskedInput.pickingMode = PickingMode.Ignore;
            maskedInput.NameAsUSS(nameof(maskedInput));


            maskedText.NameAsUSS(nameof(maskedText));
            maskedText.AddToClassList(UssClassName);
            Add(maskedText);

            this.RegisterCallback<MouseUpEvent>(evt =>
            {
                this.SelectRangeDelayed(maskedText.text.Length, maskedText.text.Length);
                evt.StopPropagation();
            });

            // Add and configure placeholder
            m_PlaceholderLabel = new Label {pickingMode = PickingMode.Ignore};
            m_PlaceholderLabel.AddToClassList(PlaceholderUssClassName);
            maskedText.Add(m_PlaceholderLabel);

            RegisterCallback<FocusInEvent>(e => HidePlaceholder());
            RegisterCallback<FocusOutEvent>(e =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    ShowPlaceholder();
                }
            });

            this.RegisterCallback<GeometryChangedEvent>(DeferredExecution);
            this.RegisterValueChangedCallback(e => OnValueChangedHandler?.Invoke(e.newValue));
        }

        private void DeferredExecution(GeometryChangedEvent evt)
        {
            this.RegisterCallback<GeometryChangedEvent>(DeferredExecution);

            this.style.color = new StyleColor(StyleKeyword.Initial);
            maskedText.style.width = this.resolvedStyle.width;
            maskedText.style.height = this.resolvedStyle.height;
            maskedText.style.right = -4;
            maskedText.style.top = -3;
            maskedText.style.backgroundColor = new StyleColor(StyleKeyword.Initial);
            maskedText.SetBorderWidth(2);

            maskedInput.BringToFront();
        }

        private StyleSheet StylesheetSetup()
        {
            if (k_StylePath == null)
                k_StylePath = editorType.GetStyleSheet("MaskedInputField");
            if (k_StylePath is null) Debug.LogError($"{editorType.Name} Stylesheet not found");
            return k_StylePath;
        }

        void UpdatePlaceholderVisibility()
        {
            if (string.IsNullOrEmpty(value))
            {
                if (focusController?.focusedElement != this)
                {
                    ShowPlaceholder();
                }
            }
            else
            {
                HidePlaceholder();
            }
        }

        void HidePlaceholder()
        {
            m_PlaceholderLabel?.AddToClassList("hidden");
        }

        void ShowPlaceholder()
        {
            m_PlaceholderLabel?.RemoveFromClassList("hidden");
        }

        public override string value
        {
            get => base.value;
            set
            {
                base.value = value;
                if (value != null)
                {
                    var privacyMask = ApplyPrivacyMask(value);
                    maskedText.SetValueWithoutNotify(privacyMask.Item2);
                }

                UpdatePlaceholderVisibility();
            }
        }

        public string Placeholder
        {
            get => m_PlaceholderLabel.text;
            set => m_PlaceholderLabel.text = value;
        }

        /// <summary>
        /// Creates a privacy string in which characters at either the beginning or end are shown as asterisks ( * )
        /// Ex. 0123456 -> **2345*
        /// </summary>
        /// <param name="exposedString">The string in which to apply the privacy mask</param>
        /// <returns>Returns a Tuple value in which Item1 is the original string, Item2 is the privacy masked string</returns>
        public (string, string) ApplyPrivacyMask(string exposedString)
        {
            if (exposedString.Length <= 0) return ("", "");
            List<string> tmpString = new List<string>();
            for (int i = 0; i < exposedString.Length; i++)
            {
                tmpString.Add(i < prefixNum || i > suffixNum ? exposedString[i].ToString() : "*");
            }

            return (exposedString, tmpString.Aggregate((i, j) => i + j));
        }

        public override void SetValueWithoutNotify(string newValue)
        {
            base.SetValueWithoutNotify(newValue);
            UpdatePlaceholderVisibility();
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<MaskedInputField, UxmlTraits>
        {
        }

        public new class UxmlTraits : TextField.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Hint = new UxmlStringAttributeDescription {name = "placeholder"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var field = (MaskedInputField) ve;
                field.Placeholder = m_Hint.GetValueFromBag(bag, cc);
            }
        }
    }
}
