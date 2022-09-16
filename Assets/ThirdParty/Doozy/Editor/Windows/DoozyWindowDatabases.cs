// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private string m_newCategoryName;
        private string m_searchPattern;
        private readonly List<string> m_categoriesWithSearchedItems = new List<string>();
        private string m_categoryBeingRenamed = string.Empty;
        private View m_viewContainingCategoryThatIsBeingRenamed = View.General;

        private AnimBool m_databaseCreateCategory,
                         m_databaseRenameCategory,
                         m_databaseSearchEnabled;

        private AnimBool DatabaseCreateCategory
        {
            get
            {
                if (m_databaseCreateCategory != null) return m_databaseCreateCategory;
                m_databaseCreateCategory = GetAnimBool(NEW_CATEGORY);
                return m_databaseCreateCategory;
            }
        }

        private AnimBool DatabaseRenameCategory
        {
            get
            {
                if (m_databaseRenameCategory != null) return m_databaseRenameCategory;
                m_databaseRenameCategory = GetAnimBool(RENAME);
                return m_databaseRenameCategory;
            }
        }

        private AnimBool DatabaseSearchEnabled
        {
            get
            {
                if (m_databaseSearchEnabled != null) return m_databaseSearchEnabled;
                m_databaseSearchEnabled = GetAnimBool(SEARCH);
                return m_databaseSearchEnabled;
            }
        }

        private void DrawItemsDatabaseTopButtons(NamesDatabase database, bool showOnlyGeneralCategory, View view)
        {
            GUILayout.BeginHorizontal();
            {
                DrawDefaultViewHorizontalSpacing();
                GUILayout.FlexibleSpace();

                if (!showOnlyGeneralCategory)
                {
                    if (ButtonNew(DatabaseCreateCategory.target, UILabels.NewCategory))
                    {
                        if (DatabaseCreateCategory.target)
                        {
                            ResetCreateNewCategory();
                        }
                        else
                        {
                            CollapseAllCategories(database, view);
                            ResetRenameCategory();
                            ResetSearchDatabase();
                            StartCreateNewCategory();
                        }
                    }

                    DrawDefaultViewHorizontalSpacing();
                }


                if (ButtonSortDatabase())
                {
                    ResetCreateNewCategory();
                    ResetSearchDatabase();
                    ResetRenameCategory();
                    database.RemoveEmptyNames(true);
                    database.Sort(false);
                    database.UpdateListOfCategoryNames();
                }

                DrawDefaultViewHorizontalSpacing();

                if (ButtonRefreshDatabase())
                {
                    ResetCreateNewCategory();
                    ResetSearchDatabase();
                    ResetRenameCategory();
                    database.RefreshDatabase(true, false);
                }

                DrawDefaultViewHorizontalSpacing();

                if (ButtonResetDatabase())
                {
                    ResetCreateNewCategory();
                    ResetSearchDatabase();
                    ResetRenameCategory();
                    database.ResetDatabase();
                }
                
                DrawDefaultViewHorizontalSpacing();

                if (ButtonSaveDatabase())
                {
                    Save();
                }
                
                DrawDefaultViewHorizontalSpacing();

                if (!showOnlyGeneralCategory)
                {
                    DrawDefaultViewHorizontalSpacing();
                    if (ButtonSearchFor(UILabels.SearchForCategories))
                    {
                        ResetCreateNewCategory();
                        ResetSearchDatabase();
                        ResetRenameCategory();
                        database.SearchForUnregisteredDatabases(true);
                    }
                }

                GUILayout.FlexibleSpace();
                DrawDefaultViewHorizontalSpacing();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawItemsDatabase(NamesDatabase database, bool showOnlyGeneralCategory, View view)
        {
            Event current = Event.current;
            DatabaseRenameCategory.target = view != View.General && m_categoryBeingRenamed != string.Empty;
            bool newCategoryCreated = false;
            ColorName viewColorName = CurrentViewColorName;

            DrawItemsDatabaseTopButtons(database, showOnlyGeneralCategory, view);

            #region CREATE NEW CATEGORY - TEXT FIELD

            if (DGUI.FadeOut.Begin(DatabaseCreateCategory, false))
            {
                GUILayout.Space(DGUI.Properties.Space(2) * DatabaseCreateCategory.faded);
                GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)));
                {
                    GUILayout.Space((FullViewWidth - NewCategoryNameTextFieldWidth - 32) / 2);

                    GUI.color = DGUI.Colors.GetDColor(ColorName.Green).Normal.WithAlpha(GUI.color.a);
                    m_newCategoryName = EditorGUILayout.TextField(GUIContent.none, m_newCategoryName, GUILayout.Width(NewCategoryNameTextFieldWidth));
                    GUI.color = InitialGUIColor;


                    if (DGUI.Button.IconButton.Ok() ||
                        current.type == EventType.KeyUp && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter))
                    {
                        if (database.CreateCategory(m_newCategoryName, new List<string>(), true, false))
                        {
                            newCategoryCreated = true;
                            GetAnimBool(view + m_newCategoryName, true);
                            ResetCreateNewCategory();
                        }
                        else
                        {
                            Instance.Focus();
                        }
                    }

                    if (DGUI.Button.IconButton.Cancel() ||
                        current.keyCode == KeyCode.Escape && current.type == EventType.KeyUp)
                        ResetCreateNewCategory();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }

            DGUI.FadeOut.End(DatabaseCreateCategory);

            GUI.color = InitialGUIColor;

            #endregion

            DrawDynamicViewVerticalSpace();

            #region SEARCH ROW

            DatabaseSearchEnabled.target = m_searchPattern.Length > 0; //search has been enabled if the dev typed a least on character in the search field
            if (!showOnlyGeneralCategory)
            {
                GUILayout.BeginHorizontal();
                {
                    DrawViewSearchHorizontalPadding();
                    DrawSearchIconAndText();

                    DGUI.Properties.SetNextControlName(SEARCH);
                    m_searchPattern = EditorGUILayout.TextField(GUIContent.none, m_searchPattern, GUILayout.ExpandWidth(true)); //search text field

                    if (!DatabaseSearchEnabled.target && m_searchPattern.Length > 0) //search was just initiated -> reset the categories list that contain items that match the search pattern
                        m_categoriesWithSearchedItems.Clear();

                    if (!EditorGUIUtility.editingTextField && current.keyCode == KeyCode.S && current.type == EventType.KeyUp)
                    {
                        StartSearchDatabase();
                        EditorGUI.FocusTextInControl(SEARCH);
                    }

                    GUI.color = InitialGUIColor;

                    if (DatabaseSearchEnabled.faded > 0.1f)
                    {
                        DGUI.AlphaGroup.Begin(DatabaseSearchEnabled);
                        if (ButtonClearSearch(DatabaseSearchEnabled))
                        {
                            ResetSearchDatabase();
                            GUILayout.EndHorizontal();
                            return;
                        }

                        DGUI.AlphaGroup.End();
                    }

                    DrawViewSearchHorizontalPadding();
                }
                GUILayout.EndHorizontal();

                DrawDynamicViewVerticalSpace();
            }

            #endregion

            if (newCategoryCreated) return;
            DatabaseSearchEnabled.target = m_searchPattern.Length > 0; //search has been enabled if the dev typed a least on character in the search field

            //START - draw database
            bool foundNullCategory = false;

            #region Draw Database

            GUILayout.BeginHorizontal();
            {
                DrawViewHorizontalPadding();

                GUILayout.BeginVertical();
                {
                    //DRAW CATEGORY
                    foreach (ListOfNames category in database.Categories)
                    {
                        if (category == null)
                        {
                            foundNullCategory = true;
                            continue;
                        }

                        bool isGeneralCategory = category.CategoryName.Equals(NamesDatabase.GENERAL);
                        if (!isGeneralCategory && showOnlyGeneralCategory) continue;
                        bool categoryContainsSearchedItems = false;
                        bool renamingThisCategory = !DatabaseSearchEnabled.target && m_viewContainingCategoryThatIsBeingRenamed == view && m_categoryBeingRenamed == category.CategoryName;
                        AnimBool categoryAnimBool = GetAnimBool(view + category.CategoryName);

                        GUILayout.BeginHorizontal();
                        {
                            bool enabledState = GUI.enabled;
                            GUI.enabled = !(DatabaseRenameCategory.target && renamingThisCategory) && !DatabaseSearchEnabled.target; //do not allow the dev to use this button if THIS category is being renamed or a search is happening

                            if (showOnlyGeneralCategory) categoryAnimBool.target = true;
                            else
                                DGUI.Bar.Draw(category.CategoryName,
                                              Size.L,
                                              DGUI.Bar.Caret.CaretType.Caret,
                                              DatabaseSearchEnabled.target                                        //search is enabled
                                                  ? m_categoriesWithSearchedItems.Contains(category.CategoryName) //check if this category contains items that match the search pattern
                                                        ? viewColorName                                           //search is enabled and this category contains items that match the search pattern
                                                        : ColorName.Gray                                          //search is enabled and this category DOES NOT contain items that match the search pattern
                                                  : renamingThisCategory                                          //rename for THIS category is enabled
                                                      ? RENAME_COLOR_NAME                                         //rename for THIS category is enabled
                                                      : viewColorName,                                            //rename for THIS category is NOT enabled
                                              categoryAnimBool);

                            GUI.enabled = enabledState;

                            if (!isGeneralCategory)
                                if (!DatabaseSearchEnabled.target && categoryAnimBool.faded > 0.05f && DatabaseRenameCategory.faded < 0.5f)
                                {
                                    DGUI.AlphaGroup.Begin(categoryAnimBool.faded * (1 - DatabaseRenameCategory.faded));
                                    {
                                        #region Button - RENAME CATEGORY

                                        if (DGUI.Button.Draw(DGUI.Properties.Labels.Rename,
                                                             Size.S,
                                                             TextAlign.Center,
                                                             DGUI.Colors.DisabledBackgroundColorName,
                                                             DGUI.Colors.DisabledTextColorName,
                                                             true,
                                                             DGUI.Bar.Height(Size.L),
                                                             80 * categoryAnimBool.faded * (1 - DatabaseRenameCategory.faded))
                                            ||
                                            renamingThisCategory && Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp)
                                        {
                                            if (!renamingThisCategory &&
                                                EditorUtility.DisplayDialog(UILabels.RenameCategory + " '" + category + "'",
                                                                            UILabels.RenameCategoryDialogMessage +
                                                                            "\n\n" +
                                                                            UILabels.YouAreResponsibleToUpdateYourCode,
                                                                            UILabels.Continue,
                                                                            UILabels.Cancel))
                                            {
                                                StartRenameCategory(view, category.CategoryName);
                                                Instance.Focus();
                                            }
                                            else
                                            {
                                                ResetRenameCategory();
                                            }
                                        }

                                        #endregion

                                        #region Button - DELETE CATEGORY

                                        if (DGUI.Button.IconButton.Cancel(BarHeight)) //DELETE Category
                                        {
                                            database.DeleteCategory(category);
                                            break;
                                        }

                                        #endregion
                                    }
                                    DGUI.AlphaGroup.End();
                                }
                        }
                        GUILayout.EndHorizontal();


                        if (DGUI.FadeOut.Begin(DatabaseSearchEnabled.target ? 1 : categoryAnimBool.faded))
                        {
                            GUILayout.BeginVertical();
                            {
                                //RENAME CATEGORY
                                if (renamingThisCategory)
                                {
                                    if (DGUI.FadeOut.Begin(DatabaseRenameCategory))
                                    {
                                        GUILayout.BeginVertical();
                                        {
                                            GUILayout.Space(DGUI.Properties.Space() * DatabaseRenameCategory.faded);
                                            GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)));
                                            {
                                                DrawDefaultViewHorizontalSpacing();

                                                GUI.color = DGUI.Colors.GetDColor(RENAME_COLOR_NAME).Normal.WithAlpha(GUI.color.a);

                                                DGUI.Label.Draw(UILabels.RenameTo, Size.S, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)); //rename to label

                                                GUI.SetNextControlName(RENAME);
                                                m_newCategoryName = EditorGUILayout.TextField(GUIContent.none, m_newCategoryName); //rename to field
                                                GUI.color = InitialGUIColor;

                                                if (DGUI.Button.IconButton.Ok() ||
                                                    Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp) //rename OK button
                                                {
                                                    Instance.Focus();

                                                    if (database.Rename(category.CategoryName, m_newCategoryName))
                                                    {
                                                        GetAnimBool(view + m_newCategoryName).value = true;
                                                        ResetRenameCategory();
                                                        Event.current.Use();
                                                        break;
                                                    }
                                                }

                                                if (DGUI.Button.IconButton.Cancel() ||
                                                    Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp) //rename CANCEL button
                                                {
                                                    ResetRenameCategory();
                                                    Event.current.Use();
                                                }

                                                EditorGUI.FocusTextInControl(RENAME);

                                                DrawDefaultViewHorizontalSpacing();
                                            }
                                            GUILayout.EndHorizontal();

                                            GUILayout.Space(DGUI.Properties.Space(4) * DatabaseRenameCategory.faded);
                                        }
                                        GUILayout.EndVertical();
                                        GUI.color = InitialGUIColor;
                                    }

                                    DGUI.FadeOut.End(DatabaseRenameCategory, false);
                                }

                                GUI.enabled = !(DatabaseRenameCategory.target && renamingThisCategory);

                                if (category.Names.Count == 0)
                                {
                                    GUILayout.Space(DGUI.Properties.Space());
                                    DGUI.Colors.SetDisabledGUIColorAlpha();
                                    DGUI.Label.Draw(UILabels.CategoryIsEmpty, Size.S);
                                    DGUI.Colors.SetNormalGUIColorAlpha();
                                }
                                else
                                {
                                    for (int index = 0; index < category.Names.Count; index++)
                                    {
                                        string currentName = category.Names[index];

                                        if (DatabaseSearchEnabled.target)
                                        {
                                            if (!Regex.IsMatch(category.Names[index], m_searchPattern, RegexOptions.IgnoreCase))
                                                continue;
                                            categoryContainsSearchedItems = true;
                                        }

                                        GUILayout.BeginHorizontal();
                                        {
                                            if (showOnlyGeneralCategory) GUILayout.Space(-DGUI.Properties.Space(3) * categoryAnimBool.faded);
                                            string newName = currentName;
                                            bool guiEnabled = GUI.enabled;
                                            if (isGeneralCategory && !NamesDatabase.CanDeleteItem(database, currentName))
                                                GUI.enabled = false;
                                            EditorGUI.BeginChangeCheck();
                                            newName = EditorGUILayout.DelayedTextField(GUIContent.none, newName);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                category.UndoRecord(UILabels.Rename);
                                                category.Names[index] = newName;
                                                category.SetDirty(false);
                                                m_needsSave = true;
//                                                break;
                                            }

                                            if (DGUI.Button.IconButton.Minus()) //remove entry
                                            {
                                                if (!isGeneralCategory || isGeneralCategory && NamesDatabase.CanDeleteItem(database, currentName))
                                                {
                                                    category.UndoRecord(UILabels.RemoveItem);
                                                    category.Names.RemoveAt(index);
                                                    category.SetDirty(false);
                                                    m_needsSave = true;
                                                }

//                                                break;
                                            }

                                            GUI.enabled = guiEnabled;
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }

                                if (!DatabaseSearchEnabled.target)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.FlexibleSpace();
                                        if (DGUI.Button.IconButton.Plus())
                                        {
                                            category.UndoRecord(UILabels.AddItem);
                                            category.Names.Add("");
                                            category.SetDirty(false);
                                            m_needsSave = true;
                                            break;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }

                                GUILayout.Space(DGUI.Properties.Space(2) * categoryAnimBool.faded);
                                GUI.enabled = true;
                            }
                            GUILayout.EndVertical();
                        }

                        DGUI.FadeOut.End(DatabaseSearchEnabled.target ? 1 : categoryAnimBool.faded);

                        if (!DatabaseSearchEnabled.target) continue;
                        if (categoryContainsSearchedItems && !m_categoriesWithSearchedItems.Contains(category.CategoryName))
                        {
                            m_categoriesWithSearchedItems.Add(category.CategoryName);
                            categoryAnimBool.target = true;
                        }
                        else if (!categoryContainsSearchedItems && m_categoriesWithSearchedItems.Contains(category.CategoryName))
                        {
                            m_categoriesWithSearchedItems.Remove(category.CategoryName);
                            categoryAnimBool.target = false;
                        }
                    }
                }
                GUILayout.EndVertical();

                DrawViewHorizontalPadding();
            }
            GUILayout.EndHorizontal();

            #endregion

            if (foundNullCategory) database.RefreshDatabase(false, false);
        }

        private void CollapseAllCategories(NamesDatabase database, View view)
        {
            foreach (string databaseCategory in database.CategoryNames)
                GetAnimBool(view + databaseCategory).target = false;
        }

        private void ExpandAllCategories(NamesDatabase database, View view)
        {
            foreach (string databaseCategory in database.CategoryNames)
                GetAnimBool(view + databaseCategory).target = true;
        }

        private void ResetCreateNewCategory()
        {
            m_newCategoryName = "";
            DatabaseCreateCategory.target = false;
            DGUI.Properties.ResetKeyboardFocus();
        }

        private void StartCreateNewCategory()
        {
            ResetSearchDatabase();
            ResetRenameCategory();

            m_newCategoryName = "";
            DatabaseCreateCategory.target = true;
        }

        private void ResetSearchDatabase()
        {
            m_searchPattern = "";
            DGUI.Properties.ResetKeyboardFocus();
        }

        private void StartSearchDatabase()
        {
            ResetCreateNewCategory();
            ResetRenameCategory();

            m_searchPattern = "";
        }

        private void ResetRenameCategory()
        {
            m_viewContainingCategoryThatIsBeingRenamed = View.General;
            m_categoryBeingRenamed = "";
            m_newCategoryName = "";
            DGUI.Properties.ResetKeyboardFocus();
        }

        private void StartRenameCategory(View view, string categoryBeingRenamed)
        {
            ResetCreateNewCategory();
            ResetSearchDatabase();

            m_viewContainingCategoryThatIsBeingRenamed = view;
            m_categoryBeingRenamed = categoryBeingRenamed;
            m_newCategoryName = categoryBeingRenamed;
        }
    }
}