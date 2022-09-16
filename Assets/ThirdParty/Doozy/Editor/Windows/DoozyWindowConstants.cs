// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Settings;
using Doozy.Engine.Settings;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private const string NEW_SOUND_DATABASE = "NewSoundDatabase";
        private const string RENAME_SOUND_DATABASE = "RenameSoundDatabase";
        private const string NEW_CATEGORY = "NewCategory";
        private const string SEARCH = "Search";
        private const string RENAME = "Rename";

        private const ColorName RENAME_COLOR_NAME = ColorName.Orange;
        
        private const Size TOP_BUTTON_TEXT_SIZE = Size.M;
        private const TextAlign TOP_BUTTON_TEXT_ALIGN = TextAlign.Left;
        private static float TopButtonHeight { get { return DGUI.Sizes.BarHeight(Size.L); } }

        private const Size WINDOW_TAB_BUTTON_TEXT_SIZE = Size.L;
        private const TextAlign WINDOW_TAB_BUTTON_TEXT_ALIGN = TextAlign.Left;
        private static float WindowTabButtonHeight { get { return DGUI.Sizes.BarHeight(Size.XL); } }
        
        private const Size BAR_SIZE = Size.L;
        private static float BarHeight { get { return DGUI.Sizes.BarHeight(BAR_SIZE); } }
        
        private float NewCategoryNameTextFieldWidth { get { return Mathf.Max(FullViewWidth * 0.6f, DGUI.Properties.DefaultFieldWidth * 4); } }
        private static float SearchRowHeight { get { return DGUI.Properties.SingleLineHeight; } }
        private static float NormalRowHeight { get { return DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2); } }
        
       
        
        private static DoozySettings Settings { get { return DoozySettings.Instance; } }
        private static DoozyWindowSettings WindowSettings { get { return DoozyWindowSettings.Instance; } }
    }
}