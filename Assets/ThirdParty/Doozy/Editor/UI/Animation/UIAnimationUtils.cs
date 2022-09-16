// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Animation;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Animation
{
    public static class UIAnimationUtils
    {
        private static string s_newPresetCategory = string.Empty;
        private static string s_newPresetName = string.Empty;
        private static bool s_createNewPresetCategory;
        private static bool s_createNewPresetName;
        private static SerializedProperty s_behaviorPropertyBeingEdited;
        private static bool s_selectPresetCategoryTextField;
        private static bool s_selectPresetNameTextField;

        public static void ResetPresetSettings()
        {
            s_behaviorPropertyBeingEdited = null;
            s_createNewPresetCategory = false;
            s_createNewPresetName = false;
            s_newPresetCategory = string.Empty;
            s_newPresetName = string.Empty;
            s_selectPresetCategoryTextField = false;
            s_selectPresetNameTextField = false;
        }

        public static bool DrawAnimationPreset(SerializedObject serializedObject,
                                               SerializedProperty loadSelectedPresetAtRuntime,
                                               SerializedProperty presetCategory,
                                               SerializedProperty presetName,
                                               UIAnimationsDatabase databases,
                                               UIAnimation animation,
                                               ColorName colorName,
                                               out UIAnimation outAnimation)
        {
            bool presetUpdated = false;

            UIAnimationDatabase database = databases.Get(presetCategory.stringValue);
            if (database == null || !database.Contains(presetName.stringValue))
            {
                presetCategory.stringValue = UIAnimations.DEFAULT_DATABASE_NAME;
                presetName.stringValue = UIAnimations.DEFAULT_PRESET_NAME;
                serializedObject.ApplyModifiedProperties();
                presetUpdated = true;
            }

            if (database == null) database = databases.Get(presetCategory.stringValue);

            float buttonHeight = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
            float lineHeight = DGUI.Properties.SingleLineHeight;
            float backgroundHeight = lineHeight * 2 + DGUI.Properties.Space(2);
            GUILayoutOption backgroundHeightOption = GUILayout.Height(backgroundHeight);

            bool loadAtRuntime = loadSelectedPresetAtRuntime.boolValue;
            float labelAlpha = DGUI.Properties.TextIconAlphaValue(loadAtRuntime);
            Color initialColor = GUI.color;

            bool isBeingEdited = s_behaviorPropertyBeingEdited == loadSelectedPresetAtRuntime;
            if (isBeingEdited && Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp) ResetPresetSettings();

            UIAnimation returnAnimation = animation;

            DGUI.Line.Draw(false, colorName, false, buttonHeight,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space());

                               if (isBeingEdited)
                               {
                                   GUILayout.Space(DGUI.Properties.Space());
                                   EditorGUI.BeginChangeCheck();
                                   s_createNewPresetCategory = DGUI.Toggle.Checkbox.Draw(s_createNewPresetCategory, colorName, buttonHeight - DGUI.Properties.Space(), serializedObject.isEditingMultipleObjects);
                                   if (EditorGUI.EndChangeCheck())
                                       if (s_createNewPresetCategory)
                                           s_selectPresetCategoryTextField = true;

                                   GUILayout.Space(DGUI.Properties.Space());
                                   DGUI.Label.Draw(DGUI.Properties.Labels.CreateNewCategory, Size.M, colorName, buttonHeight);
                                   GUILayout.FlexibleSpace();
                                   if ((Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) && GUI.GetNameOfFocusedControl().Equals(DGUI.Properties.Labels.PresetName)) |
                                       DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconSave), DGUI.Properties.Labels.SavePreset, Size.S, TextAlign.Left, ColorName.Green, ColorName.Green, buttonHeight, false))
                                   {
                                       if (s_createNewPresetCategory)
                                       {
                                           if (s_newPresetCategory == null) s_newPresetCategory = string.Empty;
                                           s_newPresetCategory = s_newPresetCategory.Trim();
                                           if (string.IsNullOrEmpty(s_newPresetCategory))
                                           {
                                               EditorUtility.DisplayDialog(DGUI.Properties.Labels.NewCategory, DGUI.Properties.Labels.NewCategoryNameCannotBeEmpty, DGUI.Properties.Labels.Ok);
                                           }
                                           else if (databases.DatabaseNames.Contains(s_newPresetCategory))
                                           {
                                               EditorUtility.DisplayDialog(DGUI.Properties.Labels.NewCategory, DGUI.Properties.Labels.AnotherEntryExists, DGUI.Properties.Labels.Ok);
                                           }
                                           else
                                           {
                                               UIAnimationDatabase targetDatabase = UIAnimations.Instance.CreateDatabase(databases.DatabaseType, s_newPresetCategory, true);
                                               if (string.IsNullOrEmpty(s_newPresetName))
                                               {
                                                   EditorUtility.DisplayDialog(DGUI.Properties.Labels.NewPreset, DGUI.Properties.Labels.NewPresetNameCannotBeEmpty, DGUI.Properties.Labels.Ok);
                                               }
                                               else if (targetDatabase.Contains(s_newPresetName))
                                               {
                                                   EditorUtility.DisplayDialog(DGUI.Properties.Labels.NewPreset, DGUI.Properties.Labels.AnotherEntryExists, DGUI.Properties.Labels.Ok);
                                               }
                                               else
                                               {
                                                   targetDatabase.CreatePreset(s_newPresetName, animation);
                                                   targetDatabase.RefreshDatabase(true);
                                                   databases.Update();
                                                   presetCategory.stringValue = s_newPresetCategory;
                                                   presetName.stringValue = s_newPresetName;
                                                   serializedObject.ApplyModifiedProperties();
                                                   ResetPresetSettings();
                                                   presetUpdated = true;
                                               }
                                           }
                                       }
                                       else
                                       {
                                           if (s_newPresetName == null) s_newPresetName = string.Empty;
                                           s_newPresetName = s_newPresetName.Trim();
                                           if (string.IsNullOrEmpty(s_newPresetName))
                                           {
                                               EditorUtility.DisplayDialog(DGUI.Properties.Labels.NewPreset, DGUI.Properties.Labels.NewPresetNameCannotBeEmpty, DGUI.Properties.Labels.Ok);
                                           }
                                           else if (database.Contains(s_newPresetName))
                                           {
                                               EditorUtility.DisplayDialog(DGUI.Properties.Labels.NewPreset, DGUI.Properties.Labels.AnotherEntryExists, DGUI.Properties.Labels.Ok);
                                           }
                                           else
                                           {
                                               database.CreatePreset(s_newPresetName, animation);
                                               database.RefreshDatabase(true);
                                               databases.Update();
                                               presetName.stringValue = s_newPresetName;
                                               serializedObject.ApplyModifiedProperties();
                                               ResetPresetSettings();
                                           }
                                       }
                                   }

                                   GUILayout.Space(DGUI.Properties.Space());
                                   if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconClose), DGUI.Properties.Labels.Cancel, Size.S, TextAlign.Left, ColorName.Red, ColorName.Red, buttonHeight, false)) ResetPresetSettings();
                               }
                               else
                               {
                                   if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaBinoculars), colorName, colorName, buttonHeight))
                                   {
                                       UIAnimations.Instance.SearchForUnregisteredDatabases(true);
                                   }
                                   
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   
                                   if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconNew), DGUI.Properties.Labels.NewPreset, Size.S, TextAlign.Left, colorName, colorName, buttonHeight, false))
                                   {
                                       s_behaviorPropertyBeingEdited = loadSelectedPresetAtRuntime;
                                       s_createNewPresetName = true;
                                       s_newPresetCategory = presetCategory.stringValue;
                                       s_selectPresetNameTextField = true;
                                   }

                                   GUILayout.FlexibleSpace();

                                   if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconLoad), DGUI.Properties.Labels.LoadPreset, Size.S, TextAlign.Left, colorName, colorName, buttonHeight, false))
                                   {
                                       UIAnimation presetAnimation = UIAnimations.LoadPreset(animation.AnimationType, presetCategory.stringValue, presetName.stringValue);
                                       if (presetAnimation != null)
                                       {
                                           Undo.RecordObjects(serializedObject.targetObjects, DGUI.Properties.Labels.LoadPreset);
                                           returnAnimation = presetAnimation;
                                           presetUpdated = true;
                                       }
                                   }

                                   GUILayout.Space(DGUI.Properties.Space(2));

                                   if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconClose), DGUI.Properties.Labels.DeletePreset, Size.S, TextAlign.Left, colorName, colorName, buttonHeight, false))
                                   {
                                       if (presetCategory.stringValue.Equals(UIAnimations.DEFAULT_DATABASE_NAME) && presetName.stringValue.Equals(UIAnimations.DEFAULT_PRESET_NAME)) return;
                                       if (EditorUtility.DisplayDialog(DGUI.Properties.Labels.DeletePreset,
                                                                       DGUI.Properties.Labels.PresetCategory + ": " + presetCategory.stringValue +
                                                                       "\n" +
                                                                       DGUI.Properties.Labels.PresetName + ": " + presetName.stringValue,
                                                                       DGUI.Properties.Labels.Yes,
                                                                       DGUI.Properties.Labels.No))
                                       {
                                           database.Delete(presetName.stringValue, true);
                                           if (database != null && database.Database.Count > 0)
                                           {
                                               presetName.stringValue = database.AnimationNames[0];
                                           }
                                           else
                                           {
                                               UIAnimations.Instance.Initialize();
                                               presetCategory.stringValue = UIAnimations.DEFAULT_DATABASE_NAME;
                                               presetName.stringValue = UIAnimations.DEFAULT_PRESET_NAME;
                                               presetUpdated = true;
                                           }

                                           serializedObject.ApplyModifiedProperties();
                                       }
                                   }
                               }

