// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Utils;
using Doozy.Editor.Nody.Windows;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Nody.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(SwitchBackNode))]
    public class SwitchBackNodeEditor : BaseNodeEditor
    {
        private static float SourceVerticalSpacing { get { return DGUI.Properties.Space(8); } }

        private SwitchBackNode Node { get { return (SwitchBackNode) target; } }

        private InfoMessage
            m_infoMessageUnnamedNodeName,
            m_infoMessageDuplicateNodeName,
            m_infoMessageErrorNoTargetConnected,
            m_infoMessageErrorNoSourceConnected;


        private SwitchBackNode m_switchBackNode;

        private NodyWindow m_window;

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_window = NodyWindow.Instance;

            m_switchBackNode = (SwitchBackNode) target;

            m_infoMessageUnnamedNodeName = new InfoMessage(InfoMessage.MessageType.Error, UILabels.UnnamedNodeTitle, UILabels.UnnamedNodeMessage);
            m_infoMessageDuplicateNodeName = new InfoMessage(InfoMessage.MessageType.Error, UILabels.DuplicateNodeTitle, UILabels.DuplicateNodeMessage);
            m_infoMessageErrorNoTargetConnected = new InfoMessage(InfoMessage.MessageType.Error, UILabels.NoTargetConnectedTitle, UILabels.NoTargetConnectedMessage);
            m_infoMessageErrorNoSourceConnected = new InfoMessage(InfoMessage.MessageType.Error, UILabels.NoSourceConnectedTitle, UILabels.NoSourceConnectedMessage);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSwitchBackNode), MenuUtils.SwitchBackNode_Manual, MenuUtils.SwitchBackNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            m_infoMessageUnnamedNodeName.Draw(m_switchBackNode.ErrorNodeNameIsEmpty, InspectorWidth);
            m_infoMessageDuplicateNodeName.Draw(m_switchBackNode.ErrorDuplicateNameFoundInGraph, InspectorWidth);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawTarget();
            GUILayout.Space(DGUI.Properties.Space(32));
            DrawSources();
            GUILayout.Space(DGUI.Properties.Space());
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawTarget()
        {
            DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaBullseye), UILabels.Target, DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName);
            m_infoMessageErrorNoTargetConnected.Draw(m_switchBackNode.ErrorNoTargetConnected, InspectorWidth);
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(6));
                GUILayout.BeginVertical();
                {
                    DrawSocketState(Node.TargetInputSocket);
                    if (DrawCurveModifier(Node, Node.TargetInputSocket, ShowCurveModifier)) NodeUpdated = true;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(6));
                GUILayout.BeginVertical();
                {
                    DrawSocketState(Node.TargetOutputSocket);
                    if (DrawCurveModifier(Node, Node.TargetOutputSocket, ShowCurveModifier)) NodeUpdated = true;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSources()
        {
            DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaCompress), UILabels.Sources, DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName);
            m_infoMessageErrorNoSourceConnected.Draw(m_switchBackNode.ErrorNoSourceConnected, InspectorWidth);
            GUILayout.Space(DGUI.Properties.Space(2));
            foreach (SwitchBackNode.SourceInfo source in Node.Sources)
                if (DrawSource(source))
                    break;

            GUILayout.Space(SourceVerticalSpacing);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconPlus),
                                                       UILabels.AddSource,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName,
                                                       DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                {
                    Undo.RecordObject(Node, "Add Source");
                    Node.AddSourceSocketPair();
                    NodeUpdated = true;
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }


        private bool DrawSource(SwitchBackNode.SourceInfo source)
        {
            Color color = GUI.color;
            bool registeredAsReturnSocket = Node.ReturnSourceOutputSocketId == source.OutputSocketId;
            bool removedSocket = false;
            ColorName backgroundColorName = registeredAsReturnSocket ? DGUI.Colors.ActionColorName : DGUI.Colors.DisabledBackgroundColorName;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(6));

                DGUI.Line.Draw(false, backgroundColorName,
                               () =>
                               {
                                   if (registeredAsReturnSocket)
                                   {
                                       GUILayout.Space(DGUI.Properties.Space(2));
                                       DGUI.Icon.Draw(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaChevronRight), DGUI.Properties.SingleLineHeight * 0.6f, DGUI.Properties.SingleLineHeight, DGUI.Colors.ActionColorName);
                                       DGUI.Icon.Draw(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaChevronRight), DGUI.Properties.SingleLineHeight * 0.6f, DGUI.Properties.SingleLineHeight, DGUI.Colors.ActionColorName);
                                       DGUI.Icon.Draw(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaChevronRight), DGUI.Properties.SingleLineHeight * 0.6f, DGUI.Properties.SingleLineHeight, DGUI.Colors.ActionColorName);
                                       GUILayout.Space(DGUI.Properties.Space());
                                   }

