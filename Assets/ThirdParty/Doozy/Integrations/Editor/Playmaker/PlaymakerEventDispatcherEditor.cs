// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_Playmaker

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Utils;
using Doozy.Integrations.Playmaker;
using UnityEditor;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Integrations.Editor.Playmaker
{
    [CustomEditor(typeof(PlaymakerEventDispatcher))]
    public class PlaymakerEventDispatcherEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.PlaymakerEventDispatcherColorName; } }
        private PlaymakerEventDispatcher m_target;

        private PlaymakerEventDispatcher Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (PlaymakerEventDispatcher) target;
                return m_target;
            }
        }

        private const string DISABLED_MISSING_TARGET_FSM = "DisabledMissingTargetFSM";
        private const string DISABLED_SELECT_LISTENER = "DisabledSelectListener";
        private const string HOW_TO_USE = "HowToUse";

        private SerializedProperty
            m_dispatchButtonClicks,
            m_dispatchGameEvents,
            m_overrideTargetFsm,
            m_targetFsm;

        private bool m_showHowToUse;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_dispatchButtonClicks = GetProperty(PropertyName.DispatchButtonClicks);
            m_dispatchGameEvents = GetProperty(PropertyName.DispatchGameEvents);
            m_overrideTargetFsm = GetProperty(PropertyName.OverrideTargetFsm);
            m_targetFsm = GetProperty(PropertyName.TargetFsm);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(DISABLED_MISSING_TARGET_FSM, new InfoMessage(InfoMessage.MessageType.Error,
                                                                        UILabels.Disabled,
                                                                        UILabels.MissingTargetFsmMessage,
                                                                        false,
                                                                        Repaint));

            AddInfoMessage(DISABLED_SELECT_LISTENER, new InfoMessage(InfoMessage.MessageType.Error,
                                                                     UILabels.Disabled,
                                                                     UILabels.SelectListenerToActivateMessage,
                                                                     false,
                                                                     Repaint));

            AddInfoMessage(HOW_TO_USE, new InfoMessage(InfoMessage.MessageType.Info,
                                                       UILabels.HowToUse,
                                                       UILabels.HowToUsePlaymakerEventDispatcherMessage,
                                                       false,
                                                       Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderPlaymakerEventDispatcher), MenuUtils.PlaymakerEventDispatcher_Manual, MenuUtils.PlaymakerEventDispatcher_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawTargetFsm();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawDispatchOptions();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawHelp();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTargetFsm()
        {
            bool hasTargetFsm = Target.TargetFsm != null;
            GetInfoMessage(DISABLED_MISSING_TARGET_FSM).Draw(!hasTargetFsm, InspectorWidth);
            GUILayout.BeginHorizontal();
            {
                GUI.enabled = Target.OverrideTargetFsm;
                DGUI.Property.Draw(m_targetFsm, UILabels.TargetFsm, ComponentColorName, ComponentColorName, !hasTargetFsm);
                GUI.enabled = true;
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Checkbox.Draw(m_overrideTargetFsm, UILabels.Override, ComponentColorName, true, false);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Line.Draw(true, ComponentColorName,
                           () => { DGUI.Label.Draw(UILabels.FsmName + ":", Size.S, TextAlign.Left, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space()); },
                           () => { DGUI.Label.Draw(hasTargetFsm ? Target.TargetFsm.FsmName : "---", Size.M, TextAlign.Left, ComponentColorName, DGUI.Properties.SingleLineHeight); });
        }

        private void DrawDispatchOptions()
        {
            GetInfoMessage(DISABLED_SELECT_LISTENER).Draw(!Target.DispatchGameEvents && !Target.DispatchButtonClicks, InspectorWidth);
            DGUI.Toggle.Switch.Draw(m_dispatchGameEvents, UILabels.DispatchGameEvents, ComponentColorName, true, false);
            GUILayout.Space(DGUI.Properties.Space(2));
            DGUI.Toggle.Switch.Draw(m_dispatchButtonClicks, UILabels.DispatchButtonClicks, ComponentColorName, true, false);
        }

        private void DrawHelp()
        {
            m_showHowToUse = DGUI.Toggle.Switch.Draw(m_showHowToUse, UILabels.HowToUse, ComponentColorName, false, false, false);
            GetInfoMessage(HOW_TO_USE).Draw(m_showHowToUse, InspectorWidth);
        }
    }
}

#endif