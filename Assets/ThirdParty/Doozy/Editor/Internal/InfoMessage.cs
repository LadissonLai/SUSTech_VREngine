// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace Doozy.Editor.Internal
{
    [Serializable]
    public class InfoMessage
    {
        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        public enum MessageType
        {
            Help,
            Info,
            Warning,
            Error,
            Ok,
            Custom
        }

        public MessageType Type;
        public string Title;
        public string Message;
        public AnimBool Show;
        public GUIStyle Icon;
        public ColorName IconColor;
        public ColorName BackgroundColor;

        public InfoMessage(MessageType type, string message, bool show = false, UnityAction repaintCallback = null)
        {
            Reset(show, repaintCallback);
            Type = type;
            Title = GetTitleText(type);
            Message = message;
            Icon = GetIconStyle(type);
            IconColor = GetIconColor(type);
            BackgroundColor = GetBackgroundColor(type);
        }

        public InfoMessage(MessageType type, string title, string message, bool show = false, UnityAction repaintCallback = null)
        {
            Reset(show, repaintCallback);
            Type = type;
            Title = title;
            Message = message;
            Icon = GetIconStyle(type);
            IconColor = GetIconColor(type);
            BackgroundColor = GetBackgroundColor(type);
        }

        public InfoMessage(GUIStyle icon, ColorName iconColor, ColorName backgroundColor, string title, string message, bool show = false, UnityAction repaintCallback = null)
        {
            Reset(show, repaintCallback);
            Type = MessageType.Custom;
            Title = title;
            Message = message;
            Icon = icon;
            IconColor = iconColor;
            BackgroundColor = backgroundColor;
        }

        public InfoMessage(GUIStyle icon, ColorName iconColor, ColorName backgroundColor, string message, bool show = false, UnityAction repaintCallback = null)
        {
            Reset(show, repaintCallback);
            Type = MessageType.Custom;
            Title = "";
            Message = message;
            Icon = icon;
            IconColor = iconColor;
            BackgroundColor = backgroundColor;
        }

        public void Reset(bool show, UnityAction repaintCallback = null)
        {
            Type = MessageType.Help;
            Title = string.Empty;
            Message = string.Empty;
            Show = repaintCallback != null ? new AnimBool(show, repaintCallback) : new AnimBool(show);
            Icon = GetIconStyle(Type);
            IconColor = GetIconColor(Type);
            BackgroundColor = GetBackgroundColor(Type);
        }

        private const int HEADER_HEIGHT = 24;
        private const int ICON_SIZE = 16;
        private const int ICON_PADDING = 6;
        private static readonly float TopVerticalSpace = DGUI.Properties.Space(2);
        private static readonly float BottomVerticalSpace = DGUI.Properties.Space(4);

        private static GUIStyle s_titleStyle, s_messageStyle, s_solutionStyle;

        private static GUIStyle TitleStyle
        {
            get
            {
                if (s_titleStyle != null) return s_titleStyle;
                s_titleStyle = new GUIStyle
                               {
                                   fontSize = 12,
                                   alignment = TextAnchor.MiddleLeft,
                                   clipping = TextClipping.Clip,
                                   wordWrap = false,
                                   richText = true,
                                   stretchWidth = true
                               };
                return s_titleStyle;
            }
        }

        private static GUIStyle MessageStyle
        {
            get
            {
                if (s_messageStyle != null) return s_messageStyle;
                s_messageStyle = new GUIStyle
                                 {
                                     fontSize = 10,
                                     padding = new RectOffset(ICON_PADDING, ICON_PADDING, ICON_PADDING, ICON_PADDING),
                                     alignment = TextAnchor.UpperLeft,
                                     wordWrap = true,
                                     richText = true,
                                     stretchWidth = true
                                 };
                return s_messageStyle;
            }
        }

        private static GUIStyle IconOk { get { return Styles.GetStyle(Styles.StyleName.IconFaCheck); } }
        private static GUIStyle IconHelp { get { return Styles.GetStyle(Styles.StyleName.IconFaQuestion); } }
        private static GUIStyle IconInfo { get { return Styles.GetStyle(Styles.StyleName.IconFaInfo); } }
        private static GUIStyle IconWarning { get { return Styles.GetStyle(Styles.StyleName.IconFaExclamationTriangle); } }
        private static GUIStyle IconError { get { return Styles.GetStyle(Styles.StyleName.IconFaTimesCircle); } }

        public static ColorName GetBackgroundColor(MessageType type)
        {
            switch (type)
            {
                case MessageType.Help:    return DGUI.Colors.DarkOrLightColorName;
                case MessageType.Info:    return ColorName.Blue;
                case MessageType.Warning: return ColorName.Amber;
                case MessageType.Error:   return ColorName.Red;
                case MessageType.Ok:      return ColorName.LightGreen;
                default:                  return DGUI.Colors.DarkOrLightColorName;
            }
        }

        public static ColorName GetIconColor(MessageType type)
        {
            switch (type)
            {
                case MessageType.Help:    return DGUI.Colors.LightOrDarkColorName;
                case MessageType.Info:    return ColorName.Blue;
                case MessageType.Warning: return ColorName.Amber;
                case MessageType.Error:   return ColorName.Red;
                case MessageType.Ok:      return ColorName.LightGreen;
                default:                  return DGUI.Colors.LightOrDarkColorName;
            }
        }

        private static Color HeaderBackgroundColor(InfoMessage infoMessage)
        {
            return DGUI.Utility.IsProSkin
                       ? DGUI.Colors.GetDColor(infoMessage.BackgroundColor).Dark
                       : DGUI.Colors.GetDColor(infoMessage.BackgroundColor).Light;
        }

        private static Color HeaderIconAndTextColor(InfoMessage infoMessage)
        {
            return DGUI.Utility.IsProSkin
                       ? DGUI.Colors.GetDColor(infoMessage.IconColor).Light
                       : DGUI.Colors.GetDColor(infoMessage.IconColor).Dark;
        }

        private static readonly Color DarkBodyColor = new Color().ColorFrom256(26, 26, 26);
        private static readonly Color LightBodyColor = new Color().ColorFrom256(242, 242, 242);
        private static readonly Color BodyColor = DGUI.Utility.IsProSkin ? DarkBodyColor : LightBodyColor;

        private static Color MessageTextColor(InfoMessage infoMessage)
        {
            ColorName colorName = infoMessage.Type == MessageType.Help ? ColorName.Gray : infoMessage.IconColor;
            return DGUI.Utility.IsProSkin
                       ? DGUI.Colors.GetDColor(colorName).Light
                       : DGUI.Colors.GetDColor(colorName).Dark;
        }

        private static GUIStyle GetIconStyle(MessageType type)
        {
            switch (type)
            {
                case MessageType.Ok:      return IconOk;
                case MessageType.Help:    return IconHelp;
                case MessageType.Info:    return IconInfo;
                case MessageType.Warning: return IconWarning;
                case MessageType.Error:   return IconError;
                default:                  return UILabels.Error;
            }
        }

        private static string GetTitleText(MessageType type)
        {
            switch (type)
            {
                case MessageType.Help:    return UILabels.Help;
                case MessageType.Info:    return UILabels.Info;
                case MessageType.Warning: return UILabels.Warning;
                case MessageType.Error:   return UILabels.Error;
                case MessageType.Ok:      return UILabels.Ok;
                default:                  return UILabels.Custom;
            }
        }

        public void Draw(bool show, float width)
        {
            Show.target = show;
            Draw(this, width);
        }

        public static void Draw(InfoMessage infoMessage, float width)
        {
            Color color = GUI.color;
            if (DGUI.FadeOut.Begin(infoMessage.Show.faded, false))
            {
                Color headerIconAndTextColor = HeaderIconAndTextColor(infoMessage).WithAlpha(GUI.color.a);
                var titleContent = new GUIContent(string.IsNullOrEmpty(infoMessage.Title) ? GetTitleText(infoMessage.Type) : infoMessage.Title);
                var messageContent = new GUIContent(infoMessage.Message);
                float headerHeight = HEADER_HEIGHT * infoMessage.Show.faded;
                float iconSize = ICON_SIZE * infoMessage.Show.faded;
                float iconPadding = ICON_PADDING * infoMessage.Show.faded;

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    GUILayout.Space(TopVerticalSpace * infoMessage.Show.faded);
                    GUI.color = HeaderBackgroundColor(infoMessage).WithAlpha(GUI.color.a);
                    GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.InfoMessageHeader), GUILayout.ExpandWidth(true), GUILayout.Height(headerHeight));
                    GUI.color = color;
                    GUILayout.Space(-headerHeight);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(headerHeight));
                    {
                        GUILayout.Space(iconPadding);
                        DGUI.Icon.Draw(infoMessage.Icon, iconSize, headerHeight, headerIconAndTextColor);
                        GUILayout.Space(iconPadding);
                        DGUI.Label.Draw(titleContent, DGUI.Colors.ColorTextOfGUIStyle(TitleStyle, headerIconAndTextColor), headerHeight);
                    }
                    GUILayout.EndHorizontal();

                    float messageContentHeight = MessageStyle.CalcHeight(messageContent, width);

                    GUI.color = BodyColor.WithAlpha(GUI.color.a);
                    GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.InfoMessageMessage), GUILayout.ExpandWidth(true), GUILayout.Height(messageContentHeight));
                    GUILayout.Space(-messageContentHeight);
                    GUI.color = color.WithAlpha(GUI.color.a);
                    GUILayout.Label(messageContent, DGUI.Colors.ColorTextOfGUIStyle(MessageStyle, MessageTextColor(infoMessage).WithAlpha(GUI.color.a)), GUILayout.ExpandWidth(true));
                    GUILayout.Space(BottomVerticalSpace * infoMessage.Show.faded);
                }
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(infoMessage.Show.faded, false, color.a);
            GUI.color = color;
        }

        public void DrawMessageOnly(bool show)
        {
            Show.target = show;
            DrawMessageOnly(this);
        }

        public static void DrawMessageOnly(InfoMessage infoMessage)
        {
            Color color = GUI.color;
            if (DGUI.FadeOut.Begin(infoMessage.Show.faded, false))
            {
                Color headerIconAndTextColor = HeaderIconAndTextColor(infoMessage).WithAlpha(GUI.color.a);
                var messageContent = new GUIContent(infoMessage.Message);
                float headerHeight = HEADER_HEIGHT * infoMessage.Show.faded;
                float iconSize = ICON_SIZE * infoMessage.Show.faded;
                float iconPadding = ICON_PADDING * infoMessage.Show.faded;

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    GUILayout.Space(TopVerticalSpace * infoMessage.Show.faded);
                    GUI.color = HeaderBackgroundColor(infoMessage).WithAlpha(GUI.color.a);
                    GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.InfoMessageRounded), GUILayout.ExpandWidth(true), GUILayout.Height(headerHeight));
                    GUI.color = color;
                    GUILayout.Space(-headerHeight);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(headerHeight));
                    {
                        GUILayout.Space(iconPadding);
                        DGUI.Icon.Draw(infoMessage.Icon, iconSize, headerHeight, headerIconAndTextColor);
                        GUILayout.Space(iconPadding);
                        DGUI.Label.Draw(messageContent, DGUI.Colors.ColorTextOfGUIStyle(TitleStyle, headerIconAndTextColor), headerHeight);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(infoMessage.Show.faded, false, color.a);
            GUI.color = color;
        }
    }
}