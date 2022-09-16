// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System.Collections;
using Doozy.Engine;
using Doozy.Engine.UI;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Integrations.Playmaker
{
    /// <summary>
    ///     Sends game events and/or button clicks (button names of the buttons that were clicked) to any Playmaker FSM.
    /// </summary>
#if dUI_Playmaker
    [AddComponentMenu(MenuUtils.PlaymakerEventDispatcher_AddComponentMenu_MenuName, MenuUtils.PlaymakerEventDispatcher_AddComponentMenu_Order)]
#endif
    public class PlaymakerEventDispatcher : MonoBehaviour
    {
#if dUI_Playmaker
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.PlaymakerEventDispatcher_MenuItem_ItemName, false, MenuUtils.PlaymakerEventDispatcher_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Properties

        /// <summary> Returns TRUE if this event dispatcher is enabled </summary>
        public bool IsEnabled { get { return TargetFsm != null && (DispatchGameEvents || DispatchButtonClicks); } }

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> If TRUE, all the button clicks (names) will get dispatched to the target FSM </summary>
        public bool DispatchButtonClicks;

        /// <summary> If TRUE, all the game events will get dispatched to the target FSM </summary>
        public bool DispatchGameEvents;

        /// <summary> [Editor Only] Override the target FSM reference in the Inspector  </summary>
        public bool OverrideTargetFsm;

        /// <summary> The target FSM that will receive all the game events and/or button clicks (names) </summary>
        public PlayMakerFSM TargetFsm;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            TargetFsm = GetComponent<PlayMakerFSM>();
#if UNITY_EDITOR
            if (TargetFsm == null)
            {
                if (EditorUtility.DisplayDialog("Action Required", "The Playmaker Event Dispatcher attached to the '" + gameObject.name + "' gameObject did not find any PlayMakerFSM component attached as well." +
                                                                   "\n\n" +
                                                                   "Would you like to attach one now and reference it as the Target FSM for this dispatcher?", "Yes", "No"))
                {
                    TargetFsm = gameObject.AddComponent<PlayMakerFSM>();
                    OverrideTargetFsm = false;
                }
                else
                {
                    EditorUtility.DisplayDialog("Info", "You chose not to attach a PlayMakerFSM." +
                                                        "\n\n" +
                                                        "Remember that you need to reference one in order for this dispatcher to do anything." +
                                                        "\n\n", "Ok");
                    OverrideTargetFsm = true;
                }
            }
#endif
            DispatchGameEvents = false;
            DispatchButtonClicks = false;
        }

        private void Awake()
        {
            if (TargetFsm != null) return;
            DDebug.Log("The Target FSM, for the Playmaker Event Dispatcher attached to the '" + gameObject.name + "' gameObject, is null. This dispatcher is disabled.");
            enabled = false;
        }

        private void OnEnable()
        {
            if (DispatchGameEvents) RegisterGameEventListener();
            if (DispatchButtonClicks) RegisterButtonClicksListener();
        }

        private void OnDisable()
        {
            if (DispatchGameEvents) UnregisterGameEventListener();
            if (DispatchButtonClicks) UnregisterButtonClicksListener();
        }

        #endregion

        #region Private Methods

        private void RegisterGameEventListener()
        {
            Message.AddListener<GameEventMessage>(OnGameEventMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Started listening for GameEvents", this);
        }

        private void UnregisterGameEventListener()
        {
            Message.RemoveListener<GameEventMessage>(OnGameEventMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Stopped listening for GameEvents", this);
        }

        private void OnGameEventMessage(GameEventMessage message) { DispatchEvent(message.EventName); }

        private void RegisterButtonClicksListener()
        {
            Message.AddListener<UIButtonMessage>(OnUIButtonMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Started listening for UIButton actions", this);
        }

        private void UnregisterButtonClicksListener()
        {
            Message.RemoveListener<UIButtonMessage>(OnUIButtonMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Stopped listening for UIButton actions", this);
        }

        private void OnUIButtonMessage(UIButtonMessage message)
        {
            if (message.Type != UIButtonBehaviorType.OnClick) return;
            DispatchEvent(message.ButtonName);
        }

        /// <summary> Dispatch an event name that can be either a game event or button click (name) </summary>
        private void DispatchEvent(string eventValue) { StartCoroutine(DispatchEventAtEndOfFrame(eventValue)); }

        #endregion

        #region Enumerators

        private IEnumerator DispatchEventAtEndOfFrame(string eventValue)
        {
            yield return new WaitForEndOfFrame();

            if (TargetFsm == null)
            {
                DDebug.LogWarning("The targetFSM for the Event Dispatcher attached to [" + gameObject.name + "] gameObject is null. This should not happen.");
                yield break;
            }

            TargetFsm.SendEvent(eventValue);
            if (DebugMode) DDebug.Log("Sent the FSM Event '" + eventValue + "' to the '" + TargetFsm.FsmName + "' target FSM that is attached to the '" + TargetFsm.name + "' gameObject.");
        }

        #endregion

        #region Static Methods

        /// <summary> Adds PlaymakerEventDispatcher to scene and returns a reference to it </summary>
        private static PlaymakerEventDispatcher AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<PlaymakerEventDispatcher>(MenuUtils.PlaymakerEventDispatcher_GameObject_Name, false, selectGameObjectAfterCreation); }

        #endregion
#endif
    }
}