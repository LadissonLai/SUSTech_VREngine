// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Nody.Windows;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody.Nodes;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Nody.NodeGUI
{
    [CustomNodeGUI(typeof(SubGraphNode))]
    public class SubGraphNodeGUI : BaseNodeGUI
    {
        private SubGraphNode SubGraphNode { get { return (SubGraphNode) Node; } }

        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconSubGraphNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }

        protected override void OnNodeGUI()
        {
            base.OnNodeGUI();
            DrawNodeBody();
            DrawSocketsList(Node.InputSockets);
            DrawSocketsList(Node.OutputSockets);
            DrawActiveNodeName();
        }

        public override void OnDoubleClick(EditorWindow window)
        {
            base.OnDoubleClick(window);
            if (SubGraphNode.HasErrors) return;
            if (SubGraphNode.DebugMode) UnityEngine.Debug.Log(SubGraphNode.Name + " - OnDoubleClick");
            var nodyWindow = window as NodyWindow;
            if (nodyWindow != null) nodyWindow.OpenSubGraph(SubGraphNode);
        }

        private void DrawActiveNodeName()
        {
            if (SubGraphNode.HasErrors) return;

            string activeNodeName = IsActive &&
                                    SubGraphNode.SubGraph != null &&
                                    SubGraphNode.SubGraph.ActiveNode != null
                ? SubGraphNode.SubGraph.ActiveNode.Name
                : "---";

            DynamicHeight += DGUI.Properties.Space(2);
            float areaHeight = DGUI.Properties.SingleLineHeight * 2 + DGUI.Properties.Space(2);
            var areaRect = new Rect(20, DynamicHeight, Width - 40, areaHeight);
            DynamicHeight += areaHeight;
            if (ZoomedBeyondSocketDrawThreshold) return;


            Color initialColor = GUI.color;
            GUILayout.BeginArea(areaRect);
            {
                GUI.color = DGUI.Colors.TextColor(DGUI.Colors.DisabledTextColorName).WithAlpha(0.6f);
                DGUI.Label.Draw(UILabels.ActiveNode, Doozy.Editor.Size.S, DGUI.Properties.SingleLineHeight);
                GUI.color = initialColor;
                DGUI.Label.Draw(activeNodeName, Doozy.Editor.Size.S, DGUI.Properties.SingleLineHeight);
            }
            GUILayout.EndArea();
            GUI.color = initialColor;
        }
    }
}