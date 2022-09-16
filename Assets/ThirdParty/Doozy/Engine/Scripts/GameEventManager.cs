// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.Settings;
using Doozy.Engine.UI.Input;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine
{
    /// <inheritdoc />
    /// <summary>
    ///     Contains predefined actions for system events.
    ///     <para />
    ///     If the sent game event is a system event, its predefined actions are triggered.
    ///     <para />
    ///     A game event string is considered to be a system event if it is contained in the SystemGameEvent enum values.
    /// </summary>
    [AddComponentMenu(MenuUtils.GameEventManager_AddComponentMenu_MenuName, MenuUtils.GameEventManager_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.GAME_EVENT_MANAGER)]
    public class GameEventManager : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.GameEventManager_MenuItem_ItemName, false, MenuUtils.GameEventManager_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected GameEventManager() { }

        private static GameEventManager s_instance;

        /// <summary> Returns a reference to the GameEventsManager in the Scene. If one does not exist, it gets created. </summary>
        public static GameEventManager Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<GameEventManager>();
                if (s_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        private static bool ApplicationIsQuitting { get; set; }

        #endregion

        #region Properties

        private bool DebugComponent { get { return DoozySettings.Instance.DebugGameEventManager; } }

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
                DDebug.Log( "There cannot be two " + typeof(GameEventManager) + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        #endregion

        #region Static Methods

        /// <summary> Adds GameEventsManager to scene and returns a reference to it </summary>
        public static GameEventManager AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<GameEventManager>(MenuUtils.GameEventManager_GameObject_Name, true, selectGameObjectAfterCreation); }

        /// <summary>
        ///     Checks to see if the message contains a system event and, if it does, it triggers a predefined action.
        ///     <para />
        ///     A game event string is considered to be a system event if it is contained in the SystemGameEvent enum values.
        ///     <para />
        ///     This method is automatically called every time a GameEventMessage is sent.
        /// </summary>
        /// <param name="message"> Target GameEvent message </param>
        /// <param name="debug"> Print info debug messages to console? </param>
        public static void ProcessGameEvent(GameEventMessage message, bool debug = false)
        {
            if (message == null) return;
            if (Instance.DebugComponent || debug && !message.IsSystemEvent) DDebug.Log("Received '" + message.EventName + "' game event.", Instance);
            if (!message.IsSystemEvent) return;
            var @event = (SystemGameEvent) Enum.Parse(typeof(SystemGameEvent), message.EventName);
            if (Instance.DebugComponent || debug) DDebug.Log("Received '" + @event + "' system game event.", Instance);

            switch (@event)
            {
                case SystemGameEvent.Back:
                    BackButton.Instance.Execute();
                    break;
                case SystemGameEvent.ApplicationQuit:
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
                case SystemGameEvent.ActivateLoadedScenes:
                    SceneLoader.ActivateLoadedScenes();
                    break;
            }
        }

        #endregion
    }
}