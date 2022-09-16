// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private AnimBool m_isDraggingAnimBool;

        private void DrawViewGraph()
        {
            if (CurrentView != View.Graph) return;
            Event current = Event.current;

            if (m_recalculateAllPointRects && current.type == EventType.Repaint)
            {
                CalculateAllPointRects();
                CalculateAllConnectionCurves();
            }

            var graphArea = new Rect(0, 0, position.width, position.height);
            GraphBackground.DrawGrid(graphArea, CurrentZoom, CurrentPanOffset);

            m_graphAreaIncludingTab = new Rect(0, DGUI.Properties.StandardWindowTabHeight, position.width, position.height);
            m_scaledGraphArea = new Rect(0,
                                         0,
                                         graphArea.width / CurrentZoom,
                                         graphArea.height / CurrentZoom);

            Matrix4x4 initialMatrix = GUI.matrix; //save initial matrix
            GUI.EndClip();
            GUI.BeginClip(new Rect(m_graphAreaIncludingTab.position, m_scaledGraphArea.size));
            {
                Matrix4x4 translation = Matrix4x4.TRS(m_graphAreaIncludingTab.position, Quaternion.identity, Vector3.one);
                Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * CurrentZoom);
                GUI.matrix = translation * scale * translation.inverse;
                {
                    DrawConnections();
                    DrawNodes(graphArea);
                    DrawSocketsConnectionPoints();
                    DrawLineFromSocketToPosition(m_activeSocket, CurrentMousePosition);
                    DrawSelectionBox();
                }
            }
            GUI.EndClip();
            GUI.BeginClip(m_graphAreaIncludingTab);
            GUI.matrix = initialMatrix; //reset the matrix to the initial value

//            DrawOverlay();

//            DrawGraphName();
//            DrawGraphBottomInfoBar();
//            DrawGraphToolbar();

//            DrawGraphModes();
//            DrawGraphInfo();
//            DrawWindowInfo();

            HandleZoom();
            HandlePanning();


            if (current.mousePosition.x > ToolbarWidth &&
                current.mousePosition.y > NodySettings.Instance.GraphTabsAreaHeight + DGUI.Properties.Space() ||
                m_mode != GraphMode.None)
            {
                HandleMouseHover();
                HandleMouseMiddleClicks();
                HandleMouseLeftClicks();
                HandleMouseRightClicks();
            }

            HandleKeys();

            WhileDraggingUpdateSelectedNodes();

            UpdateNodesActiveNode();
        }

        // ReSharper disable once UnusedMember.Local
        private void DrawGraphModes()
        {
            m_connectMode.target = m_mode == GraphMode.Connect;
            m_selectMode.target = m_mode == GraphMode.Select;
            m_dragMode.target = m_mode == GraphMode.Drag;
            m_panMode.target = m_mode == GraphMode.Pan;
            m_deleteMode.target = m_mode == GraphMode.Delete;

//            return;

            DrawGraphMode("Connect Mode", Color.yellow, m_connectMode);
            DrawGraphMode("Select Mode", Color.magenta, m_selectMode);
            DrawGraphMode("Drag Mode", Color.green, m_dragMode);
            DrawGraphMode("Pan Mode", Color.cyan, m_panMode);
            DrawGraphMode("Delete Mode", Color.red, m_deleteMode);
        }

        private void DrawGraphMode(string modeName, Color modeColor, AnimBool showMode)
        {
            if (showMode.faded < 0.01f) return;
            float size = 32;
            var topRect = new Rect(0, 0, position.width, size);
            var bottomRect = new Rect(0, position.height - size, position.width, size);
            var leftRect = new Rect(0, 0, size, position.height);
            var rightRect = new Rect(position.width - size, 0, size, position.height);
            GUI.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(modeColor.r, modeColor.g, modeColor.b, 0.2f * showMode.faded), showMode.faded);

            GUI.Box(topRect, GUIContent.none, WhiteGradientTopToBottom);
            GUI.Box(bottomRect, GUIContent.none, WhiteGradientBottomToTop);
            GUI.Box(leftRect, GUIContent.none, WhiteGradientLeftToRight);
            GUI.Box(rightRect, GUIContent.none, WhiteGradientRightToLeft);

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);

            var tempContent = new GUIContent(modeName);
            Vector2 titleSize = GUI.skin.box.CalcSize(tempContent);
            var titleRect = new Rect(position.width / 2 - titleSize.x / 2 - 8,
                                     topRect.y - titleSize.y * (1 - showMode.faded),
                                     titleSize.x + 16,
                                     titleSize.y);

            GUI.Box(titleRect, GUIContent.none, ToolbarTabTopToBottom);
            GUI.color = Color.black;
            GUI.Box(titleRect, tempContent, new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
            GUI.color = Color.white;
        }

        private void DrawOpenedGraphAssetPath()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(3));
                GUI.color = InitialGUIColor.WithAlpha(0.6f);
                DGUI.Label.Draw(new GUIContent(m_graphAssetPath + (CurrentGraph.IsDirty ? "*" : "")), Size.S, TextAlign.Left, DGUI.Colors.DisabledBackgroundColorName, DGUI.Properties.SingleLineHeight);
                GUI.color = InitialGUIColor;
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        // ReSharper disable once UnusedMember.Local
        private void DrawGraphName()
        {
            if (CurrentGraph == null) return;
            var graphNameRect = new Rect(6, 2, position.width, 32);
            GUI.color = new Color(0.92f, 0.92f, 0.92f, 0.35f);
            GUI.Label(graphNameRect, new GUIContent(GetGraphName(CurrentGraph)), new GUIStyle(GUI.skin.label)
                                                                                 {
                                                                                     alignment = TextAnchor.UpperLeft,
                                                                                     fontSize = 14,
                                                                                     padding = new RectOffset(),
                                                                                     margin = new RectOffset(),
                                                                                     border = new RectOffset()
                                                                                 });
            GUI.color = Color.white;
        }

        // ReSharper disable once UnusedMember.Local
        private void DrawGraphBottomInfoBar()
        {
            if (CurrentGraph == null) return;
            var bottomInfoBar = new Rect(0, position.height - 24, position.width, 24);
            GUI.color = new Color(0f, 0f, 0f, 0.2f);
            GUI.Box(bottomInfoBar, GUIContent.none, WhiteGradientBottomToTop);
            GUI.color = new Color(0.92f, 0.92f, 0.92f, 0.35f);
            GUI.Label(bottomInfoBar, new GUIContent(m_graphAssetPath + (CurrentGraph.IsDirty ? "*" : "")), new GUIStyle(GUI.skin.label)
                                                                                                           {
                                                                                                               alignment = TextAnchor.MiddleLeft,
                                                                                                               fontSize = 10,
                                                                                                               padding = new RectOffset(6, 0, 0, 2),
                                                                                                               margin = new RectOffset(),
                                                                                                               border = new RectOffset()
                                                                                                           });
            GUI.color = Color.white;
        }
    }
}