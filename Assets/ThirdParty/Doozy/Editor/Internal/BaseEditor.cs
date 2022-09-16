// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Internal
{
    public class BaseEditor : EditorBase
    {
        #region Properties

        protected virtual bool HasErrors { get { return false; } }

        #endregion

        #region Unity Methods

        /// <summary> Called when object becomes enabled and active </summary>
        protected virtual void OnEnable()
        {
            LoadSerializedProperty();
            InitAnimBool();
        }

        /// <summary> Called when object becomes disabled and inactive </summary>
        protected virtual void OnDisable() { }

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

        /// <summary> Initializes defined AnimBools OnEnable (after LoadSerializedProperty) </summary>
        protected virtual void InitAnimBool() { }

        /// <summary> Loads defined SerializedProperties OnEnable </summary>
        protected virtual void LoadSerializedProperty() { }

        protected void DrawDebugMode() { DGUI.Doozy.DrawDebugMode(GetProperty(PropertyName.DebugMode), ColorName.Red); }

        #endregion
    }
}