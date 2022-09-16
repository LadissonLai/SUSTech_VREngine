// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Soundy;
using Doozy.Engine.Utils;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Plays a sound or stops, pauses, unpauses, mutes or unmutes all sounds and jumps instantly to the next node in the Graph.
    ///     <para/>
    ///     It can play a sound through a SoundyController either by using a sound name or a direct reference to an AudioClip, or it can trigger MasterAudio to play a set sound name.
    ///     <para/>
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.SoundNode_CreateNodeMenu_Name, MenuUtils.SoundNode_CreateNodeMenu_Order)]
    public class SoundNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorPlayActionHasNoSound; } }
        public bool ErrorPlayActionHasNoSound;
#endif

        public enum SoundActions
        {
            Play,
            Stop,
            Pause,
            Unpause,
            Mute,
            Unmute
        }

        /// <summary> Returns TRUE if SoundData has valid sound settings </summary>
        public bool HasSound
        {
            get
            {
                switch (SoundData.SoundSource)
                {
                    case SoundSource.Soundy:      return SoundData.SoundName != SoundyManager.NO_SOUND && !string.IsNullOrEmpty(SoundData.SoundName);
                    case SoundSource.AudioClip:   return SoundData.AudioClip != null;
                    case SoundSource.MasterAudio: return !string.IsNullOrEmpty(SoundData.SoundName);
                    default:                      return false;
                }
            }
        }

        public SoundyData SoundData;
        public SoundActions SoundAction = SoundActions.Play;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.SoundNodeName);
            SetAllowDuplicateNodeName(true);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);
            var node = (SoundNode) original;
            SoundData = new SoundyData
                        {
                            AudioClip = node.SoundData.AudioClip,
                            DatabaseName = node.SoundData.DatabaseName,
                            OutputAudioMixerGroup = node.SoundData.OutputAudioMixerGroup,
                            SoundName = node.SoundData.SoundName,
                            SoundSource = node.SoundData.SoundSource
                        };
            SoundAction = node.SoundAction;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;

            switch (SoundAction)
            {
                case SoundActions.Play:
                    if (HasSound) SoundyManager.Play(SoundData);
                    break;
                case SoundActions.Stop:
                    SoundyManager.StopAllSounds();
                    break;
                case SoundActions.Pause:
                    SoundyManager.PauseAllSounds();
                    break;
                case SoundActions.Unpause:
                    SoundyManager.UnpauseAllSounds();
                    break;
                case SoundActions.Mute:
                    SoundyManager.MuteAllSounds();
                    break;
                case SoundActions.Unmute:
                    SoundyManager.UnmuteAllSounds();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorPlayActionHasNoSound = SoundAction == SoundActions.Play && !HasSound;
#endif
        }
    }
}