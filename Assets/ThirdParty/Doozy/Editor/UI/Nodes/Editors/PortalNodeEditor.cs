// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody;
using Doozy.Editor.Nody.Editors;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(PortalNode))]
    public class PortalNodeEditor : BaseNodeEditor
    {
        private static NamesDatabase UIViewDatabase { get { return UIViewSettings.Database; } }
        private static NamesDatabase UIButtonDatabase { get { return UIButtonSettings.Database; } }
        private static NamesDatabase UIDrawerDatabase { get { return UIDrawerSettings.Database; } }

        private PortalNode TargetNode { get { return (PortalNode) target; } }

        private InfoMessage m_infoMessageUnnamedNodeName,
                            m_infoMessageDuplicateNodeName,
                            m_infoMessageNotListeningForAnyGameEvent;

        private SerializedProperty
            m_listenFor,
            m_anyValue,
            m_gameEvent,
            m_uiViewTriggerAction,
            m_viewCategory,
            m_viewName,
            m_uiButtonTriggerAction,
            m_buttonCategory,
            m_buttonName,
            m_uiDrawerTriggerAction,
            m_drawerName,
            m_customDrawerName,
            m_switchBackMode;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_listenFor = GetProperty(PropertyName.ListenFor);
            m_anyValue = GetProperty(PropertyName.AnyValue);
            m_gameEvent = GetProperty(PropertyName.m_gameEvent);
            m_uiViewTriggerAction = GetProperty(PropertyName.UIViewTriggerAction);
            m_viewCategory = GetProperty(PropertyName.ViewCategory);
            m_viewName = GetProperty(PropertyName.ViewName);
            m_uiButtonTriggerAction = GetProperty(PropertyName.UIButtonTriggerAction);
            m_buttonCategory = GetProperty(PropertyName.ButtonCategory);
            m_buttonName = GetProperty(PropertyName.ButtonName);
            m_uiDrawerTriggerAction = GetProperty(PropertyName.UIDrawerTriggerAction);
            m_drawerName = GetProperty(PropertyName.DrawerName);
            m_customDrawerName = GetProperty(PropertyName.CustomDrawerName);
            m_switchBackMode = GetProperty(PropertyName.SwitchBackMode);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateNodeName(GetNodeName());
            m_infoMessageUnnamedNodeName = new InfoMessage(InfoMessage.MessageType.Error, UILabels.UnnamedNodeTitle, UILabels.UnnamedNodeMessage);
            m_infoMessageDuplicateNodeName = new InfoMessage(InfoMessage.MessageType.Error, UILabels.DuplicateNodeTitle, UILabels.DuplicateNodeMessage);
            m_infoMessageNotListeningForAnyGameEvent = new InfoMessage(InfoMessage.MessageType.Error, UILabels.NotListeningForAnyGameEventTitle, UILabels.NotListeningForAnyGameEventMessage);

            UpdateSwitchBackModeState(TargetNode.SwitchBackMode);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderPortalNode), MenuUtils.PortalNode_Manual, MenuUtils.PortalNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName(false);
            m_infoMessageUnnamedNodeName.Draw(TargetNode.ErrorNodeNameIsEmpty, InspectorWidth);
            m_infoMessageDuplicateNodeName.Draw(TargetNode.ErrorDuplicateNameFoundInGraph, InspectorWidth);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawSwitchBackMode();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            EditorGUI.BeginChangeCheck();
            DrawOptions();
            if (EditorGUI.EndChangeCheck())
                NodeUpdated = true;
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            if (NodeUpdated) UpdateNodeName(GetNodeName());
            SendGraphEventNodeUpdated();
        }

        private void DrawSwitchBackMode()
        {
            EditorGUI.BeginChangeCheck();
            DGUI.Toggle.Switch.Draw(m_switchBackMode, UILabels.SwitchBackMode, ComponentColorName, true, false);
            if (!EditorGUI.EndChangeCheck()) return;
            UpdateSwitchBackModeState(m_switchBackMode.boolValue);
        }

        private void UpdateSwitchBackModeState(bool value)
        {
            if (value)
            {
                if (TargetNode.InputSockets == null || 
                    TargetNode.InputSockets.Count == 0)
                    TargetNode.AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            }
            else
            {
                if (TargetNode.InputSockets != null && 
                    TargetNode.InputSockets.Count > 0)
                {
                    if (TargetNode.FirstInputSocket != null &&
                        TargetNode.FirstInputSocket.IsConnected)
                        GraphEvent.DisconnectSocket(TargetNode.FirstInputSocket);

                    TargetNode.InputSockets.Clear();
                }

                NodeUpdated = true;
            }
        }

        private string GetNodeName()
        {
            switch (TargetNode.ListenFor)
            {
                case PortalNode.ListenerType.GameEvent:
                    return TargetNode.GameEventToListenFor + " " + TargetNode.ListenFor;
                case PortalNode.ListenerType.UIButton:
                    return TargetNode.UIButtonTriggerAction + " " + (TargetNode.AnyValue
                                                                         ? UILabels.AnyUIButton
                                                                         : TargetNode.ButtonCategory + " / " + TargetNode.ButtonName) + " " + TargetNode.ListenFor;
                case PortalNode.ListenerType.UIView:
                    return TargetNode.UIViewTriggerAction + " " + (TargetNode.AnyValue
                                                                       ? UILabels.AnyUIView
                                                                       : TargetNode.ViewCategory + " / " + TargetNode.ViewName) + " " + TargetNode.ListenFor;
                case PortalNode.ListenerType.UIDrawer:
                    return TargetNode.UIDrawerTriggerAction + " " + (TargetNode.AnyValue
                                                                         ? UILabels.AnyUIDrawer
                                                                         : TargetNode.DrawerName) + " " + TargetNode.ListenFor;
                default: return string.Empty;
            }
        }

        private void DrawGameEvent(SerializedProperty property, GUIStyle iconStyle, string label)
        {
            bool hasErrors = TargetNode.ErrorNotListeningForAnyGameEvent;
            ColorName colorName = !hasErrors ? DGUI.Colors.ActionColorName : ColorName.Red;
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    DGUI.Property.Draw(property, "", DGUI.Colors.ActionColorName, hasErrors);
                    if (EditorGUI.EndChangeCheck()) NodeUpdated = true;
                    GUILayout.Space(DGUI.Properties.Space());
                    if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaPaste),
                                                           UILabels.Paste,
                                                           Size.S, TextAlign.Left,
                                                           colorName, colorName,
                                                           DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                        property.stringValue = EditorGUIUtility.systemCopyBuffer;
                    GUILayout.Space(DGUI.Properties.Space());
                    bool enabledState = GUI.enabled;
                    GUI.enabled = !hasErrors;
                    if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaCopy),
                                                           UILabels.Copy,
                                                           Size.S, TextAlign.Left,
                                                           colorName, colorName,
                                                           DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                    {
                        EditorGUIUtility.systemCopyBuffer = property.stringValue;
                        Debug.Log(UILabels.GameEvent + " '" + property.stringValue + "' " + UILabels.HasBeenAddedToClipboard);
                    }

                    GUI.enabled = enabledState;
                }
                GUILayout.EndHorizontal();

                m_infoMessageNotListeningForAnyGameEvent.Draw(TargetNode.ErrorNotListeningForAnyGameEvent, InspectorWidth);
            }
            GUILayout.EndVertical();
        }

        private void DrawOptions()
        {
            ColorName backgroundColorName = DGUI.Colors.ActionColorName;
            ColorName textColorName = DGUI.Colors.ActionColorName;
            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconFaEar), UILabels.GlobalListener, backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(m_listenFor, UILabels.ListenFor, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space());
                switch (TargetNode.ListenFor)
                {
                    case PortalNode.ListenerType.UIView:
                        DGUI.Property.Draw(m_uiViewTriggerAction, UILabels.TriggerAction, backgroundColorName, textColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        if ((UIViewBehaviorType) m_uiViewTriggerAction.enumValueIndex == UIViewBehaviorType.Unknown)
                            m_uiViewTriggerAction.enumValueIndex = (int) UIViewBehaviorType.Show;
                        DGUI.Toggle.Switch.Draw(m_anyValue, UILabels.AnyUIView, textColorName, true, false);
                        if (m_anyValue.boolValue)
                        {
                            backgroundColorName = DGUI.Colors.DisabledBackgroundColorName;
                            textColorName = DGUI.Colors.DisabledTextColorName;
                        }

                        break;
                    case PortalNode.ListenerType.UIButton:
                        DGUI.Property.Draw(m_uiButtonTriggerAction, UILabels.TriggerAction, backgroundColorName, textColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Toggle.Switch.Draw(m_anyValue, UILabels.AnyUIButton, textColorName, true, false);
                        if (m_anyValue.boolValue)
                        {
                            backgroundColorName = DGUI.Colors.DisabledBackgroundColorName;
                            textColorName = DGUI.Colors.DisabledTextColorName;
                        }

                        break;
                    case PortalNode.ListenerType.UIDrawer:
                        DGUI.Property.Draw(m_uiDrawerTriggerAction, UILabels.TriggerAction, backgroundColorName, textColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Toggle.Switch.Draw(m_anyValue, UILabels.AnyUIDrawer, textColorName, true, false);
                        if (m_anyValue.boolValue)
                        {
                            backgroundColorName = DGUI.Colors.DisabledBackgroundColorName;
                            textColorName = DGUI.Colors.DisabledTextColorName;
                        }

                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            EditorGUILayout.BeginHorizontal();
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (TargetNode.ListenFor)
                {
                    case PortalNode.ListenerType.GameEvent:
                        DrawGameEvent(m_gameEvent, Styles.GetStyle(Styles.StyleName.IconGameEvent), UILabels.GameEvent);
//                        GUI.enabled = !TargetNode.AnyValue;
//                        DGUI.Property.Draw(m_gameEvent, UILabels.GameEvent, backgroundColorName, textColorName, TargetNode.ErrorNoGameEvent);
//                        GUI.enabled = true;
                        break;
                    case PortalNode.ListenerType.UIView:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                                m_viewCategory, UILabels.ViewCategory,
                                                                m_viewName, UILabels.ViewName,
                                                                UIViewDatabase,
                                                                backgroundColorName);
                        GUI.enabled = true;
                        break;
                    case PortalNode.ListenerType.UIButton:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                                m_buttonCategory, UILabels.ButtonCategory,
                                                                m_buttonName, UILabels.ButtonName,
                                                                UIButtonDatabase,
                                                                backgroundColorName);
                        GUI.enabled = true;
                        break;
                    case PortalNode.ListenerType.UIDrawer:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Database.DrawItemsDatabaseSelectorForGeneralCategoryOnly(UIDrawer.DefaultDrawerCategory,
                                                                                      m_drawerName, UILabels.DrawerName,
                                                                                      m_customDrawerName,
                                                                                      UIDrawerDatabase,
                                                                                      backgroundColorName);
                        GUI.enabled = true;
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}