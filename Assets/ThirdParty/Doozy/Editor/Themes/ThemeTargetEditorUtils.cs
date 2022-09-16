// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Windows;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Themes
{
    public static class ThemeTargetEditorUtils
    {
        /// <summary> An enumeration of selected states of objects </summary>
        public enum SelectionState
        {
            /// <summary> The UI object can be selected </summary>
            Normal,
            /// <summary> The UI object is highlighted </summary>
            Highlighted,
            /// <summary> The UI object is pressed </summary>
            Pressed,
            /// <summary> The UI object is selected </summary>
            Selected,
            /// <summary> The UI object cannot be selected </summary>
            Disabled,
        }
        
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        public static void DrawOverrideAlpha(SerializedProperty overrideAlphaProperty, SerializedProperty alphaProperty, float currentAlpha,
                                             ColorName componentColorName, Color initialGUIColor)
        {
            GUILayout.BeginHorizontal();
            {
                bool overrideAlpha = overrideAlphaProperty.boolValue;
                DGUI.Toggle.Switch.Draw(overrideAlphaProperty, UILabels.OverrideAlpha, componentColorName, true, false);
                if (overrideAlpha != overrideAlphaProperty.boolValue)
                {
                    alphaProperty.floatValue = currentAlpha;
                }

                GUILayout.Space(DGUI.Properties.Space());

                GUI.enabled = overrideAlpha;
                GUI.color = DGUI.Colors.PropertyColor(overrideAlpha ? componentColorName : DGUI.Colors.DisabledIconColorName);
                EditorGUILayout.Slider(alphaProperty, 0, 1, GUIContent.none);
                GUI.color = initialGUIColor;
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawThemePopup(ThemesDatabase database, ThemeData themeData, string[] themeNames, int themeIndex,
                                          ColorName componentColorName, SerializedObject serializedObject, Object[] targets, ThemeTarget target, Color initialGUIColor,
                                          Action updateIds, Action updateLists)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Line.Draw(false, componentColorName, true,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   DGUI.Label.Draw(UILabels.SelectedTheme, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                   GUILayout.Space(DGUI.Properties.Space());
                                   GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                   {
                                       GUILayout.Space(0);
                                       GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                                       EditorGUI.BeginChangeCheck();
                                       themeIndex = EditorGUILayout.Popup(GUIContent.none, themeIndex, themeNames);
                                       GUI.color = initialGUIColor;
                                   }
                                   GUILayout.EndVertical();
                                   if (EditorGUI.EndChangeCheck())
                                   {
                                       if (serializedObject.isEditingMultipleObjects)
                                       {
                                           DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
                                           themeData = database.Themes[themeIndex];
                                           foreach (Object o in targets)
                                           {
                                               var themeTarget = (ThemeTarget) o;
                                               if (themeTarget == null) continue;
                                               themeTarget.ThemeId = themeData.Id;
                                           }

                                           updateIds.Invoke();
                                           updateLists.Invoke();

                                           foreach (Object o in targets)
                                           {
                                               var themeTarget = (ThemeTarget) o;
                                               if (themeTarget == null) continue;

                                               if (!themeData.ContainsColorProperty(themeTarget.PropertyId))
                                                   themeTarget.PropertyId = themeData.ColorLabels.Count > 0
                                                                                ? themeData.ColorLabels[0].Id
                                                                                : Guid.Empty;

                                               themeTarget.UpdateTarget(themeData);
                                           }
                                       }
                                       else
                                       {
                                           DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
                                           themeData = database.Themes[themeIndex];
                                           target.ThemeId = themeData.Id;
                                           updateIds.Invoke();
                                           updateLists.Invoke();
                                           target.UpdateTarget(themeData);
                                       }
                                   }
                               });

                GUILayout.Space(DGUI.Properties.Space());

                ThemeTargetEditorUtils.DrawButtonTheme(themeData, componentColorName);
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawButtonTheme(ThemeData themeData, ColorName componentColorName)
        {
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconThemeManager),
                                                   UILabels.Themes, Size.S, TextAlign.Left,
                                                   componentColorName, componentColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
            {
                DoozyWindow.Open(DoozyWindow.View.Themes);
                DoozyWindow.Instance.CurrentThemeId = themeData.Id;
            }
        }

        public static void DrawActiveVariant(ThemeData themeData, ColorName componentColorName)
        {
            DGUI.Line.Draw(false, componentColorName, true,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.ActiveVariant + " : ", Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space());
                               DGUI.Label.Draw(themeData.ActiveVariant.VariantName, Size.L, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        public static void DrawLabelNoPropertyFound()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Label.Draw(UILabels.NoPropertyFound, Size.M, DGUI.Colors.DisabledTextColorName);
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawColorProperties(ThemeData themeData, int propertyIndex,
                                               SerializedObject serializedObject, Object[] targets, ThemeTarget target,
                                               Color initialGUIColor)
        {
            GUIStyle colorButtonStyle = Styles.GetStyle(Styles.StyleName.ColorButton);
            GUIStyle colorButtonSelectedStyle = Styles.GetStyle(Styles.StyleName.ColorButtonSelected);
            for (int i = 0; i < themeData.ColorLabels.Count; i++)
            {
                LabelId colorProperty = themeData.ColorLabels[i];
                int index = i;
                bool selected = i == propertyIndex;
                GUILayout.BeginHorizontal();
                {
                    if (!selected) GUILayout.Space((colorButtonSelectedStyle.fixedWidth - colorButtonStyle.fixedWidth) / 2);
                    GUI.color = themeData.ActiveVariant.Colors[i].Color;
                    {
                        if (GUILayout.Button(GUIContent.none, selected ? colorButtonSelectedStyle : colorButtonStyle))
                        {
                            if (serializedObject.isEditingMultipleObjects)
                            {
                                DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
                                foreach (Object o in targets)
                                {
                                    var themeTarget = (ThemeTarget) o;
                                    if (themeTarget == null) continue;
                                    themeTarget.PropertyId = themeData.ColorLabels[index].Id;
                                    themeTarget.UpdateTarget(themeData);
                                }
                            }
                            else
                            {
                                DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
                                target.PropertyId = themeData.ColorLabels[index].Id;
                                target.UpdateTarget(themeData);
                            }
                        }
                    }
                    GUI.color = initialGUIColor;
                    GUILayout.Space(DGUI.Properties.Space(2));
                    GUI.enabled = selected;
                    DGUI.Label.Draw(colorProperty.Label, selected ? Size.L : Size.M, selected ? colorButtonSelectedStyle.fixedHeight : colorButtonStyle.fixedHeight);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }
        }
        
        public static void DrawSpriteProperties(ThemeData themeData, int propertyIndex,
                                                SerializedObject serializedObject, Object[] targets, ThemeTarget target,
                                                ColorName componentColorName, Color initialGUIColor)
        {
            GUIStyle buttonStyleDisabled = Styles.GetStyle(Styles.StyleName.CheckBoxDisabled);
            GUIStyle buttonStyleEnabled = Styles.GetStyle(Styles.StyleName.CheckBoxEnabled);
            for (int i = 0; i < themeData.SpriteLabels.Count; i++)
            {
                LabelId spriteProperty = themeData.SpriteLabels[i];
                int index = i;
                bool selected = i == propertyIndex;
                GUILayout.BeginHorizontal();
                {
                    GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                    if (GUILayout.Button(GUIContent.none, selected ? buttonStyleEnabled : buttonStyleDisabled))
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
                            foreach (Object o in targets)
                            {
                                var themeTarget = (ThemeTarget) o;
                                if (themeTarget == null) continue;
                                themeTarget.PropertyId = themeData.SpriteLabels[index].Id;
                                themeTarget.UpdateTarget(themeData);
                            }
                        }
                        else
                        {
                            DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
                            target.PropertyId = themeData.SpriteLabels[index].Id;
                            target.UpdateTarget(themeData);
                        }
                    }

                    GUI.color = initialGUIColor;
                    GUILayout.Space(DGUI.Properties.Space(2));
                    GUI.enabled = selected;
                    DGUI.Label.Draw(spriteProperty.Label, selected ? Size.L : Size.M);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }
        }

        public static void DrawTextureProperties(ThemeData themeData, int propertyIndex,
                                                 SerializedObject serializedObject, Object[] targets, ThemeTarget target,
                                                 ColorName componentColorName, Color initialGUIColor)
        {
            GUIStyle buttonStyleDisabled = Styles.GetStyle(Styles.StyleName.CheckBoxDisabled);
            GUIStyle buttonStyleEnabled = Styles.GetStyle(Styles.StyleName.CheckBoxEnabled);
            for (int i = 0; i < themeData.TextureLabels.Count; i++)
            {
                LabelId textureProperty = themeData.TextureLabels[i];
                int index = i;
                bool selected = i == propertyIndex;
                GUILayout.BeginHorizontal();
                {
                    GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                    if (GUILayout.Button(GUIContent.none, selected ? buttonStyleEnabled : buttonStyleDisabled))
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
                            foreach (Object o in targets)
                            {
                                var themeTarget = (ThemeTarget) o;
                                if (themeTarget == null) continue;
                                themeTarget.PropertyId = themeData.TextureLabels[index].Id;
                                themeTarget.UpdateTarget(themeData);
                            }
                        }
                        else
                        {
                            DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
                            target.PropertyId = themeData.TextureLabels[index].Id;
                            target.UpdateTarget(themeData);
                        }
                    }

                    GUI.color = initialGUIColor;
                    GUILayout.Space(DGUI.Properties.Space(2));
                    GUI.enabled = selected;
                    DGUI.Label.Draw(textureProperty.Label, selected ? Size.L : Size.M);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }
        }

        public static void DrawFontProperties(ThemeData themeData, int propertyIndex,
                                              SerializedObject serializedObject, Object[] targets, ThemeTarget target,
                                              ColorName componentColorName, Color initialGUIColor)
        {
            GUIStyle buttonStyleDisabled = Styles.GetStyle(Styles.StyleName.CheckBoxDisabled);
            GUIStyle buttonStyleEnabled = Styles.GetStyle(Styles.StyleName.CheckBoxEnabled);
            for (int i = 0; i < themeData.FontLabels.Count; i++)
            {
                LabelId fontProperty = themeData.FontLabels[i];
                int index = i;
                bool selected = i == propertyIndex;
                GUILayout.BeginHorizontal();
                {
                    GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                    if (GUILayout.Button(GUIContent.none, selected ? buttonStyleEnabled : buttonStyleDisabled))
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
                            foreach (Object o in targets)
                            {
                                var themeTarget = (ThemeTarget) o;
                                if (themeTarget == null) continue;
                                themeTarget.PropertyId = themeData.FontLabels[index].Id;
                                themeTarget.UpdateTarget(themeData);
                            }
                        }
                        else
                        {
                            DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
                            target.PropertyId = themeData.FontLabels[index].Id;
                            target.UpdateTarget(themeData);
                        }
                    }

                    GUI.color = initialGUIColor;
                    GUILayout.Space(DGUI.Properties.Space(2));
                    GUI.enabled = selected;
                    DGUI.Label.Draw(fontProperty.Label, selected ? Size.L : Size.M);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }
        }

        public static void DrawFontAssetProperties(ThemeData themeData, int propertyIndex,
                                                   SerializedObject serializedObject, Object[] targets, ThemeTarget target,
                                                   ColorName componentColorName, Color initialGUIColor)
        {
            GUIStyle buttonStyleDisabled = Styles.GetStyle(Styles.StyleName.CheckBoxDisabled);
            GUIStyle buttonStyleEnabled = Styles.GetStyle(Styles.StyleName.CheckBoxEnabled);
            for (int i = 0; i < themeData.FontAssetLabels.Count; i++)
            {
                LabelId fontAssetProperty = themeData.FontAssetLabels[i];
                int index = i;
                bool selected = i == propertyIndex;
                GUILayout.BeginHorizontal();
                {
                    GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                    if (GUILayout.Button(GUIContent.none, selected ? buttonStyleEnabled : buttonStyleDisabled))
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
                            foreach (Object o in targets)
                            {
                                var themeTarget = (ThemeTarget) o;
                                if (themeTarget == null) continue;
                                themeTarget.PropertyId = themeData.FontAssetLabels[index].Id;
                                themeTarget.UpdateTarget(themeData);
                            }
                        }
                        else
                        {
                            DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
                            target.PropertyId = themeData.FontAssetLabels[index].Id;
                            target.UpdateTarget(themeData);
                        }
                    }

                    GUI.color = initialGUIColor;
                    GUILayout.Space(DGUI.Properties.Space(2));
                    GUI.enabled = selected;
                    DGUI.Label.Draw(fontAssetProperty.Label, selected ? Size.L : Size.M);
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }
        }
    }
}