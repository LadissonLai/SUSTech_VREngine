// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Globalization;
using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(Graph))]
    public class GraphEditor : BaseEditor
    {
        private Graph Graph { get { return (Graph) target; } }

        protected override ColorName ComponentColorName { get { return Graph.IsSubGraph ? DGUI.Colors.UISubGraphColorName : DGUI.Colors.UIGraphColorName; } }
        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Graph.IsSubGraph ? Styles.StyleName.ComponentHeaderSubGraph : Styles.StyleName.ComponentHeaderGraph), MenuUtils.Graph_Manual, MenuUtils.Graph_YouTube);
            DrawDebugMode();
            DrawGraphIdInDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawGraphInfo();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawConvertGraphButton();
            DrawToggleHideFlags();
            GUILayout.Space(DGUI.Properties.Space());
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGraphIdInDebugMode()
        {
            SerializedProperty debugMode = GetProperty(PropertyName.DebugMode);
            SerializedProperty id = GetProperty(PropertyName.m_id);
            SerializedProperty version = GetProperty(PropertyName.m_version);

            AnimBool debugModeExpanded = GetAnimBool(debugMode.propertyPath);
            debugModeExpanded.target = debugMode.boolValue;

            float alpha = GUI.color.a;

            if (DGUI.FadeOut.Begin(debugModeExpanded, false))
            {
                bool enabled = GUI.enabled;
                GUI.enabled = false;
                {
                    GUILayout.Space(DGUI.Properties.Space(2) * debugModeExpanded.faded);
                    GUILayout.BeginHorizontal();
                    {
                        DGUI.Property.Draw(id, UILabels.GraphId, DGUI.Colors.DisabledBackgroundColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        GUI.enabled = enabled;
                        if (DGUI.Button.Dynamic.DrawIconButton(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaCopy), UILabels.Copy, Size.S, TextAlign.Left, DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                        {
                            EditorGUIUtility.systemCopyBuffer = id.stringValue;
                            Debug.Log("Graph Id '" + id.stringValue + "' has been added to clipboard...");
                        }

                        GUI.enabled = false;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(DGUI.Properties.Space());
                    DGUI.Property.Draw(version, UILabels.Version, DGUI.Colors.DisabledBackgroundColorName);
                }
                GUI.enabled = enabled;
            }

            DGUI.FadeOut.End(debugModeExpanded, true, alpha);
        }

        private void DrawGraphInfo()
        {
            SerializedProperty description = GetProperty(PropertyName.m_description);
            const Size graphNameTextSize = Size.XL;
            var graphNameContent = new GUIContent(Graph.name);
            Vector2 graphNameContentSize = DGUI.Label.Style(graphNameTextSize, TextAlign.Left).CalcSize(graphNameContent);

            GUILayout.BeginHorizontal();
            {
                DGUI.Label.Draw(Graph.name, graphNameTextSize, TextAlign.Left, DGUI.Colors.DisabledTextColorName);
                GUILayout.FlexibleSpace();
                GUI.color = GUI.color.WithAlpha(0.8f);
                DGUI.Label.Draw(UILabels.LastModified + ": " + Graph.LastModified.ToString(CultureInfo.InvariantCulture), Size.S, TextAlign.Right, DGUI.Colors.DisabledTextColorName, graphNameContentSize.y);
            }
            GUILayout.EndHorizontal();
            DGUI.Label.Draw(Graph.Nodes.Count + " " + UILabels.Nodes, Size.M, TextAlign.Left, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight);

            GUILayout.Space(DGUI.Properties.Space(2));
            DGUI.Label.Draw(UILabels.Description, Size.S, TextAlign.Left, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight);
            GUI.color = InitialGUIColor;
            description.stringValue = GUILayout.TextArea(description.stringValue, GUILayout.ExpandWidth(true));
        }

        private void DrawConvertGraphButton()
        {
            float iconSize = DGUI.Properties.SingleLineHeight * 1.5f;
            GUIStyle iconStyle = Styles.GetStyle(Graph.IsSubGraph ? Styles.StyleName.IconGraph : Styles.StyleName.IconSubGraph);
            string buttonLabel = Graph.IsSubGraph ? UILabels.ConvertToGraph : UILabels.ConvertToSubGraph;
            string convertMessage = Graph.IsSubGraph ? UILabels.AreYouSureConvertToGraph : UILabels.AreYouSureConvertToSubGraph;
            convertMessage += "\n\n" + UILabels.OperationCannotBeUndone;

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(iconSize));
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(iconStyle, buttonLabel, Size.M, TextAlign.Left, DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName, iconSize, false))
                    if (EditorUtility.DisplayDialog(buttonLabel, convertMessage, UILabels.Yes, UILabels.No))
                    {
                        if (Graph.IsSubGraph)
                            ConvertToGraph();
                        else
                            ConvertToSubGraph();
                    }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }


        private void ConvertToSubGraph() { GraphUtils.CreateEnterAndExitNodes(Graph); }
        private void ConvertToGraph() { GraphUtils.CreateStartNode(Graph); }

        private void DrawToggleHideFlags()
        {
            float buttonHeight = DGUI.Properties.SingleLineHeight * 1.6f;
            HideFlags currentHideFlags = Graph.Nodes[0].hideFlags;
            GUIStyle buttonIconStyle = Editor.Styles.GetStyle(currentHideFlags == HideFlags.None ? Editor.Styles.StyleName.IconHide : Editor.Styles.StyleName.IconShow);
            string buttonLabel = currentHideFlags == HideFlags.None ? "Hide Nodes - HideFlags.HideInHierarchy" : "Show Nodes - HideFlags.None";
            HideFlags targetHideFlags = currentHideFlags == HideFlags.None ? targetHideFlags = HideFlags.HideInHierarchy : HideFlags.None;

            GUILayout.Space(DGUI.Properties.Space(8));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(buttonIconStyle,
                                                       buttonLabel, Size.S, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName,
                                                       buttonHeight, false))
                {
                    foreach (Node node in Graph.Nodes)
                    {
                        node.hideFlags = targetHideFlags;
                        EditorUtility.SetDirty(node);
                    }

                    AssetDatabase.SaveAssets();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }
}