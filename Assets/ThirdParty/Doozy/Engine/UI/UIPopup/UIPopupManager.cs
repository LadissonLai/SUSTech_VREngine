// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Settings;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Responsible for showing UIPopups instantly or in a sequence, by putting them in a popup queue.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIPopupManager_AddComponentMenu_MenuName, MenuUtils.UIPopupManager_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIPOPUP_MANAGER)]
    public class UIPopupManager : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIPopupManager_MenuItem_ItemName, false, MenuUtils.UIPopupManager_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected UIPopupManager() { }

        private static UIPopupManager s_instance;

        /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
        public static UIPopupManager Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<UIPopupManager>();
                if (s_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary>
        ///     Holds the reference to the currently visible UIPopup (that is also in the PopupQueue).
        ///     <para />
        ///     Note that there can also be other visible UIPopups that were not added to the PopupQueue.
        ///     <para />
        ///     If no popup from the PopupQueue is visible, it returns null
        /// </summary>
        public static UIPopup CurrentVisibleQueuePopup;

        /// <summary> Direct reference to the UIPopupDatabase that holds all the UIPopup prefab references and their popup names </summary>
        public static UIPopupDatabase PopupDatabase { get { return UIPopupSettings.Database; } }

        // ReSharper disable once InconsistentNaming
        /// <summary> List of UIPopupQueueData entries that holds all the info for the UIPopups that await to be shown in a sequential manner </summary>
        public static readonly List<UIPopupQueueData> PopupQueue = new List<UIPopupQueueData>();

        /// <summary>
        /// 扩展方法，解决popup hide后不删除第二次找不到的bug。
        /// hide后未删除的popup
        /// </summary>
        /// <typeparam name="string">popup name</typeparam>
        /// <typeparam name="UIPopup">target uipopup</typeparam>
        /// <returns></returns>
        public static readonly Dictionary<string, UIPopup> unDestroyedPopups = new Dictionary<string, UIPopup>();

        /// <summary> Returns TRUE if there are no UIPopups, in the PopupQueue, awaiting to be shown </summary>
        public static bool QueueIsEmpty { get { return PopupQueue.Count == 0; } }

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        private static bool ApplicationIsQuitting { get; set; }

        #endregion

        #region Properties

        private bool DebugComponent { get { return DoozySettings.Instance.DebugUIPopupManager; } }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                DDebug.Log("There cannot be two " + typeof(UIPopupManager) + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        #endregion

        #region Static Methods

        /// <summary> Add the passed UIPopup to the PopupQueue </summary>
        /// <param name="popup"> Target UIPopup to be added to the PopupQueue </param>
        /// <param name="instantAction"> When shown, should the popup animate instantly? (in zero seconds) </param>
        public static void AddToQueue(UIPopup popup, bool instantAction = false)
        {
            var data = new UIPopupQueueData(popup, instantAction);
            PopupQueue.Add(data);
            popup.AddedToQueue = true;
            if (Instance.DebugComponent) DDebug.Log("UIPopup '" + popup.PopupName + "' added to the PopupQueue", Instance);
            if (CurrentVisibleQueuePopup != null) return;
            ShowNextInQueue();
        }

        /// <summary> Hides the CurrentVisibleQueuePopup (if visible) and clears the PopupQueue </summary>
        /// <param name="instantAction"> Should the CurrentVisibleQueuePopup (if visible) be hidden instantly? (animate in zero seconds) </param>
        public static void ClearQueue(bool instantAction = false)
        {
            if (CurrentVisibleQueuePopup != null) CurrentVisibleQueuePopup.Hide(instantAction);
            if (QueueIsEmpty) return;
            foreach (UIPopupQueueData data in PopupQueue)
            {
                if (data.Popup == null) continue;
                if (data.Popup == CurrentVisibleQueuePopup) continue;
                data.Popup.Hide(instantAction);
                data.Popup.AddedToQueue = false;
            }

            PopupQueue.Clear();
            if (Instance.DebugComponent) DDebug.Log("PopupQueue Cleared", Instance);
        }

        /// <summary>
        ///     Looks in the UIPopupManager PopupsDatabase for an UIPopup prefab linked to the given popup name.
        ///     If found, it instantiates a clone of it and returns a reference to it. Otherwise it returns null
        /// </summary>
        /// <param name="popupName"> Popup name to search for </param>
        public static UIPopup GetPopup(string popupName)
        {
            if (PopupDatabase.IsEmpty)
            {
                DDebug.Log("No Popups have been defined in the Popups Database. Open the Control Panel at the Popups section and add some there.");
                return null;
            }

            if (!PopupDatabase.Contains(popupName))
            {
                DDebug.Log("No Popup with the name '" + popupName + "' has been defined in the Popups Database. Open the Control Panel at the Popups section and add it there.");
                return null;
            }

            GameObject prefab = PopupDatabase.GetPrefab(popupName);
            if (prefab == null)
            {
                DDebug.Log("No Popup prefab with the '" + popupName + "' PopupName has been defined in the Popups Database. Open the Control Panel at the Popups section and add it there.");
                return null;
            }

            if (unDestroyedPopups.TryGetValue(popupName, out var popup))
            {
                if (popup != null)
                    return popup;
                else
                    unDestroyedPopups.Remove(popupName);
            }

            UICanvas canvas = prefab.GetComponent<UIPopup>().GetTargetCanvas();
            GameObject clone = Instantiate(prefab, canvas.transform);
            popup = clone.GetComponent<UIPopup>();
            popup.SetPopupName(popupName);

            if (!popup.DestroyAfterHide)
                unDestroyedPopups.Add(popupName, popup);

            return popup;
        }

        /// <summary> Retrieves the first UIPopupQueueData registered in the PopupQueue with the given popup name </summary>
        /// <param name="popupName"> The popup name to search for </param>
        private static UIPopupQueueData GetPopupData(string popupName)
        {
            if (string.IsNullOrEmpty(popupName) || QueueIsEmpty) return null;
            foreach (UIPopupQueueData link in PopupQueue)
                if (link.PopupName.Equals(popupName))
                    return link;
            return null;
        }

        /// <summary> Retrieves the first UIPopupQueueData registered in the PopupQueue with the giver popup reference </summary>
        /// <param name="popup"> Target popup to search for </param>
        private static UIPopupQueueData GetPopupData(UIPopup popup)
        {
            if (popup == null || QueueIsEmpty) return null;
            foreach (UIPopupQueueData link in PopupQueue)
                if (link.Popup == popup)
                    return link;

            return null;
        }

        /// <summary>
        ///     Hides the currently visible UIPopup (that is also in the PopupQueue).
        ///     <para />
        ///     Note that there can also be other visible UIPopups that were not added to the PopupQueue.
        ///     <para />
        ///     If no popup from the PopupQueue is visible, it returns false
        /// </summary>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public static bool HideCurrentVisiblePopup(bool instantAction = false)
        {
            if (CurrentVisibleQueuePopup == null) return false;
            CurrentVisibleQueuePopup.Hide(instantAction);
            ShowNextInQueue();
            return true;
        }

        /// <summary> Returns TRUE if at least one UIPopup with the given popup name is found in the PopupQueue </summary>
        /// <param name="popupName"> The popup name to search for </param>
        public static bool IsInQueue(string popupName)
        {
            if (string.IsNullOrEmpty(popupName) || QueueIsEmpty) return false;
            foreach (UIPopupQueueData link in PopupQueue)
                if (link.PopupName.Equals(popupName))
                    return true;

            return false;
        }

        /// <summary> Returns TRUE if at least one entry of the given popup is found in the PopupQueue </summary>
        /// <param name="popup"> Target popup to search for </param>
        public static bool IsInQueue(UIPopup popup)
        {
            if (popup == null || QueueIsEmpty) return false;
            foreach (UIPopupQueueData link in PopupQueue)
                if (link.Popup == popup)
                    return true;

            return false;
        }

        /// <summary> Removes the first UIPopup registered with the given popupName from the PopupQueue (if it exists) </summary>
        /// <param name="popupName"> The popup name to search for </param>
        /// <param name="showNextInQueue"> After removing the corresponding UIPopup from the PopupQueue, should the next popup in queue be shown? </param>
        public static void RemoveFromQueue(string popupName, bool showNextInQueue = true)
        {
            if (!IsInQueue(popupName)) return;
            UIPopupQueueData data = GetPopupData(popupName);
            if (data == null) return;
            PopupQueue.Remove(data);
            if (Instance.DebugComponent) DDebug.Log("UIPopup '" + data.PopupName + "' removed from the PopupQueue", Instance);
            if (data.Popup == null) return;
            data.Popup.AddedToQueue = false;
            if (CurrentVisibleQueuePopup != data.Popup) return;
            CurrentVisibleQueuePopup = null;
            if (showNextInQueue) ShowNextInQueue();
        }

        /// <summary> Removes the given popup reference from the PopupQueue (if it exists) </summary>
        /// <param name="popup"> Target popup to search for </param>
        /// <param name="showNextInQueue"> After removing the corresponding UIPopup from the PopupQueue, should the next popup in queue be shown? </param>
        public static void RemoveFromQueue(UIPopup popup, bool showNextInQueue = true)
        {
            if (!IsInQueue(popup)) return;
            PopupQueue.Remove(GetPopupData(popup));
            if (Instance.DebugComponent) DDebug.Log("UIPopup '" + popup.PopupName + "' added to the PopupQueue", Instance);
            popup.AddedToQueue = false;
            if (CurrentVisibleQueuePopup != popup) return;
            CurrentVisibleQueuePopup = null;
            if (showNextInQueue) ShowNextInQueue();
        }

        /// <summary> Shows the next popup in the PopupQueue (if any) </summary>
        public static void ShowNextInQueue()
        {
            while (true)
            {
                if (QueueIsEmpty) return;
                if (PopupQueue[0].Popup == null)
                {
                    PopupQueue.RemoveAt(0);
                    continue;
                }

                CurrentVisibleQueuePopup = PopupQueue[0].Show();
                break;
            }
        }

        /// <summary> Shows the given popup with the given settings </summary>
        /// <param name="popup"> Target popup that needs to be shown </param>
        /// <param name="addToPopupQueue"> If the popup is added to the PopupQueue, it will be shown when its turn comes up. Until then it will remain hidden. </param>
        /// <param name="instantAction"> When shown, should the popup animate instantly? (in zero seconds) </param>
        /// <param name="targetCanvasName"> Sets a new UICanvas target by looking for an UICanvas with the given name (also re-parents the popup to it) </param>
        public static void ShowPopup(UIPopup popup, bool addToPopupQueue, bool instantAction, string targetCanvasName)
        {
            if (popup == null) return;
            popup.SetTargetCanvasName(targetCanvasName);
            ShowPopup(popup, addToPopupQueue, instantAction);
        }

        /// <summary> Shows the given popup with the given settings </summary>
        /// <param name="popup"> Target popup that needs to be shown </param>
        /// <param name="addToPopupQueue"> If the popup is added to the PopupQueue, it will be shown when its turn comes up. Until then it will remain hidden. </param>
        /// <param name="instantAction"> When shown, should the popup animate instantly? (in zero seconds) </param>
        public static void ShowPopup(UIPopup popup, bool addToPopupQueue, bool instantAction)
        {
            if (popup == null) return;
            if (addToPopupQueue)
            {
                AddToQueue(popup, instantAction);
                return;
            }

            if (Instance.DebugComponent) DDebug.Log("Showing UIPopup '" + popup.PopupName + "'", Instance);
            popup.Show(instantAction);
        }

        /// <summary> Shows the given popup with the given settings and returns a reference to it </summary>
        /// <param name="popupName"> The popup name to search for in the PopupDatabase linked to a UIPopup prefab. If the prefab is found, a clone of it will get instantiated and then shown </param>
        /// <param name="addToPopupQueue"> If the popup is added to the PopupQueue, it will be shown when its turn comes up. Until then it will remain hidden </param>
        /// <param name="instantAction"> When shown, should the popup animate instantly? (in zero seconds) </param>
        /// <param name="targetCanvasName"> Sets a new UICanvas target by looking for an UICanvas with the given name (also re-parents the popup to it) </param>
        public static UIPopup ShowPopup(string popupName, bool addToPopupQueue, bool instantAction, string targetCanvasName)
        {
            UIPopup popup = GetPopup(popupName);
            ShowPopup(popup, addToPopupQueue, instantAction, targetCanvasName);
            return popup;
        }

        /// <summary> Shows the given popup with the given settings and returns a reference to it </summary>
        /// <param name="popupName"> The popup name to search for in the PopupDatabase linked to a UIPopup prefab. If the prefab is found, a clone of it will get instantiated and then shown </param>
        /// <param name="addToPopupQueue"> If the popup is added to the PopupQueue, it will be shown when its turn comes up. Until then it will remain hidden </param>
        /// <param name="instantAction"> When shown, should the popup animate instantly? (in zero seconds) </param>
        public static UIPopup ShowPopup(string popupName, bool addToPopupQueue, bool instantAction)
        {
            UIPopup popup = GetPopup(popupName);
            ShowPopup(popup, addToPopupQueue, instantAction);
            return popup;
        }

        /// <summary> Adds UIPopupManager to scene and returns a reference to it </summary>
        private static UIPopupManager AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<UIPopupManager>(MenuUtils.UIPopupManager_GameObject_Name, true, selectGameObjectAfterCreation); }

        #endregion
    }
}