//                               GUILayout.Space(DGUI.Properties.Space());
                           });

            GUILayout.Space(DGUI.Properties.Space());

            GUILayout.BeginHorizontal(backgroundHeightOption, GUILayout.ExpandWidth(true));
            {
                GUILayout.BeginVertical(backgroundHeightOption, GUILayout.ExpandWidth(true));
                {
                    DGUI.Background.Draw(colorName, backgroundHeight);
                    GUILayout.Space(-backgroundHeight);
                    GUILayout.Space(DGUI.Properties.Space());
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        GUILayout.Space(DGUI.Properties.Space(2));
                        GUI.color = GUI.color.WithAlpha(labelAlpha);
                        DGUI.Label.Draw(DGUI.Properties.Labels.PresetCategory, Size.S, colorName);
                        GUI.color = initialColor;
                    }
                    GUILayout.EndHorizontal();

                    GUI.color = DGUI.Colors.PropertyColor(colorName);

                    if (isBeingEdited && s_createNewPresetCategory)
                    {
                        GUI.SetNextControlName(DGUI.Properties.Labels.PresetCategory);
                        s_newPresetCategory = EditorGUILayout.TextField(GUIContent.none, s_newPresetCategory, GUILayout.ExpandWidth(true));
                        if (s_selectPresetCategoryTextField)
                        {
                            EditorGUI.FocusTextInControl(DGUI.Properties.Labels.PresetCategory);
                            s_selectPresetCategoryTextField = false;
                        }
                    }
                    else
                    {
                        int presetCategoryIndex = databases.DatabaseNames.IndexOf(presetCategory.stringValue);
                        EditorGUI.BeginChangeCheck();
                        presetCategoryIndex = EditorGUILayout.Popup(presetCategoryIndex, databases.DatabaseNames.ToArray(), GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            presetCategory.stringValue = databases.DatabaseNames[presetCategoryIndex];
                            database = databases.Get(databases.DatabaseNames[presetCategoryIndex]);
                            database.RefreshDatabase(false);
                            presetName.stringValue = database.AnimationNames[0];
                            DGUI.Properties.ResetKeyboardFocus();
                            presetUpdated = true;
                        }
                    }


                    GUI.color = initialColor;
                }
                GUILayout.EndVertical();
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginVertical(backgroundHeightOption, GUILayout.ExpandWidth(true));
                {
                    DGUI.Background.Draw(colorName, backgroundHeight);
                    GUILayout.Space(-backgroundHeight);
                    GUILayout.Space(DGUI.Properties.Space());
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        GUILayout.Space(DGUI.Properties.Space(2));
                        GUI.color = GUI.color.WithAlpha(labelAlpha);
                        DGUI.Label.Draw(DGUI.Properties.Labels.PresetName, Size.S, colorName);
                        GUI.color = initialColor;
                    }
                    GUILayout.EndHorizontal();

                    GUI.color = DGUI.Colors.PropertyColor(colorName);

                    if (isBeingEdited && s_createNewPresetName)
                    {
                        GUI.SetNextControlName(DGUI.Properties.Labels.PresetName);
                        s_newPresetName = EditorGUILayout.TextField(GUIContent.none, s_newPresetName, GUILayout.ExpandWidth(true));
                        if (s_selectPresetNameTextField)
                        {
                            EditorGUI.FocusTextInControl(DGUI.Properties.Labels.PresetName);
                            s_selectPresetNameTextField = false;
                        }
                    }
                    else
                    {
                        int presetNameIndex = database.AnimationNames.IndexOf(presetName.stringValue);
                        EditorGUI.BeginChangeCheck();
                        presetNameIndex = EditorGUILayout.Popup(presetNameIndex, database.AnimationNames.ToArray(), GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            presetName.stringValue = database.AnimationNames[presetNameIndex];
                            presetUpdated = true;
                        }
                    }

                    GUI.color = initialColor;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Toggle.Checkbox.Draw(loadSelectedPresetAtRuntime, DGUI.Properties.Labels.LoadSelectedPresetAtRuntime, colorName, true, true, DGUI.Properties.SingleLineHeight);
            GUILayout.Space(DGUI.Properties.Space(2));

            outAnimation = returnAnimation;
            return presetUpdated;
        }
    }
}