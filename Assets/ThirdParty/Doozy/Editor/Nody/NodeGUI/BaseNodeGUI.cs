// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Nody.Windows;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable NotAccessedField.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.Nody.NodeGUI
{
    [Serializable]
    [CustomNodeGUI(typeof(Node))]
    public abstract class BaseNodeGUI : NodeGUIBase<BaseNodeGUI, BaseNodeGUI.CustomNodeGUI, Node>
    {
        /// <summary> Direct reference to the active language pack </summary>
        protected static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        protected static readonly float kSocketIconSize = DGUI.Properties.SingleLineHeight - DGUI.Properties.Space();

        [SerializeField] private int m_windowId;
        [SerializeField] private Node m_node;
        [SerializeField] private Graph m_graph;
        [SerializeField] private string m_normalStyleName;
        [SerializeField] private string m_selectedStyleName;
        [SerializeField] private string m_iconStyleName;
        [SerializeField] private string m_headerNormalStyleName;
        [SerializeField] private string m_headerSelectedStyleName;
        [SerializeField] private bool m_useNodeCustomColor;
        [SerializeField] private Color m_nodeCustomColor;

        [NonSerialized] private float m_dynamicHeight;

        [NonSerialized] private GUIStyle m_currentStyle;
        [NonSerialized] private GUIStyle m_normalStyle;
        [NonSerialized] private GUIStyle m_selectedStyle;

        [NonSerialized] private GUIStyle m_currentHeaderStyle;
        [NonSerialized] private GUIStyle m_headerNormalStyle;
        [NonSerialized] private GUIStyle m_headerSelectedStyle;

        [NonSerialized] private GUIStyle m_iconStyle;

        [NonSerialized] private Vector2 m_deleteButtonSize;
        [NonSerialized] public Rect DeleteButtonRectInGridSpace;
        [NonSerialized] public Rect DeleteButtonRectInWorldSpace;

        [NonSerialized] private Vector2 m_offset;

        [NonSerialized] private Rect m_drawRect;

        [NonSerialized] public bool IsVisible = false;
        [NonSerialized] public bool ZoomedBeyondSocketDrawThreshold = false;

        [NonSerialized] public bool IsSelected;
        [NonSerialized] public bool IsActive;

        [NonSerialized] private Color m_nodeGlowColor;
        [NonSerialized] private Color m_nodeHeaderAndFooterBackgroundColor;
        [NonSerialized] private Color m_nodeBodyColor;
        [NonSerialized] private Color m_nodeOutlineColor;
        [NonSerialized] private Color m_primaryAccentColor;
        [NonSerialized] private Color m_headerTextAndIconColor;

        [NonSerialized] private Rect m_glowRect;
        [NonSerialized] private Rect m_headerRect;
        [NonSerialized] private Rect m_headerHoverRect;
        [NonSerialized] private Rect m_headerIconRect;
        [NonSerialized] private Rect m_headerTitleRect;
        [NonSerialized] private Rect m_bodyRect;
        [NonSerialized] private Rect m_footerRect;
        [NonSerialized] private Rect m_nodeOutlineRect;

        [NonSerialized] private AnimBool m_ping;

        #region GUISyles

        private static GUIStyle s_nodeArea;
        private static GUIStyle NodeArea { get { return s_nodeArea ?? (s_nodeArea = Styles.GetStyle(Styles.StyleName.NodeArea)); } }
        private static GUIStyle s_nodeBody;
        protected static GUIStyle NodeBody { get { return s_nodeBody ?? (s_nodeBody = Styles.GetStyle(Styles.StyleName.NodeBody)); } }
        private static GUIStyle s_nodeOutline;
        protected static GUIStyle NodeOutline { get { return s_nodeOutline ?? (s_nodeOutline = Styles.GetStyle(Styles.StyleName.NodeOutline)); } }
        private static GUIStyle s_nodeGlow;
        protected static GUIStyle NodeGlowStyle { get { return s_nodeGlow ?? (s_nodeGlow = Styles.GetStyle(Styles.StyleName.NodeGlow)); } }
        private static GUIStyle s_dot;
        private static GUIStyle NodeDot { get { return s_dot ?? (s_dot = Styles.GetStyle(Styles.StyleName.NodeDot)); } }
        private static GUIStyle s_nodeHeader;
        private static GUIStyle NodeHeader { get { return s_nodeHeader ?? (s_nodeHeader = Styles.GetStyle(Styles.StyleName.NodeHeader)); } }
        private static GUIStyle s_iconDebug;
        private static GUIStyle IconDebug { get { return s_iconDebug ?? (s_iconDebug = Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaBug)); } }
        private static GUIStyle s_nodeFooter;
        private static GUIStyle NodeFooter { get { return s_nodeFooter ?? (s_nodeFooter = Styles.GetStyle(Styles.StyleName.NodeFooter)); } }
        private static GUIStyle s_nodeHorizontalDivider;
        private static GUIStyle NodeHorizontalDivider { get { return s_nodeHorizontalDivider ?? (s_nodeHorizontalDivider = Styles.GetStyle(Styles.StyleName.NodeHorizontalDivider)); } }
        private static GUIStyle s_nodeIconPlus;
        protected static GUIStyle NodeIconPlus { get { return s_nodeIconPlus ?? (s_nodeIconPlus = Editor.Styles.GetStyle(Editor.Styles.StyleName.IconPlus)); } }

        #endregion

        #region Properties: WindowId, Node, Graph

        public int WindowId { get { return m_windowId; } }
        public Node Node { get { return m_node; } }
        public Graph Graph { get { return m_graph; } }

        #endregion

        #region Properties: X, Y, Width, Height, Position, Size, Rect

        public float X { get { return m_node.GetX(); } }
        public float Y { get { return m_node.GetY(); } }
        public float Width { get { return m_node.GetWidth(); } }
        public float Height { get { return m_node.GetHeight(); } }
        public Vector2 Position { get { return m_node.GetPosition(); } }
        public Vector2 Size { get { return m_node.GetSize(); } }
        public Rect Rect { get { return m_node.GetRect(); } }

        #endregion

        #region Properties: DrawRect, DynamicHeight, HeaderHeight, HeaderIconSize, HeaderIconPadding

        public Rect DrawRect { get { return m_drawRect; } }
        public float DynamicHeight { get { return m_dynamicHeight; } set { m_dynamicHeight = value; } }
        private static float HeaderHeight { get { return NodySettings.Instance.NodeHeaderHeight; } }
        private static float HeaderIconSize { get { return NodySettings.Instance.NodeHeaderIconSize; } }
        private static float HeaderIconPadding { get { return (HeaderHeight - HeaderIconSize) / 2; } }

        #endregion

        #region DeleteButtonRect, SetDeleteButtonSizeScale

        public Rect DeleteButtonRect
        {
            get
            {
                var buttonPosition = new Vector2(X + Width - m_deleteButtonSize.x / 2 - 6f,
                                                 Y - m_deleteButtonSize.y / 2 + 4f);

                return new Rect(buttonPosition, m_deleteButtonSize);
            }
        }

        public void SetDeleteButtonSizeScale(float value)
        {
            m_deleteButtonSize = new Vector2(NodySettings.Instance.NodeDeleteButtonSize * value,
                                             NodySettings.Instance.NodeDeleteButtonSize * value);
        }

        #endregion

        #region Virtual Methods: Init, OnNodeGUI, OnDoubleClick, GetIconStyle

        public virtual void Init(int windowId, Node node, Graph graph)
        {
            m_windowId = windowId;
            m_node = node;
            m_graph = graph;
//            m_dynamicHeight = node.GetHeight();
            m_ping = new AnimBool(false) {speed = NodySettings.Instance.PingColorChangeSpeed};
        }

        protected virtual void OnNodeGUI()
        {
            DrawNodeBody();
            DrawNodeSockets();
        }

        /// <summary> OnDoubleClick is used in order to be able to run custom node actions when a double click is executed on the node's header/footer in the NodeGraph. </summary>
        public virtual void OnDoubleClick(EditorWindow window) { }

        protected virtual GUIStyle GetIconStyle() { return m_iconStyle ?? (m_iconStyle = NodeDot); }

        #endregion

        #region DrawNodeGUI, DrawNode, DrawNodeBody, DrawNodeSockets, AddFooterRectHeightToNodeHeight

        /// <summary> DrawNodeGUI is called by the Graph Window when the node needs to be drawn </summary>
        public void DrawNodeGUI(Rect graphArea, Vector2 panOffset, float zoomLevel)
        {
            m_offset = panOffset;
            Vector2 windowToGridPosition = m_node.GetPosition() + m_offset / zoomLevel;
            var clientRect = new Rect(windowToGridPosition, m_node.GetSize());
            GUI.Window(m_windowId, clientRect, DrawNode, string.Empty, NodeArea);
        }

        /// <summary> DrawNode is called when this node is drawn and it updates the node's colors and calls the virtual OnNodeGUI method that facilitates the customisation of this particular node </summary>
        /// <param name="id"> Window id for this node </param>
        private void DrawNode(int id)
        {
            Color color = GUI.color;
            GUI.color = GUI.color.WithAlpha(color.a - (DGUI.Utility.IsProSkin ? 0.5f : 0.4f) * Node.IsHovered.faded);
            UpdateColors();
            OnNodeGUI();
            GUI.color = color;
            AddFooterRectHeightToNodeHeight();
            UpdateNodeHeight(DynamicHeight);
        }

        protected void DrawNodeBody()
        {
            Color initialColor = GUI.color;

            DynamicHeight = 0;
            m_drawRect = new Rect(0, 0, Width, Height); //get node draw rect

            float leftX = m_drawRect.x + 6;
            float bodyWidth = m_drawRect.width - 12;
            float debugModeIconSize = DGUI.Properties.SingleLineHeight - DGUI.Properties.Space();

            m_glowRect = new Rect(m_drawRect.x, m_drawRect.y, m_drawRect.width, m_drawRect.height - 2);                                                   //calculate glow rect
            DynamicHeight += 6;                                                                                                                           //Update HEIGHT with outline
            m_headerRect = new Rect(leftX, DynamicHeight, bodyWidth, HeaderHeight);                                                                       //calculate header rect
            m_headerIconRect = new Rect(m_headerRect.x + HeaderIconPadding * 2f, m_headerRect.y + HeaderIconPadding + 1, HeaderIconSize, HeaderIconSize); //calculate icon rect
            m_headerTitleRect = new Rect(m_headerIconRect.xMax + HeaderIconPadding * 1.5f,
                                         m_headerIconRect.y,
                                         m_headerRect.width - (HeaderIconSize + HeaderIconPadding * 5) - (Node.DebugMode ? HeaderIconPadding * 2 + debugModeIconSize : 0),
                                         m_headerIconRect.height);                                                                                          //calculate title rect
            DynamicHeight += m_headerRect.height;                                                                                                           //Update HEIGHT with header
            float footerHeight = NodySettings.Instance.FooterHeight;                                                                                        //calculate footer height
            float bodyHeight = m_drawRect.height - NodySettings.Instance.NodeHeaderHeight - NodySettings.Instance.NodeAccentLineHeight - footerHeight - 12; //calculate body height
            m_bodyRect = new Rect(leftX, DynamicHeight, bodyWidth, bodyHeight);                                                                             //calculate body rect
            m_footerRect = new Rect(leftX, m_drawRect.height - footerHeight - 8, bodyWidth, footerHeight);                                                  //calculate footer rect
            m_nodeOutlineRect = new Rect(m_drawRect.x, m_drawRect.y, m_drawRect.width, m_drawRect.height - 2);

            GUI.color = m_nodeGlowColor;
            GUI.Box(m_glowRect, GUIContent.none, NodeGlowStyle); //node glow

            GUI.color = m_nodeHeaderAndFooterBackgroundColor;
            GUI.Box(m_headerRect, GUIContent.none, NodeHeader); //header background

            GUI.color = m_headerTextAndIconColor;
            GUI.Box(m_headerIconRect, GUIContent.none, GetIconStyle()); //header icon

            GUI.color = initialColor;
            GUIStyle titleStyle = DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.L, TextAlign.Left), m_headerTextAndIconColor);
            GUI.Label(m_headerTitleRect, target.Name, titleStyle); //header title

            if (Node.DebugMode)
            {
                GUI.color = m_primaryAccentColor;
                var debugIconRect = new Rect(m_headerRect.xMax - HeaderIconPadding * 2 - debugModeIconSize,
                                             m_headerRect.y + (m_headerRect.height - debugModeIconSize) / 2 + 1,
                                             debugModeIconSize,
                                             debugModeIconSize);
                GUI.Box(debugIconRect, GUIContent.none, IconDebug); //header debugMode icon
            }

            GUI.color = m_nodeBodyColor;
            GUI.Box(m_bodyRect, GUIContent.none, NodeBody); //body background

            GUI.color = m_nodeHeaderAndFooterBackgroundColor;
            GUI.Box(m_footerRect, GUIContent.none, NodeFooter); //footer background

            GUI.color = m_nodeOutlineColor;
            GUI.Box(m_nodeOutlineRect, GUIContent.none, NodeOutline); //node outline

            GUI.color = initialColor; //reset colors
        }

        protected void DrawNodeSockets()
        {
            DrawSocketsList(m_node.InputSockets); //draw the input sockets

            DynamicHeight += NodySettings.Instance.SocketVerticalSpacing;
            GUILayout.Space(NodySettings.Instance.SocketVerticalSpacing);

            DrawSocketsList(m_node.OutputSockets); //draw the output sockets

            DynamicHeight += NodySettings.Instance.SocketVerticalSpacing;
            GUILayout.Space(NodySettings.Instance.SocketVerticalSpacing);
        }

        private void AddFooterRectHeightToNodeHeight()
        {
            //Update HEIGHT
            DynamicHeight += m_footerRect.height + 6 + 2; //6 is the glow offset and 1 is a magic number to show the socket horizontal divider (1 pixel)
        }

        #endregion

        #region DrawSocket, DrawSocketsList, DrawAddSocketButton, DrawConnectionPoints

        protected void DrawSocketsList(List<Socket> sockets)
        {
            if (sockets == null) return;
            foreach (Socket socket in sockets)
                DynamicHeight += DrawSocket(socket).height; //Update HEIGHT          
        }

        protected virtual Rect DrawSocket(Socket socket)
        {
            socket.SetX(0);                                       //set socket to the left side of the node
            socket.SetY(DynamicHeight);                           //set the y to the current draw height
            socket.SetWidth(Node.GetWidth());                     //set the socket width as the node width 
            socket.SetHeight(NodySettings.Instance.SocketHeight); //set the socket height as the default socket height

            if (ZoomedBeyondSocketDrawThreshold) return socket.GetRect();

//            DrawConnectionPoints(socket); //draw connection points -> has been moved so that they are drawn as an info overlay

            //set the socket color to white or black - depending on the current skin - (we assume it's not connected)
            Color socketColor = DGUI.Colors.DisabledTextDColor.Normal;
            Color dividerColor = DGUI.Colors.DisabledTextDColor.Dark;

            //check if the socket is connected in order to color the divider to the input or output color
            if (socket.IsConnected)
            {
                socketColor = socket.IsInput ? DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Normal : DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Normal;
                dividerColor = socket.IsInput
                                   ? DGUI.Utility.IsProSkin
                                         ? DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Normal
                                         : DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Dark
                                   : DGUI.Utility.IsProSkin
                                       ? DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Normal
                                       : DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Dark;
            }

            //in order not to overpower the design with bold colors -> we make the color fade out a bit in order to make the graph easier on the eyes
            float opacity = NodySettings.Instance.SocketDividerOpacity * (DGUI.Utility.IsProSkin ? 1f : 2f);
            socketColor.a = opacity;
            dividerColor.a = opacity * 0.8f;

            //check if we are in delete mode -> if true -> set the socket color to red (ONLY if the socket can be deleted)
//            if (Event.current.alt)
            if (NodyWindow.Instance.m_altKeyPressed)
            {
                //since we're in delete mode and this socket can be deleted -> set its color to red
                if (Node.CanDeleteSocket(socket))
                {
                    socketColor = Color.red;
                    dividerColor = Color.red;
                }

                //since we're in delete mode -> make the socket color a bit stronger (regardless if it can be deleted or not) -> this is a design choice
                socketColor.a = opacity * 1.2f;
                dividerColor.a = opacity * 1.2f;
            }


            //calculate the top divider rect position and size -> this is the thin line at the bottom of every socket (design choice)
            var topDividerRect = new Rect(socket.GetRect().x + 6,
                                          socket.GetRect().y + NodySettings.Instance.SocketDividerHeight,
                                          socket.GetRect().width - 12,
                                          NodySettings.Instance.SocketDividerHeight);


            //color the gui to the defined socket color
            GUI.color = dividerColor;
            //DRAW the horizontal divider at the top of the socket
            GUI.Box(topDividerRect, GUIContent.none, NodeHorizontalDivider);
            //reset the gui color            
            GUI.color = Color.white;


            //calculate the bottom divider rect position and size -> this is the thin line at the bottom of every socket (design choice)
            var bottomDividerRect = new Rect(socket.GetRect().x + 6,
                                             socket.GetRect().y + socket.GetRect().height - NodySettings.Instance.SocketDividerHeight,
                                             socket.GetRect().width - 12,
                                             NodySettings.Instance.SocketDividerHeight);


            //color the gui to the defined socket color
            GUI.color = dividerColor;
            //DRAW the horizontal divider at the bottom of the socket
            GUI.Box(bottomDividerRect, GUIContent.none, NodeHorizontalDivider);
            //reset the gui color            
            GUI.color = Color.white;

            //calculate the socket hover rect -> this is the 'selection' box that appears when the mouse is over the socket
            socket.UpdateHoverRect();

            if (socket.ShowHover.faded > 0.01f)
            {
                var animatedRect = new Rect(socket.HoverRect.x,
                                            socket.HoverRect.y + socket.HoverRect.height * (1 - socket.ShowHover.faded),
                                            socket.HoverRect.width,
                                            socket.HoverRect.height * socket.ShowHover.faded);


                if (socket.IsConnected)
                    switch (socket.GetDirection())
                    {
                        case SocketDirection.Input:
                            GUI.color = DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Normal.WithAlpha(0.1f);
                            break;
                        case SocketDirection.Output:
                            GUI.color = DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Normal.WithAlpha(0.1f);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }


                GUI.color = socketColor;                                       //color the gui to the defined socket color
                GUI.Box(animatedRect, GUIContent.none, NodeHorizontalDivider); //DRAW the animated overlay when the mouse is hovering over this socket 
                GUI.color = Color.white;                                       //reset the gui color            
            }

            return socket.GetRect();
        }

        protected void DrawAddSocketButton(SocketDirection direction, ConnectionMode connectionMode, Type valueType)
        {
            var area = new Rect(6, DynamicHeight, Node.GetWidth() - 12, NodySettings.Instance.SocketHeight);
            DynamicHeight += area.height; //Update HEIGHT

            if (ZoomedBeyondSocketDrawThreshold) return;

            bool isInput = direction == SocketDirection.Input;
            Color iconColor = DGUI.Colors.IconColor(isInput ? DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName) : DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName)).WithAlpha(NodySettings.Instance.NormalOpacity);


            var iconRect = new Rect(area.x + area.width / 2 - NodySettings.Instance.NodeAddSocketButtonSize / 2,
                                    area.y + area.height / 2 - NodySettings.Instance.NodeAddSocketButtonSize / 2,
                                    NodySettings.Instance.NodeAddSocketButtonSize,
                                    NodySettings.Instance.NodeAddSocketButtonSize);
            var hoveredIconRect = new Rect(iconRect.x - iconRect.width * 0.4f / 2,
                                           iconRect.y - iconRect.height * 0.4f / 2,
                                           iconRect.width * 1.4f,
                                           iconRect.height * 1.4f);

            bool mouseIsOverButton = hoveredIconRect.Contains(Event.current.mousePosition);
            if (mouseIsOverButton) iconColor.a = NodySettings.Instance.HoverOpacity;


            Color color = GUI.color;
            GUI.color = iconColor;
            if (GUI.Button(mouseIsOverButton ? hoveredIconRect : iconRect, GUIContent.none, NodeIconPlus))
            {
                Undo.RecordObject(Node, "Add " + direction + " Socket");
                if (isInput)
                    Node.AddInputSocket(connectionMode, valueType, true, true);
                else
                    Node.AddOutputSocket(connectionMode, valueType, true, true);
                GraphEvent.Send(GraphEvent.EventType.EVENT_SOCKET_CREATED, Node.Id);
            }

            GUI.color = color;
        }

        protected virtual void DrawConnectionPoints(Socket socket)
        {
//            foreach (Vector2 position in socket.ConnectionPoints)
//            {
//                var rect = new Rect(position.x,
//                                    position.y + DynamicHeight,
//                                    NodySettings.Instance.ConnectionPointWidth,
//                                    NodySettings.Instance.ConnectionPointHeight);
//            }
        }

        #endregion

        #region UpdateNodeWidth, UpdateNodeHeight, UpdateNodePosition

        protected void UpdateNodeWidth(float width) { m_node.SetWidth(width); }
        protected void UpdateNodeHeight(float height) { m_node.SetHeight(height); }
        protected void UpdateNodePosition(Vector2 position) { m_node.SetPosition(position); }

        #endregion

        #region Colors

        private void UpdateColors()
        {
            m_nodeGlowColor = Color.black.WithAlpha(GUI.color.a * NodySettings.Instance.NodeGlowOpacity);
            m_nodeBodyColor = DGUI.Utility.IsProSkin ? Color.black.Lighter() : Color.white.Darker();

            var accentColorName = ColorName.Gray;
            ColorName headerTextColorName = DGUI.Utility.IsProSkin ? ColorName.UnityLight : ColorName.UnityDark;

            if (IsActive)
            {
                accentColorName = DGUI.Colors.ActionColorName;
                headerTextColorName = accentColorName;
            }
            else if (Node.HasErrors)
            {
                accentColorName = ColorName.Red;
                headerTextColorName = accentColorName;
            }
            else if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                switch (Node.NodeType)
                {
                    case NodeType.SubGraph:
                    {
                        accentColorName = DGUI.Colors.SubGraphNodeColorName;
                        headerTextColorName = accentColorName;
                        break;
                    }
                    case NodeType.Start:
                    {
                        accentColorName = DGUI.Colors.StartNodeColorName;
                        headerTextColorName = accentColorName;
                        break;
                    }
                    case NodeType.Enter:
                    {
                        accentColorName = DGUI.Colors.EnterNodeColorName;
                        headerTextColorName = accentColorName;
                        break;
                    }
                    case NodeType.Exit:
                    {
                        accentColorName = DGUI.Colors.ExitNodeColorName;
                        headerTextColorName = accentColorName;
                        break;
                    }
                }
            }

            m_primaryAccentColor = DGUI.Colors.TextColor(accentColorName, true);
            m_headerTextAndIconColor = DGUI.Colors.TextColor(headerTextColorName, true);

            m_nodeOutlineColor = DGUI.Colors.TextColor(accentColorName, true);
            m_nodeOutlineColor.a = GUI.color.a * (IsSelected ? 0.8f : IsActive ? 1f : 0f);

            m_nodeBodyColor.a = GUI.color.a * (IsSelected ? 1f : NodySettings.Instance.NodeBodyOpacity);

            m_nodeHeaderAndFooterBackgroundColor = IsActive
                                                       ? DGUI.Utility.IsProSkin ? DGUI.Colors.GetDColor(accentColorName).Dark : DGUI.Colors.GetDColor(accentColorName).Normal
                                                       : DGUI.Utility.IsProSkin
                                                           ? DGUI.Colors.GetDColor(accentColorName).Dark
                                                           : !Node.HasErrors && (EditorApplication.isPlayingOrWillChangePlaymode || Node.NodeType == NodeType.General)
                                                               ? DGUI.Colors.GetDColor(accentColorName).Light.Lighter().Lighter()
                                                               : DGUI.Colors.GetDColor(accentColorName).Light;
            m_nodeHeaderAndFooterBackgroundColor.a = GUI.color.a * (IsSelected ? 1f : 0.8f);

            //NODE PING COLOR
            if (EditorApplication.isPlaying)
            {
                if (Node.Ping)
                {
                    m_ping.value = true;
                    m_ping.target = false;
                    Node.Ping = false;
                }

                if (m_ping.faded > 0)
                {
                    m_nodeGlowColor = Color.LerpUnclamped(m_nodeGlowColor, NodeActiveGlowColor, m_ping.faded);
                    m_nodeOutlineColor = Color.LerpUnclamped(m_nodeOutlineColor, Node.HasErrors ? NodeErrorOutlineColor : NodeActiveOutlineColor, m_ping.faded);
                    m_nodeHeaderAndFooterBackgroundColor = Color.LerpUnclamped(m_nodeHeaderAndFooterBackgroundColor, NodeActiveHeaderAndFooterColor, m_ping.faded);
                    m_primaryAccentColor = Color.LerpUnclamped(m_primaryAccentColor, PrimaryActiveAccentColor, m_ping.faded);
                    m_headerTextAndIconColor = Color.LerpUnclamped(m_headerTextAndIconColor, PrimaryActiveAccentColor, m_ping.faded);
                }
            }
            else if (IsActive || Node.Ping || m_ping.faded > 0)
            {
                IsActive = false;
                Node.Ping = false;
                m_ping.value = false;
                m_ping.target = false;
            }
        }

        private Color NodeGlowColor(ColorName colorName) { return DGUI.Utility.IsProSkin ? DGUI.Colors.GetDColor(colorName).Dark : DGUI.Colors.GetDColor(colorName).Normal; }
        private Color NodeErrorGlowColor { get { return NodeGlowColor(ColorName.Red); } }
        private Color NodeActiveGlowColor { get { return NodeGlowColor(DGUI.Colors.ActionColorName); } }

        private Color NodeOutlineColor(ColorName colorName) { return DGUI.Colors.IconColor(colorName, true); }
        private Color NodeErrorOutlineColor { get { return NodeOutlineColor(ColorName.Red); } }
        private Color NodeActiveOutlineColor { get { return NodeOutlineColor(DGUI.Colors.ActionColorName); } }

        private Color NodeErrorHeaderAndFooterColor { get { return DGUI.Utility.IsProSkin ? DGUI.Colors.GetDColor(ColorName.Red).Dark : DGUI.Colors.GetDColor(ColorName.Red).Light; } }
        private Color NodeActiveHeaderAndFooterColor { get { return DGUI.Utility.IsProSkin ? DGUI.Colors.GetDColor(DGUI.Colors.ActionColorName).Dark : DGUI.Colors.GetDColor(DGUI.Colors.ActionColorName).Normal; } }

        private Color PrimaryAccentColor(ColorName colorName) { return DGUI.Utility.IsProSkin ? DGUI.Colors.GetDColor(colorName).Light : DGUI.Colors.GetDColor(colorName).Dark; }
        private Color PrimaryErrorAccentColor { get { return PrimaryAccentColor(ColorName.Red); } }
        private Color PrimaryActiveAccentColor { get { return PrimaryAccentColor(DGUI.Colors.ActionColorName); } }

        private Color SecondaryErrorAccentColor { get { return DGUI.Colors.GetDColor(ColorName.Red).Normal; } }
        private Color SecondaryActiveAccentColor { get { return DGUI.Utility.IsProSkin ? DGUI.Colors.GetDColor(DGUI.Colors.ActionColorName).Normal : DGUI.Colors.GetDColor(DGUI.Colors.ActionColorName).Normal.Darker().Darker().Darker(); } }

        #endregion

        protected static void DrawProgressBar(Rect areaRect, float timerProgress, float barHeight, bool alwaysVisible)
        {
            Color initialColor = GUI.color;
            var timerProgressBackgroundRect = new Rect(areaRect.x, areaRect.yMax - barHeight, areaRect.width, barHeight);
            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.1f);
            GUI.color = iconAndTextColor;
            if (alwaysVisible) GUI.Label(timerProgressBackgroundRect, GUIContent.none, DGUI.Properties.White);
            if (timerProgress > 0 && timerProgress < 1)
            {
                if (!alwaysVisible) GUI.Label(timerProgressBackgroundRect, GUIContent.none, DGUI.Properties.White);
                var timerProgressRect = new Rect(timerProgressBackgroundRect.x, timerProgressBackgroundRect.y, timerProgressBackgroundRect.width * timerProgress, timerProgressBackgroundRect.height);
                GUI.color = DGUI.Colors.TextColor(DGUI.Colors.ActionColorName, true);
                GUI.Label(timerProgressRect, GUIContent.none, DGUI.Properties.White);
            }

            GUI.color = initialColor;
        }

        /// <summary>
        ///     Get a safe WindowId that does not collide with others in the list
        /// </summary>
        public static int GetSafeWindowId(IEnumerable<BaseNodeGUI> list) { return list.Select(baseNodeGUI => baseNodeGUI.WindowId).Concat(new[] {-1}).Max() + 1; }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeGUI : Attribute, INodeGUI
        {
            private readonly Type m_inspectedType;

            internal CustomNodeGUI(Type inspectedType) { m_inspectedType = inspectedType; }

            public Type GetInspectedType() { return m_inspectedType; }
        }
    }
}