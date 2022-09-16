// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        #region Instance

        private static DoozyWindow s_instance;

        public static DoozyWindow Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<DoozyWindow>();
                return s_instance;
            }
        }

        /*
       * An alternative way to get Window, because
       * GetWindow<DoozyWindow>() forces window to be active and present
       */
        private static DoozyWindow Window
        {
            get
            {
                DoozyWindow[] windows = Resources.FindObjectsOfTypeAll<DoozyWindow>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        #endregion

        #region Toolbar and View

        private AnimBool m_mainToolbarAnimBool;

        private AnimBool MainToolbarAnimBool
        {
            get
            {
                if (m_mainToolbarAnimBool != null) return m_mainToolbarAnimBool;
                m_mainToolbarAnimBool = GetAnimBool(DoozyWindowSettings.Instance.EditorPrefsKeyWindowToolbarState);
                m_mainToolbarAnimBool.speed = DoozyWindowSettings.Instance.ToolbarAnimationSpeed;
                return m_mainToolbarAnimBool;
            }
        }

        private float MainToolbarWidth { get { return DoozyWindowSettings.Instance.ToolbarCollapsedWidth + (DoozyWindowSettings.Instance.ToolbarExpandedWidth - DoozyWindowSettings.Instance.ToolbarCollapsedWidth) * MainToolbarAnimBool.faded; } }
        private Rect MainToolbarRect { get { return new Rect(0, 0, MainToolbarWidth, position.height); } }
        private Rect MainToolbarShadowRect { get { return new Rect(MainToolbarWidth, MainToolbarRect.y, DoozyWindowSettings.Instance.ToolbarShadowWidth, position.height); } }
        private float FullViewWidth { get { return position.width - MainToolbarWidth; } }
        private Rect FullViewRect { get { return new Rect(MainToolbarWidth, 0, FullViewWidth, position.height); } }

        private float m_windowHeaderHeight = 96f;
        private float m_viewTopMenuHeight = 48f;
        private float m_viewLeftMenuWidth = 160f;
        
        private Vector2 m_viewLeftMenuScrollPosition;
        private Vector2 m_viewScrollPosition;

        private Rect ViewTopMenuRect
        {
            get
            {
                return new Rect(0,
                                FullViewRect.y + m_windowHeaderHeight,
                                FullViewRect.width,
                                m_viewTopMenuHeight);
            }
        }

        private Rect ViewLeftMenuRect
        {
            get
            {
                return new Rect(0,
                                ViewTopMenuRect.y + ViewTopMenuRect.height,
                                m_viewLeftMenuWidth * CurrentViewExpanded.faded,
                                FullViewRect.height - m_viewTopMenuHeight - m_windowHeaderHeight);
            }
        }

        private Rect ViewWithMenusRect
        {
            get
            {
                return new Rect(ViewLeftMenuRect.x + m_viewLeftMenuWidth,
                                ViewTopMenuRect.y + m_viewTopMenuHeight,
                                FullViewRect.width - m_viewLeftMenuWidth,
                                FullViewRect.height - m_viewTopMenuHeight - m_windowHeaderHeight);
            }
        }

        #endregion
        
        #region Functionality
        
        private string m_newDatabaseName;
        
        private AnimBool m_editMode;
        public AnimBool EditMode
        {
            get
            {
                if (m_editMode != null) return m_editMode;
                m_editMode = GetAnimBool("EditMode");
                return m_editMode;
            }
        }

        private AnimBool m_newDatabase;
        public AnimBool NewDatabase
        {
            get
            {
                if (m_newDatabase != null) return m_newDatabase;
                m_newDatabase = GetAnimBool("NewDatabase");
                return m_newDatabase;
            }
        }
        
        #endregion

        #region Area Draw Methods

        private Rect BeginDrawViewTopMenuArea()
        {
            Rect viewTopMenuRect = ViewTopMenuRect;
            
            //Draw Background
            EditorGUI.DrawRect(viewTopMenuRect, EditorGUIUtility.isProSkin ? new Color(0.05f, 0.05f, 0.05f) : new Color(0.95f, 0.95f, 095f));
            
            GUILayout.BeginArea(viewTopMenuRect);
            return viewTopMenuRect;
        }

        private void EndDrawViewTopMenuArea() { GUILayout.EndArea(); }


        private Rect BeginDrawViewLeftMenuArea()
        {
            Rect viewLeftMenuRect = ViewLeftMenuRect;
            
            //Draw Background
            Color leftMenuBackgroundColor = EditorGUIUtility.isProSkin ? EditorColors.Instance.UnityDark.Dark : EditorColors.Instance.UnityLight.Light;
            float colorStep = 0.05f;
            leftMenuBackgroundColor = new Color(leftMenuBackgroundColor.r - colorStep, leftMenuBackgroundColor.g - colorStep, leftMenuBackgroundColor.b - colorStep);
            EditorGUI.DrawRect(viewLeftMenuRect, leftMenuBackgroundColor);
            
            GUILayout.BeginArea(viewLeftMenuRect);
            m_viewLeftMenuScrollPosition = GUILayout.BeginScrollView(m_viewLeftMenuScrollPosition);
            return viewLeftMenuRect;
        }

        private void EndDrawViewLeftMenuArea()
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //Left Menu Shadows
            GUI.color = Color.black.WithAlpha(EditorGUIUtility.isProSkin ? 0.4f : 0.3f);
            {
                Rect viewLeftMenuRect = ViewLeftMenuRect;
                
                //Top Shadow
                GUI.Label(new Rect(viewLeftMenuRect.x,
                                   viewLeftMenuRect.y,
                                   viewLeftMenuRect.width,
                                   DoozyWindowSettings.Instance.ToolbarShadowWidth),
                          GUIContent.none,
                          Styles.GetStyle(Styles.StyleName.WhiteGradientTopToBottom));

                //Right Shadow
                GUI.Label(new Rect(viewLeftMenuRect.x + viewLeftMenuRect.width - DoozyWindowSettings.Instance.ToolbarShadowWidth,
                                   viewLeftMenuRect.y,
                                   DoozyWindowSettings.Instance.ToolbarShadowWidth,
                                   viewLeftMenuRect.height),
                          GUIContent.none,
                          Styles.GetStyle(Styles.StyleName.WhiteGradientRightToLeft));
            }
            GUI.color = InitialGUIColor;

            //Left Menu Left Shadow
        }

        private Rect BeginDrawViewWithMenusArea()
        {
            Rect viewWithMenusRect = ViewWithMenusRect;
            GUILayout.BeginArea(viewWithMenusRect);
            m_viewScrollPosition = GUILayout.BeginScrollView(m_viewScrollPosition);
            return viewWithMenusRect;
        }

        private void EndDrawViewWithMenusArea()
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion
    }
}