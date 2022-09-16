// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Button
        {
            public const Size DEFAULT_SIZE = Size.M;
            public const State DEFAULT_STATE = State.Disabled;
            public const TextAlign DEFAULT_TEXT_ALIGN = TextAlign.Center;

            public static GUIStyle ButtonStyle(bool enabled) { return ButtonStyle(enabled ? State.Enabled : State.Disabled); }

            public static GUIStyle ButtonStyle(State state = DEFAULT_STATE)
            {
                switch (state)
                {
                    case State.Disabled: return Styles.GetStyle(Styles.StyleName.ButtonDisabled);
                    case State.Enabled:  return Styles.GetStyle(Styles.StyleName.ButtonEnabled);
                    default:             throw new ArgumentOutOfRangeException("state", state, null);
                }
            }

            public static GUIStyle ButtonStyle(TabPosition position, bool enabled = false) { return ButtonStyle(position, enabled ? State.Enabled : State.Disabled); }

            public static GUIStyle ButtonStyle(TabPosition position, State state = DEFAULT_STATE)
            {
                switch (position)
                {
                    case TabPosition.Left:
                        switch (state)
                        {
                            case State.Disabled: return Styles.GetStyle(Styles.StyleName.ButtonLeftDisabled);
                            case State.Enabled:  return Styles.GetStyle(Styles.StyleName.ButtonLeftEnabled);
                            default:             throw new ArgumentOutOfRangeException("state", state, null);
                        }
                    case TabPosition.Middle:
                        switch (state)
                        {
                            case State.Disabled: return Styles.GetStyle(Styles.StyleName.ButtonMiddleDisabled);
                            case State.Enabled:  return Styles.GetStyle(Styles.StyleName.ButtonMiddleEnabled);
                            default:             throw new ArgumentOutOfRangeException("state", state, null);
                        }
                    case TabPosition.Right:
                        switch (state)
                        {
                            case State.Disabled: return Styles.GetStyle(Styles.StyleName.ButtonRightDisabled);
                            case State.Enabled:  return Styles.GetStyle(Styles.StyleName.ButtonRightEnabled);
                            default:             throw new ArgumentOutOfRangeException("state", state, null);
                        }
                    default: throw new ArgumentOutOfRangeException("position", position, null);
                }
            }

            public static GUIStyle LabelStyle(Size size = DEFAULT_SIZE, TextAlign textAlign = DEFAULT_TEXT_ALIGN)
            {
                switch (size)
                {
                    case Size.S:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.ButtonLabelSLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.ButtonLabelSCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.ButtonLabelSRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }
                    case Size.M:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.ButtonLabelMLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.ButtonLabelMCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.ButtonLabelMRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }
                    case Size.L:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.ButtonLabelLLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.ButtonLabelLCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.ButtonLabelLRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }
                    case Size.XL:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.ButtonLabelXLLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.ButtonLabelXLCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.ButtonLabelXLRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }
                    default: throw new ArgumentOutOfRangeException("size", size, null);
                }
            }

            public static bool Draw(Rect rect, string label, ColorName buttonColorName, bool enabled) { return Draw(rect, new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled); }
            public static bool Draw(Rect rect, string label, DColor buttonDColor, bool enabled) { return Draw(rect, new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled); }
            public static bool Draw(Rect rect, string label, ColorName buttonColorName, ColorName textColorName, bool enabled) { return Draw(rect, new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled); }
            public static bool Draw(Rect rect, string label, DColor buttonDColor, DColor textDColor, bool enabled) { return Draw(rect, new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, ColorName buttonColorName, bool enabled) { return Draw(rect, new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, DColor buttonDColor, bool enabled) { return Draw(rect, new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, ColorName buttonColorName, ColorName textColorName, bool enabled) { return Draw(rect, new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, DColor buttonDColor, DColor textDColor, bool enabled) { return Draw(rect, new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, TextAlign textAlign, ColorName buttonColorName, bool enabled) { return Draw(rect, new GUIContent(label), textSize, textAlign, buttonColorName, buttonColorName, enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, TextAlign textAlign, DColor buttonDColor, bool enabled) { return Draw(rect, new GUIContent(label), textSize, textAlign, buttonDColor, buttonDColor, enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, TextAlign textAlign, ColorName buttonColorName, ColorName textColorName, bool enabled) { return Draw(rect, new GUIContent(label), textSize, textAlign, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled); }
            public static bool Draw(Rect rect, string label, Size textSize, TextAlign textAlign, DColor buttonDColor, DColor textDColor, bool enabled) { return Draw(rect, new GUIContent(label), textSize, textAlign, buttonDColor, textDColor, enabled); }
            public static bool Draw(Rect rect, GUIContent content, ColorName buttonColorName, bool enabled) { return Draw(rect, content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled); }
            public static bool Draw(Rect rect, GUIContent content, DColor buttonDColor, bool enabled) { return Draw(rect, content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled); }
            public static bool Draw(Rect rect, GUIContent content, ColorName buttonColorName, ColorName textColorName, bool enabled) { return Draw(rect, content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled); }
            public static bool Draw(Rect rect, GUIContent content, DColor buttonDColor, DColor textDColor, bool enabled) { return Draw(rect, content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, ColorName buttonColorName, bool enabled) { return Draw(rect, content, textSize, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, DColor buttonDColor, bool enabled) { return Draw(rect, content, textSize, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, ColorName buttonColorName, ColorName textColorName, bool enabled) { return Draw(rect, content, textSize, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, DColor buttonDColor, DColor textDColor, bool enabled) { return Draw(rect, content, textSize, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, TextAlign textAlign, ColorName buttonColorName, bool enabled) { return Draw(rect, content, textSize, textAlign, Colors.GetDColor(buttonColorName), Colors.GetDColor(buttonColorName), enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, TextAlign textAlign, DColor buttonDColor, bool enabled) { return Draw(rect, content, textSize, textAlign, buttonDColor, buttonDColor, enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, TextAlign textAlign, ColorName buttonColorName, ColorName textColorName, bool enabled) { return Draw(rect, content, textSize, textAlign, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled); }
            public static bool Draw(Rect rect, GUIContent content, Size textSize, TextAlign textAlign, DColor buttonDColor, DColor textDColor, bool enabled) { return Draw(rect, content, ButtonStyle(enabled), LabelStyle(textSize, textAlign), buttonDColor, textDColor); }

            public static bool Draw(Rect rect, GUIContent content, GUIStyle buttonStyle, GUIStyle labelStyle, DColor buttonDColor, DColor textDColor)
            {
                Color color = GUI.color;
                GUI.color = Colors.BarColor(buttonDColor, true);
                bool result = GUI.Button(rect, GUIContent.none, buttonStyle);
                GUI.color = color;
                GUI.Label(rect, content, new GUIStyle(labelStyle) {normal = {textColor = Colors.TextColor(textDColor)}});
                if (result)
                {
                    Properties.ResetKeyboardFocus();
                    Event.current.Use();
                }
                return result;
            }

            public static bool Draw(string label, ColorName buttonColorName, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled, height, width); }
            public static bool Draw(string label, DColor buttonDColor, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled, height, width); }
            public static bool Draw(string label, ColorName buttonColorName, ColorName textColorName, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled, height, width); }
            public static bool Draw(string label, DColor buttonDColor, DColor textDColor, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled, height, width); }
            public static bool Draw(string label, Size textSize, ColorName buttonColorName, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled, height, width); }
            public static bool Draw(string label, Size textSize, DColor buttonDColor, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled, height, width); }
            public static bool Draw(string label, Size textSize, ColorName buttonColorName, ColorName textColorName, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled, height, width); }
            public static bool Draw(string label, Size textSize, DColor buttonDColor, DColor textDColor, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled, height, width); }
            public static bool Draw(string label, Size textSize, TextAlign textAlign, ColorName buttonColorName, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, textAlign, buttonColorName, buttonColorName, enabled, height, width); }
            public static bool Draw(string label, Size textSize, TextAlign textAlign, DColor buttonDColor, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, textAlign, buttonDColor, buttonDColor, enabled, height, width); }
            public static bool Draw(string label, Size textSize, TextAlign textAlign, ColorName buttonColorName, ColorName textColorName, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, textAlign, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled, height, width); }
            public static bool Draw(string label, Size textSize, TextAlign textAlign, DColor buttonDColor, DColor textDColor, bool enabled, float height, float width = -1) { return Draw(new GUIContent(label), textSize, textAlign, buttonDColor, textDColor, enabled, height, width); }
            public static bool Draw(GUIContent content, ColorName buttonColorName, bool enabled, float height, float width = -1) { return Draw(content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled, height, width); }
            public static bool Draw(GUIContent content, DColor buttonDColor, bool enabled, float height, float width = -1) { return Draw(content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled, height, width); }
            public static bool Draw(GUIContent content, ColorName buttonColorName, ColorName textColorName, bool enabled, float height, float width = -1) { return Draw(content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled, height, width); }
            public static bool Draw(GUIContent content, DColor buttonDColor, DColor textDColor, bool enabled, float height, float width = -1) { return Draw(content, DEFAULT_SIZE, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, ColorName buttonColorName, bool enabled, float height, float width = -1) { return Draw(content, textSize, DEFAULT_TEXT_ALIGN, buttonColorName, buttonColorName, enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, DColor buttonDColor, bool enabled, float height, float width = -1) { return Draw(content, textSize, DEFAULT_TEXT_ALIGN, buttonDColor, buttonDColor, enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, ColorName buttonColorName, ColorName textColorName, bool enabled, float height, float width = -1) { return Draw(content, textSize, DEFAULT_TEXT_ALIGN, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, DColor buttonDColor, DColor textDColor, bool enabled, float height, float width = -1) { return Draw(content, textSize, DEFAULT_TEXT_ALIGN, buttonDColor, textDColor, enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, TextAlign textAlign, ColorName buttonColorName, bool enabled, float height, float width = -1) { return Draw(content, textSize, textAlign, buttonColorName, buttonColorName, enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, TextAlign textAlign, DColor buttonDColor, bool enabled, float height, float width = -1) { return Draw(content, textSize, textAlign, buttonDColor, buttonDColor, enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, TextAlign textAlign, ColorName buttonColorName, ColorName textColorName, bool enabled, float height, float width = -1) { return Draw(content, textSize, textAlign, Colors.GetDColor(buttonColorName), Colors.GetDColor(textColorName), enabled, height, width); }
            public static bool Draw(GUIContent content, Size textSize, TextAlign textAlign, DColor buttonDColor, DColor textDColor, bool enabled, float height, float width = -1) { return Draw(content, ButtonStyle(enabled), LabelStyle(textSize, textAlign), buttonDColor, textDColor, height, width); }

            public static bool Draw(GUIContent content, GUIStyle buttonStyle, GUIStyle labelStyle, DColor buttonDColor, DColor textDColor, float height, float width = -1)
            {
                var options = new List<GUILayoutOption> {GUILayout.Height(height)};
                if (width > 0) options.Add(GUILayout.Width(width));
                bool result;
                Color color = GUI.color;
                GUILayout.BeginVertical(options.ToArray());
                {
                    GUI.color = Colors.BarColor(buttonDColor, true);
                    result = GUILayout.Button(GUIContent.none, buttonStyle, options.ToArray());
                    GUILayout.Space(-height);
                    GUI.color = color;
                    GUILayout.Label(content, new GUIStyle(labelStyle) {normal = {textColor = Colors.TextColor(textDColor)}}, options.ToArray());
                }
                GUILayout.EndVertical();

                if (result)
                {
                    Properties.ResetKeyboardFocus();
                    Event.current.Use();
                }
                return result;
            }


            public static class Dynamic
            {
                public static bool DrawIconButton(GUIStyle iconStyle, string text, Size textSize, TextAlign textAlign, ColorName backgroundColorName, ColorName textColorName, float height, bool expandWidth = true)
                {
                    return DrawIconButton(iconStyle, new GUIContent(text), textSize, textAlign, Colors.GetDColor(backgroundColorName), Colors.GetDColor(textColorName), height, expandWidth);
                }

                public static bool DrawIconButton(GUIStyle iconStyle, string text, Size textSize, TextAlign textAlign, DColor buttonDColor, DColor textDColor, float height, bool expandWidth = true) { return DrawIconButton(iconStyle, new GUIContent(text), textSize, textAlign, buttonDColor, buttonDColor, height, expandWidth); }

                public static bool DrawIconButton(GUIStyle iconStyle, GUIContent content, Size textSize, TextAlign textAlign, ColorName backgroundColorName, ColorName textColorName, float height, bool expandWidth = true)
                {
                    return DrawIconButton(iconStyle, content, textSize, textAlign, Colors.GetDColor(backgroundColorName), Colors.GetDColor(textColorName), height, expandWidth);
                }

                public static bool DrawIconButton(GUIStyle iconStyle, GUIContent content, Size textSize, TextAlign textAlign, DColor buttonDColor, DColor textDColor, float height, bool expandWidth = true)
                {
                    float iconSize = height * 0.6f;
                    float iconPadding = height * 0.1f;
                    Vector2 contentSize = LabelStyle(textSize, textAlign).CalcSize(content);
                    var options = new List<GUILayoutOption> {GUILayout.Height(height)};
                    if (expandWidth)
                    {
                        options.Add(GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        float width = 0;
                        width += iconPadding * 3;
                        width += iconSize;
                        width += iconPadding;
                        width += contentSize.x;
                        width += iconPadding;
                        options.Add(GUILayout.Width(width));
                    }

                    bool result;
                    Color color = GUI.color;
                    GUILayout.BeginVertical(options.ToArray());
                    {
                        GUI.color = Colors.BarColor(buttonDColor, true);
                        result = GUILayout.Button(GUIContent.none, ButtonStyle(DEFAULT_STATE), options.ToArray());
                        GUILayout.Space(-height);
                        GUI.color = color;
                        GUILayout.BeginHorizontal(options.ToArray());
                        {
                            GUILayout.Space(iconPadding * 3);
                            Icon.Draw(iconStyle, iconSize, height, Colors.TextColor(textDColor));
                            GUILayout.Space(iconPadding);
                            GUILayout.Label(content, new GUIStyle(LabelStyle(textSize, textAlign)) {normal = {textColor = Colors.TextColor(textDColor)}}, GUILayout.Width(contentSize.x), GUILayout.Height(height));
                            GUILayout.Space(iconPadding);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    if (result)
                    {
                        Properties.ResetKeyboardFocus();
                        Event.current.Use();
                    }
                    return result;
                }

                public static bool DrawIconButton(GUIStyle iconStyle, ColorName buttonColorName, ColorName iconColorName, float height) { return DrawIconButton(iconStyle, Colors.GetDColor(buttonColorName), Colors.GetDColor(iconColorName), height); }
                
                public static bool DrawIconButton(GUIStyle iconStyle, DColor buttonDColor, DColor iconDColor, float height)
                {
                    float iconSize = height * 0.7f;
                    float iconPadding = height * 0.1f;
                    var options = new List<GUILayoutOption> {GUILayout.Height(height)};
                    float width = 0;
                    width += iconPadding * 2;
                    width += iconSize;
                    width += iconPadding * 2;
                    options.Add(GUILayout.Width(width));

                    bool result;
                    Color color = GUI.color;
                    GUILayout.BeginVertical(options.ToArray());
                    {
                        GUI.color = Colors.BarColor(buttonDColor, true);
                        result = GUILayout.Button(GUIContent.none, ButtonStyle(DEFAULT_STATE), options.ToArray());
                        GUILayout.Space(-height);
                        GUI.color = color;
                        GUILayout.BeginHorizontal(options.ToArray());
                        {
                            GUILayout.Space(iconPadding * 2);
                            Icon.Draw(iconStyle, iconSize, height, Colors.IconColor(iconDColor));
                            GUILayout.Space(iconPadding * 2);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    if (result)
                    {
                        Properties.ResetKeyboardFocus();
                        Event.current.Use();
                    }
                    return result;
                }
            }


            public static bool DrawToolbarButton(string text, GUIStyle normalStyle, GUIStyle selectedStyle, bool selected, ColorName colorName, params GUILayoutOption[] options) { return DrawToolbarButton(text, normalStyle, selectedStyle, selected, colorName, colorName, options); }

            public static bool DrawToolbarButton(string text, GUIStyle normalStyle, GUIStyle selectedStyle, bool selected, ColorName colorName, ColorName textColorName, params GUILayoutOption[] options)
            {
                Color initialColor = GUI.color;
                GUI.color = Colors.GetDColor(colorName).Normal;
                bool result = GUILayout.Button(new GUIContent(text), Colors.ColorTextOfGUIStyle(selected ? selectedStyle : normalStyle, textColorName), options);
                GUI.color = initialColor;

                return result;
            }

            public static class IconButton
            {
                private const float ICON_BUTTON_WIDTH = Sizes.ICON_BUTTON_SIZE;
                private const float ICON_BUTTON_HEIGHT = Sizes.ICON_BUTTON_SIZE;

                public static Vector2 Size { get { return new Vector2(ICON_BUTTON_WIDTH, ICON_BUTTON_HEIGHT); } }
                public static float Width { get { return ICON_BUTTON_WIDTH; } }
                public static float Height { get { return ICON_BUTTON_HEIGHT; } }


                public static bool Draw(Rect rect, GUIStyle style, ColorName iconColorName) { return Draw(rect, style, Colors.IconColor(iconColorName)); }

                public static bool Draw(Rect rect, GUIStyle style, Color iconColor)
                {
                    Color color = GUI.color;
                    GUI.color = iconColor;
                    bool result = GUI.Button(rect, GUIContent.none, style);
                    GUI.color = color;
                    if (result)
                    {
                        Properties.ResetKeyboardFocus();
                        Event.current.Use();
                    }
                    return result;
                }

                public static bool Draw(GUIStyle style, ColorName iconColorName, float rowHeight = -1) { return Draw(style, Colors.IconColor(iconColorName), rowHeight); }
                public static bool Draw(GUIStyle style, ColorName iconColorName, float rowHeight, float iconSize) { return Draw(style, Colors.IconColor(iconColorName), rowHeight, iconSize); }

                public static bool Draw(GUIStyle style, Color iconColor, float rowHeight = -1)
                {
                    Color color = GUI.color;
                    if (rowHeight > 0)
                    {
                        GUILayout.BeginVertical(GUILayout.Width(Width), GUILayout.Height(rowHeight));
                        GUILayout.Space((rowHeight - Height) / 2);
                    }

                    GUI.color = iconColor;
                    bool buttonClicked = GUILayout.Button(GUIContent.none, style);
                    GUI.color = color;
                    if (rowHeight > 0) GUILayout.EndVertical();

                    if (!buttonClicked) return false;
                    Properties.ResetKeyboardFocus();
                    Event.current.Use();
                    return true;
                }
                
                public static bool Draw(GUIStyle style, Color iconColor, float rowHeight, float iconSize)
                {
                    Color color = GUI.color;
                    if (rowHeight > 0)
                    {
                        GUILayout.BeginVertical(GUILayout.Width(iconSize), GUILayout.Height(rowHeight));
                        GUILayout.Space((rowHeight - iconSize) / 2);
                    }

                    GUI.color = iconColor;
                    var resizedStyle = new GUIStyle(style)
                                       {
                                           fixedWidth = iconSize,
                                           fixedHeight = iconSize
                                       };
                    bool buttonClicked = GUILayout.Button(GUIContent.none, resizedStyle);
                    GUI.color = color;
                    if (rowHeight > 0) GUILayout.EndVertical();

                    if (!buttonClicked) return false;
                    Properties.ResetKeyboardFocus();
                    Event.current.Use();
                    return true;
                }


                public static bool Cancel(float rowHeight = -1, ColorName colorName = ColorName.Red) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonCancel), colorName, rowHeight); }
                public static bool Close(float rowHeight = -1, ColorName colorName = ColorName.Red) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonClose), colorName, rowHeight); }
                public static bool Minus(float rowHeight = -1, ColorName colorName = ColorName.Red) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonMinus), colorName, rowHeight); }
                public static bool Ok(float rowHeight = -1, ColorName colorName = ColorName.Green) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonOk), colorName, rowHeight); }
                public static bool Play(float rowHeight = -1, ColorName colorName = ColorName.White) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonPlaySound), colorName, rowHeight); }
                public static bool Plus(float rowHeight = -1, ColorName colorName = ColorName.Green) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonPlus), colorName, rowHeight); }
                public static bool Reset(float rowHeight = -1, ColorName colorName = ColorName.White) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonReset), colorName, rowHeight); }
                public static bool Sort(float rowHeight = -1, ColorName colorName = ColorName.White) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonSort), colorName, rowHeight); }
                public static bool Stop(float rowHeight = -1, ColorName colorName = ColorName.White) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonStop), colorName, rowHeight); }
                public static bool Link(float rowHeight = -1, ColorName colorName = ColorName.White) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonLink), colorName, rowHeight); }
                public static bool Unlink(float rowHeight = -1, ColorName colorName = ColorName.White) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonUnlink), colorName, rowHeight); }

                public static bool Play(Color iconColor, float rowHeight = -1) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonPlaySound), iconColor, rowHeight); }
                public static bool Reset(Color iconColor, float rowHeight = -1) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonReset), iconColor, rowHeight); }
                public static bool Sort(Color iconColor, float rowHeight = -1) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonSort), iconColor, rowHeight); }
                public static bool Stop(Color iconColor, float rowHeight = -1) { return Draw(Styles.GetStyle(Styles.StyleName.IconButtonStop), iconColor, rowHeight); }

                public static bool Cancel(Rect rect, ColorName colorName = ColorName.Red) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonCancel), colorName); }
                public static bool Close(Rect rect, ColorName colorName = ColorName.Red) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonClose), colorName); }
                public static bool Minus(Rect rect, ColorName colorName = ColorName.Red) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonMinus), colorName); }
                public static bool Ok(Rect rect, ColorName colorName = ColorName.Green) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonOk), colorName); }
                public static bool Play(Rect rect, ColorName colorName = ColorName.White) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonPlaySound), colorName); }
                public static bool Plus(Rect rect, ColorName colorName = ColorName.Green) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonPlus), colorName); }
                public static bool Reset(Rect rect, ColorName colorName = ColorName.White) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonReset), colorName); }
                public static bool Sort(Rect rect, ColorName colorName = ColorName.White) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonSort), colorName); }
                public static bool Stop(Rect rect, ColorName colorName = ColorName.White) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonStop), colorName); }

                public static bool Play(Rect rect, Color iconColor, float rowHeight = -1) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonPlaySound), iconColor); }
                public static bool Reset(Rect rect, Color iconColor, float rowHeight = -1) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonReset), iconColor); }
                public static bool Sort(Rect rect, Color iconColor, float rowHeight = -1) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonSort), iconColor); }
                public static bool Stop(Rect rect, Color iconColor, float rowHeight = -1) { return Draw(rect, Styles.GetStyle(Styles.StyleName.IconButtonStop), iconColor); }
            }
        }
    }
}