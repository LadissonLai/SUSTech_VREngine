// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private float m_debugHorizontalButtonSpacing = DGUI.Properties.Space(2);
        private float m_debugVerticalButtonSpacing = DGUI.Properties.Space(2);

        private static bool AnyDebugActive
        {
            get
            {
                return
                    Settings.DebugBackButton ||
                    Settings.DebugGameEventListener ||
                    Settings.DebugGameEventManager ||
                    Settings.DebugGestureListener ||
                    Settings.DebugGraphController ||
                    Settings.DebugKeyToAction ||
                    Settings.DebugKeyToGameEvent ||
                    Settings.DebugOrientationDetector ||
                    Settings.DebugProgressor ||
                    Settings.DebugProgressorGroup ||
                    Settings.DebugSceneDirector ||
                    Settings.DebugSceneLoader ||
                    Settings.DebugSoundyController ||
                    Settings.DebugSoundyManager ||
                    Settings.DebugSoundyPooler ||
                    Settings.DebugTouchDetector ||
                    Settings.DebugUIButton ||
                    Settings.DebugUIButtonListener ||
                    Settings.DebugUICanvas ||
                    Settings.DebugUIDrawer ||
                    Settings.DebugUIDrawerListener ||
                    Settings.DebugUIPopup ||
                    Settings.DebugUIPopupManager ||
                    Settings.DebugUIToggle ||
                    Settings.DebugUIView ||
                    Settings.DebugUIViewListener;
            }
        }

        private void DrawViewDebug()
        {
            if (CurrentView != View.Debug) return;

            #region DoozyUI

            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconDoozyUI, "DoozyUI", "UI Components", DGUI.Colors.LightOrDarkColorName);
            DrawDynamicViewVerticalSpace(0.5f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    #region UIButton, UICanvas, UIDrawer, UIPopup, UIToggle, UIView

                    GUILayout.BeginHorizontal();
                    {
                        bool debugUIButton = DoozySettings.Instance.DebugUIButton;
                        EditorGUI.BeginChangeCheck();
                        debugUIButton = DrawDebugButton(debugUIButton, "UIButton", Styles.GetStyle(Styles.StyleName.IconUIButton), DGUI.Colors.UIButtonColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIButton = debugUIButton;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugUICanvas = DoozySettings.Instance.DebugUICanvas;
                        EditorGUI.BeginChangeCheck();
                        debugUICanvas = DrawDebugButton(debugUICanvas, "UICanvas", Styles.GetStyle(Styles.StyleName.IconUICanvas), DGUI.Colors.UICanvasColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUICanvas = debugUICanvas;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugUIDrawer = DoozySettings.Instance.DebugUIDrawer;
                        EditorGUI.BeginChangeCheck();
                        debugUIDrawer = DrawDebugButton(debugUIDrawer, "UIDrawer", Styles.GetStyle(Styles.StyleName.IconUIDrawer), DGUI.Colors.UIDrawerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIDrawer = debugUIDrawer;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugUIToggle = DoozySettings.Instance.DebugUIToggle;
                        EditorGUI.BeginChangeCheck();
                        debugUIToggle = DrawDebugButton(debugUIToggle, "UIToggle", Styles.GetStyle(Styles.StyleName.IconUIToggle), DGUI.Colors.UIToggleColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIToggle = debugUIToggle;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);


                        bool debugUIView = DoozySettings.Instance.DebugUIView;
                        EditorGUI.BeginChangeCheck();
                        debugUIView = DrawDebugButton(debugUIView, "UIView", Styles.GetStyle(Styles.StyleName.IconUIView), DGUI.Colors.UIViewColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIView = debugUIView;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    GUILayout.Space(m_debugVerticalButtonSpacing);
                    
                    #region UIPopup, UIPopupManager
                      GUILayout.BeginHorizontal();
                    {
                        bool debugUIPopup = DoozySettings.Instance.DebugUIPopup;
                        EditorGUI.BeginChangeCheck();
                        debugUIPopup = DrawDebugButton(debugUIPopup, "UIPopup", Styles.GetStyle(Styles.StyleName.IconUIPopup), DGUI.Colors.UIPopupColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIPopup = debugUIPopup;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugUIPopupManager = DoozySettings.Instance.DebugUIPopupManager;
                        EditorGUI.BeginChangeCheck();
                        debugUIPopupManager = DrawDebugButton(debugUIPopupManager, "UIPopup Manager", Styles.GetStyle(Styles.StyleName.IconUIPopupManager), DGUI.Colors.UIPopupManagerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIPopupManager = debugUIPopupManager;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    
                    GUILayout.Space(m_debugVerticalButtonSpacing);

                    #region UIButton Listener, UIDrawer Listener, UIView Listener

                    GUILayout.BeginHorizontal();
                    {
                        bool debugUIButtonListener = DoozySettings.Instance.DebugUIButtonListener;
                        EditorGUI.BeginChangeCheck();
                        debugUIButtonListener = DrawDebugButton(debugUIButtonListener, "UIButton Listener", Styles.GetStyle(Styles.StyleName.IconUIButtonListener), DGUI.Colors.UIButtonListenerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIButtonListener = debugUIButtonListener;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugUIDrawerListener = DoozySettings.Instance.DebugUIDrawerListener;
                        EditorGUI.BeginChangeCheck();
                        debugUIDrawerListener = DrawDebugButton(debugUIDrawerListener, "UIDrawer Listener", Styles.GetStyle(Styles.StyleName.IconUIDrawerListener), DGUI.Colors.UIDrawerListenerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIDrawerListener = debugUIDrawerListener;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugUIViewListener = DoozySettings.Instance.DebugUIViewListener;
                        EditorGUI.BeginChangeCheck();
                        debugUIViewListener = DrawDebugButton(debugUIViewListener, "UIView Listener", Styles.GetStyle(Styles.StyleName.IconUIViewListener), DGUI.Colors.UIViewListenerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugUIViewListener = debugUIViewListener;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    GUILayout.Space(m_debugVerticalButtonSpacing);

                    #region Game Event Manager, Game Event Listener

                    GUILayout.BeginHorizontal();
                    {
                        bool debugGameEventManager = DoozySettings.Instance.DebugGameEventManager;
                        EditorGUI.BeginChangeCheck();
                        debugGameEventManager = DrawDebugButton(debugGameEventManager, "Game Event Manager", Styles.GetStyle(Styles.StyleName.IconGameEventManager), DGUI.Colors.GameEventManagerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugGameEventManager = debugGameEventManager;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugGameEventListener = DoozySettings.Instance.DebugGameEventListener;
                        EditorGUI.BeginChangeCheck();
                        debugGameEventListener = DrawDebugButton(debugGameEventListener, "Game Event Listener", Styles.GetStyle(Styles.StyleName.IconGameEventListener), DGUI.Colors.GameEventListenerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugGameEventListener = debugGameEventListener;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion
                    
                    GUILayout.Space(m_debugVerticalButtonSpacing);
                    
                    #region Scene Director, Scene Loader

                    GUILayout.BeginHorizontal();
                    {
                        bool debugSceneDirector = DoozySettings.Instance.DebugSceneDirector;
                        EditorGUI.BeginChangeCheck();
                        debugSceneDirector = DrawDebugButton(debugSceneDirector, "Scene Director", Styles.GetStyle(Styles.StyleName.IconSceneDirector), DGUI.Colors.SceneDirectorColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugSceneDirector = debugSceneDirector;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }
                        
                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugSceneLoader = DoozySettings.Instance.DebugSceneLoader;
                        EditorGUI.BeginChangeCheck();
                        debugSceneLoader = DrawDebugButton(debugSceneLoader, "Scene Loader", Styles.GetStyle(Styles.StyleName.IconSceneLoader), DGUI.Colors.SceneLoaderColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugSceneLoader = debugSceneLoader;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    GUILayout.Space(m_debugVerticalButtonSpacing);
                    
                    #region Progressor, Progressor Group

                    GUILayout.BeginHorizontal();
                    {
                        bool debugProgressor = DoozySettings.Instance.DebugProgressor;
                        EditorGUI.BeginChangeCheck();
                        debugProgressor = DrawDebugButton(debugProgressor, "Progressor", Styles.GetStyle(Styles.StyleName.IconProgressor), DGUI.Colors.ProgressorColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugProgressor = debugProgressor;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugProgressorGroup = DoozySettings.Instance.DebugProgressorGroup;
                        EditorGUI.BeginChangeCheck();
                        debugProgressorGroup = DrawDebugButton(debugProgressorGroup, "Progressor Group", Styles.GetStyle(Styles.StyleName.IconProgressorGroup), DGUI.Colors.ProgressorGroupColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugProgressorGroup = debugProgressorGroup;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    GUILayout.Space(m_debugVerticalButtonSpacing);

                    #region Orientation Detector

                    GUILayout.BeginHorizontal();
                    {
                        bool debugOrientationDetector = DoozySettings.Instance.DebugOrientationDetector;
                        EditorGUI.BeginChangeCheck();
                        debugOrientationDetector = DrawDebugButton(debugOrientationDetector, "Orientation Detector", Styles.GetStyle(Styles.StyleName.IconOrientationDetector), DGUI.Colors.OrientationDetectorColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugOrientationDetector = debugOrientationDetector;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion

                    GUILayout.Space(m_debugVerticalButtonSpacing);

                    #region Back Button, Key To Game Event

                    GUILayout.BeginHorizontal();
                    {
                        bool debugBackButton = DoozySettings.Instance.DebugBackButton;
                        EditorGUI.BeginChangeCheck();
                        debugBackButton = DrawDebugButton(debugBackButton, "Back Button", Styles.GetStyle(Styles.StyleName.IconBackButton), DGUI.Colors.BackButtonColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugBackButton = debugBackButton;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugKeyToAction = DoozySettings.Instance.DebugKeyToAction;
                        EditorGUI.BeginChangeCheck();
                        debugKeyToAction = DrawDebugButton(debugKeyToAction, "Key To Action", Styles.GetStyle(Styles.StyleName.IconKeyToAction), DGUI.Colors.KeyToActionColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugKeyToAction = debugKeyToAction;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        
                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugKeyToGameEvent = DoozySettings.Instance.DebugKeyToGameEvent;
                        EditorGUI.BeginChangeCheck();
                        debugKeyToGameEvent = DrawDebugButton(debugKeyToGameEvent, "Key To Game Event", Styles.GetStyle(Styles.StyleName.IconKeyToGameEvent), DGUI.Colors.KeyToGameEventColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugKeyToGameEvent = debugKeyToGameEvent;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        
                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            #endregion

            #region Soundy

            DrawDynamicViewVerticalSpace();
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconSoundy, "Soundy", "Sound Components", ColorName.Orange);
            DrawDynamicViewVerticalSpace(0.5f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    #region Soundy Manager, Soundy Pooler, Soundy Controller

                    GUILayout.BeginHorizontal();
                    {
                        bool debugSoundyManager = DoozySettings.Instance.DebugSoundyManager;
                        EditorGUI.BeginChangeCheck();
                        debugSoundyManager = DrawDebugButton(debugSoundyManager, "Soundy Manager", Styles.GetStyle(Styles.StyleName.IconSoundy), DGUI.Colors.SoundyManagerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugSoundyManager = debugSoundyManager;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugSoundyPooler = DoozySettings.Instance.DebugSoundyPooler;
                        EditorGUI.BeginChangeCheck();
                        debugSoundyPooler = DrawDebugButton(debugSoundyPooler, "Soundy Pooler", Styles.GetStyle(Styles.StyleName.IconSoundyPooler), DGUI.Colors.SoundyPoolerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugSoundyPooler = debugSoundyPooler;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugSoundyController = DoozySettings.Instance.DebugSoundyController;
                        EditorGUI.BeginChangeCheck();
                        debugSoundyController = DrawDebugButton(debugSoundyController, "Soundy Controller", Styles.GetStyle(Styles.StyleName.IconSoundyController), DGUI.Colors.SoundyControllerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugSoundyController = debugSoundyController;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            #endregion

            #region Touchy

            DrawDynamicViewVerticalSpace();
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconTouchy, "Touchy", "Touch Components", ColorName.Green);
            DrawDynamicViewVerticalSpace(0.5f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    #region Touch Detector, Gesture Listener

                    GUILayout.BeginHorizontal();
                    {
                        bool debugTouchDetector = DoozySettings.Instance.DebugTouchDetector;
                        EditorGUI.BeginChangeCheck();
                        debugTouchDetector = DrawDebugButton(debugTouchDetector, "Touch Detector", Styles.GetStyle(Styles.StyleName.IconTouchy), DGUI.Colors.TouchDetectorColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugTouchDetector = debugTouchDetector;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        bool debugGestureListener = DoozySettings.Instance.DebugGestureListener;
                        EditorGUI.BeginChangeCheck();
                        debugGestureListener = DrawDebugButton(debugGestureListener, "Gesture Listener", Styles.GetStyle(Styles.StyleName.IconGestureListener), DGUI.Colors.GestureListenerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugGestureListener = debugGestureListener;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            #endregion

            #region Nody

            DrawDynamicViewVerticalSpace();
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconNody, "Nody", "Node Graph Components", ColorName.LightBlue);
            DrawDynamicViewVerticalSpace(0.5f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    #region UIButton, UICanvas, UIDrawer, UIPopup, UIToggle, UIView

                    GUILayout.BeginHorizontal();
                    {
                        bool debugGraphController = DoozySettings.Instance.DebugGraphController;
                        EditorGUI.BeginChangeCheck();
                        debugGraphController = DrawDebugButton(debugGraphController, "Graph Controller", Nody.Styles.GetStyle(Nody.Styles.StyleName.IconGraphController), DGUI.Colors.GraphControllerColorName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            DoozySettings.Instance.UndoRecord("DDebug");
                            DoozySettings.Instance.DebugGraphController = debugGraphController;
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                        }

                        GUILayout.Space(m_debugHorizontalButtonSpacing);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    #endregion                  
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            #endregion
            
            DrawDynamicViewVerticalSpace(2);
        }

        private static bool DrawDebugButton(bool enabled, string componentName, GUIStyle componentIcon, ColorName componentColorName)
        {
            bool value = enabled;
            GUILayout.BeginVertical();
            if (DGUI.Button.Dynamic.DrawIconButton(componentIcon, (value ? "    " : "") + componentName, Size.L, TextAlign.Left,
                                                   DGUI.Colors.GetBackgroundColorName(enabled, componentColorName),
                                                   DGUI.Colors.GetTextColorName(enabled, componentColorName),
                                                   DGUI.Properties.SingleLineHeight * 2, false))
                value = !enabled;

            if (value)
            {
                GUILayout.Space(-DGUI.Properties.SingleLineHeight - 8);
                GUILayout.BeginHorizontal();
                GUILayout.Space(36);
                DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconDebug), 16, 16, ColorName.Red);
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.SingleLineHeight - 8);
            }

            GUILayout.EndVertical();

            return value;
        }
    }
}