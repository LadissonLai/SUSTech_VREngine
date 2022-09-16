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
    ///     Implements a microphone-like device, that interprets UIView UIViewBehaviorType.
    ///     <para />
    ///     When triggered it executes a set of callbacks.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIViewListener_AddComponentMenu_MenuName, MenuUtils.UIViewListener_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIVIEW_LISTENER)]
    public class UIViewListener : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIViewListener_MenuItem_ItemName, false, MenuUtils.UIViewListener_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(menuCommand.context as GameObject, true); }
#endif

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> UnityEvent executed when this listener has been triggered </summary>
        public UIViewEvent Event;

        /// <summary> If TRUE, this listener's events will get invoked every time an UIView is activated </summary>
        public bool ListenForAllUIViews;

        /// <summary> The action that an UIView performs in order for this listener's events to get executed </summary>
        public UIViewBehaviorType TriggerAction;

        /// <summary> The View Category filter that will invoke this listener's events </summary>
        public string ViewCategory;

        /// <summary> The View Name filter that will invoke this listener's events </summary>
        public string ViewName;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            ViewCategory = UIView.DefaultViewCategory;
            ViewName = UIView.DefaultViewName;
            ListenForAllUIViews = false;
            TriggerAction = UIViewBehaviorType.Show;
            Event = new UIViewEvent();
        }

        private void OnEnable() { RegisterListener(); }

        private void OnDisable() { UnregisterListener(); }

        #endregion

        #region Private Methods

        private void RegisterListener()
        {
            Message.AddListener<UIViewMessage>(OnMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Started listening for UIView actions", this);
        }

        private void UnregisterListener()
        {
            Message.RemoveListener<UIViewMessage>(OnMessage);
            if (DebugMode) DDebug.Log("[" + name + "] Stopped listening for UIView actions", this);
        }

        private void OnMessage(UIViewMessage message)
        {
            if (ListenForAllUIViews ||
                message.View.ViewCategory.Equals(ViewCategory) && message.View.ViewName.Equals(ViewName))
                InvokeEvent(message);
        }

        private void InvokeEvent(UIViewMessage message)
        {
            if (Event == null) return;
            if (TriggerAction != message.Type) return;
            Event.Invoke(message.View);
            if (DebugMode) DDebug.Log("[" + name + "] Triggered Event: " + "[" + message.Type + "] " + message.View.ViewCategory + " - " + message.View.ViewName, this);
        }

        #endregion

        #region Static Methods

        /// <summary> Adds UIViewListener to scene and returns a reference to it </summary>
        // ReSharper disable once UnusedMember.Local
        private static UIViewListener AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene(null, selectGameObjectAfterCreation); }

        /// <summary> Adds UIViewListener to scene and returns a reference to it </summary>
        private static UIViewListener AddToScene(GameObject parent, bool selectGameObjectAfterCreation = false)
        {
            var result = DoozyUtils.AddToScene<UIViewListener>(MenuUtils.UIViewListener_GameObject_Name, false, selectGameObjectAfterCreation);
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