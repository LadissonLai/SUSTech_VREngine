// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Sizes
        {
            public const int HEADER_LABEL_FONT_SIZE = 20;
            public const float DIVIDER_HEIGHT = 2f;
            public const float SWITCH_WIDTH = 24f;
            public const float SWITCH_HEIGHT = 16f;
            public const float CHECKBOX_WIDTH = 16f;
            public const float CHECKBOX_HEIGHT = 16f;
            public const float ICON_BUTTON_SIZE = 14f;

            public static int LabelFontSize(Size size)
            {
                switch (size)
                {
                    case Size.S:  return 9;
                    case Size.M:  return 11;
                    case Size.L:  return 13;
                    case Size.XL: return 15;
                    default:      throw new ArgumentOutOfRangeException("size", size, null);
                }
            }

            public static int ButtonFontSize(Size size)
            {
                switch (size)
                {
                    case Size.S:  return 10;
                    case Size.M:  return 12;
                    case Size.L:  return 14;
                    case Size.XL: return 16;
                    default:      throw new ArgumentOutOfRangeException("size", size, null);
                }
            }

            public static float BarHeight(Size size)
            {
                switch (size)
                {
                    case Size.S:  return 18;
                    case Size.M:  return 20;
                    case Size.L:  return 24;
                    case Size.XL: return 32;
                    default:      throw new ArgumentOutOfRangeException("size", size, null);
                }
            }
        }
    }
}