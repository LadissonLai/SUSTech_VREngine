// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Database
        {
            private static List<string> s_namesList = new List<string>();

            public static void DrawItemsDatabaseSelector(SerializedObject serializedObject,
                                                         SerializedProperty category, string categoryLabel,
                                                         SerializedProperty name, string nameLabel,
                                                         NamesDatabase database,
                                                         ColorName colorName)
            {
                if (s_namesList == null) s_namesList = new List<string>();
                s_namesList.Clear();

                int categorySelectedIndex = database.CategoryNames.Contains(category.stringValue) ? database.CategoryNames.IndexOf(category.stringValue) : database.CategoryNames.IndexOf(NamesDatabase.CUSTOM);
                bool hasCustomName = category.stringValue.Equals(NamesDatabase.CUSTOM);
                int nameSelectedIndex = 0;

                if (!database.Contains(category.stringValue)) //database does not contain this category -> reset it to custom
                {
                    hasCustomName = true;
                    category.stringValue = NamesDatabase.CUSTOM;
                }

                if (!hasCustomName)
                {
                    s_namesList = database.GetNamesList(category.stringValue);
                    if (s_namesList.Count == 0)
                    {
                        if (!database.GetNamesList(NamesDatabase.GENERAL, true).Contains(NamesDatabase.UNNAMED))
                        {
                            database.GetNamesList(NamesDatabase.GENERAL, true).Add(NamesDatabase.UNNAMED);
                            database.SetDirty(true);
                        }

                        category.stringValue = NamesDatabase.GENERAL;
                        s_namesList = database.GetNamesList(category.stringValue);
                    }


                    if (s_namesList.Contains(name.stringValue))
                    {
                        nameSelectedIndex = s_namesList.IndexOf(name.stringValue);
                    }
                    else
                    {
                        if (category.stringValue.Equals(NamesDatabase.GENERAL))
                        {
                            name.stringValue = NamesDatabase.UNNAMED;
                            nameSelectedIndex = s_namesList.IndexOf(NamesDatabase.UNNAMED);
                        }
                        else if (name.stringValue != NamesDatabase.UNNAMED && EditorUtility.DisplayDialog("Add Name", "Add the '" + name.stringValue + "' name to the '" + category.stringValue + "' category?", "Yes", "No"))
                        {
                            string cleanName = name.stringValue.Trim();
                            if (string.IsNullOrEmpty(cleanName))
                            {
                                name.stringValue = s_namesList[nameSelectedIndex];
                            }
                            else if (s_namesList.Contains(cleanName))
                            {
                                name.stringValue = cleanName;
                                nameSelectedIndex = s_namesList.IndexOf(cleanName);
                            }
                            else
                            {
                                ListOfNames categoryAsset = database.GetCategory(category.stringValue);
                                categoryAsset.Names.Add(cleanName);
                                categoryAsset.SetDirty(false);
                                database.Sort(false, true);
                                name.stringValue = cleanName;
                                nameSelectedIndex = s_namesList.IndexOf(name.stringValue);
                            }
                        }
                        else
                        {
                            name.stringValue = s_namesList[nameSelectedIndex];
                        }
                    }
                }

                float lineHeight = Properties.SingleLineHeight;
                float backgroundHeight = lineHeight * 2 + Properties.Space(2);
                GUILayoutOption backgroundHeightOption = GUILayout.Height(backgroundHeight);

                bool isUnnamed = name.stringValue.Equals(NamesDatabase.UNNAMED);
                Color initialColor = GUI.color;
                float labelAlpha = Properties.TextIconAlphaValue(!isUnnamed);

                GUILayout.BeginHorizontal(backgroundHeightOption);
                {
                    GUILayout.BeginVertical(backgroundHeightOption);
                    {
                        Background.Draw(colorName, backgroundHeight);
                        GUILayout.Space(-backgroundHeight);
                        GUILayout.Space(Properties.Space());
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(Properties.Space(2));
                        GUI.color = GUI.color.WithAlpha(labelAlpha);
                        Label.Draw(categoryLabel, Size.S, colorName);
                        GUI.color = initialColor;
                        GUILayout.EndHorizontal();

                        GUI.color = Colors.PropertyColor(colorName);
                        EditorGUI.BeginChangeCheck();
                        categorySelectedIndex = EditorGUILayout.Popup(categorySelectedIndex, database.CategoryNames.ToArray());
                        if (EditorGUI.EndChangeCheck())
                        {
                            category.stringValue = database.CategoryNames[categorySelectedIndex];
                            if (database.CategoryNames[categorySelectedIndex] != NamesDatabase.CUSTOM)
                            {
                                ListOfNames newCategory = database.GetCategory(database.CategoryNames[categorySelectedIndex]);

                                if (newCategory.Names.Count == 0)
                                {
                                    DDebug.Log("'" + database.CategoryNames[categorySelectedIndex] + "' " + Properties.Labels.CategoryIsEmpty);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(name.stringValue.Trim()))
                                    {
                                        name.stringValue = database.GetNamesList(database.CategoryNames[categorySelectedIndex])[0];
                                    }
                                    else if (name.stringValue != NamesDatabase.UNNAMED && !database.GetNamesList(database.CategoryNames[categorySelectedIndex]).Contains(name.stringValue))
                                    {
                                        if (EditorUtility.DisplayDialog("Add Name", "Add the '" + name.stringValue + "' to the '" + database.CategoryNames[categorySelectedIndex] + "' category?", "Yes", "No"))
                                        {
                                            ListOfNames categoryAsset = database.GetCategory(database.CategoryNames[categorySelectedIndex]);
                                            categoryAsset.Names.Add(name.stringValue);
                                            categoryAsset.SetDirty(false);
                                            database.Sort(false, true);
                                        }
                                        else if (database.CategoryNames[categorySelectedIndex] == NamesDatabase.GENERAL)
                                        {
                                            name.stringValue = NamesDatabase.UNNAMED;
                                        }
                                        else
                                        {
                                            name.stringValue = database.GetNamesList(database.CategoryNames[categorySelectedIndex])[0];
                                        }
                                    }
                                    hasCustomName = false;
                                }
                            }

                            Properties.ResetKeyboardFocus();
                        }

                        GUI.color = initialColor;
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(Properties.Space());
                    GUILayout.BeginVertical(backgroundHeightOption);
                    {
                        Background.Draw(colorName, backgroundHeight);
                        GUILayout.Space(-backgroundHeight);
                        GUILayout.Space(Properties.Space());
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(Properties.Space(2));
                        GUI.color = GUI.color.WithAlpha(labelAlpha);
                        Label.Draw(nameLabel, Size.S, colorName);
                        GUI.color = initialColor;
                        GUILayout.EndHorizontal();

                        GUI.color = Colors.PropertyColor(colorName);
                        if (hasCustomName)
                        {
                            EditorGUILayout.PropertyField(name, GUIContent.none, true);
                        }
                        else
                        {
                            EditorGUI.BeginChangeCheck();
                            nameSelectedIndex = EditorGUILayout.Popup(nameSelectedIndex, s_namesList.ToArray());
                            if (EditorGUI.EndChangeCheck())
                            {
                                name.stringValue = s_namesList[nameSelectedIndex];
                                Properties.ResetKeyboardFocus();
                            }
                        }

                        GUI.color = initialColor;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(-Properties.Space() - 1);
            }

            public static float NameBackgroundHeight { get { return Properties.SingleLineHeight * 1.9f; } }

            public static void DrawItemsDatabaseSelectorForGeneralCategoryOnly(string defaultCategory,                    //UIDrawer.DefaultDrawerCategory
                                                                               SerializedProperty name, string nameLabel, //UILabels.DrawerName
                                                                               SerializedProperty customName,
                                                                               NamesDatabase database,
                                                                               ColorName componentColorName)
            {
                Color color = GUI.color;
                Line.Draw(true, componentColorName, NameBackgroundHeight,
                          () =>
                          {
                              List<string> items = database.GetNamesList(UIDrawer.DefaultDrawerCategory);
                              int selectedIndex = 0;
                              if (!customName.boolValue)
                              {
                                  if (items.Contains(name.stringValue))
                                      selectedIndex = items.IndexOf(name.stringValue);
                                  else
                                      customName.boolValue = true;
                              }

                              GUILayout.BeginVertical();
                              {
                                  Label.Draw(nameLabel, Size.S, TextAlign.Left, componentColorName); //draw 'Name' label
                                  GUILayout.BeginHorizontal(GUILayout.Height(Properties.SingleLineHeight));
                                  {
                                      GUI.color = Colors.GetDColor(componentColorName).Light.WithAlpha(GUI.color.a);
                                      if (customName.boolValue)
                                      {
                                          EditorGUILayout.PropertyField(name, GUIContent.none, true); //draw name property field (text field)
                                      }
                                      else
                                      {
                                          EditorGUI.BeginChangeCheck();
                                          selectedIndex = EditorGUILayout.Popup(selectedIndex, items.ToArray()); //draw name selector (dropdown field)
                                          if (EditorGUI.EndChangeCheck()) name.stringValue = items[selectedIndex];
                                      }

                                      GUI.color = color;
                                      EditorGUI.BeginChangeCheck();
                                      Toggle.Checkbox.Draw(customName, Properties.Labels.CustomName, componentColorName, Properties.SingleLineHeight + Properties.Space(2)); //draw 'custom' toggle
                                      if (EditorGUI.EndChangeCheck())
                                          if (!customName.boolValue)
                                              if (!items.Contains(name.stringValue))
                                              {
                                                  if (EditorUtility.DisplayDialog("New Name",
                                                                                  "Add the '" + name.stringValue + "' name to the database?",
                                                                                  "Yes",
                                                                                  "No"))
                                                  {
                                                      database.GetNamesList(defaultCategory, true).Add(name.stringValue);
                                                      database.SetDirty(true);
                                                  }
                                                  else
                                                  {
                                                      name.stringValue = database.GetNamesList(defaultCategory)[0];
                                                  }
                                              }
                                  }
                                  GUILayout.EndHorizontal();
                              }
                              GUILayout.EndVertical();
                          });
            }
        }
    }
}