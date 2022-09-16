// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Events;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine
{
    /// <inheritdoc />
    /// <summary>
    ///     Implements a microphone-like device, that interprets game events.
    ///     When triggered it executes a set of callbacks.
    /// </summary>
    [AddComponentMenu(MenuUtils.GameEventListener_AddComponentMenu_MenuName, MenuUtils.GameEventListener_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.GAME_EVENT_LISTENER)]
    public class GameEventListener : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.GameEventListener_MenuItem_ItemName, false, MenuUtils.GameEventListener_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(menuCommand.context as GameObject, true); }
#endif

        #endregion

        #region Properties
        
        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugGameEventListener; } }
        
        #endregion
        
        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;
        
        /// <summary> UnityEvent executed when this listener has been triggered </summary>
        public StringEvent Event;
        
        /// <summary> Game event string value that will trigger this listener's callback </summary>
        public string GameEvent;

        /// <summary> Enables the execution of the Event callback every time a game event is received </summary>
        public bool ListenForAllGameEvents;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            ListenForAllGameEvents = false;
            GameEvent = string.Empty;
            Event = new StringEvent();
        }

        private void Awake() { GameEvent = GameEvent.Trim(); }

        private void OnEnable() { RegisterListener(); }

        private void OnDisable() { UnregisterListener(); }

        #endregion

        #region Private Methods

        private void RegisterListener()
        {
            Message.AddListener<GameEventMessage>(OnMessage);
            if (DebugComponent) DDebug.Log( "[" + name + "] Started listening for GameEvents", this);
            
        }

        private void UnregisterListener()
        {
            Message.RemoveListener<GameEventMessage>(OnMessage);
            if (DebugComponent) DDebug.Log( "[" + name + "] Stopped listening for GameEvents", this);
        }

        private void OnMessage(GameEventMessage message)
        {
            if (ListenForAllGameEvents || GameEvent.Equals(message.EventName))
                InvokeEvent(message);
        }

        private void InvokeEvent(GameEventMessage message)
        {
            if (!message.HasGameEvent) return;
            if (Event == null) return;
            Event.Invoke(message.EventName);
            if (DebugComponent) DDebug.Log( "[" + name + "] Triggered Event: " + message.EventName, this);
        }

        #endregion
        
        #region Static Methods

        // ReSharper disable once UnusedMember.Local
        /// <summary> Adds GameEventListener to scene and returns a reference to it </summary>
        private static GameEventListener AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene(null, selectGameObjectAfterCreation); }

        /// <summary> Adds GameEventListener to scene and returns a reference to it </summary>
        private static GameEventListener AddToScene(GameObject parent, bool selectGameObjectAfterCreation = false)
        {
            var result = DoozyUtils.AddToScene<GameEventListener>(MenuUtils.GameEventListener_GameObject_Name, false, selectGameObjectAfterCreation);
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