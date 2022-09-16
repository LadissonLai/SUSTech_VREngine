// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Nody.Nodes;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private void DrawGraphTabs()
        {
            if (GraphTabs.Count == 0) return;                            //if no tabs have been opened stop here -> do not draw anything
            int clickedTabIndex = -1;                                    //index of the clicked tab
            float tabWidth = NodySettings.Instance.GraphTabMaximumWidth; //get the tab max width
            float tabsCount = GraphTabs.Count;                           //count the tabs
            bool calculateTabWith = true;                                //start dynamic tab width calculations 
            while (calculateTabWith)
            {
                float totalSpaceWidth = tabsCount * 2 * NodySettings.Instance.GraphTabElementSpacing;
                float totalDividersWidth = (tabsCount - 1) * NodySettings.Instance.GraphTabDividerWidth;
                float totalTabsAvailableWidth = GraphTabsRect.width - totalSpaceWidth - totalDividersWidth;
                float calculatedTabWidth = totalTabsAvailableWidth / tabsCount;

                if (calculatedTabWidth > NodySettings.Instance.GraphTabMaximumWidth)
                {
                    calculateTabWith = false;
                    tabWidth = NodySettings.Instance.GraphTabMaximumWidth;
                }
                else if (calculatedTabWidth >= NodySettings.Instance.GraphTabMinimumWidth)
                {
                    calculateTabWith = false;
                    tabWidth = calculatedTabWidth;
                }
                else
                {
                    tabsCount--;
                }
            }

            float x = GraphTabsRect.x;

            //Start drawing the tabs
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(NodySettings.Instance.GraphTabElementSpacing);
                x += NodySettings.Instance.GraphTabElementSpacing;
                for (int index = 0; index < tabsCount; index++)
                {
                    if (index > 0)
                    {
                        float dividerHeight = NodySettings.Instance.GraphTabsAreaHeight - NodySettings.Instance.GraphTabElementSpacing * 3;
                        GUILayout.BeginVertical(GUILayout.Height(GraphTabsRect.height));
                        {
                            GUILayout.Space((GraphTabsRect.height - dividerHeight) / 2 - DGUI.Properties.Space());
                            Color color = GUI.color;
                            GUI.color = DGUI.Colors.TextColor(DGUI.Colors.DisabledTextColorName).WithAlpha(color.a * 0.4f);
                            GUILayout.Label(GUIContent.none, DGUI.Properties.White,
                                            GUILayout.Width(NodySettings.Instance.GraphTabDividerWidth),
                                            GUILayout.Height(dividerHeight));
                            GUI.color = color;
                        }
                        GUILayout.EndVertical();
                        GUILayout.Space(NodySettings.Instance.GraphTabElementSpacing);
                        x += NodySettings.Instance.GraphTabElementSpacing;
                        x += NodySettings.Instance.GraphTabDividerWidth;
                        x += NodySettings.Instance.GraphTabElementSpacing;
                    }

                    GraphTab graphTab = GraphTabs[index];
                    var buttonRect = new Rect(x, GraphTabsRect.y + NodySettings.Instance.GraphTabElementSpacing, tabWidth, GraphTabsRect.height - 2 * NodySettings.Instance.GraphTabElementSpacing);

                    GUI.color = InitialGUIColor;
                    if (index < tabsCount - 1) GUI.color = GUI.color.WithAlpha(DGUI.Utility.IsProSkin ? 0.6f : 0.8f);
                    if (graphTab.DrawButton(buttonRect))
                    {
                        if (graphTab.IsRootTab && graphTab.Graph != null && CurrentGraph != graphTab.Graph)
                            OpenGraph(graphTab.Graph, true, false, false);
                        else if (!graphTab.IsRootTab && graphTab.SubGraphNode != null && graphTab.SubGraphNode.SubGraph != null && CurrentGraph != graphTab.SubGraphNode.SubGraph) OpenGraph(graphTab.SubGraphNode.SubGraph, true, false, false);

                        CurrentZoom = graphTab.Zoom;
                        CurrentPanOffset = graphTab.PanOffset;
                        SelectedNodes = new List<Node>(graphTab.SelectedNodes);
                        clickedTabIndex = index;

                        GraphEvent.Send(GraphEvent.EventType.EVENT_GRAPH_OPENED);

                        break;
                    }

                    GUILayout.Space(tabWidth);
                    x += tabWidth;
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            if (clickedTabIndex == -1) return;
            for (int i = GraphTabs.Count - 1; i >= 0; i--)
                if (i > clickedTabIndex)
                    GraphTabs.RemoveAt(i);
        }

        [Serializable]
        public class GraphTab
        {
            private const Size TAB_LABEL_SIZE = Size.M;
            private const TextAlign TAB_LABEL_TEXT_ALIGN = TextAlign.Left;

            private GUIStyle TabIcon { get { return IsRootTab ? Styles.GetStyle(Styles.StyleName.IconGraph) : Styles.GetStyle(Styles.StyleName.NodeIconSubGraphNode); } }
            private string TabLabel { get { return IsRootTab ? GetGraphName(Graph) : SubGraphNode.Name; } }
            private ColorName TabBackgroundColorName { get { return IsRootTab ? DGUI.Colors.DisabledBackgroundColorName : DGUI.Colors.SubGraphNodeColorName; } }
            private ColorName TabTextColorName { get { return IsRootTab ? DGUI.Colors.DisabledTextColorName : DGUI.Colors.SubGraphNodeColorName; } }

            public bool IsRootTab;
            public Graph Graph;
            public SubGraphNode SubGraphNode;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public float Zoom;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public Vector2 PanOffset;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public List<Node> SelectedNodes;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public List<Node> DeletedNodes;

            public GraphTab(Graph graph, float zoom, Vector2 panOffset, List<Node> selectedNodes, List<Node> deletedNodes)
            {
                IsRootTab = true;
                Graph = graph;
                SubGraphNode = null;
                Zoom = zoom;
                PanOffset = panOffset;
                SelectedNodes = new List<Node>(selectedNodes);
                DeletedNodes = new List<Node>(deletedNodes);
            }

            public GraphTab(Graph graph, SubGraphNode subGraphNode, float zoom, Vector2 panOffset, List<Node> selectedNodes, List<Node> deletedNodes)
            {
                IsRootTab = false;
                Graph = graph;
                SubGraphNode = subGraphNode;
                Zoom = zoom;
                PanOffset = panOffset;
                SelectedNodes = new List<Node>(selectedNodes);
                DeletedNodes = new List<Node>(deletedNodes);
            }


            public bool DrawButton(Rect buttonRect, UnityAction callback = null)
            {
                bool result = false;

                GUILayout.BeginArea(buttonRect);
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(TabIcon,
                                                           TabLabel,
                                                           TAB_LABEL_SIZE,
                                                           TAB_LABEL_TEXT_ALIGN,
                                                           TabBackgroundColorName,
                                                           TabTextColorName,
                                                           buttonRect.height))
                    {
                        if (callback != null) callback.Invoke();
                        Event.current.Use();
                        result = true;
                    }
                }
                GUILayout.EndArea();


                GUILayout.Space(NodySettings.Instance.GraphTabElementSpacing); //add space between tabs
                return result;
            }
        }
    }
}