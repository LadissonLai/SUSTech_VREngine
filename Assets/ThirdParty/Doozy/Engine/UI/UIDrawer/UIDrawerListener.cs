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
    ///     Implements a microphone-like device, that interprets UIDrawer UIDrawerBehaviorType.
    ///     When triggered it executes a set of callbacks.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIDrawerListener_AddComponentMenu_MenuName, MenuUtils.UIDrawerListener_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIDRAWER_LISTENER)]
    public class UIDrawerListener : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIDrawerListener_MenuItem_ItemName, false, MenuUtils.UIDrawerListener_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(menuCommand.context as GameObject, true); }
#endif

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> The Drawer Name filter that will invoke this listener's events </summary>
        public string DrawerName;

        // ReSharper disable once NotAccessedField.Global
        /// <summary> Internal variable used by the custom inspector to allow you to type a custom drawer name instead of selecting it from the database </summary>
        public bool CustomDrawerName;

        /// <summary> UnityEvent executed when this listener has been triggered </summary>
        public UIDrawerEvent Event;

        /// <summary> If TRUE, this listener's events will get invoked every time an UIDrawer is activated </summary>
        public bool ListenForAllUIDrawers;

        /// <summary> The action that an UIDrawer performs in order for this listener's events to get executed </summary>
        public UIDrawerBehaviorType TriggerAction;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            DrawerName = UIDrawer.DefaultDrawerName;
            ListenForAllUIDrawers = false;
            TriggerAction = UIDrawerBehaviorType.Open;
            Event = new UIDrawerEvent();
        }

        private void OnEnable() { RegisterListener(); }

        private void OnDisable() { UnregisterListener(); }

        #endregion

        #region Private Methods

        private void RegisterListener()
        {
            Message.AddListener<UIDrawerMessage>(OnMessage);
            if (DebugMode) DDebug.Log( "[" + name + "] Started listening for UIDrawer actions", this);
        }

        private void UnregisterListener()
        {
            Message.RemoveListener<UIDrawerMessage>(OnMessage);
            if (DebugMode) DDebug.Log( "[" + name + "] Stopped listening for UIDrawer actions", this);
        }

        private void OnMessage(UIDrawerMessage message)
        {
            if (ListenForAllUIDrawers ||
                message.Drawer.DrawerName.Equals(DrawerName))
                InvokeEvent(message);
        }

        private void InvokeEvent(UIDrawerMessage message)
        {
            if (Event == null) return;
            if (TriggerAction != message.Type) return;
            Event.Invoke(message.Drawer);
            if (DebugMode) DDebug.Log( "[" + name + "] Triggered Event: " + "[" + message.Type + "] " + message.Drawer.DrawerName, this);
        }

        #endregion

        #region Static Methods

        /// <summary> Adds UIDrawerListener to scene and returns a reference to it </summary>
        // ReSharper disable once UnusedMember.Local
        private static UIDrawerListener AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene(null, selectGameObjectAfterCreation); }

        /// <summary> Adds UIButtonListener to scene and returns a reference to it </summary>
        private static UIDrawerListener AddToScene(GameObject parent, bool selectGameObjectAfterCreation = false)
        {
            var result = DoozyUtils.AddToScene<UIDrawerListener>(MenuUtils.UIDrawerListener_GameObject_Name, false, selectGameObjectAfterCreation);
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