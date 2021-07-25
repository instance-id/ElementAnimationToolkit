// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/Extensions                    --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

namespace instance.id.EATK.Extensions
{
    /// <summary>
    /// Class containing data needed to perform the JumpToCode method from a VisualElement context menu.
    /// <see cref="EditorExtensions.JumpToCode"/>
    /// </summary>
    public class JumpTarget
    {
        private string menuTitle;
        private string locator;
        private bool externalLocator;
        private JumpType jumpType;

        /// <summary>
        /// The title of the context menu for the JumpToCode entry
        /// </summary>
        public string MenuTitle
        {
            get => menuTitle;
            set => menuTitle = value;
        }

        /// <summary>
        /// The line of code in which to jump to is found by locating the "locator" string. This is where in the code in which to jump
        /// </summary>
        public string Locator
        {
            get => locator;
            set => locator = value;
        }

        /// <summary>
        /// Deprecated: Used to mark if the locator is on a different line than the calling method
        /// </summary>
        public bool ExternalLocator
        {
            get => externalLocator;
            set => externalLocator = value;
        }

        /// <summary>
        /// Used to determine the proper AssetDatabase search string to locate the file in which to perform the JumpToCode() method.
        /// </summary>
        public JumpType JumpType
        {
            get => jumpType;
            set => jumpType = value;
        }

    }
}
