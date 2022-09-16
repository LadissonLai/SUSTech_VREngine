// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PossibleLossOfFraction

namespace Doozy.Editor.Internal
{
    public class BaseEditorWindow : EditorWindow
    {
        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        protected static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Public Variables

        protected bool HasFocus;
        protected bool MouseInsideWindow;
        protected bool LeftMouseButtonIsDown;
        protected double LastRepaintTime;
        protected Color InitialGUIColor;
        protected Vector2 CurrentMousePosition;

        #endregion

        #region Properties

        protected virtual bool UseCustomRepaintInterval { get { return false; } }
        protected virtual double CustomRepaintIntervalDuringPlayMode { get { return 0.4f; } }
        protected virtual double CustomRepaintIntervalWhileIdle { get { return 0.6f; } }
        protected virtual bool RepaintOnInspectorUpdate { get { return true; } }

        #endregion

        #region Private Variables

        private readonly Dictionary<string, AnimBool> m_animBools = new Dictionary<string, AnimBool>();

        #endregion

        #region Unity Methods

        // <summary> Called when object becomes enabled and active </summary>
        public virtual void OnEnable()
        {
            wantsMouseEnterLeaveWindow = true;
            HasFocus = focusedWindow == this;
        }

        /// <summary> Called when window gets keyboard focus </summary>
        protected virtual void OnFocus() { HasFocus = true; }

        /// <summary> Called when window loses keyboard focus </summary>
        protected virtual void OnLostFocus() { HasFocus = false; }

        /// <summary> Called when object becomes disabled and inactive </summary>
        protected virtual void OnDisable() { HasFocus = false; }

        /// <summary> Called when EditorWindow is closed </summary>
        protected virtual void OnDestroy() { }

        /// <summary> Called 10 frames per second to give the inspector a chance to update </summary>
        protected virtual void OnInspectorUpdate()
        {
            if (RepaintOnInspectorUpdate) Repaint();
        }

        /// <summary> The GUI editor </summary>
        protected virtual void OnGUI()
        {
            Event current = Event.current;

            InitialGUIColor = GUI.color; //save initial color
            CurrentMousePosition = current.mousePosition; //save a quick access for the mouse position

            switch (current.type)
            {
                case EventType.Repaint:
                    LastRepaintTime = EditorApplication.timeSinceStartup;
                    break;
                case EventType.MouseDown:
                    if (current.button == 0) LeftMouseButtonIsDown = true; //mark if the left mouse button is down)
                    break;
                case EventType.MouseUp:
                    if (current.button == 0) LeftMouseButtonIsDown = false; //mark if the left mouse button is up
                    break;
                case EventType.MouseEnterWindow:
                    MouseInsideWindow = true;
                    Focus(); //when the mouse enters the window -> focus it
                    break;
                case EventType.MouseLeaveWindow:
                    MouseInsideWindow = false;
                    break;
            }

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

        protected internal AnimBool GetAnimBool(string key, bool defaultValue = false)
        {
            if (m_animBools.ContainsKey(key)) return m_animBools[key];
            m_animBools.Add(key, new AnimBool(defaultValue, Repaint));
            return m_animBools[key];
        }


        /// <summary>
        ///     Calls for a window Repaint when certain Event.current.type happen
        /// </summary>
        protected void ContextualRepaint()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseMove:
                case EventType.MouseDrag:
//                case EventType.KeyDown:
//                case EventType.KeyUp:
                case EventType.ScrollWheel:
//                case EventType.DragUpdated:
//                case EventType.DragPerform:
//                case EventType.DragExited:
//                case EventType.Ignore:
                case EventType.Used:
//                case EventType.ValidateCommand:
//                case EventType.ExecuteCommand:
//                case EventType.ContextClick:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    Repaint();
                    break;
            }
        }

        #endregion

        #region Static Methods

        protected static void DrawView(Action drawViewMethod, AnimBool showViewAnimBool, float leftHorizontalPadding = 0, float rightHorizontalPadding = 0)
        {
            DGUI.AlphaGroup.Begin(showViewAnimBool.faded);
            {
                GUILayout.BeginHorizontal();
                {
                    if (leftHorizontalPadding > 0) GUILayout.Space(DGUI.Properties.Space(leftHorizontalPadding));
                    GUILayout.BeginVertical();
                    {
                        drawViewMethod.Invoke();
                    }
                    GUILayout.EndVertical();
                    if (rightHorizontalPadding > 0) GUILayout.Space(DGUI.Properties.Space(rightHorizontalPadding));
                }
                GUILayout.EndHorizontal();
            }
            DGUI.AlphaGroup.End();
        }

        protected static void ResetKeyboardFocus(Event current)
        {
            if (current.type != EventType.KeyUp) return;

            if (current.keyCode == KeyCode.Escape ||
                current.keyCode == KeyCode.Return ||
                current.keyCode == KeyCode.KeypadEnter)
                DGUI.Properties.ResetKeyboardFocus();
        }

        public static void CenterWindow(EditorWindow editorWindow)
        {
            editorWindow.position = new Rect(Screen.currentResolution.width / 2 - editorWindow.position.width / 2,
                                             Screen.currentResolution.height / 2 + editorWindow.position.height / 2,
                                             editorWindow.position.width,
                                             editorWindow.position.height);
        }

        protected static void CenterWindow(EditorWindow editorWindow, float width, float height)
        {
            editorWindow.position = new Rect(Screen.currentResolution.width / 2 - width / 2,
                                             Screen.currentResolution.height / 2 - height / 2,
                                             width,
                                             height);
        }

        #endregion
    }
}