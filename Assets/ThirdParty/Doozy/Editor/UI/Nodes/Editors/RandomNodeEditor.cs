// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody;
using Doozy.Editor.Nody.Editors;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(RandomNode))]
    public class RandomNodeEditor : BaseNodeEditor
    {
        private RandomNode TargetNode { get { return (RandomNode) target; } }

        private List<WeightedConnection> m_outputValues;

        private List<WeightedConnection> OutputValues
        {
            get
            {
                if (m_outputValues != null) return m_outputValues;
                m_outputValues = new List<WeightedConnection>();
                foreach (Socket socket in TargetNode.OutputSockets)
                    OutputValues.Add(WeightedConnection.GetValue(socket));
                return m_outputValues;
            }
        }

        private void UpdateSocketValue(Socket socket, WeightedConnection value)
        {
            WeightedConnection.SetValue(socket, value);
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_UPDATED, TargetNode.Id);
        }

        protected override void OnGraphEvent(GraphEvent graphEvent) { UpdateNodeData(); }

        protected override void UpdateNodeData()
        {
            base.UpdateNodeData();
            m_outputValues = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderRandomNode), MenuUtils.RandomNode_Manual, MenuUtils.RandomNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(TargetNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets();
            GUILayout.Space(DGUI.Properties.Space(16));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawOutputSockets()
        {
            SerializedProperty socketsProperty = GetProperty(PropertyName.m_outputSockets);
            List<Socket> sockets = BaseNode.OutputSockets;
            ColorName directionColorName = OutputColorName;
            string directionTitle = UILabels.OutputConnections;
            List<WeightedConnection> values = OutputValues;
            GUIStyle directionIconStyle = OutputSocketIconStyle;

            if (socketsProperty.arraySize == 0) return;

            if (sockets.Count != values.Count)
            {
                UpdateNodeData();
                return;
            }

            DrawSmallTitleWithBackground(directionIconStyle, directionTitle, directionColorName);
            Color initialColor = GUI.color;
            GUILayout.Space(DGUI.Properties.Space());

            for (int i = 0; i < socketsProperty.arraySize; i++)
            {
                Socket socket = sockets[i];

//                string socketName = socket.SocketName;
                float curveModifier = socket.CurveModifier;
                int connectionsCount = socket.Connections.Count;

                int weight = values[i].Weight;

//                ColorName colorName = socket.IsConnected ? directionColorName : ColorName.White;

                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginHorizontal(GUILayout.Height(kNodeLineHeight));
                {
                    DrawSocketIndex(i, directionColorName);

                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal(GUILayout.Height(kNodeLineHeight), GUILayout.ExpandWidth(true));
                        {
                            DGUI.Icon.Draw(socket.OverrideConnection
                                               ? Nody.Styles.GetStyle(Nody.Styles.StyleName.ConnectionPointOverrideConnected)
                                               : Nody.Styles.GetStyle(Nody.Styles.StyleName.ConnectionPointMultipleConnected),
                                           kConnectionTypeIconSize,
                                           kNodeLineHeight,
                                           connectionsCount > 0 ? ConnectionTypeIconColor(directionColorName) : NotConnectedIconColor()); //Draw socket connection type icon (Override / Multiple)

                            GUILayout.Space(kConnectionTypeIconPadding);

                            bool valueUpdated = false;

                            GUI.color = socket.IsConnected ? FieldsColor(directionColorName) : GUI.color;
                            GUILayout.Space(DGUI.Properties.Space(2));
                            DGUI.Label.Draw(UILabels.Weight, Size.M, kNodeLineHeight);
                            GUILayout.Space(DGUI.Properties.Space());
                            EditorGUI.BeginChangeCheck();
                            GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight));
                            {
                                GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);
                                weight = EditorGUILayout.IntSlider(GUIContent.none, weight, 0, 100, GUILayout.Height(DGUI.Properties.SingleLineHeight)); //Draw output connection weight
                            }
                            GUILayout.EndVertical();
                            if (EditorGUI.EndChangeCheck()) valueUpdated = true;
                            GUI.color = initialColor;


//                            DGUI.Icon.Draw(OutputSocketIconStyle, kConnectionTypeIconSize, kNodeLineHeight, connectionsCount > 0 ? TriggerTypeIconColor(directionColorName) : NotConnectedIconColor());

                            if (valueUpdated)
                            {
                                Undo.RecordObject(TargetNode, "Update Socket Value");
                                socket.CurveModifier = curveModifier;
                                values[i].Weight = weight;
                                UpdateSocketValue(sockets[i], values[i]);
                                TargetNode.UpdateMaxChance();
                                NodeUpdated = true;
                            }
                        }
                        GUILayout.EndHorizontal();
                        if (DrawCurveModifier(TargetNode, socket, ShowCurveModifier)) NodeUpdated = true;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(DGUI.Properties.Space());
            }

            GUI.color = initialColor;
        }
    }
}