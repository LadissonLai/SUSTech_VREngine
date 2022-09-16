// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow : BaseEditorWindow
    {
        protected override bool UseCustomRepaintInterval { get { return true; } }
        protected override double CustomRepaintIntervalDuringPlayMode { get { return NodySettings.Instance.RepaintIntervalDuringPlayMode; } }
        protected override double CustomRepaintIntervalWhileIdle { get { return NodySettings.Instance.RepaintIntervalWhileIdle; } }

        public override void OnEnable()
        {
            base.OnEnable();
            WindowSettings.Initialize();
            InitWindow();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DeselectCurrentGraphOrAnyOfItsNodes();
            WindowSettings.SetDirty(true);
            Undo.ClearAll();
        }

        protected override void OnFocus()
        {
            base.OnFocus();
            GraphEvent.OnGraphEvent = HandleOnGraphEvent;
            SetGraphMode(GraphMode.None);
        }

        protected override void OnLostFocus()
        {
            base.OnLostFocus();
            SetGraphMode(GraphMode.None);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            DetectKeys(Event.current); //detect any pre-defined shortcuts
            UpdateCursorIcon();        //update the cursor if panning
            DrawViews();               //draw the views
            DrawViewAreaTabsAndInfo(); //draw view area tabs and info
            DrawLeftToolbar();         //draw left toolbar
            ContextualRepaint();       //perform a Repaint if any relevant event happened
        }

        private void DrawViewAreaTabsAndInfo()
        {
            //View Area
            GUILayout.BeginArea(ViewRect);
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    if (CurrentGraph != null)
                    {
                        EditorGUI.DrawRect(GraphTabsRect, (EditorGUIUtility.isProSkin
                                                               ? Color.black.Lighter()
                                                               : Color.white.Darker()).WithAlpha(NodySettings.Instance.GraphTabsBackgroundOpacity));
                        DrawGraphTabs(); //draw graph tabs (if a graph is opened)
                        GUILayout.FlexibleSpace();
                        DrawOpenedGraphAssetPath(); //draw graph asset path (if a graph is opened)
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

        private void DrawLeftToolbar()
        {
            //Toolbar Shadow Area
            GUILayout.BeginArea(ToolbarShadowRect);
            {
                GUI.color = Color.black.WithAlpha(EditorGUIUtility.isProSkin ? 0.3f : 0.2f);
                GUILayout.Label(GUIContent.none, Editor.Styles.GetStyle(Editor.Styles.StyleName.WhiteGradientLeftToRight), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUI.color = InitialGUIColor;
            }
            GUILayout.EndArea();

            //Toolbar Area
            GUILayout.BeginArea(ToolbarRect);
            {
                EditorGUI.DrawRect(ToolbarRect, (EditorGUIUtility.isProSkin
                                                     ? EditorColors.Instance.UnityDark.Dark
                                                     : EditorColors.Instance.UnityLight.Light).WithAlpha(NodyWindowSettings.TOOLBAR_OPACITY));
                DrawToolbar(); //toolbar is drawn over the graph area in order to maximize graph coverage (design choice)
            }
            GUILayout.EndArea();
        }

        private void HandleOnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange != PlayModeStateChange.EnteredEditMode &&
                playModeStateChange != PlayModeStateChange.EnteredPlayMode)
                return;

            if (CurrentGraph == null) return;

            InitWindow();
            Repaint();
        }

        private void InitAnimBools()
        {
            m_showGeneralView = new AnimBool(CurrentView == View.General, Repaint);
            m_showGraphView = new AnimBool(CurrentView == View.Graph, Repaint);

            m_isDraggingAnimBool = new AnimBool(m_mode != GraphMode.Drag, Repaint);
            m_altKeyPressedAnimBool = new AnimBool(false, Repaint);

            m_connectMode = new AnimBool(false, Repaint);
            m_selectMode = new AnimBool(false, Repaint);
            m_dragMode = new AnimBool(false, Repaint);
            m_panMode = new AnimBool(false, Repaint);
            m_deleteMode = new AnimBool(false, Repaint);
        }

        private void InitWindow()
        {
            //window setup
            s_instance = this;
            titleContent = new GUIContent("Nody");
            wantsMouseMove = true;
            minSize = new Vector2((ToolbarAnimBool.target ? DoozyWindowSettings.Instance.ToolbarExpandedWidth : DoozyWindowSettings.Instance.ToolbarCollapsedWidth) + 300, 300);
            ToolbarAnimBool.value = DoozyWindowSettings.Instance.DynamicToolbarExpanded;

            InitAnimBools();              //initialize the animation bools
            SetGraphMode(GraphMode.None); //set the graph to idle mode
            m_undo = new UndoUtility();   //init the UndoUtility

            //subscribe to events
            EditorApplication.playModeStateChanged += HandleOnPlayModeStateChanged;
            Undo.undoRedoPerformed = UndoRedoPerformed;
            GraphEvent.OnGraphEvent = HandleOnGraphEvent;

            if (!ReopenCurrentGraph()) SetView(View.General);
        }       
    }
}