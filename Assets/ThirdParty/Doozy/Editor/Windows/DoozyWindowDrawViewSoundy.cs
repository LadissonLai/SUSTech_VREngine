// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Doozy.Editor.Soundy;
using Doozy.Engine.Extensions;
using Doozy.Engine.Soundy;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Audio;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private static void StopAllSounds() { SoundyAudioPlayer.StopAllPlayers(); }

        private static SoundyDatabase SoundyDatabase { get { return SoundySettings.Database; } }
        private readonly List<SoundDatabase> m_soundDatabasesWithSearchedSoundNames = new List<SoundDatabase>();

        private string m_newSoundDatabaseName = "";
        private SoundDatabase m_soundDatabaseBeingRenamed;
        private string m_soundySearchPattern = "";
        private bool m_soundySearchAudioClipNames;

        private AnimBool m_soundyDatabaseExpanded, m_soundySettingsExpanded;

        private void InitViewSoundy()
        {
            if (m_soundyDatabaseExpanded == null) m_soundyDatabaseExpanded = new AnimBool(true, Repaint);
            if (m_soundySettingsExpanded == null) m_soundySettingsExpanded = new AnimBool(false, Repaint);
            m_soundyDatabaseExpanded.value = true;
            m_soundySettingsExpanded.value = false;
            SoundyDatabase.InitializeSoundDatabases();
        }

        private void DrawViewSoundy()
        {
            if (CurrentView != View.Soundy) return;

            DrawSoundyWindowTabButtonsForDatabaseAndSettings();
            DrawDynamicViewVerticalSpace();

            if (DGUI.FadeOut.Begin(m_soundySettingsExpanded, false)) DrawViewSoundySettings();
            DGUI.FadeOut.End(m_soundySettingsExpanded, false);

            if (DGUI.FadeOut.Begin(m_soundyDatabaseExpanded, false)) DrawViewSoundyDatabase();
            DGUI.FadeOut.End(m_soundyDatabaseExpanded, false);

            DrawDynamicViewVerticalSpace(2);
        }

        private void DrawViewSoundySettings()
        {
            DrawDynamicViewVerticalSpace(2);
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaCog, UILabels.Settings, UILabels.SoundySettings, CurrentViewColorName);
            DrawDynamicViewVerticalSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    DrawSoundyMinimumNumberOfControllers();
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawSoundyAutoKillIdleControllers();
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawSoundyControllerIdleKillDuration(SoundySettings.Instance.AutoKillIdleControllers);
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawSoundyIdleCheckInterval(SoundySettings.Instance.AutoKillIdleControllers);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSoundyWindowTabButtonsForDatabaseAndSettings()
        {
            GUILayout.BeginHorizontal();
            {
                DrawDefaultViewHorizontalSpacing();
                GUILayout.FlexibleSpace();

                if (WindowTabButton(Styles.GetStyle(Styles.StyleName.IconSoundyDatabase), UILabels.SoundyDatabase, DGUI.Colors.SoundyColorName, m_soundyDatabaseExpanded.target))
                {
                    m_soundyDatabaseExpanded.target = true;
                    m_soundySettingsExpanded.target = false;
                }

                DrawDefaultViewHorizontalSpacing(2);

                if (WindowTabButton(Styles.GetStyle(Styles.StyleName.IconSettings), UILabels.Settings, DGUI.Colors.SoundyColorName, m_soundySettingsExpanded.target))
                {
                    m_soundySettingsExpanded.target = true;
                    m_soundyDatabaseExpanded.target = false;
                }

                GUILayout.FlexibleSpace();
                DrawDefaultViewHorizontalSpacing();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSoundyMinimumNumberOfControllers()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.MinimumNumberOfControllers, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                int controllersCount = SoundySettings.Instance.MinimumNumberOfControllers;
                EditorGUI.BeginChangeCheck();
                controllersCount = EditorGUILayout.IntSlider(controllersCount, SoundySettings.MINIMUM_NUMBER_OF_CONTROLLERS_MIN, SoundySettings.MINIMUM_NUMBER_OF_CONTROLLERS_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    SoundySettings.Instance.MinimumNumberOfControllers = controllersCount;
                    SoundySettings.Instance.SetDirty(false);
                }

                GUILayout.Space(DGUI.Properties.Space(2));

                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                       UILabels.Reset,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       NormalRowHeight,
                                                       false))
                {
                    SoundySettings.Instance.MinimumNumberOfControllers = SoundySettings.MINIMUM_NUMBER_OF_CONTROLLERS_DEFAULT_VALUE;
                    SoundySettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.MinimumNumberOfControllersDescription);
        }

        private void DrawSoundyAutoKillIdleControllers()
        {
            GUILayout.BeginHorizontal();
            {
                bool autoKill = SoundySettings.Instance.AutoKillIdleControllers;
                EditorGUI.BeginChangeCheck();
                autoKill = DGUI.Toggle.Switch.Draw(autoKill, UILabels.AutoKillIdleControllers, CurrentViewColorName, false, false, false, NormalRowHeight);
                if (EditorGUI.EndChangeCheck())
                {
                    SoundySettings.Instance.AutoKillIdleControllers = autoKill;
                    SoundySettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.AutoKillIdleControllersDescription);
        }

        private void DrawSoundyControllerIdleKillDuration(bool enabled)
        {
            GUI.enabled = enabled;
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.ControllerIdleKillDuration, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                float killDuration = SoundySettings.Instance.ControllerIdleKillDuration;
                EditorGUI.BeginChangeCheck();
                killDuration = EditorGUILayout.Slider(killDuration, SoundySettings.CONTROLLER_IDLE_KILL_DURATION_MIN, SoundySettings.CONTROLLER_IDLE_KILL_DURATION_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    SoundySettings.Instance.ControllerIdleKillDuration = (float) Math.Round(killDuration, 1);
                    SoundySettings.Instance.SetDirty(false);
                }

                GUILayout.Space(DGUI.Properties.Space(2));

                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                       UILabels.Reset,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       NormalRowHeight,
                                                       false))
                {
                    SoundySettings.Instance.ControllerIdleKillDuration = SoundySettings.CONTROLLER_IDLE_KILL_DURATION_DEFAULT_VALUE;
                    SoundySettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.ControllerIdleKillDurationDescription);
            GUI.enabled = true;
        }

        private void DrawSoundyIdleCheckInterval(bool enabled)
        {
            GUI.enabled = enabled;
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.IdleCheckInterval, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                float checkInterval = SoundySettings.Instance.IdleCheckInterval;
                EditorGUI.BeginChangeCheck();
                checkInterval = EditorGUILayout.Slider(checkInterval, SoundySettings.IDLE_CHECK_INTERVAL_MIN, SoundySettings.IDLE_CHECK_INTERVAL_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    SoundySettings.Instance.IdleCheckInterval = (float) Math.Round(checkInterval, 1);
                    SoundySettings.Instance.SetDirty(false);
                }

                GUILayout.Space(DGUI.Properties.Space(2));

                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                       UILabels.Reset,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       NormalRowHeight,
                                                       false))
                {
                    SoundySettings.Instance.IdleCheckInterval = SoundySettings.IDLE_CHECK_INTERVAL_DEFAULT_VALUE;
                    SoundySettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.IdleCheckIntervalDescription);
            GUI.enabled = true;
        }

        private void DrawViewSoundyTopButtons(AnimBool createSoundDatabase)
        {
            GUILayout.BeginHorizontal();
            {
                DrawDefaultViewHorizontalSpacing();
                GUILayout.FlexibleSpace();

                if (ButtonNew(createSoundDatabase.target, UILabels.NewSoundDatabase))
                {
                    if (createSoundDatabase.target)
                    {
                        ResetCreateNewSoundDatabase();
                    }
                    else
                    {
                        CollapseAllSoundyDatabases();
                        ResetRenameSoundDatabase();
                        ResetSearchSoundyDatabase();
                        StartCreateNewSoundDatabase();
                    }
                }

                DrawDefaultViewHorizontalSpacing();

                if (ButtonRefreshDatabase())
                {
                    ResetCreateNewSoundDatabase();
                    ResetRenameSoundDatabase();
                    ResetSearchSoundyDatabase();
                    SoundyDatabase.RefreshDatabase(true, true);
                }

                DrawDefaultViewHorizontalSpacing();

                if (ButtonSaveDatabase())
                {
                    Save();
                }
                
                DrawDefaultViewHorizontalSpacing();
                
                if (ButtonSearchFor(UILabels.SoundDatabases))
                {
                    ResetCreateNewSoundDatabase();
                    ResetRenameSoundDatabase();
                    ResetSearchSoundyDatabase();
                    SoundyDatabase.SearchForUnregisteredDatabases(true);
                }

                GUILayout.FlexibleSpace();
                DrawDefaultViewHorizontalSpacing();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawViewSoundyDatabase()
        {
            Event current = Event.current;
//            AnimBool viewExpanded = GetAnimBool(View.Soundy.ToString());
            AnimBool createSoundDatabase = GetAnimBool(NEW_SOUND_DATABASE);
            AnimBool renameSoundDatabase = GetAnimBool(RENAME_SOUND_DATABASE);
            renameSoundDatabase.target = m_soundDatabaseBeingRenamed != null;
            AnimBool searchEnabled = GetAnimBool(SEARCH);
            bool newSoundDatabaseCreated = false;

            DrawViewSoundyTopButtons(createSoundDatabase);

            #region CREATE NEW SOUND DATABASE - TEXT FIELD

            if (DGUI.FadeOut.Begin(createSoundDatabase, false))
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(DGUI.Properties.Space(2) * createSoundDatabase.faded);
                    GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)));
                    {
                        GUILayout.Space((FullViewWidth - NewCategoryNameTextFieldWidth - 32) / 2);

                        GUI.color = DGUI.Colors.GetDColor(ColorName.Green).Normal.WithAlpha(GUI.color.a);
                        GUI.SetNextControlName(NEW_SOUND_DATABASE);
                        m_newSoundDatabaseName = EditorGUILayout.TextField(GUIContent.none, m_newSoundDatabaseName, GUILayout.Width(NewCategoryNameTextFieldWidth));
                        GUI.color = InitialGUIColor;


                        if (DGUI.Button.IconButton.Ok() ||
                            current.type == EventType.KeyUp && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter))
                        {
                            if (SoundyDatabase.CreateSoundDatabase(m_newSoundDatabaseName, true, true))
                            {
                                newSoundDatabaseCreated = true;
                                GetSoundDatabaseAnimBool(m_newSoundDatabaseName).value = true;
                                ResetCreateNewSoundDatabase();
                            }
                            else
                            {
                                Instance.Focus();
                            }
                        }

                        if (DGUI.Button.IconButton.Cancel() ||
                            current.keyCode == KeyCode.Escape && current.type == EventType.KeyUp)
                            ResetCreateNewSoundDatabase();

                        if (!newSoundDatabaseCreated && createSoundDatabase.target) EditorGUI.FocusTextInControl(NEW_SOUND_DATABASE);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUI.color = InitialGUIColor;
            }

            DGUI.FadeOut.End(createSoundDatabase);

            #endregion

            DrawDynamicViewVerticalSpace();

            #region SEARCH ROW

            searchEnabled.target = m_soundySearchPattern.Length > 0; //search has been enabled if the dev typed a least on character in the search field
            GUILayout.BeginHorizontal(GUILayout.Height(SearchRowHeight));
            {
                DrawViewSearchHorizontalPadding();
                DrawSearchIconAndText();

                DGUI.Properties.SetNextControlName(SEARCH);
                m_soundySearchPattern = EditorGUILayout.TextField(GUIContent.none, m_soundySearchPattern, GUILayout.ExpandWidth(true)); //search text field

                if (!searchEnabled.target && m_soundySearchPattern.Length > 0) //search was just initiated -> reset the database that contain sound names that match the search pattern
                    m_soundDatabasesWithSearchedSoundNames.Clear();

                if (!EditorGUIUtility.editingTextField && current.keyCode == KeyCode.S && current.type == EventType.KeyUp)
                {
                    StartSearchSoundyDatabase();
                    EditorGUI.FocusTextInControl(SEARCH);
                }

                GUI.color = InitialGUIColor;

                if (searchEnabled.faded > 0.1f)
                {
                    DGUI.AlphaGroup.Begin(searchEnabled);
                    if (ButtonClearSearch(searchEnabled))
                    {
                        ResetSearchSoundyDatabase();
                        GUILayout.EndHorizontal();
                        return;
                    }

                    DGUI.AlphaGroup.End();

                    DrawViewSearchHorizontalPadding();
                }

                m_soundySearchAudioClipNames = DGUI.Toggle.Switch.Draw(m_soundySearchAudioClipNames, UILabels.IncludeAudioClipNamesInSearch, CurrentViewColorName, false, true, false);

                DrawViewSearchHorizontalPadding();
            }
            GUILayout.EndHorizontal();

            #endregion

            DrawDynamicViewVerticalSpace();

            if (newSoundDatabaseCreated) return;
            searchEnabled.target = m_soundySearchPattern.Length > 0; //search has been enabled if the dev typed a least on character in the search field

            DrawViewSoundySoundDatabases(searchEnabled);

            GUI.color = InitialGUIColor;
        }

        private void DrawViewSoundySoundDatabases(AnimBool searchEnabled)
        {
            bool foundNullReference = false;

            foreach (SoundDatabase soundDatabase in SoundyDatabase.SoundDatabases)
            {
                if (soundDatabase == null)
                {
                    foundNullReference = true;
                    break;
                }

                AnimBool soundDatabaseAnimBool = GetSoundDatabaseAnimBool(soundDatabase);
                AnimBool renameSoundDatabaseAnimBool = GetAnimBool(RENAME_SOUND_DATABASE);
                bool isGeneralSoundDatabase = soundDatabase.DatabaseName.Equals(SoundyManager.GENERAL);
                bool soundDatabaseContainsSearchedSound = false;
                bool renamingThisDatabase = !searchEnabled.target && m_soundDatabaseBeingRenamed == soundDatabase;
                bool databaseHasIssues = soundDatabase.HasSoundsWithMissingAudioClips; //database issues check
                ColorName viewColorName = CurrentViewColorName;
                ColorName mixerColorName = soundDatabase.OutputAudioMixerGroup != null ? viewColorName : DGUI.Colors.DisabledTextColorName;

                GUILayout.BeginHorizontal();
                {
                    DrawViewHorizontalPadding();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            bool enabledState = GUI.enabled;
                            GUI.enabled = !(renameSoundDatabaseAnimBool.target && renamingThisDatabase) && !searchEnabled.target; //do not allow the dev to use this button if THIS database is being renamed or a search is happening
                            {
                                GUILayout.BeginVertical(GUILayout.Height(BarHeight));
                                {
                                    if (DGUI.Bar.Draw(soundDatabase.DatabaseName, BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, viewColorName, soundDatabaseAnimBool)) //database BAR
                                        if (Selection.activeObject != null && Selection.activeObject is SoundGroupData)
                                            Selection.activeObject = null;

                                    GUILayout.Space(-BarHeight - DGUI.Properties.Space());

                                    if (soundDatabase.OutputAudioMixerGroup == null) DGUI.Colors.SetDisabledGUIColorAlpha();

                                    float barIconSize = BarHeight * 0.6f;

                                    SoundDatabase database = soundDatabase;
                                    DGUI.Line.Draw(false,
                                                   () =>
                                                   {
                                                       GUILayout.FlexibleSpace();
                                                       DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconAudioMixerGroup), barIconSize, BarHeight, mixerColorName); //mixer icon
                                                       if (database.OutputAudioMixerGroup != null)
                                                       {
                                                           GUILayout.Space(DGUI.Properties.Space(2));
                                                           DGUI.Label.Draw(database.OutputAudioMixerGroup.name + " (" + database.OutputAudioMixerGroup.audioMixer.name + ")", Size.S, mixerColorName, BarHeight); //mixer label
                                                       }

                                                       if (databaseHasIssues)
                                                       {
                                                           GUILayout.Space(DGUI.Properties.Space(4));
                                                           DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconError), barIconSize, BarHeight, Color.red);
                                                       }

                                                       GUILayout.Space(DGUI.Properties.Space(2));
                                                   });
                                    GUI.color = InitialGUIColor;
                                }
                                GUILayout.EndVertical();
                            }
                            GUI.enabled = enabledState;

                            if (!isGeneralSoundDatabase)
                                if (!searchEnabled.target && soundDatabaseAnimBool.faded > 0.05f && renameSoundDatabaseAnimBool.faded < 0.5f)
                                {
                                    DGUI.AlphaGroup.Begin(soundDatabaseAnimBool.faded * (1 - renameSoundDatabaseAnimBool.faded));
                                    GUILayout.Space(DGUI.Properties.Space() * soundDatabaseAnimBool.faded * (1 - renameSoundDatabaseAnimBool.faded));
                                    if (DGUI.Button.Draw(DGUI.Properties.Labels.Rename,
                                                         Size.S,
                                                         TextAlign.Center,
                                                         viewColorName,
                                                         true,
                                                         BarHeight,
                                                         80 * soundDatabaseAnimBool.faded * (1 - renameSoundDatabaseAnimBool.faded)) ||
                                        renamingThisDatabase && Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp) //rename database button
                                    {
                                        if (!renamingThisDatabase &&
                                            EditorUtility.DisplayDialog(UILabels.RenameSoundDatabase + " '" + soundDatabase.DatabaseName + "'",
                                                                        UILabels.RenameSoundDatabaseDialogMessage +
                                                                        "\n\n" +
                                                                        UILabels.YouAreResponsibleToUpdateYourCode,
                                                                        UILabels.Continue,
                                                                        UILabels.Cancel))
                                        {
                                            StartRenameSoundDatabase(soundDatabase);
                                            Instance.Focus();
                                        }
                                        else
                                        {
                                            ResetRenameSoundDatabase();
                                        }
                                    }

                                    if (DGUI.Button.IconButton.Cancel(BarHeight)) //delete database button
                                    {
                                        SoundyDatabase.DeleteDatabase(soundDatabase);
                                        break;
                                    }

                                    DGUI.AlphaGroup.End();
                                }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(DGUI.Properties.Space());

                        if (DGUI.FadeOut.Begin(searchEnabled.target ? 1 : soundDatabaseAnimBool.faded))
                        {
                            GUILayout.BeginVertical();

                            //RENAME DATABASE
                            if (renamingThisDatabase)
                            {
                                if (DGUI.FadeOut.Begin(renameSoundDatabaseAnimBool.faded, false))
                                {
                                    GUILayout.BeginVertical();
                                    {
                                        GUILayout.Space(DGUI.Properties.Space() * renameSoundDatabaseAnimBool.faded);
                                        GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)));
                                        {
                                            DrawDefaultViewHorizontalSpacing();

                                            GUI.color = DGUI.Colors.BackgroundColor(RENAME_COLOR_NAME);

                                            DGUI.Label.Draw(UILabels.RenameTo, Size.S, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)); //rename to label

                                            GUI.SetNextControlName(RENAME);
                                            m_newSoundDatabaseName = EditorGUILayout.TextField(GUIContent.none, m_newSoundDatabaseName); //rename to field
                                            GUI.color = InitialGUIColor;

                                            if (DGUI.Button.IconButton.Ok() ||
                                                Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp) //rename OK button
                                            {
                                                Instance.Focus();

                                                if (SoundyDatabase.RenameSoundDatabase(soundDatabase, m_newSoundDatabaseName))
                                                {
                                                    GetSoundDatabaseAnimBool(m_newSoundDatabaseName).value = true;
                                                    ResetRenameSoundDatabase();
                                                    break;
                                                }
                                            }

                                            if (DGUI.Button.IconButton.Cancel() ||
                                                Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp) //rename CANCEL button
                                                ResetRenameSoundDatabase();

                                            EditorGUI.FocusTextInControl(RENAME);

                                            DrawDefaultViewHorizontalSpacing();
                                        }
                                        GUILayout.EndHorizontal();

                                        GUILayout.Space(DGUI.Properties.Space(4) * renameSoundDatabaseAnimBool.faded);
                                    }
                                    GUILayout.EndVertical();
                                    GUI.color = InitialGUIColor;
                                }

                                DGUI.FadeOut.End(renameSoundDatabaseAnimBool, false);
                            }

                            GUILayout.Space(DGUI.Properties.Space() * soundDatabaseAnimBool.faded);

                            if (!searchEnabled.target)
                            {
                                float barIconSize = DGUI.Properties.SingleLineHeight * 0.8f;
                                float barIconRowHeight = DGUI.Properties.SingleLineHeight - 1;
                                GUILayout.BeginHorizontal();
                                {
                                    SoundDatabase database = soundDatabase;
                                    DGUI.Line.Draw(true, mixerColorName,
                                                   () =>
                                                   {
                                                       if (database.OutputAudioMixerGroup == null) DGUI.Colors.SetDisabledGUIColorAlpha();
                                                       DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconAudioMixerGroup), barIconSize, barIconRowHeight, mixerColorName); //mixer icon
                                                       GUILayout.Space(DGUI.Properties.Space());
                                                       DGUI.Label.Draw(UILabels.OutputMixerGroup, Size.S, mixerColorName, DGUI.Properties.SingleLineHeight); //mixer label
                                                       GUI.color = InitialGUIColor;
                                                       AudioMixerGroup outputAudioMixerGroup = database.OutputAudioMixerGroup;
                                                       GUI.color = DGUI.Colors.GetDColor(mixerColorName).Light.WithAlpha(GUI.color.a);
                                                       GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                                       {
                                                           GUILayout.Space(0f);
                                                           EditorGUI.BeginChangeCheck();
                                                           outputAudioMixerGroup = (AudioMixerGroup) EditorGUILayout.ObjectField(GUIContent.none, outputAudioMixerGroup, typeof(AudioMixerGroup), false); //mixer field
                                                           if (EditorGUI.EndChangeCheck())
                                                           {
                                                               Undo.RecordObject(database, "Update AudioMixerGroup");
                                                               database.OutputAudioMixerGroup = outputAudioMixerGroup;
                                                               database.SetDirty(true);
                                                           }
                                                       }
                                                       GUILayout.EndVertical();
                                                       GUI.color = InitialGUIColor;
                                                   });
                                    GUILayout.Space(DGUI.Button.IconButton.Width + DGUI.Properties.Space(4));
                                }
                                GUILayout.EndHorizontal();
                            }

                            GUILayout.Space(DGUI.Properties.Space(2) * soundDatabaseAnimBool.faded);

                            float lineHeight = DGUI.Properties.SingleLineHeight;
                            float iconSize = lineHeight * 0.8f;

                            GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
                            Rect rect = GUILayoutUtility.GetLastRect();
                            float x;
                            float y = rect.y;

                            float totalWidth = rect.width;
                            float buttonAndPlayerWidth = rect.width - DGUI.Properties.Space(6) - iconSize - DGUI.Button.IconButton.Width;
                            float buttonWidth = buttonAndPlayerWidth * 0.4f;
                            float playerWidth = buttonAndPlayerWidth - buttonWidth;

                            foreach (SoundGroupData soundGroupData in soundDatabase.Database)
                            {
                                if (searchEnabled.target)
                                {
                                    if (!Regex.IsMatch(soundGroupData.SoundName, m_soundySearchPattern, RegexOptions.IgnoreCase)) //regex search check
                                    {
                                        if (!m_soundySearchAudioClipNames)
                                            continue;

                                        if (soundGroupData.Sounds == null || soundGroupData.Sounds.Count == 0)
                                            continue;

                                        bool foundAudioClipMatch = false;
                                        foreach (AudioData audioData in soundGroupData.Sounds)
                                        {
                                            if (audioData == null || audioData.AudioClip == null) continue;
                                            if (!Regex.IsMatch(audioData.AudioClip.name, m_soundySearchPattern, RegexOptions.IgnoreCase)) continue;
                                            foundAudioClipMatch = true;
                                            break;
                                        }

                                        if (!foundAudioClipMatch) continue;
                                    }

                                    soundDatabaseContainsSearchedSound = true;
                                }


                                //draw check - if the item was scrolled out of view (up or down) -> do not draw it
                                bool drawItem = y > m_viewScrollPosition.y + 16 || y < m_viewScrollPosition.y + FullViewRect.height - 16;
                                if (!drawItem) continue;

                                bool removedEntry = false;

                                GUILayout.Space(DGUI.Properties.Space() * soundDatabaseAnimBool.faded);
                                GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(lineHeight));

                                x = rect.x;
                                y += DGUI.Properties.Space() * soundDatabaseAnimBool.faded;


                                bool noSound = soundGroupData.SoundName.Equals(SoundyManager.NO_SOUND); //is no sound check
                                bool isSelected = Selection.activeObject == soundGroupData;             //selected check
                                bool hasIssues = !noSound && soundGroupData.HasMissingAudioClips;       //has issues check
                                bool enabledState = GUI.enabled;
                                GUI.enabled = !noSound;
                                {
                                    SoundyAudioPlayer.Player player = SoundyAudioPlayer.GetPlayer(soundGroupData, soundDatabase.OutputAudioMixerGroup); //player reference

                                    ColorName buttonColorName =
                                        isSelected
                                            ? hasIssues
                                                  ? ColorName.Red
                                                  : CurrentViewColorName
                                            : DGUI.Colors.DisabledBackgroundColorName;

                                    ColorName buttonTextColorName =
                                        hasIssues
                                            ? ColorName.Red
                                            : CurrentViewColorName;

//                                    if (i % 2 == 0)
                                    if (player.IsPlaying)
                                    {
                                        var backgroundRect = new Rect(x - DGUI.Properties.Space(2), y - DGUI.Properties.Space(), totalWidth + DGUI.Properties.Space(4), lineHeight + DGUI.Properties.Space(2));
                                        GUI.color = DGUI.Colors.BackgroundColor(buttonColorName).WithAlpha(0.6f);
                                        GUI.Label(backgroundRect, GUIContent.none, Styles.GetStyle(Styles.StyleName.BackgroundRound));
                                        GUI.color = InitialGUIColor;
                                    }

                                    var iconRect = new Rect(x, y + (lineHeight - iconSize) / 2, iconSize, iconSize);
                                    if (!player.IsPlaying) GUI.color = GUI.color.WithAlpha(DGUI.Utility.IsProSkin ? 0.4f : 0.6f);
                                    DGUI.Icon.Draw(iconRect, Styles.GetStyle(noSound || hasIssues ? Styles.StyleName.IconMuteSound : Styles.StyleName.IconSound),
                                                   DGUI.Colors.TextColor(hasIssues ? ColorName.Red : buttonTextColorName)); //speaker icon
                                    GUI.color = InitialGUIColor;
                                    x += iconSize;
                                    x += DGUI.Properties.Space(2);
                                    var buttonRect = new Rect(x, y - DGUI.Properties.Space(), buttonWidth, lineHeight + DGUI.Properties.Space(2));
                                    if (DGUI.Button.Draw(buttonRect, soundGroupData.SoundName, Size.S, TextAlign.Left, buttonColorName, buttonTextColorName, isSelected)) //select button
                                        Selection.activeObject = soundGroupData;

                                    if (hasIssues)
                                    {
                                        var errorIconRect = new Rect(buttonRect.xMax - iconSize - DGUI.Properties.Space(2), buttonRect.y + (buttonRect.height - iconSize) / 2, iconSize, iconSize);
                                        DGUI.Icon.Draw(errorIconRect, Styles.GetStyle(Styles.StyleName.IconError), Color.red); //issues icon
                                    }

                                    x += buttonWidth;
                                    x += DGUI.Properties.Space(2);
                                    var playerRect = new Rect(x, y, playerWidth, lineHeight);
                                    player.DrawPlayer(playerRect, buttonTextColorName);
                                    x += playerWidth;
                                    x += DGUI.Properties.Space(2);
                                    var deleteButtonRect = new Rect(x, y, DGUI.Button.IconButton.Width, DGUI.Button.IconButton.Height);
                                    if (DGUI.Button.IconButton.Minus(deleteButtonRect))
                                    {
                                        bool wasSelected = Selection.activeObject == soundGroupData;
                                        if (soundDatabase.Remove(soundGroupData, true))
                                        {
                                            if (wasSelected)
                                                Selection.activeObject = null;
                                            soundDatabase.SetDirty(true);
                                            if (player.IsPlaying) player.Stop();
                                            removedEntry = true;
                                        }
                                    }

                                    y += lineHeight;
                                    y += DGUI.Properties.Space(2) * soundDatabaseAnimBool.faded;
                                }
                                GUI.enabled = enabledState;

                                GUILayout.Space(DGUI.Properties.Space() * soundDatabaseAnimBool.faded);
                                if (removedEntry) break;
                            }

                            GUILayout.Space(DGUI.Properties.Space() * soundDatabaseAnimBool.faded);

                            if (!searchEnabled.target)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.FlexibleSpace();
                                    DrawSoundDatabaseDropZone(soundDatabase);
                                    GUILayout.Space(DGUI.Properties.Space());
                                    if (DGUI.Button.IconButton.Plus()) Selection.activeObject = soundDatabase.Add(SoundyManager.NEW_SOUND_GROUP, true, true);
                                }
                                GUILayout.EndHorizontal();
                            }


                            GUI.enabled = !(GetAnimBool(RENAME_SOUND_DATABASE).target && renamingThisDatabase);

                            GUILayout.EndVertical();
                        }

                        DGUI.FadeOut.End(searchEnabled.target ? 1 : soundDatabaseAnimBool.faded);

                        if (searchEnabled.target)
                        {
                            if (soundDatabaseContainsSearchedSound && !m_soundDatabasesWithSearchedSoundNames.Contains(soundDatabase))
                            {
                                m_soundDatabasesWithSearchedSoundNames.Add(soundDatabase);
                                soundDatabaseAnimBool.target = true;
                            }
                            else if (!soundDatabaseContainsSearchedSound && m_soundDatabasesWithSearchedSoundNames.Contains(soundDatabase))
                            {
                                m_soundDatabasesWithSearchedSoundNames.Remove(soundDatabase);
                                soundDatabaseAnimBool.target = false;
                            }
                        }
                    }
                    GUILayout.EndVertical();

                    DrawViewHorizontalPadding();
                }
                GUILayout.EndHorizontal();
            }

            if (foundNullReference) SoundyDatabase.RefreshDatabase(false);
        }

        private void DrawSoundDatabaseDropZone(SoundDatabase soundDatabase)
        {
            GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            var dropRect = new Rect(lastRect.x + DGUI.Properties.Space(), lastRect.y - 2, DGUI.Properties.DefaultFieldWidth * 5, DGUI.Properties.SingleLineHeight); //calculate rect

            bool containsMouse = dropRect.Contains(Event.current.mousePosition);
            if (containsMouse)
            {
                DGUI.Colors.SetNormalGUIColorAlpha();

                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        bool containsAudioClip = DragAndDrop.objectReferences.OfType<AudioClip>().Any();
                        DragAndDrop.visualMode = containsAudioClip ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                        Event.current.Use();
                        Repaint();
                        break;
                    case EventType.DragPerform:
                        IEnumerable<AudioClip> audioClips = DragAndDrop.objectReferences.OfType<AudioClip>();
                        bool undoRecorded = false;
                        foreach (AudioClip audioClip in audioClips)
                        {
                            if (!undoRecorded)
                            {
                                DoozyUtils.UndoRecordObject(soundDatabase, UILabels.AddSounds);
                                undoRecorded = true;
                            }

                            SoundGroupData soundGroupData = soundDatabase.Add(audioClip.name, false, false);
                            soundGroupData.Sounds.Add(new AudioData(audioClip));
                            soundGroupData.SetDirty(false);
                            soundDatabase.SetDirty(false);
                            m_needsSave = true;
                        }

                        Event.current.Use();
                        Repaint();
                        break;
                }
            }

            DGUI.Doozy.DrawDropZone(dropRect, containsMouse);
            GUILayout.Space(dropRect.width);
        }

        private AnimBool GetSoundDatabaseAnimBool(SoundDatabase soundDatabase) { return GetSoundDatabaseAnimBool(soundDatabase.DatabaseName); }
        public AnimBool GetSoundDatabaseAnimBool(string databaseName) { return GetAnimBool(SoundyManager.SOUNDY + databaseName); }

        private void CollapseAllSoundyDatabases()
        {
            foreach (SoundDatabase soundDatabase in SoundyDatabase.SoundDatabases)
                GetSoundDatabaseAnimBool(soundDatabase).target = false;
        }

        private void ResetCreateNewSoundDatabase()
        {
            m_newSoundDatabaseName = "";
            GetAnimBool(NEW_SOUND_DATABASE).target = false;
            DGUI.Properties.ResetKeyboardFocus();
        }

        private void StartCreateNewSoundDatabase()
        {
            ResetSearchSoundyDatabase();
            ResetRenameSoundDatabase();
            m_newSoundDatabaseName = "";
            GetAnimBool(NEW_SOUND_DATABASE).target = true;
        }

        private void ResetRenameSoundDatabase()
        {
            m_soundDatabaseBeingRenamed = null;
            m_newSoundDatabaseName = "";
            DGUI.Properties.ResetKeyboardFocus();
        }

        private void StartRenameSoundDatabase(SoundDatabase soundDatabaseBeingRenamed)
        {
            ResetCreateNewSoundDatabase();
            ResetSearchSoundyDatabase();
            m_soundDatabaseBeingRenamed = soundDatabaseBeingRenamed;
            m_newSoundDatabaseName = soundDatabaseBeingRenamed.DatabaseName;
        }

        private void ResetSearchSoundyDatabase()
        {
            m_soundySearchPattern = "";
            DGUI.Properties.ResetKeyboardFocus();
        }

        private void StartSearchSoundyDatabase()
        {
            ResetCreateNewSoundDatabase();
            ResetRenameSoundDatabase();
            m_soundySearchPattern = "";
        }
    }
}