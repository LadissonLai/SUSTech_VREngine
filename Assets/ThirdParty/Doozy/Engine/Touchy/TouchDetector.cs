// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Touchy
{
    /// <summary>
    /// Responsible of capturing any inputs that are touch related and sending the touch info to all the relevant components. It also detects mouse inputs and converts them into simulated touches in order to help with tests in the Editor.
    /// </summary>
    [AddComponentMenu(MenuUtils.TouchDetector_AddComponentMenu_MenuName, MenuUtils.TouchDetector_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.TOUCH_DETECTOR)]
    public class TouchDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.TouchDetector_MenuItem_ItemName, false, MenuUtils.TouchDetector_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected TouchDetector()
        {
            TouchInProgress = false;
            m_swipeEnded = false;
        }

        private static TouchDetector s_instance;

        /// <summary> Returns a reference to the TouchDetector in the Scene. If one does not exist, it gets created. </summary>
        public static TouchDetector Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<TouchDetector>();
                if (s_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary> Direct reference to the TouchySettings asset </summary>
        private static TouchySettings Settings { get { return TouchySettings.Instance; } }

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        public static bool ApplicationIsQuitting { get; private set; }

        /// <summary> Returns the minimum swipe distance in order for a touch to be considered a swipe </summary>
        public static float SwipeLength { get { return Settings.SwipeLength; } }

        /// <summary> Returns the time period needed for a finger to be touching the target device, in order for the tap to be considered a long tap (long press) </summary>
        public static float LongTapDuration { get { return Settings.LongTapDuration; } }

        private static bool DebugComponent { get { return DoozySettings.Instance.DebugTouchDetector; } }

        #endregion

        #region Properties

        /// <summary> Returns TRUE if a touch is in progress </summary>
        public bool TouchInProgress { get; private set; }

        /// <summary> If a touch is in progress, it returns its touch info </summary>
        public TouchInfo CurrentTouchInfo { get { return m_currentTouchInfo; } }

        #endregion

        #region Public Variables

        /// <summary> Action triggered when a tap is detected </summary>
        public Action<TouchInfo> OnTapAction;

        /// <summary> Action triggered when a long tap is detected </summary>
        public Action<TouchInfo> OnLongTapAction;

        /// <summary> Action triggered when a swipe is detected </summary>
        public Action<TouchInfo> OnSwipeAction;

        #endregion

        #region Private Variables

        private Vector2 m_currentSwipe;
        private bool m_swipeEnded;
        private TouchInfo m_currentTouchInfo;
        private List<Touch> m_touches = new List<Touch>();
        private Touch m_touch;
        private PointerEventData m_pointerEventData;
        private List<RaycastResult> m_raycastResults = new List<RaycastResult>();

        #endregion

        #region Unity Methods

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            ApplicationIsQuitting = false;
        }
#endif
        
        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                DDebug.Log("There cannot be two '" + GetType().Name + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();

            if (EventSystem.current != null) return;
            if (FindObjectOfType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private void Update() { DetectTouch(); }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (DebugComponent) DDebug.Log(gameObject.name + ": " + "OnBeginDrag", this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (DebugComponent) DDebug.Log(gameObject.name + ": " + "OnDrag", this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DebugComponent) DDebug.Log(gameObject.name + ": " + "OnEndDrag", this);
        }

        #endregion

        #region Public Methods

        /// <summary> Sets a new dragged target </summary>
        /// <param name="target"> The new drag target </param>
        public void SetDraggedObject(GameObject target) { m_currentTouchInfo.DraggedObject = target; }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            if (m_touches == null) m_touches = new List<Touch>();
            if (m_raycastResults == null) m_raycastResults = new List<RaycastResult>();

            OnSwipeAction += HandleSwipe;
            OnLongTapAction += HandleLongTap;
            OnTapAction += HandleTap;

            if (DebugComponent) DDebug.Log("Initialized", Instance);
        }

        private void DetectTouch()
        {
            m_touches = TouchHelper.GetTouches();
            if (m_touches.Count == 0 || m_swipeEnded)
            {
                TouchInProgress = false;
                return;
            }

            m_touch = m_touches[0];

            if (m_touch.phase == TouchPhase.Began)
            {
                m_pointerEventData = new PointerEventData(EventSystem.current) {position = m_touch.position};
                m_raycastResults.Clear();
                EventSystem.current.RaycastAll(m_pointerEventData, m_raycastResults);
                m_currentTouchInfo.Update(m_touch, m_raycastResults.Count > 0 ? m_raycastResults[0].gameObject : null); //reset
                m_pointerEventData = null;
                TouchInProgress = true;
                return;
            }

            UpdateCurrentTouchInfo(m_touch);

            if (m_touch.phase == TouchPhase.Moved || m_touch.phase == TouchPhase.Stationary)
                if (!m_currentTouchInfo.LongTap && m_currentTouchInfo.Duration > LongTapDuration && m_currentTouchInfo.LongestDistance < SwipeLength) //LONG TAP
                {
                    m_currentTouchInfo.Direction = Swipe.None; //invalidate current swipe action
                    m_currentTouchInfo.LongTap = true;
                    if (OnLongTapAction != null)
                        OnLongTapAction(m_currentTouchInfo); //fire event - LONG TAP

                    return;
                }

            if (m_touch.phase == TouchPhase.Ended || m_touch.phase == TouchPhase.Canceled)
            {
                if (m_currentTouchInfo.Duration < LongTapDuration && m_currentTouchInfo.LongestDistance < SwipeLength) //TAP
                {
                    m_currentTouchInfo.Direction = Swipe.None; //invalidate current swipe action
                    m_currentTouchInfo.Tap = true;
                    if (OnTapAction != null)
                        OnTapAction(m_currentTouchInfo); //fire event - TAP

                    return;
                }

                if (m_currentTouchInfo.Distance < SwipeLength || m_currentTouchInfo.LongTap) //didnt swipe enough or this is a long tap
                {
                    m_currentTouchInfo.Direction = Swipe.None; //invalidate current swipe action
                    return;
                }

                //SWIPE
                if (OnSwipeAction != null)
                    OnSwipeAction(m_currentTouchInfo); //fire event - SWIPE
            }
        }

        private void UpdateCurrentTouchInfo(Touch touch)
        {
            m_currentTouchInfo.Touch = touch;

            m_currentTouchInfo.PreviousTouchPosition = m_currentTouchInfo.CurrentTouchPosition;
            m_currentTouchInfo.CurrentTouchPosition = touch.position;
            m_currentTouchInfo.TouchDeltaTime = touch.deltaTime;

            m_currentTouchInfo.EndPosition = new Vector2(touch.position.x, touch.position.y);
            m_currentTouchInfo.EndTime = Time.time;
            m_currentTouchInfo.Duration = m_currentTouchInfo.EndTime - m_currentTouchInfo.StartTime;
            m_currentSwipe = m_currentTouchInfo.EndPosition - m_currentTouchInfo.StartPosition;
            m_currentTouchInfo.RawDirection = m_currentSwipe;
            m_currentTouchInfo.Direction = m_currentTouchInfo.LongestDistance < SwipeLength ? Swipe.None : GetSwipeDirection(m_currentSwipe);
            m_currentTouchInfo.Distance = Vector2.Distance(m_currentTouchInfo.StartPosition, m_currentTouchInfo.EndPosition);
            //currentTouchInfo.velocity = currentSwipe * (currentTouchInfo.endTime - currentTouchInfo.startTime);
            m_currentTouchInfo.Velocity = (m_currentTouchInfo.EndPosition - m_currentTouchInfo.StartPosition) / (m_currentTouchInfo.EndTime - m_currentTouchInfo.StartTime);
            //velocity = (endPos - startPos) / (endTime  - startTime)
            //velocity = distance / duration
            if (m_currentTouchInfo.Distance > m_currentTouchInfo.LongestDistance) // If new distance is longer than previously longest
                m_currentTouchInfo.LongestDistance = m_currentTouchInfo.Distance; // Update longest distance
        }

        private void HandleSwipe(TouchInfo touchInfo)
        {
            if (DebugComponent) DDebug.Log(string.Format("HandleSwipe: {0}", touchInfo), this);
        }

        private void HandleTap(TouchInfo touchInfo)
        {
            if (DebugComponent) DDebug.Log(string.Format("HandleTap: {0}", touchInfo), this);
        }

        private void HandleLongTap(TouchInfo touchInfo)
        {
            if (DebugComponent) DDebug.Log(string.Format("HandleLongPress: {0}", touchInfo), this);
        }

        #endregion

        #region Static Methods

        /// <summary> Initialize the TouchDetector </summary>
        public static void Init()
        {
            if (s_instance != null) return;
            s_instance = Instance;
        }

        /// <summary> Get the Vector2 representation of the given swipe direction </summary>
        /// <param name="swipe"> Target swipe direction </param>
        public static Vector2 GetCardinalDirection(Swipe swipe) { return CardinalDirection.Get(swipe); }

        /// <summary> Convert a SimpleSwipe into a Swipe </summary>
        /// <param name="simpleSwipe"> Target simple swipe </param>
        /// <param name="reverse"> Should the reversed value be returned? (eg. if SimpleSwipe.Left and reverse is true, it will return Swipe.Right) </param>
        public static Swipe GetSwipe(SimpleSwipe simpleSwipe, bool reverse = false)
        {
            switch (simpleSwipe)
            {
                case SimpleSwipe.None:  return Swipe.None;
                case SimpleSwipe.Left:  return reverse ? Swipe.Right : Swipe.Left;
                case SimpleSwipe.Right: return reverse ? Swipe.Left : Swipe.Right;
                case SimpleSwipe.Up:    return reverse ? Swipe.Down : Swipe.Up;
                case SimpleSwipe.Down:  return reverse ? Swipe.Up : Swipe.Down;
                default:                return Swipe.None;
            }
        }

        /// <summary> Convert a Swipe into a SimpleSwipe </summary>
        /// <param name="swipe"> Target swipe </param>
        /// <param name="reverse"> Should the reversed value be returned? (eg. if Swipe.Left and reverse is true, it will return SimpleSwipe.Right) </param>
        public static SimpleSwipe GetSimpleSwipe(Swipe swipe, bool reverse = false)
        {
            switch (swipe)
            {
                case Swipe.None:      return SimpleSwipe.None;
                case Swipe.UpLeft:    return reverse ? SimpleSwipe.Right : SimpleSwipe.Left;
                case Swipe.Up:        return reverse ? SimpleSwipe.Down : SimpleSwipe.Up;
                case Swipe.UpRight:   return reverse ? SimpleSwipe.Left : SimpleSwipe.Right;
                case Swipe.Left:      return reverse ? SimpleSwipe.Right : SimpleSwipe.Left;
                case Swipe.Right:     return reverse ? SimpleSwipe.Left : SimpleSwipe.Right;
                case Swipe.DownLeft:  return reverse ? SimpleSwipe.Right : SimpleSwipe.Left;
                case Swipe.Down:      return reverse ? SimpleSwipe.Up : SimpleSwipe.Down;
                case Swipe.DownRight: return reverse ? SimpleSwipe.Left : SimpleSwipe.Right;
                default:              return SimpleSwipe.None;
            }
        }

        /// <summary> Get a Swipe direction by analyzing a given direction </summary>
        /// <param name="direction"> Swipe direction </param>
        public static Swipe GetSwipeDirection(Vector2 direction)
        {
            float angle = Vector2.Angle(Vector2.up, direction.normalized); // Degrees
            var swipeDirection = Swipe.None;

            if (direction.x > 0) // Right
            {
                if (angle < 22.5f) // 0.0 - 22.5
                    swipeDirection = Swipe.Up;
                else if (angle < 67.5f) // 22.5 - 67.5
                    swipeDirection = Swipe.UpRight;
                else if (angle < 112.5f) // 67.5 - 112.5
                    swipeDirection = Swipe.Right;
                else if (angle < 157.5f) // 112.5 - 157.5
                    swipeDirection = Swipe.DownRight;
                else if (angle < 180.0f) // 157.5 - 180.0
                    swipeDirection = Swipe.Down;
            }
            else // Left
            {
                if (angle < 22.5f) // 0.0 - 22.5
                    swipeDirection = Swipe.Up;
                else if (angle < 67.5f) // 22.5 - 67.5
                    swipeDirection = Swipe.UpLeft;
                else if (angle < 112.5f) // 67.5 - 112.5
                    swipeDirection = Swipe.Left;
                else if (angle < 157.5f) // 112.5 - 157.5
                    swipeDirection = Swipe.DownLeft;
                else if (angle < 180.0f) // 157.5 - 180.0
                    swipeDirection = Swipe.Down;
            }

            return swipeDirection;
        }

        /// <summary> Get a SimpleSwipe direction by analyzing a given direction </summary>
        /// <param name="direction"> Swipe direction </param>
        public static SimpleSwipe GetSimpleSwipeDirection(Vector2 direction)
        {
            float angle = Vector2.Angle(Vector2.up, direction.normalized); // Degrees
            var swipeDirection = SimpleSwipe.None;

            if (direction.x > 0) // Right
            {
                if (angle < 22.5f) // 0.0 - 22.5
                    swipeDirection = SimpleSwipe.Up;
                else if (angle < 67.5f) // 22.5 - 67.5
                    swipeDirection = SimpleSwipe.Right;
                else if (angle < 112.5f) // 67.5 - 112.5
                    swipeDirection = SimpleSwipe.Right;
                else if (angle < 157.5f) // 112.5 - 157.5
                    swipeDirection = SimpleSwipe.Right;
                else if (angle < 180.0f) // 157.5 - 180.0
                    swipeDirection = SimpleSwipe.Down;
            }
            else // Left
            {
                if (angle < 22.5f) // 0.0 - 22.5
                    swipeDirection = SimpleSwipe.Up;
                else if (angle < 67.5f) // 22.5 - 67.5
                    swipeDirection = SimpleSwipe.Left;
                else if (angle < 112.5f) // 67.5 - 112.5
                    swipeDirection = SimpleSwipe.Left;
                else if (angle < 157.5f) // 112.5 - 157.5
                    swipeDirection = SimpleSwipe.Left;
                else if (angle < 180.0f) // 157.5 - 180.0
                    swipeDirection = SimpleSwipe.Down;
            }

            return swipeDirection;
        }

        /// <summary> Adds TouchDetector to scene and returns a reference to it </summary>
        private static TouchDetector AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<TouchDetector>(MenuUtils.TouchDetector_GameObject_Name, true, selectGameObjectAfterCreation); }

        #endregion
    }
}