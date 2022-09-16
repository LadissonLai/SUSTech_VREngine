// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Implements a microphone-like device, that interprets UIButton UIButtonBehaviorType.
    ///     <para />
    ///     When triggered it executes a set of callbacks.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIButtonListener_AddComponentMenu_MenuName, MenuUtils.UIButtonListener_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIBUTTON_LISTENER)]
    public class UIButtonListener : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIButtonListener_MenuItem_ItemName, false, MenuUtils.UIButtonListener_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(menuCommand.context as GameObject, true); }
#endif

        #endregion

        #region Public Variables

        /// <summary> UIButton Button Category filter that will invoke this listener's events </summary>
        public string ButtonCategory;

        /// <summary> UIButton Button Name filter that will invoke this listener's events </summary>
        public string ButtonName;

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> UnityEvent executed when this listener has been triggered </summary>
        public UIButtonEvent Event;

        /// <summary> If TRUE, this listener's events will get invoked every time an UIButton is activated </summary>
        public bool ListenForAllUIButtons;

        /// <summary> The action that an UIButton performs in order for this listener's events to get executed </summary>
        public UIButtonBehaviorType TriggerAction;

        #endregion

        #region Private Variables

        /// <summary> Internal variable used to test if this UIButton listener is listening for the 'Back' button </summary>
        private bool m_listeningForBackButton;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            ButtonCategory = UIButton.DefaultButtonCategory;
            ButtonName = UIButton.DefaultButtonName;
            ListenForAllUIButtons = false;
            TriggerAction = UIButtonBehaviorType.OnClick;
            Event = new UIButtonEvent();
        }

        private void OnEnable() { RegisterListener(); }

        private void OnDisable() { UnregisterListener(); }

        #endregion

        #region Private Methods

        private void RegisterListener()
        {
            Message.AddListener<UIButtonMessage>(OnMessage);
            m_listeningForBackButton = ButtonName.Equals(UIButton.BackButtonName);
            if (DebugMode) DDebug.Log("[" + name + "] Started listening for UIButton actions", this);
        }

        private void UnregisterListener()
        {
            Message.RemoveListener<UIButtonMessage>(OnMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Stopped listening for UIButton actions", this);
        }

        private void OnMessage(UIButtonMessage message)
        {
            if (m_listeningForBackButton && (message.ButtonName.Equals(UIButton.BackButtonName) || message.Button != null && message.Button.IsBackButton))
            {
                InvokeEvent(message);
                return;
            }

            if (ListenForAllUIButtons)
            {
                InvokeEvent(message);
                return;
            }

            if (message.Button != null && message.Button.ButtonCategory.Equals(ButtonCategory) && message.Button.ButtonName.Equals(ButtonName))
                InvokeEvent(message);
        }

        private void InvokeEvent(UIButtonMessage message)
        {
            if (Event == null) return;
            if (TriggerAction != message.Type) return;
            Event.Invoke(message.Button);
            if (DebugMode)
                DDebug.Log("[" + name + "] Triggered Event: " + "[" + message.Type + "] " + (message.Button != null
                                                                                                 ? (message.Button.ButtonCategory + " - " + message.Button.ButtonName)
                                                                                                 : message.ButtonName),
                           this);
        }

        #endregion

        #region Static Methods

        /// <summary> Adds UIButtonListener to scene and returns a reference to it </summary>
        // ReSharper disable once UnusedMember.Local
        private static UIButtonListener AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene(null, selectGameObjectAfterCreation); }

        /// <summary> Adds UIButtonListener to scene and returns a reference to it </summary>
        private static UIButtonListener AddToScene(GameObject parent, bool selectGameObjectAfterCreation = false)
        {
            var result = DoozyUtils.AddToScene<UIButtonListener>(MenuUtils.UIButtonListener_GameObject_Name, false, selectGameObjectAfterCreation);
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