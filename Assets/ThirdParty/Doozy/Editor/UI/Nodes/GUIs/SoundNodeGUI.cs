// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.Soundy;
using Doozy.Engine.UI.Nodes;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    // ReSharper disable once UnusedMember.Global
    [CustomNodeGUI(typeof(SoundNode))]
    public class SoundNodeGUI : BaseNodeGUI
    {
        private SoundNode TargetNode { get { return (SoundNode) Node; } }

        protected override GUIStyle GetIconStyle()
        {
            switch (TargetNode.SoundAction)
            {
                case SoundNode.SoundActions.Play: return Styles.GetStyle(Styles.StyleName.NodeIconSoundNode);
                case SoundNode.SoundActions.Stop: return Styles.GetStyle(Styles.StyleName.IconFaStop);
                case SoundNode.SoundActions.Pause: return Styles.GetStyle(Styles.StyleName.IconFaPause);
                case SoundNode.SoundActions.Unpause: return Styles.GetStyle(Styles.StyleName.IconFaPlay);
                case SoundNode.SoundActions.Mute: return Styles.GetStyle(Styles.StyleName.IconFaVolumeMute);
                case SoundNode.SoundActions.Unmute: return Styles.GetStyle(Styles.StyleName.IconFaVolume);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private GUIStyle m_icon;
        private string m_title;
        private string m_description;
        
        protected override void OnNodeGUI()
        {
            DrawNodeBody();
            DrawSocketsList(Node.InputSockets);
            DrawSocketsList(Node.OutputSockets);
            DrawActionDescription();
        }
 
        private void DrawActionDescription()
        {
            bool playSound = TargetNode.SoundAction == SoundNode.SoundActions.Play;

            DynamicHeight += DGUI.Properties.Space(4);
            float x = DrawRect.x + 16;
            float lineHeight = DGUI.Properties.SingleLineHeight;

            var soundActionRect = new Rect(x, DynamicHeight, DrawRect.width - 32, lineHeight);
            DynamicHeight += soundActionRect.height;
            DynamicHeight += DGUI.Properties.Space(2);

            float iconLineHeight = lineHeight * 2;
            float iconSize = iconLineHeight * 0.6f;
            var iconRect = new Rect(x, DynamicHeight + (iconLineHeight - iconSize) / 2, iconSize, iconSize);
            float textX = iconRect.xMax + DGUI.Properties.Space(4);
            float textWidth = DrawRect.width - iconSize - DGUI.Properties.Space(4) - 32;
            var titleRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);

            if (playSound) DynamicHeight += titleRect.height;
            var descriptionRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);

            if (playSound)
            {
                DynamicHeight += descriptionRect.height;
                DynamicHeight += DGUI.Properties.Space(4);
            }

            if (ZoomedBeyondSocketDrawThreshold) return;

            string soundAction;
            switch (TargetNode.SoundAction)
            {
                case SoundNode.SoundActions.Play:
                    soundAction = UILabels.PlaySound;
                    break;
                
                case SoundNode.SoundActions.Stop:
                    soundAction = UILabels.StopAllSounds;
                    break;
                
                case SoundNode.SoundActions.Pause:
                    soundAction = UILabels.PauseAllSounds;
                    break;
                
                case SoundNode.SoundActions.Unpause:
                    soundAction = UILabels.UnpauseAllSounds;
                    break;
                
                case SoundNode.SoundActions.Mute: 
                    soundAction = UILabels.MuteAllSounds;
                    break;
                
                case SoundNode.SoundActions.Unmute:
                    soundAction = UILabels.UnmuteAllSounds;
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
            
            Color soundActionTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            GUI.Label(soundActionRect, soundAction, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Doozy.Editor.Size.S, TextAlign.Center), soundActionTextColor));

            if (!playSound) return;

            m_icon = null;
            m_title = TargetNode.SoundData.SoundSource.ToString();
            m_description = "---";

            switch (TargetNode.SoundData.SoundSource)
            {
                case SoundSource.Soundy:
                    m_icon = Styles.GetStyle(Styles.StyleName.IconSoundy);
                    m_title = TargetNode.SoundData.DatabaseName;
                    m_description = TargetNode.SoundData.SoundName;
                    break;
                case SoundSource.AudioClip:
                    m_icon = Styles.GetStyle(Styles.StyleName.IconSound);
                    m_description = TargetNode.SoundData.AudioClip != null ? TargetNode.SoundData.AudioClip.name : m_description;
                    break;
                case SoundSource.MasterAudio:
                    m_icon = Styles.GetStyle(Styles.StyleName.IconMasterAudio);
                    m_description = TargetNode.SoundData.SoundName;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            Color iconAndTextColor = DGUI.Colors.TextColor(TargetNode.HasSound ? DGUI.Colors.SoundyColorName : ColorName.Red).WithAlpha(0.6f);
            DGUI.Icon.Draw(iconRect, m_icon, TargetNode.SoundData.SoundSource == SoundSource.MasterAudio ? Color.white.WithAlpha(0.6f) : iconAndTextColor);
            GUI.Label(titleRect, m_title, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Doozy.Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(descriptionRect, m_description, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Doozy.Editor.Size.M, TextAlign.Left), iconAndTextColor));
        }
    }
}