// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Toolbar
        {
            public static int Draw(int index, List<ToolbarButton> buttons, float toolbarHeight = -1)
            {
                int result = index;
                GUILayout.BeginHorizontal();
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        ToolbarButton button = buttons[i];
                        bool isSelected = index == i;
                        if (i == 0)
                        {
                            if (DrawButton(button, TabPosition.Left, isSelected, toolbarHeight)) result = i;
                        }
                        else if (i == buttons.Count - 1)
                        {
                            if (DrawButton(button, TabPosition.Right, isSelected, toolbarHeight)) result = i;
                        }
                        else
                        {
                            if (DrawButton(button, TabPosition.Middle, isSelected, toolbarHeight)) result = i;
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                return result;
            }

            private static bool DrawButton(ToolbarButton button, TabPosition tabPosition, bool isSelected, float toolbarHeight = -1)
            {
                Color initialColor = GUI.color;
                var buttonStyle = new GUIStyle(Button.ButtonStyle(tabPosition, isSelected))
                                  {
                                      fixedHeight = toolbarHeight >= 0 ? toolbarHeight : Button.ButtonStyle(tabPosition, isSelected).fixedHeight
                                  };
                float iconPadding = Mathf.Max(buttonStyle.fixedHeight * 0.2f, Properties.Space(2));

                if (button.IconStyle != null)
                {
                    GUILayout.BeginVertical();
                    buttonStyle.alignment = TextAnchor.MiddleLeft;
                    buttonStyle.padding.left = (int) (buttonStyle.fixedHeight);
                    buttonStyle.padding.right = (int) iconPadding;
                }

                buttonStyle.padding.right = (int) Properties.Space(3);

                GUI.color = Colors.GetDColor(isSelected ? button.SelectedColorName : button.NormalColorName).Normal.WithAlpha(GUI.color.a * 0.8f);
                bool result = GUILayout.Button(new GUIContent(button.Name), buttonStyle);
                GUI.color = initialColor;

                if (button.IconStyle != null)
                {
                    float iconSize = buttonStyle.fixedHeight * 0.6f;
                    GUILayout.Space(-buttonStyle.fixedHeight);
                    GUILayout.BeginHorizontal(GUILayout.Width(iconSize));
                    {
                        GUILayout.Space(iconPadding);
                        Icon.Draw(button.IconStyle, iconSize, buttonStyle.fixedHeight, isSelected ? button.SelectedColorName : button.NormalColorName);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                // ReSharper disable once InvertIf
                if (result)
                {
                    Properties.ResetKeyboardFocus();
                    if (button.Callback != null) button.Callback.Invoke();
                }

                return result;
            }

            [Serializable]
            public class ToolbarButton
            {
                public string Name;
                public GUIStyle IconStyle;
                public ColorName NormalColorName;
                public ColorName SelectedColorName;
                public Action Callback;

                public ToolbarButton(string name, GUIStyle iconStyle, ColorName normalColorName, ColorName selectedColorName, Action callback = null)
                {
                    Name = name;
                    IconStyle = iconStyle;
                    NormalColorName = normalColorName;
                    SelectedColorName = selectedColorName;
                    Callback = callback;
                }

                public ToolbarButton(string name, GUIStyle iconStyle, ColorName normalColorName, Action callback = null)
                {
                    Name = name;
                    IconStyle = iconStyle;
                    NormalColorName = normalColorName;
                    SelectedColorName = normalColorName;
                    Callback = callback;
                }

                public ToolbarButton(string name, ColorName normalColorName, ColorName selectedColorName, Action callback = null)
                {
                    Name = name;
                    IconStyle = null;
                    NormalColorName = normalColorName;
                    SelectedColorName = selectedColorName;
                    Callback = callback;
                }

                public ToolbarButton(string name, ColorName normalColorName, Action callback = null)
                {
                    Name = name;
                    IconStyle = null;
                    NormalColorName = normalColorName;
                    SelectedColorName = normalColorName;
                    Callback = callback;
                }
            }
        }
    }
}