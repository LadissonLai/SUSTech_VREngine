// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.UI.Input;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private void DrawViewSettings()
        {
            if (CurrentView != View.Settings) return;

            GUILayout.BeginHorizontal();
            {
                DrawViewHorizontalPadding();
                GUILayout.BeginVertical();
                {
                    DrawComponentSettings("UIButton", DGUI.Colors.UIButtonColorName, DrawUIButtonSettings);
                    GUILayout.Space(DGUI.Properties.Space());
                    DrawComponentSettings("UICanvas", DGUI.Colors.UICanvasColorName, DrawUICanvasSettings);
                    GUILayout.Space(DGUI.Properties.Space());
                    DrawComponentSettings("UIDrawer", DGUI.Colors.UIDrawerColorName, DrawUIDrawerSettings);
                    GUILayout.Space(DGUI.Properties.Space());
                    DrawComponentSettings("UIToggle", DGUI.Colors.UIToggleColorName, DrawUIToggleSettings);
                    GUILayout.Space(DGUI.Properties.Space());
                    DrawComponentSettings("UIView", DGUI.Colors.UIViewColorName, DrawUIViewSettings);
                }
                GUILayout.EndVertical();
                DrawViewHorizontalPadding();
            }
            GUILayout.EndHorizontal();
            
            DrawDynamicViewVerticalSpace(2);
        }

        private void DrawComponentSettings(string componentName, ColorName componentColorName, UnityAction<string, ColorName, AnimBool> drawCallback)
        {
            AnimBool expanded = GetAnimBool(componentName + "Settings");
            DGUI.Bar.Draw(componentName, Size.L, DGUI.Bar.Caret.CaretType.Caret, componentColorName, expanded);
            if (DGUI.FadeOut.Begin(expanded))
            {
                GUILayout.BeginVertical();
                drawCallback.Invoke(componentName, componentColorName, expanded);
                GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(expanded);
        }

        private void DrawUIButtonSettings(string componentName, ColorName componentColorName, AnimBool expanded)
        {
            UIButtonSettings settings = UIButtonSettings.Instance;
            ColorName allowClickColor = settings.AllowMultipleClicks ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName disableIntervalBackgroundColor = !settings.AllowMultipleClicks ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName disableIntervalTextColor = !settings.AllowMultipleClicks ? componentColorName : DGUI.Colors.DisabledTextColorName;
            ColorName inputModeBackgroundColor = settings.InputMode == InputMode.None ? DGUI.Colors.DisabledBackgroundColorName : componentColorName;
            ColorName inputModeTextColor = settings.InputMode == InputMode.None ? DGUI.Colors.DisabledTextColorName : componentColorName;
            AnimBool alternateExpanded = GetAnimBool("UIButtonInputMode", settings.InputMode != InputMode.None);
            alternateExpanded.target = settings.InputMode != InputMode.None;

            float generalSpacing = DGUI.Properties.Space(2) * expanded.faded;
            float buttonSpacing = DGUI.Properties.Space(4) * expanded.faded;

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconUIButton, componentName + " - " + UILabels.DefaultValues, UILabels.DefaultValuesDescription, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                #region UIButton - Default Values

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(-DGUI.Properties.Space(13));
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                                   UILabels.Reset,
                                                                   Size.M, TextAlign.Left,
                                                                   componentColorName,
                                                                   componentColorName,
                                                                   NormalRowHeight,
                                                                   false))
                            {
                                settings.Reset(false);
                                m_needsSave = true;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                        //RENAME PREFIX - RENAME SUFFIX
                        DGUI.Line.Draw(true, componentColorName,
                                       () =>
                                       {
                                           DGUI.Label.Draw(UILabels.RenamePrefix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenamePrefix = EditorGUILayout.TextField(settings.RenamePrefix);
                                           GUILayout.EndVertical();
                                           DGUI.Label.Draw(UILabels.RenameSuffix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.Space(generalSpacing);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenameSuffix = EditorGUILayout.TextField(settings.RenameSuffix);
                                           GUILayout.EndVertical();
                                       }
                                      );


                        GUILayout.Space(generalSpacing);

                        // ALLOW MULTIPLE CLICKS - DISABLE BUTTON INTERVAL
                        DGUI.Line.Draw(false,
                                       () =>
                                       {
                                           settings.AllowMultipleClicks = DGUI.Toggle.Switch.Draw(settings.AllowMultipleClicks, UILabels.AllowMultipleClicks, allowClickColor, false, true, false);
                                           GUILayout.Space(generalSpacing);
                                           bool enabledState = GUI.enabled;
                                           GUI.enabled = !settings.AllowMultipleClicks;
                                           DGUI.Line.Draw(false, disableIntervalBackgroundColor,
                                                          () =>
                                                          {
                                                              GUILayout.Space(generalSpacing);
                                                              float alpha = GUI.color.a;
                                                              GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(!settings.AllowMultipleClicks));
                                                              DGUI.Label.Draw(UILabels.DisableButtonInterval, Size.S, disableIntervalTextColor, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.BeginVertical();
                                                              GUILayout.Space(0);
                                                              settings.DisableButtonBetweenClicksInterval = EditorGUILayout.FloatField(settings.DisableButtonBetweenClicksInterval, GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                                                              GUILayout.EndVertical();
                                                              DGUI.Label.Draw(UILabels.Seconds, Size.S, disableIntervalTextColor, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.Space(generalSpacing);
                                                              GUI.color = GUI.color.WithAlpha(alpha);
                                                          });
                                           GUI.enabled = enabledState;
                                       },
                                       GUILayout.FlexibleSpace
                                      );

                        GUILayout.Space(generalSpacing);

                        //DESELECT BUTTON AFTER CLICK
                        settings.DeselectButtonAfterClick = DGUI.Toggle.Switch.Draw(settings.DeselectButtonAfterClick, UILabels.DeselectButtonAfterClick, settings.DeselectButtonAfterClick ? componentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true, false);

                        GUILayout.Space(generalSpacing);

                        //INPUT MODE
                        DGUI.Line.Draw(false,
                                       () =>
                                       {
                                           DGUI.Line.Draw(false, inputModeBackgroundColor,
                                                          () =>
                                                          {
                                                              GUILayout.Space(generalSpacing);
                                                              DGUI.Label.Draw(UILabels.InputMode, Size.S, inputModeTextColor, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.BeginVertical();
                                                              GUILayout.Space(0);
                                                              settings.InputMode = (InputMode) EditorGUILayout.EnumPopup(settings.InputMode);
                                                              GUILayout.EndVertical();
                                                              GUILayout.Space(generalSpacing);
                                                          });
                                       },
                                       () =>
                                       {
                                           switch (settings.InputMode)
                                           {
                                               case InputMode.KeyCode:
                                                   GUILayout.Space(generalSpacing);
                                                   DGUI.Line.Draw(false, componentColorName,
                                                                  () =>
                                                                  {
                                                                      GUILayout.Space(generalSpacing);
                                                                      DGUI.Label.Draw(UILabels.KeyCode, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                                                      GUILayout.BeginVertical();
                                                                      GUILayout.Space(0);
                                                                      settings.KeyCode = (KeyCode) EditorGUILayout.EnumPopup(settings.KeyCode);
                                                                      GUILayout.EndVertical();
                                                                      GUILayout.Space(generalSpacing);
                                                                  });
                                                   break;
                                               case InputMode.VirtualButton:
                                                   GUILayout.Space(generalSpacing);
                                                   DGUI.Line.Draw(false, componentColorName,
                                                                  () =>
                                                                  {
                                                                      GUILayout.Space(generalSpacing);
                                                                      DGUI.Label.Draw(UILabels.VirtualButton, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                                                      GUILayout.BeginVertical();
                                                                      GUILayout.Space(0);
                                                                      settings.VirtualButtonName = EditorGUILayout.TextField(settings.VirtualButtonName);
                                                                      GUILayout.EndVertical();
                                                                      GUILayout.Space(generalSpacing);
                                                                  });
                                                   break;
                                           }
                                       }
                                      );
                        if (DGUI.FadeOut.Begin(alternateExpanded, false))
                        {
                            GUILayout.Space(generalSpacing);
                            DGUI.Line.Draw(false,
                                           () =>
                                           {
                                               settings.EnableAlternateInputs = DGUI.Toggle.Checkbox.Draw(settings.EnableAlternateInputs, UILabels.AlternateInput, componentColorName, false, true, false);
                                           },
                                           () =>
                                           {
                                               bool enabled = GUI.enabled;
                                               GUI.enabled = settings.EnableAlternateInputs;

                                               inputModeBackgroundColor = !settings.EnableAlternateInputs ? DGUI.Colors.DisabledBackgroundColorName : componentColorName;
                                               inputModeTextColor = !settings.EnableAlternateInputs ? DGUI.Colors.DisabledTextColorName : componentColorName;

                                               switch (settings.InputMode)
                                               {
                                                   case InputMode.KeyCode:
                                                       GUILayout.Space(generalSpacing);
                                                       DGUI.Line.Draw(false, inputModeBackgroundColor,
                                                                      () =>
                                                                      {
                                                                          GUILayout.Space(generalSpacing);
                                                                          DGUI.Label.Draw(UILabels.AlternateKeyCode, Size.S, inputModeTextColor, DGUI.Properties.SingleLineHeight);
                                                                          GUILayout.BeginVertical();
                                                                          GUILayout.Space(0);
                                                                          settings.KeyCodeAlt = (KeyCode) EditorGUILayout.EnumPopup(settings.KeyCodeAlt);
                                                                          GUILayout.EndVertical();
                                                                          GUILayout.Space(generalSpacing);
                                                                      });
                                                       break;
                                                   case InputMode.VirtualButton:
                                                       GUILayout.Space(generalSpacing);
                                                       DGUI.Line.Draw(false, inputModeBackgroundColor,
                                                                      () =>
                                                                      {
                                                                          GUILayout.Space(generalSpacing);
                                                                          DGUI.Label.Draw(UILabels.AlternateVirtualButton, Size.S, inputModeTextColor, DGUI.Properties.SingleLineHeight);
                                                                          GUILayout.BeginVertical();
                                                                          GUILayout.Space(0);
                                                                          settings.VirtualButtonNameAlt = EditorGUILayout.TextField(settings.VirtualButtonNameAlt);
                                                                          GUILayout.EndVertical();
                                                                          GUILayout.Space(generalSpacing);
                                                                      });
                                                       break;
                                               }

                                               GUI.enabled = enabled;
                                           }
                                          );
                        }

                        DGUI.FadeOut.End(alternateExpanded, false);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion

                GUILayout.Space(DGUI.Properties.Space(16) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconBehaviorSettings, componentName + " - " + UILabels.Behaviors, UILabels.ToggleComponentBehaviors, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                #region UIButton - Behaviors

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnPointerEnter), "OnPointerEnter", settings.ShowOnPointerEnter, componentColorName)) settings.ShowOnPointerEnter = !settings.ShowOnPointerEnter;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnPointerExit), "OnPointerExit", settings.ShowOnPointerExit, componentColorName)) settings.ShowOnPointerExit = !settings.ShowOnPointerExit;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnPointerDown), "OnPointerDown", settings.ShowOnPointerDown, componentColorName)) settings.ShowOnPointerDown = !settings.ShowOnPointerDown;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnPointerUp), "OnPointerUp", settings.ShowOnPointerUp, componentColorName)) settings.ShowOnPointerUp = !settings.ShowOnPointerUp;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(buttonSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconButtonClick), "OnClick", settings.ShowOnClick, componentColorName)) settings.ShowOnClick = !settings.ShowOnClick;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconButtonDoubleClick), "OnDoubleClick", settings.ShowOnDoubleClick, componentColorName)) settings.ShowOnDoubleClick = !settings.ShowOnDoubleClick;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconButtonLongClick), "OnLongClick", settings.ShowOnLongClick, componentColorName)) settings.ShowOnLongClick = !settings.ShowOnLongClick;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(buttonSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnButtonSelect), "OnSelected", settings.ShowOnButtonSelected, componentColorName)) settings.ShowOnButtonSelected = !settings.ShowOnButtonSelected;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnButtonDeselect), "OnDeselected", settings.ShowOnButtonDeselected, componentColorName)) settings.ShowOnButtonDeselected = !settings.ShowOnButtonDeselected;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(buttonSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconLoop), "Normal Loop Animation", settings.ShowNormalLoopAnimation, componentColorName)) settings.ShowNormalLoopAnimation = !settings.ShowNormalLoopAnimation;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconLoop), "Selected Loop Animation", settings.ShowSelectedLoopAnimation, componentColorName)) settings.ShowSelectedLoopAnimation = !settings.ShowSelectedLoopAnimation;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion
            }

            if (!EditorGUI.EndChangeCheck()) return;
            settings.SetDirty(false);
            m_needsSave = true;
        }

        private void DrawUICanvasSettings(string componentName, ColorName componentColorName, AnimBool expanded)
        {
            UICanvasSettings settings = UICanvasSettings.Instance;
            ColorName dontDestroyCanvasOnLoadColor = settings.DontDestroyCanvasOnLoad ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            float generalSpacing = DGUI.Properties.Space(2) * expanded.faded;

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconUICanvas, componentName + " - " + UILabels.DefaultValues, UILabels.DefaultValuesDescription, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                #region UICanvas - Default Values

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(-DGUI.Properties.Space(13));
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                                   UILabels.Reset,
                                                                   Size.M, TextAlign.Left,
                                                                   componentColorName,
                                                                   componentColorName,
                                                                   NormalRowHeight,
                                                                   false))
                            {
                                settings.Reset(false);
                                m_needsSave = true;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                        //RENAME PREFIX - RENAME SUFFIX
                        DGUI.Line.Draw(true, componentColorName,
                                       () =>
                                       {
                                           DGUI.Label.Draw(UILabels.RenamePrefix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenamePrefix = EditorGUILayout.TextField(settings.RenamePrefix);
                                           GUILayout.EndVertical();
                                           DGUI.Label.Draw(UILabels.RenameSuffix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.Space(generalSpacing);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenameSuffix = EditorGUILayout.TextField(settings.RenameSuffix);
                                           GUILayout.EndVertical();
                                       }
                                      );

                        GUILayout.Space(generalSpacing);

                        //DONT_DESTROY_CANVAS_ON_LOAD
                        settings.DontDestroyCanvasOnLoad = DGUI.Toggle.Switch.Draw(settings.DontDestroyCanvasOnLoad, UILabels.DontDestroyGameObjectOnLoad, dontDestroyCanvasOnLoadColor, false, true, false);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion
            }
            if (!EditorGUI.EndChangeCheck()) return;
            settings.SetDirty(false);
            m_needsSave = true;
        }

        private void DrawUIDrawerSettings(string componentName, ColorName componentColorName, AnimBool expanded)
        {
            UIDrawerSettings settings = UIDrawerSettings.Instance;
            ColorName detectGesturesColor = settings.DetectGestures ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName customStartPositionBackgroundColorName = settings.UseCustomStartAnchoredPosition ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            float generalSpacing = DGUI.Properties.Space(2) * expanded.faded;
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconUIDrawer, componentName + " - " + UILabels.DefaultValues, UILabels.DefaultValuesDescription, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                #region UIDrawer - Default Values

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(-DGUI.Properties.Space(13));
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                                   UILabels.Reset,
                                                                   Size.M, TextAlign.Left,
                                                                   componentColorName,
                                                                   componentColorName,
                                                                   NormalRowHeight,
                                                                   false))
                            {
                                settings.Reset(false);
                                m_needsSave = true;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                        //RENAME PREFIX - RENAME SUFFIX
                        DGUI.Line.Draw(true, componentColorName,
                                       () =>
                                       {
                                           DGUI.Label.Draw(UILabels.RenamePrefix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenamePrefix = EditorGUILayout.TextField(settings.RenamePrefix);
                                           GUILayout.EndVertical();
                                           DGUI.Label.Draw(UILabels.RenameSuffix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.Space(generalSpacing);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenameSuffix = EditorGUILayout.TextField(settings.RenameSuffix);
                                           GUILayout.EndVertical();
                                       }
                                      );

                        GUILayout.Space(generalSpacing);

                        //CLOSE DIRECTION - DETECT GESTURES
                        GUILayout.BeginHorizontal();
                        {
                            DGUI.Line.Draw(false, componentColorName, true,
                                           () =>
                                           {
                                               GUILayout.Space(generalSpacing);
                                               DGUI.Label.Draw(UILabels.CloseDirection, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                               GUILayout.BeginVertical();
                                               GUILayout.Space(0);
                                               settings.CloseDirection = (SimpleSwipe) EditorGUILayout.EnumPopup(settings.CloseDirection);
                                               GUILayout.EndVertical();
                                               GUILayout.Space(generalSpacing);
                                           });
                            GUILayout.Space(generalSpacing);
                            settings.DetectGestures = DGUI.Toggle.Switch.Draw(settings.DetectGestures, UILabels.DetectGestures, detectGesturesColor, false, true, false);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(generalSpacing);

                        //CUSTOM START POSITION
                        DGUI.Line.Draw(false, customStartPositionBackgroundColorName,
                                       () =>
                                       {
                                           GUILayout.Space(DGUI.Properties.Space(2));
                                           settings.UseCustomStartAnchoredPosition = DGUI.Toggle.Switch.Draw(settings.UseCustomStartAnchoredPosition, UILabels.CustomStartPosition, componentColorName, false, false, false);
                                           bool enabled = GUI.enabled;
                                           GUI.enabled = settings.UseCustomStartAnchoredPosition;
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.CustomStartAnchoredPosition = EditorGUILayout.Vector3Field("", settings.CustomStartAnchoredPosition);
                                           GUILayout.EndVertical();
                                           GUI.enabled = enabled;
                                           GUILayout.Space(DGUI.Properties.Space(2));
                                       });

                        GUILayout.Space(generalSpacing);

                        //OPEN SPEED - CLOSE SPEED
                        DGUI.Line.Draw(true, componentColorName,
                                       () =>
                                       {
                                           DGUI.Label.Draw(UILabels.OpenSpeed, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.OpenSpeed = EditorGUILayout.FloatField(settings.OpenSpeed);
                                           GUILayout.EndVertical();
                                           DGUI.Label.Draw(UILabels.CloseSpeed, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.Space(generalSpacing);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.CloseSpeed = EditorGUILayout.FloatField(settings.CloseSpeed);
                                           GUILayout.EndVertical();
                                       }
                                      );
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion
            }
            if (!EditorGUI.EndChangeCheck()) return;
            settings.SetDirty(false);
            m_needsSave = true;
        }

        private void DrawUIToggleSettings(string componentName, ColorName componentColorName, AnimBool expanded)
        {
            UIToggleSettings settings = UIToggleSettings.Instance;
            ColorName allowClickColor = settings.AllowMultipleClicks ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName disableIntervalBackgroundColor = !settings.AllowMultipleClicks ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName disableIntervalTextColor = !settings.AllowMultipleClicks ? componentColorName : DGUI.Colors.DisabledTextColorName;
            ColorName inputModeBackgroundColor = settings.InputMode == InputMode.None ? DGUI.Colors.DisabledBackgroundColorName : componentColorName;
            ColorName inputModeTextColor = settings.InputMode == InputMode.None ? DGUI.Colors.DisabledTextColorName : componentColorName;
            AnimBool alternateExpanded = GetAnimBool("UIToggleInputMode", settings.InputMode != InputMode.None);
            alternateExpanded.target = settings.InputMode != InputMode.None;

            float generalSpacing = DGUI.Properties.Space(2) * expanded.faded;
            float buttonSpacing = DGUI.Properties.Space(4) * expanded.faded;

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconUIToggle, componentName + " - " + UILabels.DefaultValues, UILabels.DefaultValuesDescription, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                 #region UIButton - Default Values

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(-DGUI.Properties.Space(13));
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                                   UILabels.Reset,
                                                                   Size.M, TextAlign.Left,
                                                                   componentColorName,
                                                                   componentColorName,
                                                                   NormalRowHeight,
                                                                   false))
                            {
                                settings.Reset(false);
                                m_needsSave = true;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                        // ALLOW MULTIPLE CLICKS - DISABLE BUTTON INTERVAL
                        DGUI.Line.Draw(false,
                                       () =>
                                       {
                                           settings.AllowMultipleClicks = DGUI.Toggle.Switch.Draw(settings.AllowMultipleClicks, UILabels.AllowMultipleClicks, allowClickColor, false, true, false);
                                           GUILayout.Space(generalSpacing);
                                           bool enabledState = GUI.enabled;
                                           GUI.enabled = !settings.AllowMultipleClicks;
                                           DGUI.Line.Draw(false, disableIntervalBackgroundColor,
                                                          () =>
                                                          {
                                                              GUILayout.Space(generalSpacing);
                                                              float alpha = GUI.color.a;
                                                              GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(!settings.AllowMultipleClicks));
                                                              DGUI.Label.Draw(UILabels.DisableButtonInterval, Size.S, disableIntervalTextColor, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.BeginVertical();
                                                              GUILayout.Space(0);
                                                              settings.DisableButtonBetweenClicksInterval = EditorGUILayout.FloatField(settings.DisableButtonBetweenClicksInterval, GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                                                              GUILayout.EndVertical();
                                                              DGUI.Label.Draw(UILabels.Seconds, Size.S, disableIntervalTextColor, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.Space(generalSpacing);
                                                              GUI.color = GUI.color.WithAlpha(alpha);
                                                          });
                                           GUI.enabled = enabledState;
                                       },
                                       GUILayout.FlexibleSpace
                                      );

                        GUILayout.Space(generalSpacing);

                        //DESELECT BUTTON AFTER CLICK
                        settings.DeselectButtonAfterClick = DGUI.Toggle.Switch.Draw(settings.DeselectButtonAfterClick, UILabels.DeselectButtonAfterClick, settings.DeselectButtonAfterClick ? componentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true, false);

                        GUILayout.Space(generalSpacing);

                        //INPUT MODE
                        DGUI.Line.Draw(false,
                                       () =>
                                       {
                                           DGUI.Line.Draw(false, inputModeBackgroundColor,
                                                          () =>
                                                          {
                                                              GUILayout.Space(generalSpacing);
                                                              DGUI.Label.Draw(UILabels.InputMode, Size.S, inputModeTextColor, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.BeginVertical();
                                                              GUILayout.Space(0);
                                                              settings.InputMode = (InputMode) EditorGUILayout.EnumPopup(settings.InputMode);
                                                              GUILayout.EndVertical();
                                                              GUILayout.Space(generalSpacing);
                                                          });
                                       },
                                       () =>
                                       {
                                           switch (settings.InputMode)
                                           {
                                               case InputMode.KeyCode:
                                                   GUILayout.Space(generalSpacing);
                                                   DGUI.Line.Draw(false, componentColorName,
                                                                  () =>
                                                                  {
                                                                      GUILayout.Space(generalSpacing);
                                                                      DGUI.Label.Draw(UILabels.KeyCode, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                                                      GUILayout.BeginVertical();
                                                                      GUILayout.Space(0);
                                                                      settings.KeyCode = (KeyCode) EditorGUILayout.EnumPopup(settings.KeyCode);
                                                                      GUILayout.EndVertical();
                                                                      GUILayout.Space(generalSpacing);
                                                                  });
                                                   break;
                                               case InputMode.VirtualButton:
                                                   GUILayout.Space(generalSpacing);
                                                   DGUI.Line.Draw(false, componentColorName,
                                                                  () =>
                                                                  {
                                                                      GUILayout.Space(generalSpacing);
                                                                      DGUI.Label.Draw(UILabels.VirtualButton, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                                                      GUILayout.BeginVertical();
                                                                      GUILayout.Space(0);
                                                                      settings.VirtualButtonName = EditorGUILayout.TextField(settings.VirtualButtonName);
                                                                      GUILayout.EndVertical();
                                                                      GUILayout.Space(generalSpacing);
                                                                  });
                                                   break;
                                           }
                                       }
                                      );
                        if (DGUI.FadeOut.Begin(alternateExpanded, false))
                        {
                            GUILayout.Space(generalSpacing);
                            DGUI.Line.Draw(false,
                                           () =>
                                           {
                                               settings.EnableAlternateInputs = DGUI.Toggle.Checkbox.Draw(settings.EnableAlternateInputs, UILabels.AlternateInput, componentColorName, false, true, false);
                                           },
                                           () =>
                                           {
                                               bool enabled = GUI.enabled;
                                               GUI.enabled = settings.EnableAlternateInputs;

                                               inputModeBackgroundColor = !settings.EnableAlternateInputs ? DGUI.Colors.DisabledBackgroundColorName : componentColorName;
                                               inputModeTextColor = !settings.EnableAlternateInputs ? DGUI.Colors.DisabledTextColorName : componentColorName;

                                               switch (settings.InputMode)
                                               {
                                                   case InputMode.KeyCode:
                                                       GUILayout.Space(generalSpacing);
                                                       DGUI.Line.Draw(false, inputModeBackgroundColor,
                                                                      () =>
                                                                      {
                                                                          GUILayout.Space(generalSpacing);
                                                                          DGUI.Label.Draw(UILabels.AlternateKeyCode, Size.S, inputModeTextColor, DGUI.Properties.SingleLineHeight);
                                                                          GUILayout.BeginVertical();
                                                                          GUILayout.Space(0);
                                                                          settings.KeyCodeAlt = (KeyCode) EditorGUILayout.EnumPopup(settings.KeyCodeAlt);
                                                                          GUILayout.EndVertical();
                                                                          GUILayout.Space(generalSpacing);
                                                                      });
                                                       break;
                                                   case InputMode.VirtualButton:
                                                       GUILayout.Space(generalSpacing);
                                                       DGUI.Line.Draw(false, inputModeBackgroundColor,
                                                                      () =>
                                                                      {
                                                                          GUILayout.Space(generalSpacing);
                                                                          DGUI.Label.Draw(UILabels.AlternateVirtualButton, Size.S, inputModeTextColor, DGUI.Properties.SingleLineHeight);
                                                                          GUILayout.BeginVertical();
                                                                          GUILayout.Space(0);
                                                                          settings.VirtualButtonNameAlt = EditorGUILayout.TextField(settings.VirtualButtonNameAlt);
                                                                          GUILayout.EndVertical();
                                                                          GUILayout.Space(generalSpacing);
                                                                      });
                                                       break;
                                               }

                                               GUI.enabled = enabled;
                                           }
                                          );
                        }

                        DGUI.FadeOut.End(alternateExpanded, false);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion

                GUILayout.Space(DGUI.Properties.Space(16) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconBehaviorSettings, componentName + " - " + UILabels.Behaviors, UILabels.ToggleComponentBehaviors, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                #region UIToggle - Behaviors

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnPointerEnter), "OnPointerEnter", settings.ShowOnPointerEnter, componentColorName)) settings.ShowOnPointerEnter = !settings.ShowOnPointerEnter;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnPointerExit), "OnPointerExit", settings.ShowOnPointerExit, componentColorName)) settings.ShowOnPointerExit = !settings.ShowOnPointerExit;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(buttonSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconButtonClick), "OnClick", settings.ShowOnClick, componentColorName)) settings.ShowOnClick = !settings.ShowOnClick;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(buttonSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnButtonSelect), "OnSelected", settings.ShowOnButtonSelected, componentColorName)) settings.ShowOnButtonSelected = !settings.ShowOnButtonSelected;
                            GUILayout.Space(buttonSpacing);
                            if (DrawToggleSettingsButton(Styles.GetStyle(Styles.StyleName.IconOnButtonDeselect), "OnDeselected", settings.ShowOnButtonDeselected, componentColorName)) settings.ShowOnButtonDeselected = !settings.ShowOnButtonDeselected;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion
            }
            if (!EditorGUI.EndChangeCheck()) return;
            settings.SetDirty(false);
            m_needsSave = true;
        }

        private void DrawUIViewSettings(string componentName, ColorName componentColorName, AnimBool expanded)
        {
            UIViewSettings settings = UIViewSettings.Instance;
            float targetOrientationRowHeight = DGUI.Properties.SingleLineHeight;
            float targetOrientationIconSize = targetOrientationRowHeight * 0.8f;

            bool behaviorAtStartEnabled = settings.BehaviorAtStart != UIViewStartBehavior.DoNothing;
            ColorName behaviorAtStartBackgroundColorName = behaviorAtStartEnabled ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName behaviorAtStartTextColorName = behaviorAtStartEnabled ? componentColorName : DGUI.Colors.DisabledTextColorName;

            ColorName customStartPositionBackgroundColorName = settings.UseCustomStartAnchoredPosition ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;

            bool deselectButtonEnabled = settings.DeselectAnyButtonSelectedOnShow || settings.DeselectAnyButtonSelectedOnHide;
            ColorName deselectButtonBackgroundColorName = deselectButtonEnabled ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName deselectButtonTextColorName = deselectButtonEnabled ? componentColorName : DGUI.Colors.DisabledTextColorName;

            bool disableWhenHiddenEnabled = settings.DisableGameObjectWhenHidden || settings.DisableCanvasWhenHidden || settings.DisableGraphicRaycasterWhenHidden;
            ColorName disableWhenHiddenBackgroundColorName = disableWhenHiddenEnabled ? componentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName disableWhenHiddenTextColorName = disableWhenHiddenEnabled ? componentColorName : DGUI.Colors.DisabledTextColorName;
            float disableWhenHiddenBackgroundHeight = DGUI.Properties.SingleLineHeight * 2 + DGUI.Properties.Space();

            float generalSpacing = DGUI.Properties.Space(2) * expanded.faded;

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconUIView, componentName + " - " + UILabels.DefaultValues, UILabels.DefaultValuesDescription, componentColorName);
                GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                #region UIView - DefaultValues

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(-DGUI.Properties.Space(13));
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                                   UILabels.Reset,
                                                                   Size.M, TextAlign.Left,
                                                                   componentColorName,
                                                                   componentColorName,
                                                                   NormalRowHeight,
                                                                   false))
                            {
                                settings.Reset(false);
                                m_needsSave = true;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                        //RENAME PREFIX - RENAME SUFFIX
                        DGUI.Line.Draw(true, componentColorName,
                                       () =>
                                       {
                                           DGUI.Label.Draw(UILabels.RenamePrefix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenamePrefix = EditorGUILayout.TextField(settings.RenamePrefix);
                                           GUILayout.EndVertical();
                                           DGUI.Label.Draw(UILabels.RenameSuffix, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                           GUILayout.Space(generalSpacing);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.RenameSuffix = EditorGUILayout.TextField(settings.RenameSuffix);
                                           GUILayout.EndVertical();
                                       }
                                      );


                        GUILayout.Space(generalSpacing);

                        //TARGET ORIENTATION
                        GUILayout.BeginHorizontal();
                        DGUI.Line.Draw(false, componentColorName,
                                       () =>
                                       {
                                           Color color = GUI.color;
                                           GUI.color = DGUI.Colors.TextColor(componentColorName).WithAlpha(0.8f);

                                           switch (settings.TargetOrientation)
                                           {
                                               case TargetOrientation.Portrait:
                                                   GUILayout.Space(DGUI.Properties.Space());
                                                   DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconPortrait), targetOrientationIconSize, targetOrientationRowHeight);
                                                   GUILayout.Space(DGUI.Properties.Space());
                                                   break;
                                               case TargetOrientation.Landscape:
                                                   GUILayout.Space(DGUI.Properties.Space(2));
                                                   DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconLandscape), targetOrientationIconSize, targetOrientationRowHeight);
                                                   GUILayout.Space(DGUI.Properties.Space(2));
                                                   break;
                                               case TargetOrientation.Any:
                                                   GUILayout.Space(DGUI.Properties.Space(2));
                                                   DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconOrientationDetector), targetOrientationIconSize, targetOrientationRowHeight);
                                                   GUILayout.Space(DGUI.Properties.Space(2));
                                                   break;
                                           }

                                           GUI.color = color;
                                       },
                                       () => { DGUI.Label.Draw(UILabels.TargetOrientation, Size.S, componentColorName, targetOrientationRowHeight); },
                                       () =>
                                       {
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.TargetOrientation = (TargetOrientation) EditorGUILayout.EnumPopup(settings.TargetOrientation, GUILayout.Width(DGUI.Properties.DefaultFieldWidth * 2));
                                           GUILayout.EndVertical();
                                       });
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(generalSpacing);

                        //BEHAVIOR AT START
                        DGUI.Line.Draw(false, behaviorAtStartBackgroundColorName,
                                       () =>
                                       {
                                           GUILayout.Space(DGUI.Properties.Space(2));
                                           float alpha = GUI.color.a;
                                           GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(behaviorAtStartEnabled));
                                           DGUI.Label.Draw(UILabels.AtStart, Size.S, behaviorAtStartTextColorName, DGUI.Properties.SingleLineHeight);
                                           GUI.color = GUI.color.WithAlpha(alpha);
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.BehaviorAtStart = (UIViewStartBehavior) EditorGUILayout.EnumPopup(settings.BehaviorAtStart);
                                           GUILayout.EndVertical();
                                       });
                        GUILayout.Space(generalSpacing);

                        //CUSTOM START POSITION
                        DGUI.Line.Draw(false, customStartPositionBackgroundColorName,
                                       () =>
                                       {
                                           GUILayout.Space(DGUI.Properties.Space(2));
                                           settings.UseCustomStartAnchoredPosition = DGUI.Toggle.Switch.Draw(settings.UseCustomStartAnchoredPosition, UILabels.CustomStartPosition, componentColorName, false, false, false);
                                           bool enabled = GUI.enabled;
                                           GUI.enabled = settings.UseCustomStartAnchoredPosition;
                                           GUILayout.BeginVertical();
                                           GUILayout.Space(0);
                                           settings.CustomStartAnchoredPosition = EditorGUILayout.Vector3Field("", settings.CustomStartAnchoredPosition);
                                           GUILayout.EndVertical();
                                           GUI.enabled = enabled;
                                           GUILayout.Space(DGUI.Properties.Space(2));
                                       });
                        GUILayout.Space(generalSpacing);

                        //DESELECT ON SHOW / HIDE
                        DGUI.Line.Draw(true, deselectButtonBackgroundColorName,
                                       () =>
                                       {
                                           float alpha = GUI.color.a;
                                           GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(deselectButtonEnabled));
                                           DGUI.Label.Draw(UILabels.DeselectAnyButton, Size.S, deselectButtonTextColorName, DGUI.Properties.SingleLineHeight);
                                           GUI.color = GUI.color.WithAlpha(alpha);
                                           GUILayout.Space(DGUI.Properties.Space());
                                           settings.DeselectAnyButtonSelectedOnShow = DGUI.Toggle.Switch.Draw(settings.DeselectAnyButtonSelectedOnShow, UILabels.Show, componentColorName, false, false, false);
                                           GUILayout.Space(DGUI.Properties.Space());
                                           settings.DeselectAnyButtonSelectedOnHide = DGUI.Toggle.Switch.Draw(settings.DeselectAnyButtonSelectedOnHide, UILabels.Hide, componentColorName, false, false, false);
                                           GUILayout.FlexibleSpace();
                                       });
                        GUILayout.Space(generalSpacing);

                        //DISABLE WHEN HIDDEN
                        DGUI.Background.Draw(disableWhenHiddenBackgroundColorName, GUILayout.Height(disableWhenHiddenBackgroundHeight));
                        GUILayout.Space(-disableWhenHiddenBackgroundHeight);
                        GUILayout.Space(generalSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(DGUI.Properties.Space(2));
                            float alpha = GUI.color.a;
                            GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(disableWhenHiddenEnabled));
                            DGUI.Label.Draw(UILabels.WhenUIViewIsHiddenDisable, Size.S, disableWhenHiddenTextColorName);
                            GUI.color = GUI.color.WithAlpha(alpha);
                            GUILayout.Space(DGUI.Properties.Space(2));
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(generalSpacing);
                        GUILayout.BeginHorizontal();
                        {
                            settings.DisableGameObjectWhenHidden = DGUI.Toggle.Switch.Draw(settings.DisableGameObjectWhenHidden, UILabels.GameObject, componentColorName, false, false, false);
                            GUILayout.Space(DGUI.Properties.Space());
                            settings.DisableCanvasWhenHidden = DGUI.Toggle.Switch.Draw(settings.DisableCanvasWhenHidden, UILabels.Canvas, componentColorName, false, false, false);
                            GUILayout.Space(DGUI.Properties.Space());
                            settings.DisableGraphicRaycasterWhenHidden = DGUI.Toggle.Switch.Draw(settings.DisableGraphicRaycasterWhenHidden, UILabels.GraphicRaycaster, componentColorName, false, false, false);
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                #endregion
            }

            if (!EditorGUI.EndChangeCheck()) return;

            settings.SetDirty(false);
            m_needsSave = true;
        }

        private bool DrawToggleSettingsButton(GUIStyle style, string text, bool enabled, ColorName componentColorName)
        {
            return DGUI.Button.Dynamic.DrawIconButton(style,
                                                      text, Size.L, TextAlign.Left,
                                                      DGUI.Colors.GetBackgroundColorName(enabled, componentColorName),
                                                      DGUI.Colors.GetTextColorName(enabled, componentColorName),
                                                      DGUI.Properties.SingleLineHeight * 1.5f + DGUI.Properties.Space(2));
        }
    }
}