//                                   DGUI.Label.Draw(UILabels.SourceName, Size.M, DGUI.Properties.SingleLineHeight, textColorName);
                                   string sourceName = source.SourceName;
                                   EditorGUI.BeginChangeCheck();
                                   GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight - DGUI.Properties.Space()));
                                   {
                                       GUILayout.Space(0);
                                       if (registeredAsReturnSocket) GUI.color = DGUI.Colors.GetDColor(DGUI.Colors.ActionColorName).Light;
                                       sourceName = EditorGUILayout.TextField(sourceName, GUILayout.ExpandWidth(true));
                                       GUI.color = color;
                                   }
                                   GUILayout.EndVertical();
                                   if (EditorGUI.EndChangeCheck())
                                   {
                                       Undo.RecordObject(Node, "Rename Source Name");
                                       source.SourceName = sourceName;
                                       NodeUpdated = true;
                                   }

                                   if (Node.Sources.Count > 2)
                                       if (DGUI.Button.IconButton.Cancel(DGUI.Properties.SingleLineHeight))
                                       {
                                           Undo.RecordObject(Node, "Remove Source");
                                           RemoveSourceSocketPair(source);
                                           removedSocket = true;
                                       }
                               });
            }
            GUILayout.EndHorizontal();
            if (removedSocket) return true;
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(6));
                GUILayout.BeginVertical();
                {
                    Socket inputSocket = Node.GetSocketFromId(source.InputSocketId);
                    DrawSocketState(inputSocket);
                    if (DrawCurveModifier(Node, inputSocket, ShowCurveModifier)) NodeUpdated = true;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(6));
                GUILayout.BeginVertical();
                {
                    Socket outputSocket = Node.GetSocketFromId(source.OutputSocketId);
                    DrawSocketState(outputSocket);
                    if (DrawCurveModifier(Node, outputSocket, ShowCurveModifier)) NodeUpdated = true;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(SourceVerticalSpacing);
            return removedSocket;
        }

        private void RemoveSourceSocketPair(SwitchBackNode.SourceInfo source)
        {
            if (m_window == null) return;
            if (NodyWindow.CurrentGraph == null) return;
            if (NodyWindow.CurrentGraph.Id != Node.GraphId) return;
            if (Node.Sources == null) return;
            if (source == null) return;
            m_window.RemoveSocket(Node.GetSocketFromId(source.InputSocketId), true, false, true);
            m_window.RemoveSocket(Node.GetSocketFromId(source.OutputSocketId), false, false, true);
            Node.Sources.Remove(source);
            EditorUtility.SetDirty(Node);
            NodyWindow.CurrentGraph.SetDirty(false);
            AssetDatabase.Refresh();
            m_window.ConstructGraphGUI();
            m_window.RecalculateAllPointRects();
            NodeUpdated = true;
        }

        private void RemoveSourceSocketPairAtIndex(int index)
        {
            if (Node.Sources == null) return;
            if (index < 0) return;
            if (index >= Node.Sources.Count) return;
            SwitchBackNode.SourceInfo source = Node.Sources[index];
            RemoveSourceSocketPair(source);
        }
    }
}