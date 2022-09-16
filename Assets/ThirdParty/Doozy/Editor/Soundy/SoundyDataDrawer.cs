// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Settings;
using Doozy.Engine.Soundy;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Doozy.Editor.Soundy
{
    [CustomPropertyDrawer(typeof(SoundyData))]
    public class SoundyDataDrawer : BaseDrawer
    {
        private const float
            TOP_ROW_HEIGHT = 10,
            OUTPUT_LABEL_WIDTH = 86f;

        protected override ColorName DrawerColorName { get { return DGUI.Colors.SoundyColorName; } }
        private static GUIStyle DrawerIconStyle { get { return Styles.GetStyle(Styles.StyleName.IconSoundy); } }

        private static readonly Dictionary<string, SoundyData> SoundyDataDatabase = new Dictionary<string, SoundyData>();

        private static SoundGroupData GetAudioData(string databaseName, string soundName) { return SoundySettings.Database.GetAudioData(databaseName, soundName); }

        private static AudioMixerGroup GetAudioMixerGroup(string databaseName)
        {
            SoundDatabase database = SoundySettings.Database.GetSoundDatabase(databaseName);
            return database != null ? database.OutputAudioMixerGroup : null;
        }

        private void Init(SerializedProperty property)
        {
            if (Initialized.ContainsKey(property.propertyPath) && Initialized[property.propertyPath]) return;

            GUIStyle labelStyle = DGUI.Label.Style(Size.S);

            Elements.Add(Properties.Add(PropertyName.SoundSource, property), Contents.Add(UILabels.SoundSource, labelStyle));
            Elements.Add(Properties.Add(PropertyName.DatabaseName, property), Contents.Add(UILabels.DatabaseName, labelStyle));
            Elements.Add(Properties.Add(PropertyName.SoundName, property), Contents.Add(UILabels.SoundName, labelStyle));
            Elements.Add(Properties.Add(PropertyName.AudioClip, property), Contents.Add(UILabels.AudioClip, labelStyle));
            Elements.Add(Properties.Add(PropertyName.OutputAudioMixerGroup, property), Contents.Add(UILabels.OutputAudioMixerGroup, labelStyle));

            UpdateThisAudioDataPreviewReference(property);

            if (!Initialized.ContainsKey(property.propertyPath))
                Initialized.Add(property.propertyPath, true);
            else
                Initialized[property.propertyPath] = true;
        }

        private void UpdateThisAudioDataPreviewReference(SerializedProperty property)
        {
            if (SoundyDataDatabase.ContainsKey(property.propertyPath)) SoundyDataDatabase.Remove(property.propertyPath);

            var soundyData = new SoundyData
                             {
                                 DatabaseName = Properties.Get(PropertyName.DatabaseName, property).stringValue,
                                 SoundName = Properties.Get(PropertyName.SoundName, property).stringValue,
                                 AudioClip = (AudioClip) Properties.Get(PropertyName.AudioClip, property).objectReferenceValue,
                                 OutputAudioMixerGroup = (AudioMixerGroup) Properties.Get(PropertyName.OutputAudioMixerGroup, property).objectReferenceValue
                             };

            SoundyDataDatabase.Add(property.propertyPath, soundyData);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // ReSharper disable once Unity.PropertyDrawerOnGUIBase
            base.OnGUI(position, property, label);

            Init(property);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            {
                // don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                Draw(position, property);

                // set indent back to what it was
                EditorGUI.indentLevel = indent;

                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        private void Draw(Rect position, SerializedProperty property)
        {
            Color initialColor = GUI.color; //save the GUI color

            SoundyAudioPlayer.Player player = null;
            bool nameIsNoSound = false;
            bool hasSound = false;
            bool hasMissingAudioClips = false;
            GUIStyle icon;

            var soundSource = (SoundSource) Properties.Get(PropertyName.SoundSource, property).intValue;
            switch (soundSource)
            {
                case SoundSource.Soundy:
                    NumberOfLines[property.propertyPath] = 4;
                    player = SoundyAudioPlayer.GetPlayer(GetAudioData(Properties.Get(PropertyName.DatabaseName, property).stringValue, Properties.Get(PropertyName.SoundName, property).stringValue),
                                                         GetAudioMixerGroup(Properties.Get(PropertyName.DatabaseName, property).stringValue));
                    if (player != null && player.SoundGroupData != null)
                    {
                        nameIsNoSound = Properties.Get(PropertyName.SoundName, property).stringValue.Equals(SoundyManager.NO_SOUND);
                        hasSound = player.SoundGroupData.HasSound;
                        hasMissingAudioClips = player.SoundGroupData.HasMissingAudioClips;
                    }

                    icon = Styles.GetStyle(Styles.StyleName.IconSoundy);
                    break;
                case SoundSource.AudioClip:
                    NumberOfLines[property.propertyPath] = 4;
                    player = SoundyAudioPlayer.GetPlayer((AudioClip) Properties.Get(PropertyName.AudioClip, property).objectReferenceValue,
                                                         (AudioMixerGroup) Properties.Get(PropertyName.OutputAudioMixerGroup, property).objectReferenceValue);
                    hasSound = player != null && player.AudioClip != null;
                    icon = Styles.GetStyle(Styles.StyleName.IconSound);
                    break;
                case SoundSource.MasterAudio:
                    NumberOfLines[property.propertyPath] = 2;
                    hasSound = !string.IsNullOrEmpty(Properties.Get(PropertyName.SoundName, property).stringValue);
                    icon = Styles.GetStyle(DoozySettings.Instance.UseMasterAudio ? Styles.StyleName.IconMasterAudio : Styles.StyleName.IconError);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            ColorName backgroundColorName = hasSound ? DrawerColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName textColorName = hasSound ? DrawerColorName : DGUI.Colors.DisabledTextColorName;

            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], backgroundColorName); //calculate draw rect and draw background
            drawRect = InsertDrawerIcon(drawRect, property, soundSource != SoundSource.MasterAudio ? backgroundColorName : DoozySettings.Instance.UseMasterAudio ? ColorName.White : ColorName.Red, icon);
            drawRect.y += DGUI.Properties.Space();

            switch (soundSource)
            {
                case SoundSource.Soundy:
                    DrawSoundy(drawRect, property, hasSound, hasMissingAudioClips, nameIsNoSound, player, backgroundColorName, textColorName);
                    break;
                case SoundSource.AudioClip:
                    DrawAudioClip(drawRect, property, hasSound, player, backgroundColorName, textColorName);
                    break;
                case SoundSource.MasterAudio:
                    DrawMasterAudio(drawRect, property, backgroundColorName, textColorName);
                    break;
            }

            GUI.color = initialColor; //restore the GUI color
        }

        private void DrawSoundy(Rect drawRect, SerializedProperty property, bool hasSound, bool hasMissingAudioClips, bool nameIsNoSound, SoundyAudioPlayer.Player player, ColorName backgroundColorName, ColorName textColorName)
        {
            Color initialColor = GUI.color;

            //LINE 1A
            SerializedProperty soundSource = Properties.Get(PropertyName.SoundSource, property);
            SerializedProperty databaseName = Properties.Get(PropertyName.DatabaseName, property);
            SerializedProperty soundName = Properties.Get(PropertyName.SoundName, property);

            float x = drawRect.x + DGUI.Properties.Space();
            float y = drawRect.y;

            var outputLabelRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, TOP_ROW_HEIGHT);
            float databaseNameSoundNameWidth = (drawRect.width - OUTPUT_LABEL_WIDTH - DGUI.Properties.Space(5)) / 2;
            x += OUTPUT_LABEL_WIDTH + DGUI.Properties.Space();
            var databaseNameLabelRect = new Rect(x, y, databaseNameSoundNameWidth, TOP_ROW_HEIGHT);
            x += databaseNameSoundNameWidth + DGUI.Properties.Space();
            var soundNameLabelRect = new Rect(x, y, databaseNameSoundNameWidth, TOP_ROW_HEIGHT);

            DGUI.Label.Draw(outputLabelRect, UILabels.SoundSource, Size.S, textColorName);
            DGUI.Label.Draw(databaseNameLabelRect, UILabels.DatabaseName, Size.S, textColorName);
            DGUI.Label.Draw(soundNameLabelRect, UILabels.SoundName, Size.S, textColorName);

            //LINE 1B
            x = drawRect.x + DGUI.Properties.Space();
            y += TOP_ROW_HEIGHT + DGUI.Properties.Space();
            var outputDropdownRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, DGUI.Properties.SingleLineHeight);
            x += OUTPUT_LABEL_WIDTH + DGUI.Properties.Space();
            var databaseNameDropdownRect = new Rect(x, y, databaseNameSoundNameWidth, DGUI.Properties.SingleLineHeight);
            x += databaseNameSoundNameWidth + DGUI.Properties.Space();
            var soundNameDropdownRect = new Rect(x, y, databaseNameSoundNameWidth, DGUI.Properties.SingleLineHeight);

            SoundDatabase soundDatabase = SoundySettings.Database.GetSoundDatabase(databaseName.stringValue);
            if (soundDatabase == null)
            {
                databaseName.stringValue = SoundyManager.GENERAL;
                soundDatabase = SoundySettings.Database.GetSoundDatabase(SoundyManager.GENERAL);
                if (soundDatabase == null) SoundySettings.Database.Initialize();
                if (soundDatabase == null) return;
            }

            int databaseIndex = SoundySettings.Database.DatabaseNames.IndexOf(databaseName.stringValue);

            int soundNameIndex;
            if (soundDatabase.SoundNames.Contains(soundName.stringValue))
            {
                soundNameIndex = soundDatabase.SoundNames.IndexOf(soundName.stringValue);
            }
            else
            {
                soundName.stringValue = SoundyManager.NO_SOUND;
                soundNameIndex = soundDatabase.SoundNames.IndexOf(SoundyManager.NO_SOUND);
            }

            GUI.color = DGUI.Colors.PropertyColor(backgroundColorName);

            //DRAW - OUTPUT DROPDOWN
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(outputDropdownRect, soundSource, GUIContent.none, true);
            if (EditorGUI.EndChangeCheck())
            {
                soundName.stringValue = "";
                if (player != null && player.IsPlaying) player.Stop();
                DGUI.Properties.ResetKeyboardFocus();
            }

            //DRAW - DATABASE NAME DROPDOWN
            EditorGUI.BeginChangeCheck();
            databaseIndex = EditorGUI.Popup(databaseNameDropdownRect, databaseIndex, SoundySettings.Database.DatabaseNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                databaseName.stringValue = SoundySettings.Database.DatabaseNames[databaseIndex];
                DGUI.Properties.ResetKeyboardFocus();
                UpdateThisAudioDataPreviewReference(property);
                if (player != null && player.IsPlaying) player.Stop();
            }

            if (hasMissingAudioClips && !nameIsNoSound) GUI.color = DGUI.Colors.PropertyColor(ColorName.Red);

            //DRAW - SOUND NAME NAME DROPDOWN
            EditorGUI.BeginChangeCheck();
            soundNameIndex = EditorGUI.Popup(soundNameDropdownRect, soundNameIndex, soundDatabase.SoundNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                soundName.stringValue = soundDatabase.SoundNames[soundNameIndex];
                DGUI.Properties.ResetKeyboardFocus();
                UpdateThisAudioDataPreviewReference(property);
                if (player != null && player.IsPlaying) player.Stop();
            }

            GUI.color = initialColor;

            //LINE 2
            x = drawRect.x + DGUI.Properties.Space();
            y += DGUI.Properties.SingleLineHeight + DGUI.Properties.Space();

            const float iconSize = 12f;

            var openSoundyButtonRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, DGUI.Properties.SingleLineHeight);
            x += OUTPUT_LABEL_WIDTH;
            x += DGUI.Properties.Space(2);
            var outputAudioMixerGroupIconRect = new Rect(x, y + (DGUI.Properties.SingleLineHeight - iconSize) / 2, iconSize, iconSize);
            x += outputAudioMixerGroupIconRect.width;
            x += DGUI.Properties.Space();
            var outputAudioMixerGroupLabelRect = new Rect(x, y, drawRect.width - OUTPUT_LABEL_WIDTH - outputAudioMixerGroupIconRect.width - DGUI.Properties.Space(5), DGUI.Properties.SingleLineHeight);

            if (DGUI.Button.Draw(openSoundyButtonRect, DGUI.Properties.Labels.Soundy, Size.S, TextAlign.Center, hasMissingAudioClips && !nameIsNoSound ? ColorName.Red : DGUI.Colors.SoundyColorName, hasMissingAudioClips && !nameIsNoSound)) //draw Soundy sutton
                DoozyWindow.Open(DoozyWindow.View.Soundy,
                                 () =>
                                 {
                                     DoozyWindow.Instance.GetSoundDatabaseAnimBool(databaseName.stringValue).target = true;
                                 });


            bool hasOutputAudioMixerGroup = player != null && player.OutputAudioMixerGroup != null;
            DGUI.Icon.Draw(outputAudioMixerGroupIconRect, Styles.GetStyle(Styles.StyleName.IconAudioMixerGroup), hasOutputAudioMixerGroup ? textColorName : DGUI.Colors.DisabledTextColorName);               //draw audio mixer group icon
            DGUI.Label.Draw(outputAudioMixerGroupLabelRect, player != null ? player.OutputAudioMixerGroupName : "---", Size.S, hasOutputAudioMixerGroup ? textColorName : DGUI.Colors.DisabledTextColorName); //draw audio mixer group name
            GUI.color = initialColor;

            //LINE 3
            x = drawRect.x + DGUI.Properties.Space();
            y += DGUI.Properties.SingleLineHeight + DGUI.Properties.Space();

            DrawPlayer(drawRect, x, y, hasSound, player, textColorName);

            GUI.color = initialColor;
        }


        private void DrawAudioClip(Rect drawRect, SerializedProperty property, bool hasSound, SoundyAudioPlayer.Player player, ColorName backgroundColorName, ColorName textColorName)
        {
            Color initialColor = GUI.color;

            SerializedProperty soundSource = Properties.Get(PropertyName.SoundSource, property);
            SerializedProperty audioClip = Properties.Get(PropertyName.AudioClip, property);
            SerializedProperty outputAudioMixerGroup = Properties.Get(PropertyName.OutputAudioMixerGroup, property);

            //LINE 1A
            float x = drawRect.x + DGUI.Properties.Space();
            float y = drawRect.y;

            var outputLabelRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, TOP_ROW_HEIGHT);
            float audioClipLabelWidth = drawRect.width - OUTPUT_LABEL_WIDTH - DGUI.Properties.Space(4);
            x += OUTPUT_LABEL_WIDTH + DGUI.Properties.Space();
            var audioClipLabelRect = new Rect(x, y, audioClipLabelWidth, TOP_ROW_HEIGHT);


            DGUI.Label.Draw(outputLabelRect, UILabels.SoundSource, Size.S, textColorName);
            DGUI.Label.Draw(audioClipLabelRect, UILabels.AudioClip, Size.S, textColorName);

            //LINE 1B
            x = drawRect.x + DGUI.Properties.Space();
            y += TOP_ROW_HEIGHT + DGUI.Properties.Space();
            var outputDropdownRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, DGUI.Properties.SingleLineHeight);
            x += OUTPUT_LABEL_WIDTH + DGUI.Properties.Space();
            var audioClipRect = new Rect(x, y, audioClipLabelWidth, DGUI.Properties.SingleLineHeight);

            GUI.color = DGUI.Colors.PropertyColor(backgroundColorName);

            //DRAW - OUTPUT DROPDOWN
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(outputDropdownRect, soundSource, GUIContent.none, true);
            if (EditorGUI.EndChangeCheck())
            {
                Properties.Get(PropertyName.SoundName, property).stringValue = "";
                if (player != null && player.IsPlaying) player.Stop();
                DGUI.Properties.ResetKeyboardFocus();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(audioClipRect, audioClip, GUIContent.none, true);
            if (EditorGUI.EndChangeCheck())
            {
                if (player != null && player.IsPlaying) player.Stop();
                DGUI.Properties.ResetKeyboardFocus();
            }

            GUI.color = initialColor;

            //LINE 2
            x = drawRect.x + DGUI.Properties.Space();
            y += DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);

            var outputAudioMixerGroupContent = new GUIContent(UILabels.OutputAudioMixerGroup);
            Vector2 outputAudioMixerGroupContentSize = DGUI.Label.Style(Size.S).CalcSize(outputAudioMixerGroupContent);

            const float iconSize = 12f;

            var outputAudioMixerGroupIconRect = new Rect(x, y + (DGUI.Properties.SingleLineHeight - iconSize) / 2, iconSize, iconSize);
            x += iconSize;
            x += DGUI.Properties.Space();
            var outputAudioMixerGroupLabelRect = new Rect(x, y + (DGUI.Properties.SingleLineHeight - outputAudioMixerGroupContentSize.y) / 2, outputAudioMixerGroupContentSize.x, outputAudioMixerGroupContentSize.y);
            x += outputAudioMixerGroupContentSize.x;
            x += DGUI.Properties.Space(2);
            var outputAudioMixerGroupRect = new Rect(x, y, drawRect.width - DGUI.Properties.Space(6) - outputAudioMixerGroupIconRect.width - outputAudioMixerGroupLabelRect.width, DGUI.Properties.SingleLineHeight);

            bool hasOutputAudioMixerGroup = outputAudioMixerGroup.objectReferenceValue != null;

            DGUI.Icon.Draw(outputAudioMixerGroupIconRect, Styles.GetStyle(Styles.StyleName.IconAudioMixerGroup), hasOutputAudioMixerGroup ? textColorName : DGUI.Colors.DisabledTextColorName);
            DGUI.Label.Draw(outputAudioMixerGroupLabelRect, outputAudioMixerGroupContent, Size.S, hasOutputAudioMixerGroup ? textColorName : DGUI.Colors.DisabledTextColorName);

            GUI.color = initialColor;

            EditorGUI.BeginChangeCheck();
            DGUI.Property.Draw(outputAudioMixerGroupRect, outputAudioMixerGroup, hasOutputAudioMixerGroup ? backgroundColorName : DGUI.Colors.DisabledBackgroundColorName);
            if (EditorGUI.EndChangeCheck())
            {
                if (player != null && player.IsPlaying) player.Stop();
                DGUI.Properties.ResetKeyboardFocus();
            }

            GUI.color = initialColor;

            //LINE 3
            x = drawRect.x + DGUI.Properties.Space();
            y += DGUI.Properties.SingleLineHeight + DGUI.Properties.Space();

            DrawPlayer(drawRect, x, y, hasSound, player, textColorName);

            GUI.color = initialColor;
        }

        private void DrawPlayer(Rect drawRect, float x, float y, bool hasSound, SoundyAudioPlayer.Player player, ColorName colorName)
        {
            const float iconSize = 12f;

            if (hasSound && player != null)
            {
                var playerRect = new Rect(x, y, drawRect.width - DGUI.Properties.Space(2), DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2));
                player.DrawPlayer(playerRect, colorName);
            }
            else
            {
                y += DGUI.Properties.Space() + 1;

                var messageContent = new GUIContent("Soundy Player - Not Available");
                Vector2 messageContentSize = DGUI.Label.Style(Size.S).CalcSize(messageContent);

                var iconRect = new Rect(x, y + (DGUI.Properties.SingleLineHeight - iconSize) / 2, iconSize, iconSize);
                x += iconRect.width;
                x += DGUI.Properties.Space();
                var messageRect = new Rect(x, y + (DGUI.Properties.SingleLineHeight - messageContentSize.y) / 2, messageContentSize.x, messageContentSize.y);

                GUI.color = DGUI.Colors.BackgroundColor(colorName);

                DGUI.Icon.Draw(iconRect, Styles.GetStyle(Styles.StyleName.IconSoundy));
                DGUI.Label.Draw(messageRect, messageContent, Size.S);
            }
        }

        private void DrawMasterAudio(Rect drawRect, SerializedProperty property, ColorName backgroundColorName, ColorName textColorName)
        {
            Color initialColor = GUI.color;

            SerializedProperty soundSource = Properties.Get(PropertyName.SoundSource, property);
            SerializedProperty soundName = Properties.Get(PropertyName.SoundName, property);

            //LINE 1A
            float x = drawRect.x + DGUI.Properties.Space();
            float y = drawRect.y + DGUI.Properties.Space();

            var outputLabelRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, TOP_ROW_HEIGHT);
            float soundNameLabelWidth = drawRect.width - OUTPUT_LABEL_WIDTH - DGUI.Properties.Space(6);
            x += OUTPUT_LABEL_WIDTH + DGUI.Properties.Space();
            var soundNameLabelRect = new Rect(x, y, soundNameLabelWidth, TOP_ROW_HEIGHT);

            DGUI.Label.Draw(outputLabelRect, UILabels.SoundSource, Size.S, textColorName);
            DGUI.Label.Draw(soundNameLabelRect, DoozySettings.Instance.UseMasterAudio ? UILabels.SoundName : UILabels.EnableSupportForMasterAudio, Size.S, textColorName);

            //LINE 1B
            x = drawRect.x + DGUI.Properties.Space();
            y += TOP_ROW_HEIGHT + DGUI.Properties.Space();
            var outputDropdownRect = new Rect(x, y, OUTPUT_LABEL_WIDTH, DGUI.Properties.SingleLineHeight);
            x += OUTPUT_LABEL_WIDTH + DGUI.Properties.Space();
            var soundNameFieldRect = new Rect(x, y, soundNameLabelWidth, DGUI.Properties.SingleLineHeight);

            GUI.color = DGUI.Colors.PropertyColor(backgroundColorName);

            //DRAW - OUTPUT DROPDOWN
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(outputDropdownRect, soundSource, GUIContent.none, true);
            if (EditorGUI.EndChangeCheck()) DGUI.Properties.ResetKeyboardFocus();

            if (DoozySettings.Instance.UseMasterAudio)
            {
                //DRAW - SOUND NAME - for MasterAudio
                EditorGUI.PropertyField(soundNameFieldRect, soundName, GUIContent.none, true);
            }
            else
            {
                var masterAudioIconRect = new Rect(soundNameFieldRect.x, soundNameFieldRect.y, soundNameFieldRect.height, soundNameFieldRect.height);
                DGUI.Icon.Draw(masterAudioIconRect, Styles.GetStyle(Styles.StyleName.IconMasterAudio));
                var buttonRect = new Rect(masterAudioIconRect.xMax + DGUI.Properties.Space(), masterAudioIconRect.y, soundNameFieldRect.width - masterAudioIconRect.width - DGUI.Properties.Space(), soundNameFieldRect.height);
                if (DGUI.Button.Draw(buttonRect, UILabels.OpenControlPanel, Size.S, ColorName.Red, ColorName.Red, true))
                    DoozyWindow.Open(DoozyWindow.View.General);
            }


            GUI.color = initialColor;
        }
    }
}