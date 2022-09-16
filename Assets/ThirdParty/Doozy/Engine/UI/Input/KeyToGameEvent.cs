// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Input
{
    /// <inheritdoc />
    /// <summary> Listens for the set input key or virtual button and, when triggered, sends the set game event </summary>
    [AddComponentMenu(MenuUtils.KeyToGameEvent_AddComponentMenu_MenuName, MenuUtils.KeyToGameEvent_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.KEY_TO_GAME_EVENT)]
    public class KeyToGameEvent : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.KeyToGameEvent_MenuItem_ItemName, false, MenuUtils.KeyToGameEvent_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Properties

        /// <summary> Returns TRUE if a valid game event is set (non null & non empty) </summary>
        public bool HasGameEvent { get { return string.IsNullOrEmpty(GameEvent.Trim()) == false; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugKeyToGameEvent; } }
        
        #endregion

        #region Public Variables

        /// <summary> Enable relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Key code and virtual button input settings </summary>
        public InputData InputData = new InputData();

        /// <summary> Game Event to send when this Key To Game Event is triggered </summary>
        public string GameEvent;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (InputData == null) enabled = false;
        }

        private void Update()
        {
            if (InputData.InputMode == InputMode.None) return;
            bool execute = false;
            switch (InputData.InputMode)
            {
                case InputMode.KeyCode:
                    if (UnityEngine.Input.GetKeyDown(InputData.KeyCode))
                    {
                        if (HasGameEvent)
                        {
                            if (DebugComponent) DDebug.Log("Sent '" + GameEvent + "' via KeyCode: " + InputData.KeyCode, gameObject);
                            execute = true;
                        }
                    }
                    else if (InputData.EnableAlternateInputs && UnityEngine.Input.GetKeyDown(InputData.KeyCodeAlt))
                    {
                        if (HasGameEvent)
                        {
                            if (DebugComponent) DDebug.Log("Sent '" + GameEvent + "' via KeyCode: " + InputData.KeyCodeAlt, gameObject);
                            execute = true;
                        }
                    }

                    break;
                case InputMode.VirtualButton:
                    if (UnityEngine.Input.GetButtonDown(InputData.VirtualButtonName))
                    {
                        if (HasGameEvent)
                        {
                            if (DebugComponent) DDebug.Log("Sent '" + GameEvent + "' via Virtual Button: " + InputData.VirtualButtonName, gameObject);
                            execute = true;
                        }
                    }
                    else if (InputData.EnableAlternateInputs && UnityEngine.Input.GetButtonDown(InputData.VirtualButtonNameAlt))
                    {
                        if (HasGameEvent)
                        {
                            if (DebugComponent) DDebug.Log("Sent '" + GameEvent + "' via Virtual Button: " + InputData.VirtualButtonNameAlt, gameObject);
                            execute = true;
                        }
                    }

                    break;
            }

            if (!execute) return;
            Execute();
        }

        #endregion

        #region Public Methods

        /// <summary> Send the set game event </summary>
        public void Execute() { GameEventMessage.SendEvent(GameEvent, gameObject); }

        #endregion

        #region Static Methods

        /// <summary> Add a KeyToGameEvent to scene and get a reference to it </summary>
        private static KeyToGameEvent AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<KeyToGameEvent>(MenuUtils.KeyToGameEvent_GameObject_Name, false, selectGameObjectAfterCreation); }

        #endregion
    }
}