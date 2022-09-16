// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private static GUIStyle s_dotStyle;
        private static GUIStyle DotStyle { get { return s_dotStyle ?? (s_dotStyle = Styles.GetStyle(Styles.StyleName.NodeDot)); } }

        private Color m_connectionBackgroundColor;
        private Color m_connectionColor;
        private Color m_dotColor;
        private Color m_inputColor;
        private Color m_normalColor;
        private Color m_outputColor;
        private Vector3 m_dotPoint;
        private Vector3[] m_bezierPoints;
        private bool m_animateInput;
        private bool m_animateOutput;
        private float m_connectionAlpha;
        private float m_dotSize;
        private int m_dotPointIndex;
        private int m_numberOfPoints;

        /// <summary> Draws the connection curves of a virtual connection </summary>
        /// <param name="vc">Target virtual connection being drawn</param>
        private void DrawConnectionCurve(VirtualConnection vc)
        {
            //connect
            m_connectionAlpha = 1f;
            if (m_mode == GraphMode.Connect) m_connectionAlpha = 0.5f;

            m_normalColor = DGUI.Utility.IsProSkin
                                ? DGUI.Colors.GetDColor(ColorName.UnityDark).Light
                                : DGUI.Colors.GetDColor(ColorName.UnityLight).Dark;
            m_outputColor = DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Normal;
            m_inputColor = DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Normal;

            m_normalColor.a = m_connectionAlpha;
            m_outputColor.a = m_connectionAlpha;
            m_inputColor.a = m_connectionAlpha;

            m_connectionColor = m_normalColor;

            m_animateInput = false;
            m_animateOutput = false;

            //A node is selected and the Alt Key is not pressed -> show the connection color depending on socket type of this node (if it is an output or an input one)
            if (WindowSettings.SelectedNodes.Count == 1 && !m_altKeyPressed)
            {
//                Node selectedNode = NodesDatabase[m_selectedNodes.Ids[0]];
                Node selectedNode = WindowSettings.SelectedNodes[0];
                if (selectedNode == null) return;
                if (selectedNode.ContainsConnection(vc.ConnectionId))
                {
                    if (selectedNode == vc.OutputNode)
                    {
                        //color for output connection
                        m_connectionColor = m_outputColor;
                        m_animateOutput = true;
                    }

                    if (selectedNode == vc.InputNode)
                    {
                        //color for input connection
                        m_connectionColor = m_inputColor;
                        m_animateInput = true;
                    }
                }
            }

            float currentCurveWidth = NodySettings.Instance.CurveWidth;

            if (EditorApplication.isPlaying)
            {
                if (vc.InputNode.GetConnection(vc.ConnectionId).Ping)
                {
                    vc.Ping.value = true;
                    vc.Ping.target = false;
                    vc.InputNode.GetConnection(vc.ConnectionId).Ping = false;
                }
                else if (vc.OutputNode.GetConnection(vc.ConnectionId).Ping)
                {
                    vc.Ping.value = true;
                    vc.Ping.target = false;
                    vc.OutputNode.GetConnection(vc.ConnectionId).Ping = false;
                }

                if (vc.Ping.faded > 0)
                {
                    m_connectionColor = Color.LerpUnclamped(m_connectionColor, DGUI.Colors.GetDColor(DGUI.Colors.ActionColorName).Normal, vc.Ping.faded);
                    currentCurveWidth = NodySettings.Instance.CurveWidth * (1 + 2 * vc.Ping.faded);
                }
            }
            else if (vc.Ping.faded > 0)
            {
                vc.Ping.value = false;
                vc.Ping.target = false;
            }


            m_dotColor = m_connectionColor;


            if (m_altKeyPressed) //delete mode is enabled -> check if we should color the connection to RED
            {
                //check this connection's points by testing if the mouse if hovering over one of this connection's virtual points
                bool colorTheConnectionRed = vc.InputVirtualPoint == m_currentHoveredVirtualPoint || vc.OutputVirtualPoint == m_currentHoveredVirtualPoint;
                if (colorTheConnectionRed)
                {
                    m_connectionColor = Color.red; //set the connection color to RED -> as the developer might want to remove this connection
                    currentCurveWidth += 1;        //make the red curve just a tiny bit more thick to make it stand out even better
                }
            }

            m_connectionBackgroundColor = new Color(m_connectionColor.r * 0.2f,
                                                    m_connectionColor.g * 0.2f,
                                                    m_connectionColor.b * 0.2f,
                                                    m_connectionAlpha - 0.2f);
            HandleMaterial.SetPass(0);
            HandleUtility.handleMaterial.SetPass(0);

            Handles.DrawBezier(vc.OutputVirtualPoint.Rect.position,
                               vc.InputVirtualPoint.Rect.position,
                               vc.OutputTangent,
                               vc.InputTangent,
                               m_connectionBackgroundColor,
                               null,
                               currentCurveWidth + 2);
            Handles.DrawBezier(vc.OutputVirtualPoint.Rect.position,
                               vc.InputVirtualPoint.Rect.position,
                               vc.OutputTangent,
                               vc.InputTangent,
                               m_connectionColor,
                               null,
                               currentCurveWidth);

            if (!HasFocus) return;                           //if the window does not have focus -> return (DO NOT PLAY ANIMATION)
            if (!MouseInsideWindow) return;                  //if the mouse is not inside the window -> return (DO NOT PLAY ANIMATION)
            if (!m_animateInput && !m_animateOutput) return; //if the animation is not enabled for both points -> return (DO NOT PLAY ANIMATION)

            m_numberOfPoints = (int) (Vector2.Distance(vc.OutputVirtualPoint.Rect.position,
                                                       vc.InputVirtualPoint.Rect.position) *
                                      NodySettings.Instance.CurvePointsMultiplier); //points multiplier - useful for a smooth dot travel - smaller means fewer travel point (makes the point 'jumpy') and higher means more travel points (make the point move smoothly)
            if (m_numberOfPoints <= 0) return;
            m_bezierPoints = Handles.MakeBezierPoints(vc.OutputVirtualPoint.Rect.position,
                                                      vc.InputVirtualPoint.Rect.position,
                                                      vc.OutputTangent,
                                                      vc.InputTangent,
                                                      m_numberOfPoints);

            m_dotPointIndex = 0;
            m_numberOfPoints--; //we set the number of points as the bezierPoints length - 1

            if (m_animateInput)
                m_dotPointIndex = (int) (AnimationTime * m_numberOfPoints);
            else if (m_animateOutput) m_dotPointIndex = m_numberOfPoints - (int) ((1 - AnimationTime) * m_numberOfPoints);

            m_dotPointIndex = Mathf.Clamp(m_dotPointIndex, 0, m_numberOfPoints);

            m_dotPoint = m_bezierPoints[m_dotPointIndex];

            m_dotSize = currentCurveWidth * 2;

            //make the dot a bit brighter
            m_dotColor = new Color(m_dotColor.r * 1.2f,
                                   m_dotColor.g * 1.2f,
                                   m_dotColor.b * 1.2f,
                                   m_dotColor.a);

            GUI.color = m_dotColor;
            GUI.Box(new Rect(m_dotPoint.x - m_dotSize / 2, m_dotPoint.y - m_dotSize / 2, m_dotSize, m_dotSize), "", DotStyle);
            GUI.color = Color.white;
        }

        private void CalculateAllConnectionCurves()
        {
//            bool foundNullVirtualConnection = false;

            foreach (VirtualConnection virtualConnection in ConnectionsDatabase.Values)
            {
                if (virtualConnection == null ||
                    virtualConnection.OutputNode == null ||
                    virtualConnection.OutputSocket == null ||
                    virtualConnection.InputNode == null ||
                    virtualConnection.InputSocket == null)
                    continue;

                CalculateConnectionCurve(virtualConnection);
            }

//            if (foundNullVirtualConnection) ConstructGraphGUI();
        }

        private void CalculateAllPointRects()
        {
            foreach (string key in PointsDatabase.Keys)
            foreach (VirtualPoint virtualPoint in PointsDatabase[key])
                virtualPoint.CalculateRect();

            m_recalculateAllPointRects = false;
        }

        private void WhileDraggingUpdateSelectedNodes()
        {
            if (m_mode != GraphMode.Drag) return;

            bool validatePointsDatabase = false;      //bool that triggers a PointsDatabase validation
            bool validateConnectionsDatabase = false; //bool that triggers a ConnectionsDatabase validation

            foreach (Node selectedNode in WindowSettings.SelectedNodes) //go through all the selected nodes
            {
                foreach (Socket outputSocket in selectedNode.OutputSockets) //get node's output sockets
                {
                    List<VirtualPoint> virtualPoints = PointsDatabase[outputSocket.Id]; //get output socket's virtual points list
                    if (virtualPoints == null)                                          //list is null -> trigger validation
                    {
                        validatePointsDatabase = true;
                        continue;
                    }

                    foreach (VirtualPoint virtualPoint in virtualPoints)
                    {
                        if (virtualPoint == null) //point is null -> trigger validation
                        {
                            validatePointsDatabase = true;
                            continue;
                        }

                        if (virtualPoint.Node == null) //point parent Node is null -> trigger validation
                        {
                            validatePointsDatabase = true;
                            continue;
                        }

                        virtualPoint.CalculateRect(); //recalculate the rect to reflect the new values (the new WorldPosition)
                    }
                }

                foreach (Socket inputSocket in selectedNode.InputSockets) //get the node's input sockets
                {
                    List<VirtualPoint> virtualPoints = PointsDatabase[inputSocket.Id]; //get input socket's virtual points list
                    if (virtualPoints == null)                                         //list is null -> trigger validation
                    {
                        validatePointsDatabase = true;
                        continue;
                    }

                    foreach (VirtualPoint virtualPoint in virtualPoints)
                    {
                        if (virtualPoint == null) //point is null -> trigger validation
                        {
                            validatePointsDatabase = true;
                            continue;
                        }

                        if (virtualPoint.Node == null) //point parent Node is null -> trigger validation
                        {
                            validatePointsDatabase = true;
                            continue;
                        }

                        virtualPoint.CalculateRect(); //recalculate the rect to reflect the new values (the new WorldPosition)
                    }
                }

                foreach (VirtualConnection vc in ConnectionsDatabase.Values) //get all the virtual connections in the graph
                {
                    //check the virtual connections for nulls -> if any null is found -> trigger validation
                    if (vc == null ||
                        vc.OutputNode == null ||
                        vc.OutputSocket == null ||
                        vc.OutputVirtualPoint == null ||
                        vc.InputNode == null ||
                        vc.InputSocket == null ||
                        vc.InputVirtualPoint == null)
                    {
                        validateConnectionsDatabase = true;
                        continue;
                    }

                    if (vc.InputNode.Id != selectedNode.Id && vc.OutputNode.Id != selectedNode.Id) continue;
                    CalculateConnectionCurve(vc); //recalculate the connection curve to reflect the new values (the new WorldPosition)
                }
            }

            if (validatePointsDatabase) ValidatePointsDatabase();
            if (validateConnectionsDatabase) ValidateConnectionsDatabase();
        }

        private void CalculateConnectionCurve(VirtualConnection vc)
        {
            //get the lists of all the calculated virtual points for both sockets
            List<VirtualPoint> outputVirtualPoints = PointsDatabase[vc.OutputSocket.Id];
            List<VirtualPoint> inputVirtualPoints = PointsDatabase[vc.InputSocket.Id];

            //get both OutputSocket and InputSocket rects converted to WorldSpace
            Rect outputSocketWorldRect = GetSocketWorldRect(vc.OutputSocket);
            Rect inputSocketWorldRect = GetSocketWorldRect(vc.InputSocket);

            //get position values needed to determine the connection points and curve settings
//            float outputSocketCenter = (outputSocketWorldRect.center.x + PanOffset.x) / Zoom;
            float outputSocketCenter = outputSocketWorldRect.center.x;
//            float inputSocketCenter = (inputSocketWorldRect.center.x + PanOffset.x) / Zoom;
            float inputSocketCenter = inputSocketWorldRect.center.x;

            //get the closest virtual points for both sockets
            float minDistance = 100000;
            foreach (VirtualPoint outputVirtualPoint in outputVirtualPoints)
            foreach (VirtualPoint inputVirtualPoint in inputVirtualPoints)
            {
                float currentDistance = Vector2.Distance(outputVirtualPoint.Rect.position, inputVirtualPoint.Rect.position);
                if (currentDistance > minDistance) continue;
                vc.OutputVirtualPoint = outputVirtualPoint;
                vc.InputVirtualPoint = inputVirtualPoint;
                minDistance = currentDistance;
            }

            //set both the output and the input points as their respective tangents
            Vector2 zoomedPanOffset = CurrentPanOffset / CurrentZoom;

            Vector2 outputPoint = vc.OutputVirtualPoint.Rect.position - zoomedPanOffset;
            Vector2 inputPoint = vc.InputVirtualPoint.Rect.position - zoomedPanOffset;
            float outputNodeWidth = vc.OutputNode.GetWidth();
            float inputNodeWidth = vc.InputNode.GetWidth();
            float widthDifference = outputNodeWidth > inputNodeWidth ? outputNodeWidth - inputNodeWidth : inputNodeWidth - outputNodeWidth;

            vc.OutputTangent = outputPoint + zoomedPanOffset;
            vc.InputTangent = inputPoint + zoomedPanOffset;

            //UP TO THIS POINT WE HAVE A STRAIGHT LINE
            //from here we start calculating the custom tangent values -> thus turning our connection line into a dynamic curve

            Vector2 outputTangentArcDirection; //output point tangent
            Vector2 inputTangentArcDirection;  //input point tangent

            //OUTPUT RIGHT CONNECTION
            if (outputSocketCenter < inputSocketCenter && outputSocketCenter <= inputPoint.x ||
                outputSocketCenter >= inputSocketCenter && outputPoint.x >= inputSocketCenter && outputSocketCenter <= inputPoint.x)
            {
//                DDebug.Log("OUTPUT RIGHT");
                if (outputPoint.x <= inputSocketCenter + widthDifference / 2 && inputSocketCenter > outputPoint.x)
                {
                    outputTangentArcDirection = Vector2.right;
                    inputTangentArcDirection = Vector2.left;
                }
                else
                {
                    outputTangentArcDirection = Vector2.right;
                    inputTangentArcDirection = Vector2.right;
                }
            }
            //OUTPUT LEFT CONNECTION
            else
            {
//                DDebug.Log("OUTPUT LEFT");
                if (outputPoint.x >= inputSocketCenter)
                {
                    outputTangentArcDirection = Vector2.left;
                    inputTangentArcDirection = Vector2.right;
                }
                else
                {
                    outputTangentArcDirection = Vector2.left;
                    inputTangentArcDirection = Vector2.left;
                }
            }

            //set the curve strength (curvature) to be dynamic, by taking into account the distance between the connection points
            float outputCurveStrength = minDistance * (NodySettings.Instance.CurveStrengthModifier + vc.OutputSocket.CurveModifier);
            float inputCurveStrength = minDistance * (NodySettings.Instance.CurveStrengthModifier + vc.InputSocket.CurveModifier);
            //update the tangents with the dynamic values
            vc.OutputTangent += outputTangentArcDirection * outputCurveStrength;
            vc.InputTangent += inputTangentArcDirection * inputCurveStrength;
        }
    }
}