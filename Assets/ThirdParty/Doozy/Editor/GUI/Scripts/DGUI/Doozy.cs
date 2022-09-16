// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.UI.Animation;
using Doozy.Editor.Windows;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Doozy
        {
            public static void DrawCustomStartPosition(RectTransform target, SerializedProperty useCustomStartPosition, SerializedProperty customStartAnchoredPosition, AnimBool useCustomStartPositionExpanded, ColorName componentColorName)
            {
                useCustomStartPositionExpanded.target = useCustomStartPosition.boolValue;

                ColorName backgroundColorName = Colors.GetBackgroundColorName(useCustomStartPosition.boolValue, componentColorName);
//                ColorName textColorName = DGUI.Colors.GetTextColorName(useCustomStartPosition.boolValue, componentColorName);

                float backgroundHeight = Properties.SingleLineHeight + Properties.Space(2) + (Sizes.BarHeight(Size.S) + Properties.Space(3)) * useCustomStartPositionExpanded.faded;
                Background.Draw(backgroundColorName, GUILayout.Height(backgroundHeight));
                GUILayout.Space(-backgroundHeight);
                GUILayout.Space(Properties.Space());
                Line.Draw(true, backgroundColorName, false,
                          () =>
                          {
                              Toggle.Switch.Draw(useCustomStartPosition, Properties.Labels.CustomStartPosition, componentColorName, false, false);
                              if (AlphaGroup.Begin(useCustomStartPositionExpanded))
                                  Property.Draw(customStartAnchoredPosition, componentColorName, Properties.SingleLineHeight);
                              AlphaGroup.End();
                          });

                if (FadeOut.Begin(useCustomStartPositionExpanded))
                {
                    var size = Size.S;
                    float height = Sizes.BarHeight(size);
                    var textAlign = TextAlign.Left;
                    ColorName buttonBackgroundColorName = Colors.DisabledBackgroundColorName;
                    ColorName buttonTextColorName = Colors.DisabledTextColorName;

                    Line.Draw(true, backgroundColorName, false, height,
                              () => { GUILayout.Space(Toggle.Switch.Width / 2 * useCustomStartPositionExpanded.faded); },
                              () =>
                              {
                                  if (Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaArrowAltDown),
                                                                    Properties.Labels.GetPosition,
                                                                    size, textAlign,
                                                                    buttonBackgroundColorName,
                                                                    buttonTextColorName,
                                                                    height))
                                      customStartAnchoredPosition.vector3Value = target.anchoredPosition3D;
                              },
                              () =>
                              {
                                  if (Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaArrowAltUp),
                                                                    Properties.Labels.SetPosition,
                                                                    size, textAlign,
                                                                    buttonBackgroundColorName,
                                                                    buttonTextColorName,
                                                                    height))
                                  {
                                      Undo.RecordObject(target, Properties.Labels.SetPosition);
                                      target.anchoredPosition3D = customStartAnchoredPosition.vector3Value;
                                  }
                              },
                              () =>
                              {
                                  if (Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaRedo),
                                                                    Properties.Labels.ResetPosition,
                                                                    size, textAlign,
                                                                    buttonBackgroundColorName,
                                                                    buttonTextColorName,
                                                                    height))
                                      customStartAnchoredPosition.vector3Value = Vector3.zero;
                              });
                }

                FadeOut.End(useCustomStartPositionExpanded, false);
            }

            public static void DrawDebugMode(SerializedProperty debugMode, ColorName colorName)
            {
                Line.Draw(false,
                          () =>
                          {
                              Toggle.Switch.Draw(debugMode, Properties.Labels.DebugMode, colorName, true, false);
                              GUILayout.FlexibleSpace();
                          });
            }

            public static bool DrawOpenDatabaseButton(DoozyWindow.View view, bool expandWidth = false, UnityAction callback = null)
            {
                if (Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaDatabase),
                                                  Properties.Labels.OpenDatabase,
                                                  Size.S, TextAlign.Left,
                                                  Colors.DisabledBackgroundColorName,
                                                  Colors.DisabledTextColorName,
                                                  Properties.SingleLineHeight + Properties.Space(2),
                                                  expandWidth))
                {
                    DoozyWindow.Open(view, callback);
                    return true;
                }

                return false;
            }

            public static void DrawPreviewAnimationButtons(SerializedObject serializedObject, UIView view, UIViewBehavior viewBehavior, ColorName componentColorName)
            {
                const Size size = Size.S;
                float buttonHeight = Properties.SingleLineHeight;
                GUILayout.BeginHorizontal();
                {
                    bool enabled = GUI.enabled;
                    if (GUI.enabled)
                        GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.PreviewAnimation, size, TextAlign.Left, componentColorName, buttonHeight);
                                  GUILayout.Space(Properties.Space());
                                  if (Button.IconButton.Play(buttonHeight, componentColorName)) UIAnimatorUtils.PreviewViewAnimation(view, viewBehavior.Animation);

                                  GUILayout.Space(-Properties.Space());
                                  if (Button.IconButton.Stop(buttonHeight, componentColorName))
                                  {
                                      UIAnimatorUtils.StopViewPreview(view);
                                      view.ResetToStartValues();
                                  }
                              });

                    GUI.enabled = enabled;

                    GUILayout.FlexibleSpace();

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.ResetAnimationSettings, size, TextAlign.Left, componentColorName, buttonHeight);
                                  if (Button.IconButton.Reset(buttonHeight, componentColorName))
                                  {
                                      if (!serializedObject.isEditingMultipleObjects)
                                      {
                                          Undo.RecordObject(serializedObject.targetObject, "Reset Animation");
                                          viewBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                      }
                                      else
                                      {
                                          Undo.RecordObjects(serializedObject.targetObjects, "Reset Animation");
                                          foreach (Object targetObject in serializedObject.targetObjects)
                                          {
                                              var targetView = (UIView) targetObject;
                                              switch (viewBehavior.Animation.AnimationType)
                                              {
                                                  case AnimationType.Show:
                                                      targetView.ShowBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case AnimationType.Hide:
                                                      targetView.HideBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case AnimationType.Loop:
                                                      targetView.LoopBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                              }
                                          }
                                      }
                                  }
                              });
                }
                GUILayout.EndHorizontal();
            }

            public static void DrawPreviewAnimationButtons(SerializedObject serializedObject, UIPopup popup, UIPopupBehavior popupBehavior, ColorName componentColorName)
            {
                const Size size = Size.S;
                float buttonHeight = Properties.SingleLineHeight;
                GUILayout.BeginHorizontal();
                {
                    bool enabled = GUI.enabled;
                    if (GUI.enabled)
                        GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.PreviewAnimation, size, TextAlign.Left, componentColorName, buttonHeight);
                                  GUILayout.Space(Properties.Space());
                                  if (Button.IconButton.Play(buttonHeight, componentColorName)) UIAnimatorUtils.PreviewPopupAnimation(popup, popupBehavior.Animation);

                                  GUILayout.Space(-Properties.Space());
                                  if (Button.IconButton.Stop(buttonHeight, componentColorName))
                                  {
                                      UIAnimatorUtils.StopPopupPreview(popup);
                                      popup.ResetToStartValues();
                                  }
                              });

                    GUI.enabled = enabled;

                    GUILayout.FlexibleSpace();

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.ResetAnimationSettings, size, TextAlign.Left, componentColorName, buttonHeight);
                                  if (Button.IconButton.Reset(buttonHeight, componentColorName))
                                  {
                                      if (!serializedObject.isEditingMultipleObjects)
                                      {
                                          Undo.RecordObject(serializedObject.targetObject, "Reset Animation");
                                          popupBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                      }
                                      else
                                      {
                                          Undo.RecordObjects(serializedObject.targetObjects, "Reset Animation");
                                          foreach (Object targetObject in serializedObject.targetObjects)
                                          {
                                              var targetPopup = (UIPopup) targetObject;
                                              switch (popupBehavior.Animation.AnimationType)
                                              {
                                                  case AnimationType.Show:
                                                      targetPopup.ShowBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case AnimationType.Hide:
                                                      targetPopup.HideBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                              }
                                          }
                                      }
                                  }
                              });
                }
                GUILayout.EndHorizontal();
            }

            public static void DrawPreviewAnimationButtons(SerializedObject serializedObject, UIButton button, UIButtonBehavior buttonBehavior, ColorName componentColorName)
            {
                UIAnimation animation = null;
                switch (buttonBehavior.ButtonAnimationType)
                {
                    case ButtonAnimationType.Punch:
                        animation = buttonBehavior.PunchAnimation;
                        break;
                    case ButtonAnimationType.State:
                        animation = buttonBehavior.StateAnimation;
                        break;
                    case ButtonAnimationType.Animator:
                        return;
                }

                const Size size = Size.S;
                float buttonHeight = Properties.SingleLineHeight;
                GUILayout.BeginHorizontal();
                {
                    bool enabled = GUI.enabled;
                    GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.PreviewAnimation, size, TextAlign.Left, componentColorName, buttonHeight);
                                  GUILayout.Space(Properties.Space());
                                  if (Button.IconButton.Play(buttonHeight, componentColorName)) UIAnimatorUtils.PreviewButtonAnimation(animation, button.RectTransform, button.CanvasGroup);

                                  GUILayout.Space(-Properties.Space());
                                  if (Button.IconButton.Stop(buttonHeight, componentColorName)) UIAnimatorUtils.StopButtonPreview(button.RectTransform, button.CanvasGroup);
                              });

                    GUI.enabled = enabled;

                    GUILayout.FlexibleSpace();

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.ResetAnimationSettings, size, TextAlign.Left, componentColorName, buttonHeight);
                                  if (Button.IconButton.Reset(buttonHeight, componentColorName))
                                  {
                                      if (!serializedObject.isEditingMultipleObjects)
                                      {
                                          Undo.RecordObject(serializedObject.targetObject, "Reset Animation");
                                          buttonBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                      }
                                      else
                                      {
                                          Undo.RecordObjects(serializedObject.targetObjects, "Reset Animation");
                                          foreach (Object targetObject in serializedObject.targetObjects)
                                          {
                                              var targetButton = (UIButton) targetObject;
                                              switch (buttonBehavior.BehaviorType)
                                              {
                                                  case UIButtonBehaviorType.OnClick:
                                                      targetButton.OnClick.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnDoubleClick:
                                                      targetButton.OnDoubleClick.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnLongClick:
                                                      targetButton.OnLongClick.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnPointerEnter:
                                                      targetButton.OnPointerEnter.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnPointerExit:
                                                      targetButton.OnPointerExit.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnPointerDown:
                                                      targetButton.OnPointerDown.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnPointerUp:
                                                      targetButton.OnPointerUp.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnSelected:
                                                      targetButton.OnSelected.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIButtonBehaviorType.OnDeselected:
                                                      targetButton.OnDeselected.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                              }
                                          }
                                      }
                                  }
                              });
                }
                GUILayout.EndHorizontal();
            }

            public static void DrawPreviewAnimationButtons(SerializedObject serializedObject, UIButton button, UIButtonLoopAnimation buttonLoopAnimation, ColorName componentColorName)
            {
                UIAnimation animation = buttonLoopAnimation.Animation;

                const Size size = Size.S;
                float buttonHeight = Properties.SingleLineHeight;
                GUILayout.BeginHorizontal();
                {
                    bool enabled = GUI.enabled;
                    GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.PreviewAnimation, size, TextAlign.Left, componentColorName, buttonHeight);
                                  GUILayout.Space(Properties.Space());
                                  if (Button.IconButton.Play(buttonHeight, componentColorName)) UIAnimatorUtils.PreviewButtonAnimation(animation, button.RectTransform, button.CanvasGroup);

                                  GUILayout.Space(-Properties.Space());
                                  if (Button.IconButton.Stop(buttonHeight, componentColorName)) UIAnimatorUtils.StopButtonPreview(button.RectTransform, button.CanvasGroup);
                              });

                    GUI.enabled = enabled;

                    GUILayout.FlexibleSpace();

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.ResetAnimationSettings, size, TextAlign.Left, componentColorName, buttonHeight);
                                  if (Button.IconButton.Reset(buttonHeight, componentColorName))
                                  {
                                      if (!serializedObject.isEditingMultipleObjects)
                                      {
                                          Undo.RecordObject(serializedObject.targetObject, "Reset Animation");
                                          buttonLoopAnimation.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                      }
                                      else
                                      {
                                          Undo.RecordObjects(serializedObject.targetObjects, "Reset Animation");
                                          foreach (Object targetObject in serializedObject.targetObjects)
                                          {
                                              var targetButton = (UIButton) targetObject;
                                              switch (buttonLoopAnimation.LoopAnimationType)
                                              {
                                                  case ButtonLoopAnimationType.Normal:
                                                      targetButton.NormalLoopAnimation.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case ButtonLoopAnimationType.Selected:
                                                      targetButton.SelectedLoopAnimation.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                              }
                                          }
                                      }
                                  }
                              });
                }
                GUILayout.EndHorizontal();
            }

            public static void DrawPreviewAnimationButtons(SerializedObject serializedObject, UIToggle toggle, UIToggleBehavior toggleBehavior, ColorName componentColorName)
            {
                UIAnimation animation = null;
                switch (toggleBehavior.ButtonAnimationType)
                {
                    case ButtonAnimationType.Punch:
                        animation = toggleBehavior.PunchAnimation;
                        break;
                    case ButtonAnimationType.State:
                        animation = toggleBehavior.StateAnimation;
                        break;
                    case ButtonAnimationType.Animator:
                        return;
                }

                const Size size = Size.S;
                float buttonHeight = Properties.SingleLineHeight;
                GUILayout.BeginHorizontal();
                {
                    bool enabled = GUI.enabled;
                    GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.PreviewAnimation, size, TextAlign.Left, componentColorName, buttonHeight);
                                  GUILayout.Space(Properties.Space());
                                  if (Button.IconButton.Play(buttonHeight, componentColorName)) UIAnimatorUtils.PreviewButtonAnimation(animation, toggle.RectTransform, toggle.CanvasGroup);

                                  GUILayout.Space(-Properties.Space());
                                  if (Button.IconButton.Stop(buttonHeight, componentColorName)) UIAnimatorUtils.StopButtonPreview(toggle.RectTransform, toggle.CanvasGroup);
                              });

                    GUI.enabled = enabled;

                    GUILayout.FlexibleSpace();

                    Line.Draw(false, componentColorName, true, buttonHeight,
                              () =>
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(Properties.Labels.ResetAnimationSettings, size, TextAlign.Left, componentColorName, buttonHeight);
                                  if (Button.IconButton.Reset(buttonHeight, componentColorName))
                                  {
                                      if (!serializedObject.isEditingMultipleObjects)
                                      {
                                          Undo.RecordObject(serializedObject.targetObject, "Reset Animation");
                                          toggleBehavior.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                      }
                                      else
                                      {
                                          Undo.RecordObjects(serializedObject.targetObjects, "Reset Animation");
                                          foreach (Object targetObject in serializedObject.targetObjects)
                                          {
                                              var targetButton = (UIButton) targetObject;
                                              switch (toggleBehavior.BehaviorType)
                                              {
                                                  case UIToggleBehaviorType.OnClick:
                                                      targetButton.OnClick.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIToggleBehaviorType.OnPointerEnter:
                                                      targetButton.OnPointerEnter.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIToggleBehaviorType.OnPointerExit:
                                                      targetButton.OnPointerExit.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIToggleBehaviorType.OnSelected:
                                                      targetButton.OnSelected.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                                  case UIToggleBehaviorType.OnDeselected:
                                                      targetButton.OnDeselected.LoadPreset(UIAnimations.DEFAULT_DATABASE_NAME, UIAnimations.DEFAULT_PRESET_NAME);
                                                      break;
                                              }
                                          }
                                      }
                                  }
                              });
                }
                GUILayout.EndHorizontal();
            }

            public static void DrawRenameGameObjectAndOpenDatabaseButtons(string prefix, string newName, string suffix, GameObject gameObject, DoozyWindow.View view, bool expandOpenDatabaseButtonWidth = false, UnityAction openCallback = null)
            {
                GUILayout.BeginHorizontal();
                {
                    DrawRenameGameObjectButton(prefix, newName, suffix, gameObject);
                    GUILayout.Space(Properties.Space());
                    DrawOpenDatabaseButton(view, expandOpenDatabaseButtonWidth, openCallback);
                }
                GUILayout.EndHorizontal();
            }

            public static bool DrawRenameGameObjectButton(string prefix, string newName, string suffix, GameObject gameObject)
            {
                if (!Button.Dynamic.DrawIconButton(
                                                   Icon.Edit,
                                                   Properties.Labels.RenameGameObjectTo + " '" + prefix + newName + suffix + "'",
                                                   Size.S, TextAlign.Left,
                                                   Colors.DisabledBackgroundColorName,
                                                   Colors.DisabledTextColorName,
                                                   Properties.SingleLineHeight + Properties.Space(2))) return false;

                Undo.RecordObject(gameObject, "Rename");
                gameObject.name = prefix + newName + suffix;
                return true;
            }

            public static bool DrawSectionButtonLeft(bool enabled, string label, GUIStyle icon, ColorName color, List<IconGroup.Data> miniIcons) { return DrawSectionButton(enabled, label, icon, color, miniIcons, Button.ButtonStyle(TabPosition.Left, enabled)); }
            public static bool DrawSectionButtonMiddle(bool enabled, string label, GUIStyle icon, ColorName color, List<IconGroup.Data> miniIcons) { return DrawSectionButton(enabled, label, icon, color, miniIcons, Button.ButtonStyle(TabPosition.Middle, enabled)); }
            public static bool DrawSectionButtonRight(bool enabled, string label, GUIStyle icon, ColorName color, List<IconGroup.Data> miniIcons) { return DrawSectionButton(enabled, label, icon, color, miniIcons, Button.ButtonStyle(TabPosition.Right, enabled)); }

            private static float SectionButtonHeight { get { return Properties.SingleLineHeight * 2 + Properties.Space(4); } }
            private static float SectionIconSize { get { return SectionButtonHeight * 0.7f; } }
            private static float SectionIconPadding { get { return SectionButtonHeight - SectionIconSize; } }
            private static float SectionTextLeftPadding { get { return SectionIconPadding + SectionIconSize + SectionIconPadding * 1.5f; } }

            public static bool DrawSectionButton(bool enabled, string label, GUIStyle icon, ColorName color, List<IconGroup.Data> miniIcons, GUIStyle buttonStyle)
            {
                bool result;
                Color initialColor = GUI.color;
                Color backgroundColor = Colors.BarColor(color, enabled);
                Color textColor = Colors.TextColor(enabled ? color : Colors.DisabledTextColorName).WithAlpha(Properties.TextIconAlphaValue(enabled));
                Color iconColor = Colors.TextColor(enabled ? color : Colors.DisabledTextColorName).WithAlpha(Properties.TextIconAlphaValue(enabled));
                var customButtonStyle = new GUIStyle(buttonStyle) {fixedHeight = SectionButtonHeight};
                var customLabelStyle = new GUIStyle
                                       {
                                           normal = {textColor = textColor},
                                           padding = new RectOffset((int) SectionTextLeftPadding, (int) Properties.Space(2), (int) Properties.Space(2), 0),
                                           fontSize = 14,
                                           alignment = TextAnchor.UpperLeft,
                                           fixedHeight = SectionButtonHeight
                                       };

                GUILayout.BeginVertical();
                {
                    GUI.color = backgroundColor;
                    result = GUILayout.Button(GUIContent.none, customButtonStyle);
                    GUI.color = initialColor;
                    GUILayout.Space(-SectionButtonHeight - Properties.Space());
                    GUILayout.Label(label, customLabelStyle); //draw text
                    GUILayout.Space(-SectionButtonHeight + Properties.Space());
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(SectionIconPadding);
                        GUI.color = iconColor;
                        Icon.Draw(icon, SectionIconSize, SectionButtonHeight); //draw icon
                        GUI.color = initialColor;
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(-Properties.SingleLineHeight);
                    GUILayout.Space(-Properties.Space(2));
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(SectionTextLeftPadding);
                        IconGroup.Draw(miniIcons, 14, false); //draw mini icons
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUI.color = initialColor;
                return result;
            }

            private static bool DrawDisabledButton(GUIStyle icon, string label) { return Button.Dynamic.DrawIconButton(icon, label, Size.S, TextAlign.Left, Colors.DisabledBackgroundColorName, Colors.DisabledTextColorName, Properties.SingleLineHeight + Properties.Space(2), false); }

            public static void DrawSettingsButton(string componentName)
            {
                if (!DrawDisabledButton(Styles.GetStyle(Styles.StyleName.IconSettings), Properties.Labels.Settings)) return;
                DoozyWindow.Open(DoozyWindow.View.Settings);
                DoozyWindow.Instance.GetAnimBool(componentName + "Settings").target = true;
            }

            public static void DrawManualButton(string url)
            {
                if (!DrawDisabledButton(Styles.GetStyle(Styles.StyleName.IconFaBook), Properties.Labels.Manual)) return;
                Application.OpenURL(url);
            }

            public static void DrawYoutubeButton(string url)
            {
                if (!DrawDisabledButton(Styles.GetStyle(Styles.StyleName.IconFaYoutube), Properties.Labels.YouTube)) return;
                Application.OpenURL(url);
            }

            public static bool DrawSubSectionButtonLeft(bool enabled, string label, ColorName color, List<IconGroup.Data> miniIcons) { return DrawSubSectionButton(enabled, label, color, miniIcons, Button.ButtonStyle(TabPosition.Left, enabled)); }
            public static bool DrawSubSectionButtonMiddle(bool enabled, string label, ColorName color, List<IconGroup.Data> miniIcons) { return DrawSubSectionButton(enabled, label, color, miniIcons, Button.ButtonStyle(TabPosition.Middle, enabled)); }
            public static bool DrawSubSectionButtonRight(bool enabled, string label, ColorName color, List<IconGroup.Data> miniIcons) { return DrawSubSectionButton(enabled, label, color, miniIcons, Button.ButtonStyle(TabPosition.Right, enabled)); }

            private static float SubSectionButtonPadding { get { return Properties.Space(2); } }
            private static float SubSectionButtonHeight { get { return Properties.SingleLineHeight + SubSectionButtonPadding * 2; } }

            public static bool DrawSubSectionButton(bool enabled, string label, ColorName color, List<IconGroup.Data> miniIcons, GUIStyle buttonStyle)
            {
                bool result;
                Color initialColor = GUI.color;
                Color backgroundColor = Colors.BarColor(color, enabled);
                Color textColor = Colors.TextColor(enabled ? color : Colors.DisabledTextColorName).WithAlpha(Properties.TextIconAlphaValue(enabled));
                var customButtonStyle = new GUIStyle(buttonStyle) {fixedHeight = SubSectionButtonHeight};
                var customLabelStyle = new GUIStyle
                                       {
                                           normal = {textColor = textColor},
                                           padding = new RectOffset((int) (SubSectionButtonPadding * 2), (int) SubSectionButtonPadding * 2, 0, 0),
                                           fontSize = 12,
                                           alignment = TextAnchor.MiddleLeft,
                                           fixedHeight = SubSectionButtonHeight
                                       };

                var content = new GUIContent(label);
                Vector2 contentSize = customLabelStyle.CalcSize(content);
                float minWidth = SubSectionButtonPadding + contentSize.x + SubSectionButtonPadding;
                foreach (IconGroup.Data miniIcon in miniIcons)
                    minWidth += miniIcon.Width;
                minWidth += SubSectionButtonPadding;

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    GUI.color = backgroundColor;
                    result = GUILayout.Button(GUIContent.none, customButtonStyle, GUILayout.MinWidth(minWidth), GUILayout.ExpandWidth(true));
                    GUI.color = initialColor;
                    GUILayout.Space(-SubSectionButtonHeight);
                    GUILayout.Label(label, customLabelStyle, GUILayout.Height(SubSectionButtonHeight)); //draw text
                    GUILayout.Space(-SubSectionButtonHeight - SubSectionButtonPadding / 2);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        IconGroup.Draw(miniIcons, SubSectionButtonHeight, false); //draw mini icons
                        GUILayout.Space(SubSectionButtonPadding * 2);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUI.color = initialColor;
                return result;
            }

            public static void DrawTitleWithIcon(GUIStyle iconStyle, string text, Size textSize, float height, ColorName colorName)
            {
                float iconSize = height * 0.8f;

                GUILayout.BeginHorizontal(GUILayout.Height(height));
                {
                    if (iconStyle != null)
                    {
                        GUILayout.Space((height - iconSize) * 1.2f);
                        Icon.Draw(iconStyle, iconSize, height, colorName);
                        GUILayout.Space((height - iconSize) * 1.2f);
                    }
                    else
                    {
                        GUILayout.Space(height * 0.35f);
                    }

                    Label.Draw(text, textSize, TextAlign.Left, colorName, height);

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }


            public static void DrawTitleWithIconAndBackground(GUIStyle icon, string text, bool enabled, ColorName componentColorName)
            {
                DrawTitleWithIconAndBackground(icon,
                                               text, Size.L,
                                               Bar.Height(Size.L),
                                               Colors.GetBackgroundColorName(enabled, componentColorName),
                                               Colors.GetTextColorName(enabled, componentColorName));
            }

            public static void DrawTitleWithIconAndBackground(GUIStyle iconStyle, string text, Size textSize, float height, ColorName backgroundColorName, ColorName textColorName)
            {
                GUILayout.BeginVertical(GUILayout.Height(height));
                {
                    Color color = GUI.color;
                    GUI.color = Utility.IsProSkin ? Colors.GetDColor(backgroundColorName).Normal : Colors.GetDColor(backgroundColorName).Light;
                    GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.TitleBackground), GUILayout.Height(height), GUILayout.ExpandWidth(true));
                    GUI.color = color;

                    GUILayout.Space(-height);

                    DrawTitleWithIcon(iconStyle, text, textSize, height, textColorName);
                }
                GUILayout.EndVertical();
            }

            public static string GetAnimationTypeName(string animationName, AnimationType animationType)
            {
                switch (animationType)
                {
                    case AnimationType.Undefined: return "Undefined Animation";
                    case AnimationType.Show:      return animationName + " In";
                    case AnimationType.Hide:      return animationName + " Out";
                    case AnimationType.Loop:      return animationName + " Loop";
                    case AnimationType.Punch:     return "Punch " + animationName;
                    case AnimationType.State:     return "State " + animationName;
                    default:                      throw new ArgumentOutOfRangeException();
                }
            }

            public static void DrawUIEffect(GameObject source,
                                            UIEffect effect,
                                            SerializedProperty particleSystem,
                                            SerializedProperty effectBehavior,
                                            SerializedProperty stopBehavior,
                                            SerializedProperty autoSort,
                                            SerializedProperty sortingSteps,
                                            SerializedProperty customSortingLayer,
                                            SerializedProperty customSortingOrder,
                                            AnimBool expanded,
                                            ColorName color)
            {
                if (FadeOut.Begin(expanded, false))
                {
                    if (particleSystem.objectReferenceValue == null)
                    {
                        Property.Draw(particleSystem, Properties.Labels.ParticleSystem, Colors.DisabledTextColorName);
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        {
                            Property.Draw(particleSystem, Properties.Labels.ParticleSystem, color);
                            GUILayout.Space(Properties.Space());
                            Property.Draw(effectBehavior, Properties.Labels.Action, color);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(Properties.Space());
                        if ((UIEffectBehavior) effectBehavior.enumValueIndex == UIEffectBehavior.Stop)
                            Property.Draw(stopBehavior, Properties.Labels.StopBehavior, color);
                        else
                            switch ((DynamicSorting) autoSort.enumValueIndex)
                            {
                                case DynamicSorting.Disabled:
                                    Property.Draw(autoSort, Properties.Labels.AutoSort, color);
                                    break;
                                case DynamicSorting.InFront:
                                case DynamicSorting.Behind:
                                    GUILayout.BeginHorizontal();
                                {
                                    Property.Draw(autoSort, Properties.Labels.AutoSort, color);
                                    GUILayout.Space(Properties.Space());
                                    Property.Draw(sortingSteps, Properties.Labels.SortingSteps, color);
                                    GUILayout.Space(Properties.Space());
                                    if (Button.Dynamic.DrawIconButton(Icon.Reset, Properties.Labels.UpdateEffect, Size.M, TextAlign.Left, color, color, Properties.SingleLineHeight + Properties.Space(2), false))
                                        if (effect != null && effect.ParticleSystem != null)
                                        {
                                            Canvas canvasForEffect = null;
                                            Canvas[] canvases = source.GetComponentsInParent<Canvas>();
                                            if (canvases != null && canvases.Length != 0)
                                                foreach (Canvas canvas in canvases)
                                                {
                                                    if (!canvas.isRootCanvas && !canvas.overrideSorting) continue;
                                                    canvasForEffect = canvas;
                                                    break;
                                                }

                                            if (canvasForEffect != null)
                                                effect.UpdateSorting(canvasForEffect.sortingLayerName, canvasForEffect.sortingOrder);
                                        }
                                }
                                    GUILayout.EndHorizontal();
                                    break;
                                case DynamicSorting.Custom:
                                    GUILayout.BeginHorizontal();
                                {
                                    Property.Draw(autoSort, Properties.Labels.AutoSort, color);
                                    GUILayout.Space(Properties.Space());
                                    if (Button.Dynamic.DrawIconButton(Icon.Reset, Properties.Labels.UpdateEffect, Size.M, TextAlign.Left, color, color, Properties.SingleLineHeight + Properties.Space(2), false))
                                        if (effect != null && effect.ParticleSystem != null)
                                        {
                                            Canvas canvasForEffect = null;
                                            Canvas[] canvases = source.GetComponentsInParent<Canvas>();
                                            if (canvases != null && canvases.Length != 0)
                                                foreach (Canvas canvas in canvases)
                                                {
                                                    if (!canvas.isRootCanvas && !canvas.overrideSorting) continue;
                                                    canvasForEffect = canvas;
                                                    break;
                                                }

                                            if (canvasForEffect != null)
                                                effect.UpdateSorting(canvasForEffect.sortingLayerName, canvasForEffect.sortingOrder);
                                        }
                                }
                                    GUILayout.EndHorizontal();
                                    GUILayout.Space(Properties.Space());
                                    Line.Draw(false, color,
                                              () =>
                                              {
                                                  Color initialColor = GUI.color;

                                                  GUILayout.Space(Properties.Space(2));
                                                  Label.Draw(Properties.Labels.CustomSortingLayer, Size.S, color, Properties.SingleLineHeight + Properties.Space());
                                                  SortingLayer[] sortingLayers = SortingLayer.layers;
                                                  var sortingLayerNames = new string[sortingLayers.Length];
                                                  for (int i = 0; i < sortingLayers.Length; i++)
                                                      sortingLayerNames[i] = sortingLayers[i].name;

                                                  int index = 0;
                                                  if (!sortingLayerNames.Contains(customSortingLayer.stringValue))
                                                      customSortingLayer.stringValue = UIEffect.DEFAULT_SORTING_LAYER;
                                                  index = ArrayUtility.IndexOf(sortingLayerNames, customSortingLayer.stringValue);

                                                  EditorGUI.BeginChangeCheck();
                                                  GUI.color = Colors.PropertyColor(color);
                                                  GUILayout.BeginVertical();
                                                  {
                                                      GUILayout.Space(0);
                                                      index = EditorGUILayout.Popup(index, sortingLayerNames);
                                                  }
                                                  GUILayout.EndVertical();
                                                  GUI.color = initialColor;
                                                  if (EditorGUI.EndChangeCheck()) customSortingLayer.stringValue = sortingLayerNames[index];

                                                  GUILayout.Space(Properties.Space(2));
                                                  Label.Draw(Properties.Labels.CustomSortingOrder, Size.S, color, Properties.SingleLineHeight + Properties.Space());
                                                  GUI.color = Colors.PropertyColor(color);
                                                  GUILayout.BeginVertical();
                                                  {
                                                      GUILayout.Space(0);
                                                      EditorGUILayout.PropertyField(customSortingOrder, GUIContent.none, GUILayout.Width(Properties.DefaultFieldWidth * 2));
                                                  }
                                                  GUILayout.EndVertical();
                                                  GUI.color = initialColor;
                                                  GUILayout.Space(Properties.Space(2));
                                              });
                                    break;
                            }
                    }
                }

                FadeOut.End(expanded, false);
            }

            /// <summary> Returns a list with all the enabled icons for an UIAnimation variable </summary>
            public static List<IconGroup.Data> GetBehaviorUIAnimationIcons(List<IconGroup.Data> items, AnimationType animationType, bool moveEnabled, bool rotateEnabled, bool scaleEnabled, bool fadeEnabled, ColorName enabledIconColorName)
            {
                items.Clear();

                GUIStyle animationIcon = null;
                switch (animationType)
                {
                    case AnimationType.Undefined: return items;
                    case AnimationType.Show:
                        if (moveEnabled || rotateEnabled || scaleEnabled || fadeEnabled) animationIcon = Icon.Show;
                        break;
                    case AnimationType.Hide:
                        if (moveEnabled || rotateEnabled || scaleEnabled || fadeEnabled) animationIcon = Icon.Hide;
                        break;
                    case AnimationType.Loop:
                        if (moveEnabled || rotateEnabled || scaleEnabled || fadeEnabled) animationIcon = Icon.Loop;
                        break;
                    case AnimationType.Punch:
                        if (moveEnabled || rotateEnabled || scaleEnabled) animationIcon = Icon.PunchAnimation;
                        break;
                    case AnimationType.State:
                        if (moveEnabled || rotateEnabled || scaleEnabled || fadeEnabled) animationIcon = Icon.StateAnimation;
                        break;
                }

                //ANIMATION ICON
                if (animationIcon != null)
                {
                    items.Add(new IconGroup.Data(true, IconGroup.IconSize, animationIcon, Icon.Show, enabledIconColorName, Colors.DisabledIconColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //MOVE
                if (moveEnabled)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Move, Colors.MoveColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //ROTATE
                if (rotateEnabled)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Rotate, Colors.RotateColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //SCALE
                if (scaleEnabled)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Scale, Colors.ScaleColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //FADE
                if (fadeEnabled)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Fade, Colors.FadeColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                return items;
            }

            /// <summary> Returns a list with all the enabled icons for an Actions variable </summary>
            public static List<IconGroup.Data> GetBehaviorActionsIcons(List<IconGroup.Data> items, bool hasSound, bool hasEffect, bool hasAnimatorEvents, bool hasGameEvents, bool hasUnityEvents, ColorName enabledIconColorName)
            {
                items.Clear();

                //SOUND
                if (hasSound)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Sound, enabledIconColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //EFFECT
                if (hasEffect)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Effect, enabledIconColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //ANIMATOR EVENTS
                if (hasAnimatorEvents)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.Animator, enabledIconColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //GAME EVENTS
                if (hasGameEvents)
                {
                    items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.GameEvent, enabledIconColorName));
                    items.Add(IconGroup.IconSpacingData);
                }

                //EVENTS
                if (hasUnityEvents) items.Add(new IconGroup.Data(IconGroup.IconSize, Icon.UnityEvent, enabledIconColorName));

                return items;
            }

            public static List<IconGroup.Data> GetActionsIcons(UIAction actions, Dictionary<UIAction, List<IconGroup.Data>> dictionary, ColorName enabledIconColorName)
            {
                if (!dictionary.ContainsKey(actions)) dictionary.Add(actions, new List<IconGroup.Data>());
                dictionary[actions].Clear();
                dictionary[actions].Add(new IconGroup.Data(actions.HasSound, IconGroup.IconSize, Icon.Sound, Icon.Sound, enabledIconColorName, Colors.DisabledIconColorName));
                dictionary[actions].Add(IconGroup.IconSpacingData);
                dictionary[actions].Add(new IconGroup.Data(actions.HasEffect, IconGroup.IconSize, Icon.Effect, Icon.Effect, enabledIconColorName, Colors.DisabledIconColorName));
                dictionary[actions].Add(IconGroup.IconSpacingData);
                dictionary[actions].Add(new IconGroup.Data(actions.HasAnimatorEvents, IconGroup.IconSize, Icon.Animator, Icon.Animator, enabledIconColorName, Colors.DisabledIconColorName));
                dictionary[actions].Add(IconGroup.IconSpacingData);
                dictionary[actions].Add(new IconGroup.Data(actions.HasGameEvents, IconGroup.IconSize, Icon.GameEvent, Icon.GameEvent, enabledIconColorName, Colors.DisabledIconColorName));
                dictionary[actions].Add(IconGroup.IconSpacingData);
                dictionary[actions].Add(new IconGroup.Data(actions.HasUnityEvent, IconGroup.IconSize, Icon.UnityEvent, Icon.UnityEvent, enabledIconColorName, Colors.DisabledIconColorName));
                return dictionary[actions];
            }

            public static List<IconGroup.Data> GetUIAnimationIcons(UIAnimation animation, Dictionary<UIAnimation, List<IconGroup.Data>> database)
            {
                if (!database.ContainsKey(animation)) database.Add(animation, new List<IconGroup.Data>());
                database[animation].Clear();
                database[animation].Add(new IconGroup.Data(animation.Move.Enabled, IconGroup.IconSize, Icon.Move, Icon.Move, Colors.MoveColorName, Colors.DisabledIconColorName));
                database[animation].Add(IconGroup.IconSpacingData);
                database[animation].Add(new IconGroup.Data(animation.Rotate.Enabled, IconGroup.IconSize, Icon.Rotate, Icon.Rotate, Colors.RotateColorName, Colors.DisabledIconColorName));
                database[animation].Add(IconGroup.IconSpacingData);
                database[animation].Add(new IconGroup.Data(animation.Scale.Enabled, IconGroup.IconSize, Icon.Scale, Icon.Scale, Colors.ScaleColorName, Colors.DisabledIconColorName));
                database[animation].Add(IconGroup.IconSpacingData);
                database[animation].Add(new IconGroup.Data(animation.Fade.Enabled, IconGroup.IconSize, Icon.Fade, Icon.Fade, Colors.FadeColorName, Colors.DisabledIconColorName));
                return database[animation];
            }

            public static List<IconGroup.Data> GetButtonBehaviorAnimationIcons(UIButtonBehavior buttonBehavior, Dictionary<UIButtonBehavior, List<IconGroup.Data>> database, ColorName enabledIconColorName)
            {
                if (!database.ContainsKey(buttonBehavior)) database.Add(buttonBehavior, new List<IconGroup.Data>());
                database[buttonBehavior].Clear();

                switch (buttonBehavior.ButtonAnimationType)
                {
                    case ButtonAnimationType.Punch:
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.PunchAnimation.Enabled, IconGroup.IconSize, Icon.PunchAnimation, Icon.PunchAnimation, enabledIconColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.PunchAnimation.Move.Enabled, IconGroup.IconSize, Icon.Move, Icon.Move, Colors.MoveColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.PunchAnimation.Rotate.Enabled, IconGroup.IconSize, Icon.Rotate, Icon.Rotate, Colors.RotateColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.PunchAnimation.Scale.Enabled, IconGroup.IconSize, Icon.Scale, Icon.Scale, Colors.ScaleColorName, Colors.DisabledIconColorName));
                        break;

                    case ButtonAnimationType.State:
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.StateAnimation.Enabled, IconGroup.IconSize, Icon.StateAnimation, Icon.StateAnimation, enabledIconColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.StateAnimation.Move.Enabled, IconGroup.IconSize, Icon.Move, Icon.Move, Colors.MoveColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.StateAnimation.Rotate.Enabled, IconGroup.IconSize, Icon.Rotate, Icon.Rotate, Colors.RotateColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.StateAnimation.Scale.Enabled, IconGroup.IconSize, Icon.Scale, Icon.Scale, Colors.ScaleColorName, Colors.DisabledIconColorName));
                        database[buttonBehavior].Add(IconGroup.IconSpacingData);
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.StateAnimation.Fade.Enabled, IconGroup.IconSize, Icon.Fade, Icon.Fade, Colors.FadeColorName, Colors.DisabledIconColorName));
                        break;

                    case ButtonAnimationType.Animator:
                        if (buttonBehavior.HasAnimators) database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.AnimatorsCount.ToString(), enabledIconColorName));
                        database[buttonBehavior].Add(new IconGroup.Data(buttonBehavior.HasAnimators, IconGroup.IconSize, Icon.Animator, Icon.Animator, enabledIconColorName, Colors.DisabledIconColorName));
                        break;
                }

                return database[buttonBehavior];
            }

            public static List<IconGroup.Data> GetToggleBehaviorAnimationIcons(UIToggleBehavior toggleBehavior, Dictionary<UIToggleBehavior, List<IconGroup.Data>> database, ColorName enabledIconColorName)
            {
                if (!database.ContainsKey(toggleBehavior)) database.Add(toggleBehavior, new List<IconGroup.Data>());
                database[toggleBehavior].Clear();

                switch (toggleBehavior.ButtonAnimationType)
                {
                    case ButtonAnimationType.Punch:
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.PunchAnimation.Enabled, IconGroup.IconSize, Icon.PunchAnimation, Icon.PunchAnimation, enabledIconColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.PunchAnimation.Move.Enabled, IconGroup.IconSize, Icon.Move, Icon.Move, Colors.MoveColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.PunchAnimation.Rotate.Enabled, IconGroup.IconSize, Icon.Rotate, Icon.Rotate, Colors.RotateColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.PunchAnimation.Scale.Enabled, IconGroup.IconSize, Icon.Scale, Icon.Scale, Colors.ScaleColorName, Colors.DisabledIconColorName));
                        break;

                    case ButtonAnimationType.State:
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.StateAnimation.Enabled, IconGroup.IconSize, Icon.StateAnimation, Icon.StateAnimation, enabledIconColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.StateAnimation.Move.Enabled, IconGroup.IconSize, Icon.Move, Icon.Move, Colors.MoveColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.StateAnimation.Rotate.Enabled, IconGroup.IconSize, Icon.Rotate, Icon.Rotate, Colors.RotateColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.StateAnimation.Scale.Enabled, IconGroup.IconSize, Icon.Scale, Icon.Scale, Colors.ScaleColorName, Colors.DisabledIconColorName));
                        database[toggleBehavior].Add(IconGroup.IconSpacingData);
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.StateAnimation.Fade.Enabled, IconGroup.IconSize, Icon.Fade, Icon.Fade, Colors.FadeColorName, Colors.DisabledIconColorName));
                        break;

                    case ButtonAnimationType.Animator:
                        if (toggleBehavior.HasAnimators) database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.AnimatorsCount.ToString(), enabledIconColorName));
                        database[toggleBehavior].Add(new IconGroup.Data(toggleBehavior.HasAnimators, IconGroup.IconSize, Icon.Animator, Icon.Animator, enabledIconColorName, Colors.DisabledIconColorName));
                        break;
                }

                return database[toggleBehavior];
            }

            public static void DrawDropZone(Rect dropRect, bool containsMouse)
            {
//                float iconSize = dropRect.height * 0.6f;
//                var iconRect = new Rect(dropRect.x + dropRect.width / 2 - iconSize / 2, dropRect.y + (dropRect.height - iconSize) / 2, iconSize, iconSize);

                Color color = GUI.color;
                GUI.color = GUI.color.WithAlpha(containsMouse ? 0.8f : 0.4f);
//                Background.Draw(dropRect, Colors.DisabledBackgroundColorName);
                GUI.Box(dropRect, Properties.Labels.DropAudioClipsHere, new GUIStyle(GUI.skin.box) {fontSize = Label.Style(Size.S).fontSize});
//                if (containsMouse)
//                    Icon.Draw(iconRect, Styles.GetStyle(Styles.StyleName.IconPlus), Colors.DisabledTextColorName);
//                else
//                    Label.Draw(dropRect, Properties.Labels.DropAudioClipsHere, Size.S, TextAlign.Right, Colors.DisabledTextColorName);
                GUI.color = color;
            }
        }
    }
}