// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable UnusedMember.Local

namespace Doozy.Engine.Touchy
{
    /// <inheritdoc />
    /// <summary>
    ///     Representation of a touch listener.
    ///     This class implements a microphone-like device, that interprets touch info.
    ///     When triggered it executes a set of callbacks and sends any defined game events.
    /// </summary>
    [AddComponentMenu(MenuUtils.GestureListener_AddComponentMenu_MenuName, MenuUtils.GestureListener_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.GESTURE_LISTENER)]
    public class GestureListener : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.GestureListener_MenuItem_ItemName, false, MenuUtils.GestureListener_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(menuCommand.context as GameObject, true); }
#endif

        #endregion

        #region Properties

        /// <summary> Direct reference to the TouchySettings asset </summary>
        private static TouchySettings Settings { get { return TouchySettings.Instance; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugGestureListener; } }

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> If TRUE, it will trigger this listener regardless of the target GameObject </summary>
        public bool GlobalListener;

        /// <summary> Allows you to set another target GameObject reference (in the Inspector), other than the GameObject this component is attached to </summary>
        public bool OverrideTarget;

        /// <summary> Only gestures performed on the target game object will trigger this listener </summary>
        public GameObject TargetGameObject;

        /// <summary> The gesture type that this listener will react to </summary>
        public GestureType GestureType;

        /// <summary>
        ///     The swipe direction that this listener will react to.
        ///     This works only if the Gesture NamesDatabaseType is set to GestureType.Swipe
        /// </summary>
        public Swipe SwipeDirection = Swipe.None;

        /// <summary> Callback executed when this listener is triggered </summary>
        public TouchInfoEvent OnGestureEvent;

        /// <summary> Action executed when this listener is triggered </summary>
        public Action<TouchInfo> OnGestureAction;

        /// <summary> Game events sent when this listener is triggered </summary>
        public List<string> GameEvents;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            GlobalListener = false;
            OverrideTarget = false;
            TargetGameObject = gameObject;
            GestureType = GestureType.Tap;
            SwipeDirection = Swipe.None;
            OnGestureEvent = new TouchInfoEvent();
            OnGestureAction = delegate { };
            GameEvents = new List<string>();
        }

        private void Awake()
        {
            TouchDetector.Init();
            if (OverrideTarget && (!OverrideTarget || TargetGameObject != null)) return;
            TargetGameObject = gameObject;
            if (OnGestureAction == null) OnGestureAction = delegate { };
        }

        private void OnEnable() { RegisterToTouchDetector(); }

        private void OnDisable() { UnregisterFromTouchDetector(); }

        #endregion

        #region Private Methods

        private void RegisterToTouchDetector()
        {
            TouchDetector.Instance.OnTapAction += HandleTap;
            TouchDetector.Instance.OnLongTapAction += HandleLongTap;
            TouchDetector.Instance.OnSwipeAction += HandleSwipe;
        }

        private void UnregisterFromTouchDetector()
        {
            if (TouchDetector.ApplicationIsQuitting || TouchDetector.Instance == null) return;
            // ReSharper disable once DelegateSubtraction
            if (TouchDetector.Instance.OnTapAction != null) TouchDetector.Instance.OnTapAction -= HandleTap;
            // ReSharper disable once DelegateSubtraction
            if (TouchDetector.Instance.OnLongTapAction != null) TouchDetector.Instance.OnLongTapAction -= HandleLongTap;
            // ReSharper disable once DelegateSubtraction
            if (TouchDetector.Instance.OnSwipeAction != null) TouchDetector.Instance.OnSwipeAction -= HandleSwipe;
        }

        private void HandleTap(TouchInfo touchInfo)
        {
            if (GestureType != GestureType.Tap) return; //check that this is the gesture type this listener is listening for
            if (!HasValidTarget(touchInfo)) return;     //if this is not a global listener -> check that is a valid gameObject and the correct touchInfo target
            if (DebugComponent) DDebug.Log(string.Format("OnTap on {0}: {1}", gameObject.name, touchInfo), this);
            TriggerListener(touchInfo);
        }

        private void HandleLongTap(TouchInfo touchInfo)
        {
            if (GestureType != GestureType.LongTap) return; //check that this is the gesture type this listener is listening for
            if (!HasValidTarget(touchInfo)) return;         //if this is not a global listener -> check that is a valid gameObject and the correct touchInfo target
            if (DebugComponent) DDebug.Log(string.Format("OnLongTap on {0}: {1}", gameObject.name, touchInfo), this);
            TriggerListener(touchInfo);
        }

        private void HandleSwipe(TouchInfo touchInfo)
        {
            if (GestureType != GestureType.Swipe) return;      //check that this is the gesture type this listener is listening for
            if (!HasValidTarget(touchInfo)) return;            //if this is not a global listener -> check that is a valid gameObject and the correct touchInfo target
            if (touchInfo.Direction != SwipeDirection) return; //check that the swipe happened in the direction this listener is listening for
            if (DebugComponent) DDebug.Log(string.Format("OnSwipe on {0}: {1}", gameObject.name, touchInfo), this);
            TriggerListener(touchInfo);
        }

        /// <summary> Returns TRUE if this is a global listener or if the touch info happened over the target game object </summary>
        private bool HasValidTarget(TouchInfo touchInfo)
        {
            
            return GlobalListener || TargetGameObject != null && touchInfo.GameObject == TargetGameObject;
        }

        /// <summary> Performs the actions set on this listener </summary>
        /// <param name="touchInfo"> Touch information passed with by this listener </param>
        private void TriggerListener(TouchInfo touchInfo)
        {
            OnGestureEvent.Invoke(touchInfo);
            if (OnGestureAction != null) OnGestureAction.Invoke(touchInfo);
            SendGameEvents();
        }

        private void SendGameEvents()
        {
            if (GameEvents == null || GameEvents.Count == 0) return; //check that there is at least one game event that needs to be sent before starting a coroutine
            StartCoroutine(SendGameEventsInTheNextFrame());
        }

        #endregion

        #region IEnumerators

        private IEnumerator SendGameEventsInTheNextFrame()
        {
            yield return null;
            GameEventMessage.SendEvents(GameEvents, gameObject);
        }

        #endregion

        #region Static Methods

        /// <summary> Adds GestureListener to scene and returns a reference to it </summary>
        private static GestureListener AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene(null, selectGameObjectAfterCreation); }

        /// <summary> Adds GestureListener to scene and returns a reference to it </summary>
        private static GestureListener AddToScene(GameObject parent, bool selectGameObjectAfterCreation = false)
        {
            var result = DoozyUtils.AddToScene<GestureListener>(MenuUtils.GestureListener_GameObject_Name, false, selectGameObjectAfterCreation);
#if UNITY_EDITOR
            GameObjectUtility.SetParentAndAlign(result.gameObject, parent);
#endif
            if (result.transform.root.GetComponent<RectTransform>() == null) return result; //check to see if it was added to the UI or not
            result.gameObject.AddComponent<RectTransform>();                                //was added to the UI -> add a RectTransform component
            result.GetComponent<RectTransform>().localScale = Vector3.one;                  //reset the RectTransform component
            return result;
        }

        #endregion
    }
}