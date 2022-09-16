// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Editors;
using Doozy.Editor.Soundy;
using Doozy.Engine.Soundy;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(SoundNode))]
    public class SoundNodeEditor : BaseNodeEditor
    {
        private const string NO_SOUND = "NoSound";
        private const string SOUND_ACTION = "SoundAction";

        private SoundNode TargetNode { get { return (SoundNode) target; } }

        private SerializedProperty
            m_soundData,
            m_soundAction;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_soundData = GetProperty(PropertyName.SoundData);
            m_soundAction = GetProperty(PropertyName.SoundAction);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(NO_SOUND, new InfoMessage(InfoMessage.MessageType.Warning, UILabels.NoSound, TargetNode.SoundAction == SoundNode.SoundActions.Play && !TargetNode.HasSound, Repaint));
            AddInfoMessage(SOUND_ACTION, new InfoMessage(InfoMessage.MessageType.Info, UILabels.NoSound, TargetNode.SoundAction != SoundNode.SoundActions.Play, Repaint));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SoundyAudioPlayer.StopAllPlayers();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSoundNode), MenuUtils.SoundNode_Manual, MenuUtils.SoundNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space());
            DrawRenameNodeButton();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            DrawOptions();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawOptions()
        {
            bool playSound = TargetNode.SoundAction == SoundNode.SoundActions.Play;

            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconAction), UILabels.Actions, DGUI.Colors.ActionColorName, DGUI.Colors.ActionColorName);
            GUILayout.Space(DGUI.Properties.Space(2));

            EditorGUI.BeginChangeCheck();
            DGUI.Property.Draw(m_soundAction, UILabels.SoundAction, DGUI.Colors.ActionColorName, DGUI.Colors.ActionColorName);
            if (EditorGUI.EndChangeCheck()) NodeUpdated = true;

            GUILayout.Space(DGUI.Properties.Space(2));
            GetInfoMessage(NO_SOUND).DrawMessageOnly(playSound && !TargetNode.HasSound);
            if (!playSound)
            {
                switch (TargetNode.SoundAction)
                {
                    case SoundNode.SoundActions.Stop:
                        GetInfoMessage(SOUND_ACTION).Message = UILabels.StopAllSounds;
                        break;
                    case SoundNode.SoundActions.Pause:
                        GetInfoMessage(SOUND_ACTION).Message = UILabels.PauseAllSounds;
                        break;
                    case SoundNode.SoundActions.Unpause:
                        GetInfoMessage(SOUND_ACTION).Message = UILabels.UnpauseAllSounds;
                        break;
                    case SoundNode.SoundActions.Mute:
                        GetInfoMessage(SOUND_ACTION).Message = UILabels.MuteAllSounds;
                        break;
                    case SoundNode.SoundActions.Unmute:
                        GetInfoMessage(SOUND_ACTION).Message = UILabels.UnmuteAllSounds;
                        break;
                }
            }

            GetInfoMessage(SOUND_ACTION).DrawMessageOnly(!playSound);

            if (playSound)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_soundData, GUIContent.none, true);
                if (EditorGUI.EndChangeCheck()) NodeUpdated = true;
            }
        }

        private void DrawRenameNodeButton()
        {
            string renameTo = "---";
            switch (TargetNode.SoundAction)
            {
                case SoundNode.SoundActions.Play:
                    switch (TargetNode.SoundData.SoundSource)
                    {
                        case SoundSource.Soundy:
                        case SoundSource.MasterAudio:
                            if (!string.IsNullOrEmpty(TargetNode.SoundData.SoundName))
                                renameTo = TargetNode.SoundData.SoundName;
                            break;
                        case SoundSource.AudioClip:
                            if (TargetNode.SoundData.AudioClip != null)
                                renameTo = TargetNode.SoundData.AudioClip.name;
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }

                    break;
                case SoundNode.SoundActions.Stop:
                    renameTo = UILabels.StopAllSounds;
                    break;
                case SoundNode.SoundActions.Pause:   
                    renameTo = UILabels.PauseAllSounds;
                    break;
                case SoundNode.SoundActions.Unpause:
                    renameTo = UILabels.UnpauseAllSounds;
                    break;
                case SoundNode.SoundActions.Mute:
                    renameTo = UILabels.MuteAllSounds;
                    break;
                case SoundNode.SoundActions.Unmute:
                    renameTo = UILabels.UnmuteAllSounds;
                    break;
            }

            DrawRenameButton(renameTo);
        }
    }
}