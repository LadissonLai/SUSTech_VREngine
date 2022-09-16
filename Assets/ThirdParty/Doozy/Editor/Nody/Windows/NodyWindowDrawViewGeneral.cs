// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private void DrawViewGeneral()
        {
            if (CurrentView != View.General) return;
            m_mode = GraphMode.None;

            var graphArea = new Rect(0, 0, position.width, position.height);
            GraphBackground.DrawGrid(graphArea, 1f, Vector2.zero);

            float splashPadding = 16f;
            float splashTopOffset = 48f;

            bool drawRecentlyOpenedGraphsBar = NodyWindowSettings.Instance.RecentlyOpenedGraphs.Count > 0;
            var recentAreaRect = new Rect(graphArea.width - NodyWindowSettings.RECENT_GRAPHS_AREA_WIDTH, 0, NodyWindowSettings.RECENT_GRAPHS_AREA_WIDTH, graphArea.height);

            var splashArea = new Rect(graphArea.x + splashPadding + ToolbarWidth,
                                      graphArea.y + splashPadding,
                                      graphArea.width - splashPadding * 2 - ToolbarWidth - (drawRecentlyOpenedGraphsBar ? recentAreaRect.width : 0),
                                      graphArea.height - splashPadding * 2);

            float width = 1024;
            float height = 1024;

            float sizeWidthRatio = 1;
            float sizeHeightRatio = 1;

            if (splashArea.width < width) sizeWidthRatio = splashArea.width / width;
            if (splashArea.height - splashTopOffset < height) sizeHeightRatio = (splashArea.height - splashTopOffset) / height;

            float sizeRatio = Mathf.Min(sizeWidthRatio, sizeHeightRatio);

            width *= sizeRatio;
            height *= sizeRatio;

            float x = splashArea.x + splashArea.width / 2 - width / 2;
            float y = splashArea.y + +splashTopOffset + splashArea.height / 2 - height / 2;


            var splashScreenRect = new Rect(x, y, width, height);

            GUI.Box(splashScreenRect, GUIContent.none, SplashScreen);

            if (!drawRecentlyOpenedGraphsBar) return;


            //Recent Area
            EditorGUI.DrawRect(recentAreaRect, (EditorGUIUtility.isProSkin ? Color.black.Lighter() : Color.white.Darker()).WithAlpha(NodyWindowSettings.TOOLBAR_OPACITY * 0.6f));
            GUILayout.BeginArea(recentAreaRect);
            {
                DrawRecentOpenedGraphs();
            }
            GUILayout.EndArea();
        }

        private Vector2 m_recentOpenedGraphsScroll = Vector2.zero;

        private void DrawRecentOpenedGraphs()
        {
            GUILayout.Label(UILabels.Recent, DGUI.Label.Style(Size.S, TextAlign.Center, DGUI.Colors.DisabledTextColorName), GUILayout.ExpandWidth(true), GUILayout.Height(DoozyWindowSettings.Instance.ToolbarHeaderHeight));
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(4));

                m_recentOpenedGraphsScroll = GUILayout.BeginScrollView(m_recentOpenedGraphsScroll);
                {
                    GUILayout.BeginVertical();
                    {
                        foreach (Graph recentlyOpenedGraph in NodyWindowSettings.Instance.RecentlyOpenedGraphs)
                        {
                            if (recentlyOpenedGraph == null) continue;
                            GUILayout.BeginHorizontal();
                            {
                                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(recentlyOpenedGraph.IsSubGraph ? Styles.StyleName.IconSubGraph : Styles.StyleName.IconGraph),
                                                                       recentlyOpenedGraph.name,
                                                                       Size.S, TextAlign.Left,
                                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                                       DGUI.Colors.DisabledTextColorName,
                                                                       DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4)))
                                {
                                    LoadGraph(recentlyOpenedGraph);
                                    break;
                                }

                                if (DGUI.Button.IconButton.Minus(DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4)))
                                {
                                    DoozyUtils.UndoRecordObject(NodyWindowSettings.Instance, "Remove Recent");
                                    NodyWindowSettings.Instance.RecentlyOpenedGraphs.Remove(recentlyOpenedGraph);
                                    NodyWindowSettings.Instance.SetDirty(false);
                                    break;
                                }
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(DGUI.Properties.Space(2));
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();

                GUILayout.Space(DGUI.Properties.Space(4));
            }
            GUILayout.EndHorizontal();
        }
    }
}