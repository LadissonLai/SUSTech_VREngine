// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Colors
        {
            public static void SetDisabledGUIColorAlpha() { GUI.color = GUI.color.WithAlpha(Utility.IsProSkin ? 0.4f : 0.6f); }
            public static void SetNormalGUIColorAlpha() { GUI.color = GUI.color.WithAlpha(1f); }

            public static ColorName GetBackgroundColorName(bool enabled, ColorName componentColorName) { return enabled ? componentColorName : DisabledBackgroundColorName; }
            public static ColorName GetTextColorName(bool enabled, ColorName componentColorName) { return enabled ? componentColorName : DisabledTextColorName; }
            public static ColorName GetIconColorName(bool enabled, ColorName componentColorName) { return enabled ? componentColorName : DisabledTextColorName; }

            private static readonly Color DarkBaseColor = new Color().ColorFrom256(230, 230, 230);
            private static readonly Color LightBaseColor = new Color().ColorFrom256(26, 26, 26);

            public static readonly DColor kDarkBaseDColor = new DColor("DarkBase", DarkBaseColor);
            public static readonly DColor kLightBaseDColor = new DColor("LightBase", LightBaseColor);

            public static readonly DColor kBlack = new DColor("Black", Color.black, Color.black, Color.black);
            public static readonly DColor kWhite = new DColor("White", Color.white, Color.white, Color.white);


            public static DColor GetDColor(ColorName colorName)
            {
                switch (colorName)
                {
                    case ColorName.Black:      return kBlack;
                    case ColorName.White:      return kWhite;
                    case ColorName.Red:        return EditorColors.Instance.Red;
                    case ColorName.Pink:       return EditorColors.Instance.Pink;
                    case ColorName.Purple:     return EditorColors.Instance.Purple;
                    case ColorName.DeepPurple: return EditorColors.Instance.DeepPurple;
                    case ColorName.Indigo:     return EditorColors.Instance.Indigo;
                    case ColorName.Blue:       return EditorColors.Instance.Blue;
                    case ColorName.LightBlue:  return EditorColors.Instance.LightBlue;
                    case ColorName.Cyan:       return EditorColors.Instance.Cyan;
                    case ColorName.Teal:       return EditorColors.Instance.Teal;
                    case ColorName.Green:      return EditorColors.Instance.Green;
                    case ColorName.LightGreen: return EditorColors.Instance.LightGreen;
                    case ColorName.Lime:       return EditorColors.Instance.Lime;
                    case ColorName.Yellow:     return EditorColors.Instance.Yellow;
                    case ColorName.Amber:      return EditorColors.Instance.Amber;
                    case ColorName.Orange:     return EditorColors.Instance.Orange;
                    case ColorName.DeepOrange: return EditorColors.Instance.DeepOrange;
                    case ColorName.UnityLight: return EditorColors.Instance.UnityLight;
                    case ColorName.Gray:       return EditorColors.Instance.Gray;
                    case ColorName.UnityDark:  return EditorColors.Instance.UnityDark;
                    default:                   throw new ArgumentOutOfRangeException("colorName", colorName, null);
                }
            }

            public static Color BackgroundColor(ColorName colorName, bool ignoreAlpha = false) { return BackgroundColor(GetDColor(colorName), ignoreAlpha); }
            public static Color BackgroundColor(DColor dColor, bool ignoreAlpha = false) { return (Utility.IsProSkin ? dColor.Normal : dColor.Dark).WithAlpha(ignoreAlpha ? 1f : GUI.color.a); }

            public static Color TextColor(ColorName colorName, bool ignoreAlpha = false) { return TextColor(GetDColor(colorName), ignoreAlpha); }
            public static Color TextColor(DColor dColor, bool ignoreAlpha = false) { return (Utility.IsProSkin ? dColor.Light : dColor.Dark).WithAlpha(ignoreAlpha ? 1f : GUI.color.a); }

            public static Color IconColor(ColorName colorName, bool ignoreAlpha = false) { return IconColor(GetDColor(colorName), ignoreAlpha); }
            public static Color IconColor(DColor dColor, bool ignoreAlpha = false) { return (Utility.IsProSkin ? dColor.Normal : dColor.Dark).WithAlpha(ignoreAlpha ? 1f : GUI.color.a); }

            public static Color BarColor(ColorName colorName, bool expanded, bool ignoreAlpha = false) { return BarColor(GetDColor(colorName), expanded, ignoreAlpha); }

            public static Color BarColor(DColor dColor, bool expanded, bool ignoreAlpha = false)
            {
                DColor targetDColor = expanded ? dColor : DisabledBackgroundDColor;
                return (Utility.IsProSkin ? targetDColor.Dark : targetDColor.Light).WithAlpha(ignoreAlpha ? 1f : GUI.color.a);
            }

            /// <summary>IsProSkin ? ColorName.UnityLight : ColorName.UnityDark </summary>
            public static ColorName LightOrDarkColorName { get { return Utility.IsProSkin ? ColorName.UnityLight : ColorName.UnityDark; } }

            /// <summary>IsProSkin ? ColorName.UnityLight : ColorName.Gray </summary>
            public static ColorName LightOrGrayColorName { get { return Utility.IsProSkin ? ColorName.UnityLight : ColorName.Gray; } }

            /// <summary>IsProSkin ? ColorName.UnityDark : ColorName.White </summary>
            public static ColorName DarkOrWhiteColorName { get { return Utility.IsProSkin ? ColorName.UnityDark : ColorName.White; } }
            
            /// <summary>IsProSkin ? ColorName.UnityDark : ColorName.UnityLight </summary>
            public static ColorName DarkOrLightColorName { get { return Utility.IsProSkin ? ColorName.UnityDark : ColorName.UnityLight; } }

            /// <summary>IsProSkin ? ColorName.UnityDark : ColorName.Gray </summary>
            public static ColorName DarkOrGrayColorName { get { return Utility.IsProSkin ? ColorName.UnityDark : ColorName.Gray; } }
            
            public static Color PropertyColor(ColorName colorName, bool ignoreAlpha = false) { return PropertyColor(GetDColor(colorName), ignoreAlpha); }
            public static Color PropertyColor(DColor dColor, bool ignoreAlpha = false) { return dColor.Light.WithAlpha(ignoreAlpha ? 1f : GUI.color.a); }

            public static ColorName DisabledBackgroundColorName { get { return Utility.IsProSkin ? ColorName.Gray : ColorName.White; } }
            public static DColor DisabledBackgroundDColor { get { return GetDColor(DisabledBackgroundColorName); } }

            public static ColorName DisabledTextColorName { get { return ColorName.Gray; } }
            public static DColor DisabledTextDColor { get { return GetDColor(DisabledTextColorName); } }

            public static ColorName DisabledIconColorName { get { return ColorName.Gray; } }
            public static DColor DisabledIconDColor { get { return GetDColor(DisabledIconColorName); } }

            public static Color BaseColor(Skin skin) { return skin == Skin.Dark ? DarkBaseColor : LightBaseColor; }
            public static DColor BaseDColor(Skin skin) { return skin == Skin.Dark ? kDarkBaseDColor : kLightBaseDColor; }

            public static Color BaseColor(bool invertColors = false)
            {
                return
                    Utility.IsProSkin
                        ? invertColors ? LightBaseColor : DarkBaseColor
                        : invertColors
                            ? DarkBaseColor
                            : LightBaseColor;
            }

            public static DColor BaseDColor(bool invertColors = false)
            {
                return
                    Utility.IsProSkin
                        ? invertColors ? kLightBaseDColor : kDarkBaseDColor
                        : invertColors
                            ? kDarkBaseDColor
                            : kLightBaseDColor;
            }

            public static Color ButtonBaseColor(StyleState styleState, Skin skin)
            {
                switch (styleState)
                {
                    case StyleState.Normal:  return (skin == Skin.Dark ? DarkBaseColor : LightBaseColor).WithAlpha(0.8f);
                    case StyleState.Hover:   return skin == Skin.Dark ? DarkBaseColor : LightBaseColor;
                    case StyleState.Active:  return (skin == Skin.Dark ? DarkBaseColor : LightBaseColor).WithAlpha(0.6f);
                    case StyleState.Focused: return skin == Skin.Dark ? DarkBaseColor : LightBaseColor;
                    default:                 throw new ArgumentOutOfRangeException("styleState", styleState, null);
                }
            }

            public static ColorName AboutColorName { get { return Utility.IsProSkin ? ColorName.UnityLight : ColorName.Gray; } }
            public static ColorName ActionColorName { get { return ColorName.Teal; } }
            public static ColorName AnimationsColorName { get { return ColorName.Teal; } }
            public static ColorName BackButtonColorName { get { return ColorName.Orange; } }
            public static ColorName CurveModifierColorName { get { return ColorName.Gray; } }
            public static ColorName DebugColorName { get { return ColorName.Red; } }
            public static ColorName EnterNodeColorName { get { return StartNodeColorName; } }
            public static ColorName ExitNodeColorName { get { return ColorName.Orange; } }
            public static ColorName FadeColorName { get { return ColorName.Purple; } }
            public static ColorName GameEventListenerColorName { get { return ColorName.Pink; } }
            public static ColorName GameEventManagerColorName { get { return ColorName.Orange; } }
            public static ColorName GeneralColorName { get { return Utility.IsProSkin ? ColorName.UnityLight : ColorName.Gray; } }
            public static ColorName GestureListenerColorName { get { return ColorName.Pink; } }
            public static ColorName GraphControllerColorName { get { return ColorName.Orange; } }
            public static ColorName HelpColorName { get { return Utility.IsProSkin ? ColorName.UnityLight : ColorName.Gray; } }
            public static ColorName KeysColorName { get { return ColorName.DeepOrange; } }
            public static ColorName KeyToActionColorName { get { return ColorName.Pink; } }
            public static ColorName KeyToGameEventColorName { get { return ColorName.Pink; } }
            public static ColorName MoveColorName { get { return ColorName.Green; } }
            public static ColorName NodyColorName { get { return ColorName.LightBlue; } }
            public static ColorName NodyInputColorName { get { return ColorName.Amber; } }
            public static ColorName NodyOutputColorName { get { return ColorName.LightBlue; } }
            public static ColorName OrientationDetectorColorName { get { return ColorName.Orange; } }
            public static ColorName PlaymakerEventDispatcherColorName { get { return ColorName.Cyan; } }
            public static ColorName ProgressorColorName { get { return ColorName.Lime; } }
            public static ColorName ProgressorGroupColorName { get { return ColorName.Lime; } }
            public static ColorName RadialLayoutColorName { get { return ColorName.Teal; } }
            public static ColorName RotateColorName { get { return ColorName.Orange; } }
            public static ColorName ScaleColorName { get { return ColorName.Red; } }
            public static ColorName SceneDirectorColorName { get { return ColorName.Orange; } }
            public static ColorName SceneLoaderColorName { get { return ColorName.Orange; } }
            public static ColorName SettingsColorName { get { return Utility.IsProSkin ? ColorName.UnityLight : ColorName.Gray; } } 
            public static ColorName SoundGroupDataColorName { get { return ColorName.DeepOrange; } }
            public static ColorName SoundyColorName { get { return ColorName.Orange; } }
            public static ColorName SoundyControllerColorName { get { return ColorName.Orange; } }
            public static ColorName SoundyManagerColorName { get { return ColorName.Orange; } }
            public static ColorName SoundyPoolerColorName { get { return ColorName.Orange; } }
            public static ColorName StartNodeColorName { get { return ColorName.Lime; } }
            public static ColorName SubGraphNodeColorName { get { return ColorName.Cyan; } }
            public static ColorName TouchDetectorColorName { get { return ColorName.Orange; } }
            public static ColorName ThemesColorName { get { return ColorName.Teal; } }
            public static ColorName TouchyColorName { get { return ColorName.Green; } }
            public static ColorName UIButtonColorName { get { return ColorName.Blue; } }
            public static ColorName UIButtonListenerColorName { get { return ColorName.Pink; } }
            public static ColorName UICanvasColorName { get { return ColorName.Blue; } }
            public static ColorName UIDrawerColorName { get { return ColorName.Blue; } }
            public static ColorName UIDrawerListenerColorName { get { return ColorName.Pink; } }
            public static ColorName UIGraphColorName { get { return ColorName.Gray; } }
            public static ColorName UIImageColorName { get { return ColorName.Blue; } }
            public static ColorName UIPopupColorName { get { return ColorName.Blue; } }
            public static ColorName UIPopupManagerColorName { get { return ColorName.Orange; } }
            public static ColorName UISubGraphColorName { get { return ColorName.Cyan; } }
            public static ColorName UITemplateColorName { get { return ColorName.Teal; } }
            public static ColorName UIToggleColorName { get { return ColorName.Blue; } }
            public static ColorName UIToggleOffColorName { get { return ColorName.DeepOrange; } }
            public static ColorName UIToggleOnColorName { get { return ColorName.Green; } }
            public static ColorName UIViewColorName { get { return ColorName.Blue; } }
            public static ColorName UIViewListenerColorName { get { return ColorName.Pink; } }


            public static GUIStyle ColorTextOfGUIStyle(GUIStyle style, ColorName textColorName, bool ignoreAlpha = false) { return ColorTextOfGUIStyle(style, TextColor(textColorName, ignoreAlpha)); }
            public static GUIStyle ColorTextOfGUIStyle(GUIStyle style, DColor textDColor, bool ignoreAlpha = false) { return ColorTextOfGUIStyle(style, TextColor(textDColor, ignoreAlpha)); }
            public static GUIStyle ColorTextOfGUIStyle(GUIStyle style, Color textColor) { return new GUIStyle(style) {normal = {textColor = textColor}}; }
        }
    }
}