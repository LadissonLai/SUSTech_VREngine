// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Windows;
using Doozy.Engine.Nody.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(SubGraphNode))]
    public class SubGraphNodeEditor : BaseNodeEditor
    {
        private SubGraphNode Node { get { return (SubGraphNode) target; } }

        protected override ColorName ComponentColorName { get { return DGUI.Colors.SubGraphNodeColorName; } }

        private InfoMessage
            m_infoMessageUnnamedNodeName,
            m_infoMessageDuplicateNodeName,
            m_infoMessageNoGraphReferenced,
            m_infoMessageReferencedGraphIsNotSubGraph,
            m_infoMessageDoubleClickNodeToOpenSubGraph;


        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_infoMessageUnnamedNodeName = new InfoMessage(InfoMessage.MessageType.Error, UILabels.UnnamedNodeTitle, UILabels.UnnamedNodeMessage);
            m_infoMessageDuplicateNodeName = new InfoMessage(InfoMessage.MessageType.Error, UILabels.DuplicateNodeTitle, UILabels.DuplicateNodeMessage);
            m_infoMessageNoGraphReferenced = new InfoMessage(InfoMessage.MessageType.Error, UILabels.NoSubGraphReferencedTitle, UILabels.NoSubGraphReferencedMessage);
            m_infoMessageReferencedGraphIsNotSubGraph = new InfoMessage(InfoMessage.MessageType.Error, UILabels.ReferencedGraphIsNotSubGraphTitle, UILabels.ReferencedGraphIsNotSubGraphMessage);
            m_infoMessageDoubleClickNodeToOpenSubGraph = new InfoMessage(InfoMessage.MessageType.Info, "", UILabels.DoubleClickNodeToOpenSubGraphMessage);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSubGraphNode), MenuUtils.SubGraphNode_Manual, MenuUtils.SubGraphNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUI.enabled = !Node.ErrorNoGraphReferenced && !Node.ErrorReferencedGraphIsNotSubGraph;
            GUILayout.Space(DGUI.Properties.Space());
            DrawRenameButton(Node.SubGraph != null ? Node.SubGraph.name : "---");
            GUI.enabled = true;
            m_infoMessageUnnamedNodeName.Draw(Node.ErrorNodeNameIsEmpty, InspectorWidth);
            m_infoMessageDuplicateNodeName.Draw(Node.ErrorDuplicateNameFoundInGraph, InspectorWidth);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            DrawSubGraph();
            DrawInfoMessages();
            CreateSubGraphButton();
            GUILayout.Space(DGUI.Properties.Space());
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawSubGraph()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                DGUI.Property.Draw(GetProperty(PropertyName.m_subGraph), UILabels.SubGraph,
                                   Node.ErrorReferencedGraphIsNotSubGraph ? ColorName.Red : ColorName.White,
                                   Node.ErrorReferencedGraphIsNotSubGraph ? ColorName.Red : DGUI.Colors.DisabledTextColorName);
                if (EditorGUI.EndChangeCheck()) NodeUpdated = true;
                if (!Node.ErrorReferencedGraphIsNotSubGraph)
                {
                    GUILayout.Space(DGUI.Properties.Space());
                    if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconSubGraph), UILabels.OpenSubGraph, Size.S, TextAlign.Left, DGUI.Colors.SubGraphNodeColorName, DGUI.Colors.SubGraphNodeColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                        NodyWindow.Instance.OpenSubGraph(Node);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawInfoMessages()
        {
            m_infoMessageNoGraphReferenced.Draw(Node.ErrorNoGraphReferenced, InspectorWidth);
            if (Node.ErrorNoGraphReferenced) return;
            m_infoMessageReferencedGraphIsNotSubGraph.Draw(Node.ErrorReferencedGraphIsNotSubGraph, InspectorWidth);
            if (Node.HasErrors) return;
            GUILayout.FlexibleSpace();
            m_infoMessageDoubleClickNodeToOpenSubGraph.Draw(!Node.ErrorReferencedGraphIsNotSubGraph, InspectorWidth);
        }

        private void CreateSubGraphButton()
        {
            if (!Node.ErrorNoGraphReferenced) return;
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconSubGraph),
                                                       UILabels.CreateSubGraph,
                                                       Size.L, TextAlign.Left,
                                                       ComponentColorName,
                                                       ComponentColorName,
                                                       DGUI.Properties.SingleLineHeight * 2,
                                                       false))
                {
                    GetProperty(PropertyName.m_subGraph).objectReferenceValue = NodyWindow.CreateSubGraphWidthDialog();
                    NodeUpdated = true;
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }
}