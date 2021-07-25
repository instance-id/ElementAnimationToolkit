// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/Extensions                    --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEngine.UIElements;

#pragma warning disable 649

namespace instance.id.EATK.Extensions
{
    public struct VisualElementStyleStore
    {
        private string sourceName;
        private StyleLength width;
        private StyleLength height;
        private StyleLength maxWidth;
        private StyleLength maxHeight;
        private StyleLength minWidth;
        private StyleLength minHeight;
        private StyleLength flexBasis;
        private StyleFloat flexShrink;
        private StyleFloat flexGrow;
        private StyleInt overflow;
        private StyleInt unityOverflowClipBox;
        private StyleLength left;
        private StyleLength top;
        private StyleLength right;
        private StyleLength bottom;
        private StyleLength marginLeft;
        private StyleLength marginTop;
        private StyleLength marginRight;
        private StyleLength marginBottom;
        private StyleLength paddingLeft;
        private StyleLength paddingTop;
        private StyleLength paddingRight;
        private StyleLength paddingBottom;
        private StyleInt position;
        private StyleInt alignSelf;
        private StyleInt unityTextAlign;
        private StyleInt unityFontStyleAndWeight;
        private StyleFont unityFont;
        private StyleLength fontSize;
        private StyleInt whiteSpace;
        private StyleColor color;
        private StyleInt flexDirection;
        private StyleColor backgroundColor;
        private StyleBackground backgroundImage;
        private StyleInt unityBackgroundScaleMode;
        private StyleColor unityBackgroundImageTintColor;
        private StyleInt alignItems;
        private StyleInt alignContent;
        private StyleInt justifyContent;
        private StyleInt flexWrap;
        private StyleColor borderLeftColor;
        private StyleColor borderTopColor;
        private StyleColor borderRightColor;
        private StyleColor borderBottomColor;
        private StyleFloat borderLeftWidth;
        private StyleFloat borderTopWidth;
        private StyleFloat borderRightWidth;
        private StyleFloat borderBottomWidth;
        private StyleLength borderTopLeftRadius;
        private StyleLength borderTopRightRadius;
        private StyleLength borderBottomRightRadius;
        private StyleLength borderBottomLeftRadius;
        private StyleInt unitySliceLeft;
        private StyleInt unitySliceTop;
        private StyleInt unitySliceRight;
        private StyleInt unitySliceBottom;
        private StyleFloat opacity;
        private StyleCursor cursor;
        private StyleInt visibility;
        private StyleInt display;

        public string SourceName
        {
            get => sourceName;
            set => sourceName = value;
        }

        public StyleLength Width
        {
            get => width;
            set => width = value;
        }

        public StyleLength Height
        {
            get => height;
            set => height = value;
        }

        public StyleLength MaxWidth
        {
            get => maxWidth;
            set => maxWidth = value;
        }

        public StyleLength MaxHeight
        {
            get => maxHeight;
            set => maxHeight = value;
        }

        public StyleLength MinWidth
        {
            get => minWidth;
            set => minWidth = value;
        }

        public StyleLength MinHeight
        {
            get => minHeight;
            set => minHeight = value;
        }

        public StyleLength FlexBasis
        {
            get => flexBasis;
            set => flexBasis = value;
        }

        public StyleFloat FlexShrink
        {
            get => flexShrink;
            set => flexShrink = value;
        }

        public StyleFloat FlexGrow
        {
            get => flexGrow;
            set => flexGrow = value;
        }

        public StyleInt Overflow
        {
            get => overflow;
            set => overflow = value;
        }

        public StyleInt UnityOverflowClipBox
        {
            get => unityOverflowClipBox;
            set => unityOverflowClipBox = value;
        }

        public StyleLength Left
        {
            get => left;
            set => left = value;
        }

        public StyleLength Top
        {
            get => top;
            set => top = value;
        }

        public StyleLength Right
        {
            get => right;
            set => right = value;
        }

        public StyleLength Bottom
        {
            get => bottom;
            set => bottom = value;
        }

        public StyleLength MarginLeft
        {
            get => marginLeft;
            set => marginLeft = value;
        }

        public StyleLength MarginTop
        {
            get => marginTop;
            set => marginTop = value;
        }

        public StyleLength MarginRight
        {
            get => marginRight;
            set => marginRight = value;
        }

        public StyleLength MarginBottom
        {
            get => marginBottom;
            set => marginBottom = value;
        }

        public StyleLength PaddingLeft
        {
            get => paddingLeft;
            set => paddingLeft = value;
        }

        public StyleLength PaddingTop
        {
            get => paddingTop;
            set => paddingTop = value;
        }

