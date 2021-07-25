// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

#if UNITY_EDITOR


using System;
using instance.id.EATK.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable UnusedMember.Global
#pragma warning disable 108,114

namespace instance.id.EATK
{
    /// <summary>
    /// Provides an animated Element displaying text.
    /// </summary>
    public sealed class AnimatedLabel : VisualElement
    {
        public Color startColor;
        public Color animatedColor;
        private Label animatedLabel;
        StyleSheet k_StylePath;
        private Type editorType;

        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public static readonly string ussClassName = "animated-label-container";

        public static readonly string ussClassNameExtended = "animated-label";

        /// <summary>
        /// Constructs a label.
        /// </summary>
        private string m_text;

        public string text
        {
            get => m_text;
            set
            {
                m_text = value;
                animatedLabel.text = m_text;
            }
        }

        /// <summary>
        /// Constructs a label.
        /// </summary>
        /// <param name="text">The text to be displayed.</param>
        public AnimatedLabel()
        {
            editorType = GetType();
            styleSheets.Add(StylesheetSetup());
            new Label {text = ""}.Create(out animatedLabel).ToUSS(nameof(animatedLabel));
            animatedLabel.AddToClassList(ussClassNameExtended);
            Add(animatedLabel);

            AddToClassList(ussClassName);
            AddToClassList(this.name);

            this.text = text;
        }

        internal Label GetLabel()
        {
            return animatedLabel;
        }

        // public void AddToClassList()
        // {
        //
        // }

        public void Play()
        {
        }

        private StyleSheet StylesheetSetup()
        {
            if (k_StylePath == null)
                k_StylePath = editorType.GetStyleSheet(editorType.Name);
            if (k_StylePath is null) Debug.LogError($"{editorType.Name} Stylesheet not found");
            return k_StylePath;
        }

        public void ClearClassList()
        {
            animatedLabel.ClearClassList();
        }

        public void AddToClassList(string className)
        {
            animatedLabel.AddToClassList(className);
        }

        public void RemoveFromClassList(string className)
        {
            animatedLabel.RemoveFromClassList(className);
        }

        /// <summary>
        ///   <para>Toggles between adding and removing the given class name from the class list.</para>
        /// </summary>
        /// <param name="className">The class name to add or remove from the class list.</param>
        public void ToggleInClassList(string className)
        {
            animatedLabel.ToggleInClassList(className);
        }

        /// <summary>
        ///   <para>Enables or disables the class with the given name.</para>
        /// </summary>
        /// <param name="className">The name of the class to enable or disable.</param>
        /// <param name="enable">A boolean flag that adds or removes the class name from the class list. If true, EnableInClassList adds the class name to the class list. If false, EnableInClassList removes the class name from the class list.</param>
        public void EnableInClassList(string className, bool enable)
        {
            animatedLabel.EnableInClassList(className, enable);
        }

        public bool ClassListContains(string cls)
        {
            return animatedLabel.ClassListContains(cls);
        }

        // -- Register callbacks ------------------------------------------
        /// <summary>
        /// Register a callback on the animated label
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="TEventType"></typeparam>
        public void RegisterCallback<TEventType>(EventCallback<TEventType> action) where TEventType : EventBase<TEventType>, new()
        {
            animatedLabel.RegisterCallback<TEventType>(action);
        }

        public void RegisterCallback<TEventType, TUserArgsType>(
            EventCallback<TEventType, TUserArgsType> callback,
            TUserArgsType userArgs,
            TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType : EventBase<TEventType>, new()
        {
            animatedLabel.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, useTrickleDown);
        }

        public void UnregisterCallback<TEventType>(
            EventCallback<TEventType> callback,
            TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType : EventBase<TEventType>, new()
        {
            animatedLabel.UnregisterCallback<TEventType>(callback, useTrickleDown);
        }

        public void UnregisterCallback<TEventType, TUserArgsType>(
            EventCallback<TEventType, TUserArgsType> callback,
            TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType : EventBase<TEventType>, new()
        {
            animatedLabel.UnregisterCallback<TEventType, TUserArgsType>(callback, useTrickleDown);
        }

        public static bool RegisterValueChangedCallback<T>(
              INotifyValueChanged<T> control,
            EventCallback<ChangeEvent<T>> callback)
        {
            if (!(control is CallbackEventHandler callbackEventHandler))
                return false;
            callbackEventHandler.RegisterCallback<ChangeEvent<T>>(callback);
            return true;
        }

        public static bool UnregisterValueChangedCallback<T>(
               INotifyValueChanged<T> control,
            EventCallback<ChangeEvent<T>> callback)
        {
            if (!(control is CallbackEventHandler callbackEventHandler))
                return false;
            callbackEventHandler.UnregisterCallback<ChangeEvent<T>>(callback);
            return true;
        }

        // ---------------------------------------------------------------------------- UXML
        // -- UXML -------------------------------------------------------------------------
        /// <summary>
        /// Instantiates a <see cref="AnimatedLabel"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<AnimatedLabel, UxmlTraits>
        {
        }

        /// <summary>
        /// Defines <see cref="UxmlTraits"/> for the <see cref="AnimatedLabel"/>.
        /// </summary>
        public new class UxmlTraits : TextElement.UxmlTraits
        {
        }
    }
}
#endif
