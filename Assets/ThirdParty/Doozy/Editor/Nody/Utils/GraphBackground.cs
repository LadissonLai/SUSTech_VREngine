// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Doozy.Editor.Nody.Utils
{
// Implementation from UnityEditor.Graphs.GraphGUI
    public static class GraphBackground
    {
        private const float MINOR_GRID_SIZE = 12f;
        private const float MAJOR_GRID_SIZE = 120f;
        
        private static readonly Color GridMinorColorDark = new Color(0f, 0f, 0f, 0.18f);
        private static readonly Color GridMajorColorDark = new Color(0f, 0f, 0f, 0.28f);
        private static readonly Color GridMinorColorLight = new Color(0f, 0f, 0f, 0.1f);
        private static readonly Color GridMajorColorLight = new Color(0f, 0f, 0f, 0.15f);

        private static Color GridMinorColor { get { return DGUI.Utility.IsProSkin ? GridMinorColorDark : GridMinorColorLight; } }
        private static Color GridMajorColor { get { return DGUI.Utility.IsProSkin ? GridMajorColorDark : GridMajorColorLight; } }

        public static void DrawGrid(Rect gridRect, float zoomLevel, Vector2 panOffset)
        {
            if (Event.current.type != EventType.Repaint) return;
            
            //draw background
            UnityEditor.Graphs.Styles.graphBackground.Draw(gridRect, false, false, false, false);
            
            HandleUtility.ApplyWireMaterial();
            GL.PushMatrix();
            GL.Begin(1);
            float t = Mathf.InverseLerp(0.1f, 1f, zoomLevel);
            DrawGridLines(gridRect, MINOR_GRID_SIZE * zoomLevel, Color.Lerp(Color.clear, GridMinorColor, t), panOffset);
            DrawGridLines(gridRect, MAJOR_GRID_SIZE * zoomLevel, Color.Lerp(GridMinorColor, GridMajorColor, t), panOffset);
            GL.End();
            GL.PopMatrix();
        }

        private static void DrawGridLines(Rect gridRect, float gridSize, Color gridColor, Vector2 panOffset)
        {
            //vertical lines
            GL.Color(gridColor);
            float scaledOffsetX = -panOffset.x + panOffset.x % gridSize;
            float x = gridRect.xMin - gridRect.xMin % gridSize + scaledOffsetX;
            while (x < (double) gridRect.xMax + scaledOffsetX)
            {
                DrawLine(new Vector2(x + panOffset.x, gridRect.yMin), new Vector2(x + panOffset.x, gridRect.yMax));
                x += gridSize;
            }

            //horizontal lines
            GL.Color(gridColor);
            float scaledOffsetY = -panOffset.y + panOffset.y % gridSize;
            float y = gridRect.yMin - gridRect.yMin % gridSize + scaledOffsetY;
            while (y < (double) gridRect.yMax + scaledOffsetY)
            {
                DrawLine(new Vector2(gridRect.xMin, y + panOffset.y), new Vector2(gridRect.xMax, y + panOffset.y));
                y += gridSize;
            }
        }

        private static void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        // Implementation from UnityEditor.HandleUtility
        private static class HandleUtility
        {
            private static Material s_handleWireMaterial;
            private static Material s_handleWireMaterial2D;

            internal static void ApplyWireMaterial(CompareFunction zTest = CompareFunction.Always)
            {
                Material handleWireMaterial = HandleWireMaterial;
                handleWireMaterial.SetInt("_HandleZTest", (int) zTest);
                handleWireMaterial.SetPass(0);
            }

            private static Material HandleWireMaterial
            {
                get
                {
                    InitHandleMaterials();
                    return !Camera.current ? s_handleWireMaterial2D : s_handleWireMaterial;
                }
            }

            private static void InitHandleMaterials()
            {
                if (s_handleWireMaterial) return;
                s_handleWireMaterial = (Material) EditorGUIUtility.LoadRequired("SceneView/HandleLines.mat");
                s_handleWireMaterial2D = (Material) EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");
            }
        }
    }
}