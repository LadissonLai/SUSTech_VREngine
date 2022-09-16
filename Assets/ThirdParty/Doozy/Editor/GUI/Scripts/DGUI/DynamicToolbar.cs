// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Settings;
using Doozy.Engine;
using Doozy.Engine.Extensions;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class DynamicToolbar
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public class Button
            {
                public readonly string ButtonName;
                public readonly GUIStyle ButtonStyle;
                public readonly Color HoverColor;
                public readonly Action Callback;

                public Button(string buttonName, GUIStyle buttonStyle, Color hoverColor, Action callback)
                {
                    ButtonName = buttonName;
                    ButtonStyle = buttonStyle;
                    HoverColor = hoverColor;
                    Callback = callback;
                }
            }

            public static float Draw(Rect rect, List<Button> buttons, GUIStyle backgroundStyle, Color backgroundColor, Color defaultIconColor, AnimBool expanded, Vector2 mousePosition, bool leftMouseButtonDown)
            {
                Color initialColor = GUI.color;
                GUI.color = backgroundColor;
                GUI.Box(rect, GUIContent.none, backgroundStyle);
                GUI.color = initialColor;

                float toolbarWidth = DoozyWindowSettings.Instance.DynamicToolbarPadding;

                foreach (Button button in buttons)
                {
                    var buttonRect = new Rect(rect.x + toolbarWidth,
                                              rect.y,
                                              DoozyWindowSettings.Instance.DynamicToolbarButtonWidth,
                                              DoozyWindowSettings.Instance.DynamicToolbarButtonHeight);

                    DrawButton(buttonRect, button, defaultIconColor, expanded, mousePosition, leftMouseButtonDown);
                    toolbarWidth += DoozyWindowSettings.Instance.DynamicToolbarButtonWidth;
                    toolbarWidth += DoozyWindowSettings.Instance.DynamicToolbarButtonSpacing;
                }

                toolbarWidth -= DoozyWindowSettings.Instance.DynamicToolbarButtonSpacing;
                toolbarWidth += DoozyWindowSettings.Instance.DynamicToolbarPadding;

                GUI.color = initialColor;
                return toolbarWidth;
            }

            private static void DrawButton(Rect rect, Button button, Color defaultIconColor, AnimBool expanded, Vector2 mousePosition, bool leftMouseButtonDown)
            {
                Color initialColor = GUI.color;
                Color buttonColor = defaultIconColor.WithAlpha(expanded.faded * 0.8f); //normal button color

                rect.y = rect.y - rect.height * (1 - expanded.faded);

                const float padding = 8;


                if (rect.Contains(mousePosition))
                {
                    buttonColor = button.HoverColor.WithAlpha(expanded.faded * (leftMouseButtonDown ? 0.7f : 1f)); //hover button color

                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        if (button.Callback == null)
                        {
                            DDebug.LogError("Dynamic Toolbar button has no callback set!");
                        }
                        else
                        {
                            button.Callback.Invoke();
                            Event.current.Use();
                        }
                    }
                }

                var iconRect = new Rect(rect.x + rect.width / 2 - DoozyWindowSettings.Instance.DynamicToolbarButtonIconSize / 2 * expanded.faded,
                                        rect.y + padding,
                                        DoozyWindowSettings.Instance.DynamicToolbarButtonIconSize * expanded.faded,
                                        DoozyWindowSettings.Instance.DynamicToolbarButtonIconSize * expanded.faded);

                var buttonNameRect = new Rect(rect.x + padding,
                                              iconRect.y + iconRect.height,
                                              rect.width - padding * 2,
                                              rect.height - padding - iconRect.height);

                GUI.color = buttonColor;
                GUI.Box(iconRect, GUIContent.none, button.ButtonStyle);
                GUI.color = initialColor;

                GUI.Label(buttonNameRect, new GUIContent(button.ButtonName), new GUIStyle(GUI.skin.label)
                                                                             {
                                                                                 normal = {textColor = buttonColor},
                                                                                 alignment = TextAnchor.MiddleCenter,
                                                                                 fontSize = 9
                                                                             });
            }
        }
    }
}