        public StyleLength PaddingRight
        {
            get => paddingRight;
            set => paddingRight = value;
        }

        public StyleLength PaddingBottom
        {
            get => paddingBottom;
            set => paddingBottom = value;
        }

        public StyleInt Position
        {
            get => position;
            set => position = value;
        }

        public StyleInt AlignSelf
        {
            get => alignSelf;
            set => alignSelf = value;
        }

        public StyleInt UnityTextAlign
        {
            get => unityTextAlign;
            set => unityTextAlign = value;
        }

        public StyleInt UnityFontStyleAndWeight
        {
            get => unityFontStyleAndWeight;
            set => unityFontStyleAndWeight = value;
        }

        public StyleFont UnityFont
        {
            get => unityFont;
            set => unityFont = value;
        }

        public StyleLength FontSize
        {
            get => fontSize;
            set => fontSize = value;
        }

        public StyleInt WhiteSpace
        {
            get => whiteSpace;
            set => whiteSpace = value;
        }

        public StyleColor Color
        {
            get => color;
            set => color = value;
        }

        public StyleInt FlexDirection
        {
            get => flexDirection;
            set => flexDirection = value;
        }

        public StyleColor BackgroundColor
        {
            get => backgroundColor;
            set => backgroundColor = value;
        }

        public StyleBackground BackgroundImage
        {
            get => backgroundImage;
            set => backgroundImage = value;
        }

        public StyleInt UnityBackgroundScaleMode
        {
            get => unityBackgroundScaleMode;
            set => unityBackgroundScaleMode = value;
        }

        public StyleColor UnityBackgroundImageTintColor
        {
            get => unityBackgroundImageTintColor;
            set => unityBackgroundImageTintColor = value;
        }

        public StyleInt AlignItems
        {
            get => alignItems;
            set => alignItems = value;
        }

        public StyleInt AlignContent
        {
            get => alignContent;
            set => alignContent = value;
        }

        public StyleInt JustifyContent
        {
            get => justifyContent;
            set => justifyContent = value;
        }

        public StyleInt FlexWrap
        {
            get => flexWrap;
            set => flexWrap = value;
        }

        public StyleColor BorderLeftColor
        {
            get => borderLeftColor;
            set => borderLeftColor = value;
        }

        public StyleColor BorderTopColor
        {
            get => borderTopColor;
            set => borderTopColor = value;
        }

        public StyleColor BorderRightColor
        {
            get => borderRightColor;
            set => borderRightColor = value;
        }

        public StyleColor BorderBottomColor
        {
            get => borderBottomColor;
            set => borderBottomColor = value;
        }

        public StyleFloat BorderLeftWidth
        {
            get => borderLeftWidth;
            set => borderLeftWidth = value;
        }

        public StyleFloat BorderTopWidth
        {
            get => borderTopWidth;
            set => borderTopWidth = value;
        }

        public StyleFloat BorderRightWidth
        {
            get => borderRightWidth;
            set => borderRightWidth = value;
        }

        public StyleFloat BorderBottomWidth
        {
            get => borderBottomWidth;
            set => borderBottomWidth = value;
        }

        public StyleLength BorderTopLeftRadius
        {
            get => borderTopLeftRadius;
            set => borderTopLeftRadius = value;
        }

        public StyleLength BorderTopRightRadius
        {
            get => borderTopRightRadius;
            set => borderTopRightRadius = value;
        }

        public StyleLength BorderBottomRightRadius
        {
            get => borderBottomRightRadius;
            set => borderBottomRightRadius = value;
        }

        public StyleLength BorderBottomLeftRadius
        {
            get => borderBottomLeftRadius;
            set => borderBottomLeftRadius = value;
        }

        public StyleInt UnitySliceLeft
        {
            get => unitySliceLeft;
            set => unitySliceLeft = value;
        }

        public StyleInt UnitySliceTop
        {
            get => unitySliceTop;
            set => unitySliceTop = value;
        }

        public StyleInt UnitySliceRight
        {
            get => unitySliceRight;
            set => unitySliceRight = value;
        }

        public StyleInt UnitySliceBottom
        {
            get => unitySliceBottom;
            set => unitySliceBottom = value;
        }

        public StyleFloat Opacity
        {
            get => opacity;
            set => opacity = value;
        }

        public StyleCursor Cursor
        {
            get => cursor;
            set => cursor = value;
        }

        public StyleInt Visibility
        {
            get => visibility;
            set => visibility = value;
        }

        public StyleInt Display
        {
            get => display;
            set => display = value;
        }
    }
}
