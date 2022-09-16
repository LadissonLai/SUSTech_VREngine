// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Editors
{
    public class BaseNodeEditor : EditorBase
    {
        #region Static Properties

        protected static readonly float kBigTitleHeight = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(8);
        protected static readonly float kNodeLineHeight = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
        protected static readonly float kConnectionTypeIconSize = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space();
        protected static readonly float kConnectionTypeIconPadding = (kNodeLineHeight - kConnectionTypeIconSize) / 2;
        protected static readonly float kSmallTitleHeight = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4);
        protected static readonly float kTriggerTypeIconSize = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);

        #endregion

        #region Properties

        protected AnimBool ShowCurveModifier { get { return GetAnimBool("ShowCurveModifier"); } }
        protected Node BaseNode { get { return (Node) target; } }
        protected static ColorName InputColorName { get { return DGUI.Colors.NodyInputColorName; } }
        protected static ColorName OutputColorName { get { return DGUI.Colors.NodyOutputColorName; } }
        protected static GUIStyle InputSocketIconStyle { get { return Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaSignIn); } }
        protected static GUIStyle OutputSocketIconStyle { get { return Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaSignOut); } }

        #endregion

        #region Public Variables

        protected bool NodeUpdated;
//        protected bool HasNotes { get { return GetProperty(PropertyName.m_notes).stringValue.Length > 0; } }

        #endregion

        #region Unity Mehtods

        /// <summary> Called when object becomes enabled and active </summary>
        protected virtual void OnEnable()
        {
            LoadSerializedProperty();
            InitAnimBool();
            GraphEvent.AddReceiver(OnGraphEvent);
        }

        /// <summary> Called when object becomes disabled and inactive </summary>
        protected virtual void OnDisable() { GraphEvent.RemoveReceiver(OnGraphEvent); }

        /// <summary> Called when EditorWindow is closed </summary>
        protected virtual void OnDestroy() { }

        /// <inheritdoc />
        /// <summary> Used to create custom inspectors </summary>
        public override void OnInspectorGUI()
        {
            DGUI.Properties.InspectorWidth(InspectorWidth, out InspectorWidth);
            Event current = Event.current;
            InitialGUIColor = GUI.color;                  //save initial color
            CurrentMousePosition = current.mousePosition; //save a quick access for the mouse position

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (current.type)
            {
                case EventType.Repaint:
                    LastRepaintTime = EditorApplication.timeSinceStartup;
                    break;
                case EventType.MouseDown:
                    if (current.button == 0) LeftMouseButtonIsDown = true; //mark if the left mouse button is down
                    break;
                case EventType.MouseUp:
                    if (current.button == 0) LeftMouseButtonIsDown = false; //mark if the left mouse button is up
                    break;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (current.rawType)
            {
                case EventType.MouseUp:
                    if (current.button == 0) LeftMouseButtonIsDown = false; //mark if the left mouse button is up (this check is performed outside the window area)
                    break;
            }
        }

        /// <summary> Called multiple times per frame on all opened windows </summary>
        protected virtual void Update()
        {
            if (!UseCustomRepaintInterval) return;

            if (EditorApplication.isPlaying && EditorApplication.timeSinceStartup - LastRepaintTime < CustomRepaintIntervalDuringPlayMode ||
                !EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.timeSinceStartup - LastRepaintTime < CustomRepaintIntervalWhileIdle)
                Repaint();
        }

        #endregion

        #region Public Methods

        protected virtual void DrawOnEnterNode() { DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaSignIn), UILabels.OnEnterNode, DGUI.Colors.ActionColorName); }
        protected virtual void DrawOnExitNode() { DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaSignOut), UILabels.OnExitNode, DGUI.Colors.ActionColorName); }
        protected virtual void DrawOnFixedUpdateNode() { DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaRepeatAlt), UILabels.OnNodeFixedUpdate, DGUI.Colors.ActionColorName); }
        protected virtual void DrawOnLateUpdateNode() { DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaRepeatAlt), UILabels.OnNodeLateUpdate, DGUI.Colors.ActionColorName); }
        protected virtual void DrawOnUpdateNode() { DrawBigTitleWithBackground(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaRepeatAlt), UILabels.OnNodeUpdate, DGUI.Colors.ActionColorName); }

        /// <summary> Initializes defined AnimBools OnEnable (after LoadSerializedProperty) </summary>
        protected virtual void InitAnimBool() { }

        /// <summary> Loads defined SerializedProperties OnEnable </summary>
        protected virtual void LoadSerializedProperty() { }

        protected virtual void OnGraphEvent(GraphEvent graphEvent) { UpdateNodeData(); }

        protected virtual void UpdateNodeData() { }

        protected void DrawInputSockets(Node node) { DrawSockets(node, node.InputSockets, InputSocketIconStyle, UILabels.InputConnections, InputColorName); }
        protected void DrawOutputSockets(Node node) { DrawSockets(node, node.OutputSockets, OutputSocketIconStyle, UILabels.OutputConnections, OutputColorName); }

        protected void DrawSockets(Node node, List<Socket> sockets, GUIStyle directionIconStyle, string directionTitle, ColorName directionColorName)
        {
            if (sockets.Count == 0) return;
            DrawSmallTitleWithBackground(directionIconStyle, directionTitle, directionColorName);
            GUILayout.Space(DGUI.Properties.Space());
            for (int i = 0; i < sockets.Count; i++)
            {
                Socket socket = sockets[i];
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginHorizontal(GUILayout.Height(kNodeLineHeight));
                {
                    DrawSocketIndex(i, directionColorName);
                    GUILayout.BeginVertical();
                    {
                        DrawSocketState(socket, socket.AcceptsMultipleConnections);
                        if (DrawCurveModifier(node, socket, ShowCurveModifier)) NodeUpdated = true;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }
        }

        protected void DrawSocketIndex(int index, ColorName colorName)
        {
            float alpha = GUI.color.a;
            GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
            DGUI.Label.Draw(index.ToString(), Size.S, colorName, kNodeLineHeight); //Draw socket index
            GUI.color = GUI.color.WithAlpha(alpha);
            GUILayout.Space(DGUI.Properties.Space(2));
        }

        protected void DrawSocketState(Socket socket, bool showConnectionsCount = false)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                DrawConnectionIcon(socket);
                GUILayout.Space(DGUI.Properties.Space(2));
                if (!socket.IsConnected)
                {
                    DGUI.Icon.Draw(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconError), kNodeLineHeight * 0.6f, kNodeLineHeight, Color.red);
                    GUILayout.Space(DGUI.Properties.Space(2));
                }

                DGUI.Label.Draw(socket.IsConnected
                                    ? showConnectionsCount
                                          ? socket.Connections.Count + " " + UILabels.Connections
                                          : socket.IsInput
                                              ? UILabels.InputConnected
                                              : UILabels.OutputConnected
                                    : UILabels.NotConnected,
                                NORMAL_TEXT_SIZE,
                                socket.IsConnected
                                    ? socket.IsInput
                                          ? InputColorName
                                          : OutputColorName
                                    : DGUI.Colors.DisabledTextColorName, kNodeLineHeight);


                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        protected bool DrawCurveModifier(Node node, Socket socket, AnimBool show)
        {
            if (!socket.IsConnected || ShowCurveModifier.faded <= 0) return false;
            bool curveUpdated = false;
            ColorName colorName = socket.IsInput ? InputColorName : OutputColorName;
            if (DGUI.FoldOut.Begin(ShowCurveModifier, false))
            {
                GUILayout.Space(DGUI.Properties.Space() * show.faded);
                DGUI.Line.Draw(false, colorName,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(3));
                                   DGUI.Icon.Draw(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaSignature), kConnectionTypeIconSize * 0.8f, DGUI.Properties.SingleLineHeight, colorName);
                                   GUILayout.Space(DGUI.Properties.Space(3));
                                   float curveModifier = socket.CurveModifier;
                                   EditorGUI.BeginChangeCheck();
                                   GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                   {
                                       GUILayout.Space(0);
                                       curveModifier = EditorGUILayout.Slider(curveModifier, NodySettings.Instance.MinCurveModifier, NodySettings.Instance.MaxCurveModifier);
                                   }
                                   GUILayout.EndVertical();
                                   if (EditorGUI.EndChangeCheck())
                                   {
                                       Undo.RecordObject(node, "Update Curve Modifier");
                                       socket.CurveModifier = curveModifier;
                                       curveUpdated = true;
                                   }

                                   if (DGUI.Button.IconButton.Reset(DGUI.Properties.SingleLineHeight, colorName))
                                   {
                                       Undo.RecordObject(node, "Update Curve Modifier");
                                       socket.CurveModifier = NodySettings.Instance.DefaultCurveModifier;
                                       curveUpdated = true;
                                   }
                               });
            }

            DGUI.FoldOut.End(ShowCurveModifier, true);
            return curveUpdated;
        }

        protected void DrawRenameButton(string newNodeName)
        {
            if (DGUI.Button.Dynamic.DrawIconButton(
                                                   Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaEdit),
                                                   UILabels.RenameNodeTo + " '" + newNodeName + "'",
                                                   Size.S, TextAlign.Left,
                                                   DGUI.Colors.DisabledBackgroundColorName,
                                                   DGUI.Colors.DisabledTextColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)))
                UpdateNodeName(newNodeName);
        }

        public virtual void UpdateNodeName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) return;
            DoozyUtils.UndoRecordObject(BaseNode, "Rename");
            BaseNode.SetName(newName);
            NodeUpdated = true;
        }

        protected static Color NotConnectedIconColor() { return DGUI.Colors.IconColor(DGUI.Colors.DisabledBackgroundColorName).WithAlpha(DGUI.Properties.TextIconAlphaValue(false)); }
        protected static Color ConnectionTypeIconColor(ColorName directionColorName) { return DGUI.Colors.GetDColor(directionColorName).Normal; }
        protected static Color TriggerTypeIconColor(ColorName directionColorName) { return DGUI.Colors.IconColor(directionColorName); }
        protected static Color FieldsColor(ColorName directionColorName) { return DGUI.Colors.PropertyColor(directionColorName); }

        protected void SendGraphEventNodeUpdated()
        {
            if (!NodeUpdated) return;
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_UPDATED, GetProperty(PropertyName.m_id).stringValue);
            NodeUpdated = false;
        }

        private void DrawNodeIdSizeAndPositionInDebugMode()
        {
            SerializedProperty debugMode = GetProperty(PropertyName.m_debugMode);
            SerializedProperty id = GetProperty(PropertyName.m_id);
            SerializedProperty x = GetProperty(PropertyName.m_x);
            SerializedProperty y = GetProperty(PropertyName.m_y);
            SerializedProperty width = GetProperty(PropertyName.m_width);
            SerializedProperty height = GetProperty(PropertyName.m_height);

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
                        DGUI.Property.Draw(id, UILabels.NodeId, ComponentColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        GUI.enabled = enabled;
                        if (DGUI.Button.Dynamic.DrawIconButton(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaCopy), UILabels.Copy, Size.S, TextAlign.Left, DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                        {
                            EditorGUIUtility.systemCopyBuffer = id.stringValue;
                            Debug.Log(UILabels.NodeId + " '" + id.stringValue + "' " + UILabels.HasBeenAddedToClipboard);
                        }

                        GUI.enabled = false;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(DGUI.Properties.Space());
                    GUILayout.BeginHorizontal();
                    {
                        DGUI.Property.Draw(x, UILabels.X, ComponentColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Property.Draw(y, UILabels.Y, ComponentColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Property.Draw(width, UILabels.Width, ComponentColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Property.Draw(height, UILabels.Height, ComponentColorName);
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.enabled = enabled;
            }

            DGUI.FadeOut.End(debugModeExpanded, true, alpha);
        }

        protected void DrawCustomWidthSlider(ColorName colorName, float defaultValue)
        {
            SerializedProperty width = GetProperty(PropertyName.m_width);

            GUILayout.BeginHorizontal();
            {
                DGUI.Label.Draw(UILabels.NodeWidth, Size.M);
                float value = width.floatValue;
                EditorGUI.BeginChangeCheck();
                value = EditorGUILayout.Slider(GUIContent.none,
                                               value,
                                               NodySettings.Instance.MinNodeWidth,
                                               NodySettings.Instance.MaxNodeWidth,
                                               GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    width.floatValue = value;
                    NodeUpdated = true;
                }

                if (DGUI.Button.IconButton.Reset())
                {
                    width.floatValue = defaultValue;
                    NodeUpdated = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        protected void DrawDebugMode(bool showCurveModifier = false)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Doozy.DrawDebugMode(GetProperty(PropertyName.m_debugMode), ColorName.Red);
                GUILayout.FlexibleSpace();
                if (showCurveModifier)
                    DGUI.Line.Draw(false,
                                   () =>
                                   {
                                       ShowCurveModifier.target = DGUI.Toggle.Switch.Draw(ShowCurveModifier.target, UILabels.ShowCurveModifier, DGUI.Colors.CurveModifierColorName, false, true, false);
                                   });
            }
            GUILayout.EndHorizontal();
            DrawNodeIdSizeAndPositionInDebugMode();
        }

        protected void DrawNodeName(bool allowEdit = true, bool showNotes = true)
        {
            SerializedProperty nameProperty = GetProperty(PropertyName.m_name);
            bool enabled = GUI.enabled;
            GUI.enabled = allowEdit;
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                DGUI.Property.Draw(nameProperty, UILabels.NodeName, ColorName.White, DGUI.Colors.DisabledTextColorName, BaseNode.ErrorNodeNameIsEmpty || BaseNode.ErrorDuplicateNameFoundInGraph);
                if (EditorGUI.EndChangeCheck()) NodeUpdated = true;
                GUILayout.Space(DGUI.Properties.Space());
                GUI.enabled = enabled;
                if (DGUI.Button.Dynamic.DrawIconButton(Editor.Styles.GetStyle(Editor.Styles.StyleName.IconFaCopy), UILabels.Copy, Size.S, TextAlign.Left, DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                {
                    EditorGUIUtility.systemCopyBuffer = nameProperty.stringValue;
                    Debug.Log(UILabels.NodeName + " '" + nameProperty.stringValue + "' " + UILabels.HasBeenAddedToClipboard);
                }
            }
            GUILayout.EndHorizontal();
            
            if(!showNotes || !NodyWindowSettings.Instance.ShowNodeNotes) return;
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeNotes();
        }

        protected void DrawNodeNotes()
        {
            GUI.color = new Color().ColorFrom256(242, 232, 82);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(2));
                DGUI.Label.Draw(UILabels.Notes, Size.S, TextAlign.Left, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight);
                SerializedProperty notesProperty = GetProperty(PropertyName.m_notes);
                string notes = notesProperty.stringValue;
                EditorGUI.BeginChangeCheck();
                notes = EditorGUILayout.TextArea(notes);
                if (EditorGUI.EndChangeCheck())
                {
                    notesProperty.stringValue = notes;
                }
            }
            GUILayout.EndHorizontal();
            GUI.color = InitialGUIColor;
        }

        protected static void DrawConnectionIcon(Socket socket)
        {
            DGUI.Icon.Draw(socket.OverrideConnection
                               ? Styles.GetStyle(Styles.StyleName.ConnectionPointOverrideConnected)
                               : Styles.GetStyle(Styles.StyleName.ConnectionPointMultipleConnected),
                           kConnectionTypeIconSize,
                           kNodeLineHeight,
                           socket.Connections.Count > 0 ? ConnectionTypeIconColor(socket.IsInput ? InputColorName : OutputColorName) : NotConnectedIconColor()); //Draw socket connection type icon (Override / Multiple)
        }

        protected static void DrawBigTitleWithBackground(GUIStyle iconStyle, string label, ColorName backgroundColorName, ColorName textColorName) { DGUI.Doozy.DrawTitleWithIconAndBackground(iconStyle, label.ToUpperInvariant(), Size.XL, kBigTitleHeight, backgroundColorName, textColorName); }
        protected static void DrawBigTitleWithBackground(GUIStyle iconStyle, string label, ColorName colorName) { DGUI.Doozy.DrawTitleWithIconAndBackground(iconStyle, label.ToUpperInvariant(), Size.XL, kBigTitleHeight, colorName, colorName); }
        protected static void DrawSmallTitleWithBackground(GUIStyle iconStyle, string label, ColorName backgroundColorName, ColorName textColorName) { DGUI.Doozy.DrawTitleWithIconAndBackground(iconStyle, label.ToUpperInvariant(), Size.M, kSmallTitleHeight, backgroundColorName, backgroundColorName); }
        protected static void DrawSmallTitleWithBackground(GUIStyle iconStyle, string label, ColorName colorName) { DGUI.Doozy.DrawTitleWithIconAndBackground(iconStyle, label.ToUpperInvariant(), Size.M, kSmallTitleHeight, colorName, colorName); }
        protected static void DrawSmallTitle(GUIStyle iconStyle, string label, ColorName colorName) { DGUI.Doozy.DrawTitleWithIcon(iconStyle, label, Size.M, kSmallTitleHeight, colorName); }

        protected void DrawHorizontalInputOutputHeaderWithBackground()
        {
            GUILayout.BeginHorizontal();
            {
                DrawInputConnectionsSmallTitleWithBackground();
                GUILayout.Space(DGUI.Properties.Space());
                DrawOutputConnectionsSmallTitleWithBackground();
            }
            GUILayout.EndHorizontal();
        }

        protected void DrawInputConnectionsSmallTitleWithBackground() { DrawSmallTitleWithBackground(InputSocketIconStyle, UILabels.InputConnections, InputColorName); }
        protected void DrawOutputConnectionsSmallTitleWithBackground() { DrawSmallTitleWithBackground(OutputSocketIconStyle, UILabels.OutputConnections, OutputColorName); }

        #endregion
    }
}