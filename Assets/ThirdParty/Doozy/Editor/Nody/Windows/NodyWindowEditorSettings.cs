// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Settings;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        #region Instances

        private static NodyWindow s_instance;

        public static NodyWindow Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = Window;
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<NodyWindow>();
                return s_instance;
            }
        }

        /*
        * An alternative way to get Window, because
        * GetWindow<NodyWindow>() forces window to be active and present
        */
        private static NodyWindow Window
        {
            get
            {
                NodyWindow[] windows = Resources.FindObjectsOfTypeAll<NodyWindow>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        /// <summary> Direct reference to Nody's editor settings used to keep track of tabs and other things </summary>
        private static NodyWindowSettings WindowSettings { get { return NodyWindowSettings.Instance; } }

        #endregion

        private static List<Node> SelectedNodes { get { return WindowSettings.SelectedNodes; } set { WindowSettings.SelectedNodes = value; } }
        private static List<Node> CopiedNodes { get { return WindowSettings.CopiedNodes; } }
        private static List<Node> DeletedNodes { get { return WindowSettings.DeletedNodes; } }

        #region Toolbar and UIView

        private AnimBool m_toolbarAnimBool;

        private AnimBool ToolbarAnimBool
        {
            get
            {
                if (m_toolbarAnimBool != null) return m_toolbarAnimBool;
                m_toolbarAnimBool = GetAnimBool(NodySettings.Instance.EditorPrefsKeyWindowToolbar);
                m_toolbarAnimBool.speed = DoozyWindowSettings.Instance.ToolbarAnimationSpeed;
                return m_toolbarAnimBool;
            }
        }

        private float ToolbarWidth { get { return DoozyWindowSettings.Instance.ToolbarCollapsedWidth + (DoozyWindowSettings.Instance.ToolbarExpandedWidth - DoozyWindowSettings.Instance.ToolbarCollapsedWidth) * ToolbarAnimBool.faded; } }
        private Rect ToolbarRect { get { return new Rect(0, 0, ToolbarWidth, position.height); } }
        private Rect ToolbarShadowRect { get { return new Rect(ToolbarWidth, ToolbarRect.y, DoozyWindowSettings.Instance.ToolbarShadowWidth, position.height); } }
        private float ViewWidth { get { return position.width - ToolbarWidth; } }
        private Rect ViewRect { get { return new Rect(ToolbarWidth, 0, ViewWidth, position.height); } }
        private Rect GraphTabsRect { get { return new Rect(0, 0, position.width - ToolbarWidth, NodySettings.Instance.GraphTabsAreaHeight); } }

        private static View CurrentView
        {
            get { return WindowSettings.CurrentView; }
            set
            {
                WindowSettings.CurrentView = value;
                WindowSettings.SetDirty(false);
            }
        }

        #endregion

        #region Zoom PanOffset and DotAnimationSpeed

        private static float CurrentZoom
        {
            get { return WindowSettings.CurrentZoom; }
            set
            {
                WindowSettings.CurrentZoom = value;
                WindowSettings.SetDirty(false);
            }
        }

        private void ResetZoom() { CurrentZoom = NodyWindowSettings.Instance.DefaultZoom; }

        private static Vector2 CurrentPanOffset
        {
            get { return WindowSettings.CurrentPanOffset; }
            set
            {
                WindowSettings.CurrentPanOffset = value;
                WindowSettings.SetDirty(false);
            }
        }

        private void ResetPanOffset() { CurrentPanOffset = NodyWindowSettings.Instance.DefaultPanOffset; }

        private static float CurrentDotAnimationSpeed
        {
            get { return WindowSettings.CurrentDotAnimationSpeed; }
            set
            {
                WindowSettings.CurrentDotAnimationSpeed = value;
                WindowSettings.SetDirty(false);
            }
        }

        private void ResetDotAnimationSpeed() { CurrentDotAnimationSpeed = NodyWindowSettings.Instance.DefaultDotAnimationSpeed; }

        #endregion

        #region GraphTabs

        private static List<GraphTab> GraphTabs
        {
            get { return WindowSettings.GraphTabs; }
            set
            {
                WindowSettings.GraphTabs = value;
                WindowSettings.SetDirty(false);
            }
        }

        private void ResetGraphTabs() { GraphTabs = new List<GraphTab>(); }

        #endregion
    }
}