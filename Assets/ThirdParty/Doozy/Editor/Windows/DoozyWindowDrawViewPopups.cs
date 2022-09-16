// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Settings;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private void InitViewPopups() { m_popupsDatabaseIsEmptyInfoMessage = new InfoMessage(InfoMessage.MessageType.Info, UILabels.DatabaseIsEmpty, false, Instance.Repaint); }

        private void DrawViewPopups()
        {
            if (CurrentView != View.Popups) return;
            DrawPopupDatabase(UIPopupSettings.Database);

            DrawDynamicViewVerticalSpace(2);
        }

        private InfoMessage m_popupsDatabaseIsEmptyInfoMessage;

        private void DrawPopupDatabase(UIPopupDatabase database)
        {
            Event current = Event.current;
            DatabaseRenameCategory.target = CurrentView != View.General && m_categoryBeingRenamed != string.Empty;
            bool newCategoryCreated = false;
            ColorName viewColorName = CurrentViewColorName;

            GUILayout.BeginHorizontal();
            {
                DrawDefaultViewHorizontalSpacing();
                GUILayout.FlexibleSpace();

                if (ButtonNew(DatabaseCreateCategory.target, UILabels.NewPopup))
                {
                    if (DatabaseCreateCategory.target)
                    {
                        ResetCreateNewCategory();
                    }
                    else
                    {
                        ResetRenameCategory();
                        ResetSearchDatabase();
                        StartCreateNewCategory();
                    }
                }

                DrawDefaultViewHorizontalSpacing();

                if (ButtonSortDatabase())
                {
                    ResetCreateNewCategory();
                    ResetSearchDatabase();
                    ResetRenameCategory();
                    database.Sort(false);
                    database.UpdateListOfPopupNames();
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

                if (ButtonSearchFor(UILabels.SearchForUIPopupLinks))
                {
                    ResetCreateNewCategory();
                    ResetSearchDatabase();
                    ResetRenameCategory();
                    database.SearchForUnregisteredLinks(true);
                }

                GUILayout.FlexibleSpace();
                DrawDefaultViewHorizontalSpacing();
            }
            GUILayout.EndHorizontal();

            #region CREATE NEW POPUP - TEXT FIELD

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
                        if (database.CreateUIPopupLink(m_newCategoryName, null, true, false))
                        {
                            newCategoryCreated = true;
                            m_needsSave = true;
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

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            m_popupsDatabaseIsEmptyInfoMessage.DrawMessageOnly(database.IsEmpty && !DatabaseCreateCategory.target);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (database.IsEmpty) return;

            if (newCategoryCreated) return;

            float dynamicSpacing = DGUI.Properties.Space(2) * GetAnimBool(CurrentView.ToString()).faded;

            bool foundNullLink = false;

            #region Draw Links

            foreach (UIPopupLink link in database.Popups)
            {
                if (link == null)
                {
                    foundNullLink = true;
                    continue;
                }

                bool popupNameInvalid = string.IsNullOrEmpty(link.PopupName.Trim());
                bool prefabInvalid = link.Prefab == null;
                bool hasIssues = popupNameInvalid || prefabInvalid;
                float lineHeight = DGUI.Properties.SingleLineHeight;

                //draw popup reference
                float backgroundHeight = lineHeight + DGUI.Properties.Space(2);
                GUILayout.BeginVertical(GUILayout.Height(backgroundHeight));
                {
                    DGUI.Background.Draw(DGUI.Colors.GetBackgroundColorName(!hasIssues, viewColorName), backgroundHeight);
                    GUILayout.Space(-backgroundHeight);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(dynamicSpacing);

                        //PopupName Label
                        DGUI.Label.Draw(UILabels.PopupName, Size.M, popupNameInvalid ? ColorName.Red : viewColorName, lineHeight);
                        //PopupName Value
                        string popupName = link.PopupName;
                        GUI.color = DGUI.Colors.PropertyColor(popupNameInvalid ? ColorName.Red : viewColorName);
                        EditorGUI.BeginChangeCheck();
                        popupName = EditorGUILayout.DelayedTextField(GUIContent.none, popupName);
                        if (EditorGUI.EndChangeCheck() && popupName != link.PopupName)
                        {
                            popupName = popupName.Trim();
                            if (string.IsNullOrEmpty(popupName))
                            {
                                EditorUtility.DisplayDialog(UILabels.Rename,
                                                            UILabels.CannotAddEmptyEntry + "\n\n" + UILabels.PleaseEnterNewName,
                                                            UILabels.Ok);
                            }
                            else if (database.Contains(popupName))
                            {
                                EditorUtility.DisplayDialog(UILabels.Rename,
                                                            UILabels.AnotherEntryExists + "\n\n" + UILabels.PleaseEnterNewName,
                                                            UILabels.Ok);
                            }
                            else
                            {
                                database.UndoRecord(UILabels.UpdatePopupName);
                                link.PopupName = popupName;
                                link.UpdateAssetName(false);
                                link.SetDirty(false);
                                database.SetDirty(true);
                                m_needsSave = true;
                            }
                        }

                        GUI.color = InitialGUIColor;

                        GUILayout.Space(dynamicSpacing * 2);

                        //PopupPrefab Label
                        DGUI.Label.Draw(UILabels.PopupPrefab, Size.M, prefabInvalid ? ColorName.Red : viewColorName, lineHeight);
                        //PopupPrefab Value
                        GameObject prefab = link.Prefab;
                        GUI.color = DGUI.Colors.PropertyColor(prefabInvalid ? ColorName.Red : viewColorName);
                        EditorGUI.BeginChangeCheck();
                        prefab = (GameObject) EditorGUILayout.ObjectField(GUIContent.none, prefab, typeof(GameObject), false, GUILayout.Width(DGUI.Properties.DefaultFieldWidth * 6));
                        if (EditorGUI.EndChangeCheck())
                        {
                            database.UndoRecord(UILabels.UpdatePopupPrefab);
                            link.Prefab = prefab;
                            link.SetDirty(false);
                            database.SetDirty(true);
                            m_needsSave = true;
                        }

                        GUI.color = InitialGUIColor;

                        GUILayout.Space(dynamicSpacing);

                        if (DGUI.Button.IconButton.Minus())
                            if (database.DeletePopupLink(link))
                            {
                                GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                                m_needsSave = true;
                                break;
                            }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                //add space between entries
                GUILayout.Space(dynamicSpacing);
            }

            #endregion

            if (foundNullLink) database.RefreshDatabase(false, false);
        }
    }
}