// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Orientation
{
    /// <inheritdoc />
    /// <summary>
    /// The Orientation Detector is responsible for detecting the current screen orientation of the target device
    /// </summary>
    [AddComponentMenu(MenuUtils.OrientationDetector_AddComponentMenu_MenuName, MenuUtils.OrientationDetector_AddComponentMenu_Order)]
    [RequireComponent(typeof(RectTransform), typeof(Canvas))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.ORIENTATION_DETECTOR)]
    public class OrientationDetector : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [UnityEditor.MenuItem(MenuUtils.OrientationDetector_MenuItem_ItemName, false, MenuUtils.OrientationDetector_MenuItem_Priority)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected OrientationDetector() { }

        private static OrientationDetector s_instance;

        /// <summary> Returns a reference to the OrientationDetector in the Scene. If one does not exist, it gets created </summary>
        public static OrientationDetector Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<OrientationDetector>();
                if (s_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        public static bool ApplicationIsQuitting { get; private set; }

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Callback executed every time the device orientation changes that sends the new DetectedOrientation </summary>
        public OrientationEvent OnOrientationEvent = new OrientationEvent();

        #endregion

        #region Properties        

        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        /// <summary> Reference to the Canvas component </summary>
        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null) m_canvas = GetComponent<Canvas>();
                return m_canvas;
            }
        }

        /// <summary> Returns the current device orientation </summary>
        public DetectedOrientation CurrentOrientation { get { return m_currentOrientation; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugOrientationDetector; } }

        #endregion

        #region Private Variables        

        /// <summary> Internal variable that keeps track of the current device orientation </summary>
        private DetectedOrientation m_currentOrientation = DetectedOrientation.Unknown;

        /// <summary> Internal variable that holds a reference to the RectTransform of the component </summary>
        private RectTransform m_rectTransform;

        /// <summary> Internal variable that holds a reference to the Canvas attached to this Source </summary>
        private Canvas m_canvas;

        /// <summary> Internal variable used to count evey orientation check. This is needed to cancel two notifications passes happening OnRectTransformDimensionsChange </summary>
        private int m_deviceOrientationCheckCount;

        #endregion

        #region Unity Methods

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            ApplicationIsQuitting = false;
        }
#endif
        
        private void Reset() { Canvas.renderMode = RenderMode.ScreenSpaceOverlay; }

        private void OnValidate() { Canvas.renderMode = RenderMode.ScreenSpaceOverlay; }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                DDebug.Log("There cannot be two " + typeof(OrientationDetector) + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);

            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (Canvas.isRootCanvas) return;
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
        }

        private void OnEnable()
        {
            CheckDeviceOrientation();
            
        }

        private void Update()
        {
            if (m_currentOrientation == DetectedOrientation.Unknown) CheckDeviceOrientation();
//#if UNITY_2018_1_OR_NEWER
//CheckDeviceOrientation();
//#endif
        }

        private void OnRectTransformDimensionsChange() { CheckDeviceOrientation(); }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        #endregion

        #region Public Methods

        /// <summary> Check the current orientation and updates it if it changed. This method is called automatically by the OrientationDetector </summary>
        public void CheckDeviceOrientation(bool forceUpdate = false)
        {
#if UNITY_EDITOR

            //PORTRAIT
            if (Screen.width < Screen.height)
            {
                if (m_currentOrientation != DetectedOrientation.Portrait || forceUpdate) //Orientation changed to PORTRAIT
                    ChangeOrientation(DetectedOrientation.Portrait, forceUpdate);
            }

            //LANDSCAPE
            else
            {
                if (m_currentOrientation != DetectedOrientation.Landscape || forceUpdate) //Orientation changed to LANDSCAPE
                    ChangeOrientation(DetectedOrientation.Landscape, forceUpdate);
            }

#else
            //LANDSCAPE
            if (Screen.orientation == ScreenOrientation.Landscape ||
                Screen.orientation == ScreenOrientation.LandscapeLeft ||
                Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                if (m_currentOrientation != DetectedOrientation.Landscape || forceUpdate) //Orientation changed to LANDSCAPE
                {
                    ChangeOrientation(DetectedOrientation.Landscape, forceUpdate);
                }
            }

            //PORTRAIT
            else if (Screen.orientation == ScreenOrientation.Portrait ||
                     Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                if (m_currentOrientation != DetectedOrientation.Portrait || forceUpdate) //Orientation changed to PORTRAIT
                {
                    ChangeOrientation(DetectedOrientation.Portrait, forceUpdate);
                }
            }

            //FALLBACK option if we are in AutoRotate or if we are in Unknown
            else
            {
                ChangeOrientation(DetectedOrientation.Landscape);
            }
#endif
        }

        /// <summary> Update the current orientation to the given newOrientation and invoke OnOrientationEvent </summary>
        public void ChangeOrientation(DetectedOrientation newOrientation, bool forceUpdate = false)
        {
            m_currentOrientation = newOrientation;
            OnOrientationEvent.Invoke(m_currentOrientation);

            Message.Send("DetectedOrientation." + newOrientation);


            m_deviceOrientationCheckCount++;
            if (m_deviceOrientationCheckCount > 2 || forceUpdate)
            {
                //NotifyUIManager(newOrientation);
            }

            if (DebugComponent) DDebug.Log("Current device orientation: " + m_currentOrientation, this);
        }

        #endregion

        #region Static Methods

        /// <summary> Adds OrientationDetector to scene and returns a reference to it </summary>
        private static OrientationDetector AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<OrientationDetector>(MenuUtils.OrientationDetector_GameObject_Name, true, selectGameObjectAfterCreation); }

        #endregion
    }
}