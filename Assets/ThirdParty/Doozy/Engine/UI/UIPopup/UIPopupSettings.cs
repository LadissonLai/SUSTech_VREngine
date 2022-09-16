// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Settings
{
    [Serializable]
    public class UIPopupSettings : ScriptableObject
    {
        public const string FILE_NAME = "UIPopupSettings";
        private static string ResourcesPath { get { return DoozyPath.UIPOPUP_RESOURCES_PATH; } }

        private static UIPopupSettings s_instance;

        public static UIPopupSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<UIPopupSettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        // ReSharper disable once InconsistentNaming
        [SerializeField] private UIPopupDatabase database;

        public static UIPopupDatabase Database
        {
            get
            {
                if (Instance.database != null) return Instance.database;
                UpdateDatabase();
                return Instance.database;
            }
        }

        public static void UpdateDatabase()
        {
            Instance.database = AssetUtils.GetScriptableObject<UIPopupDatabase>("_" + DoozyPath.UIPOPUP_DATABASE, DoozyPath.GetDataPath(DoozyPath.ComponentName.UIPopup), false, false);
#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.SearchForUnregisteredLinks(false);
            Instance.database.RefreshDatabase(false, false);
            Instance.SetDirty(true);
#endif
        }


        public const bool ADD_TO_POPUP_QUEUE_DEFAULT_VALUE = true;
        public const bool AUTO_HIDE_AFTER_SHOW_DEFAULT_VALUE = false;
        public const bool AUTO_SELECT_BUTTON_AFTER_SHOW_DEFAULT_VALUE = false;
        public const bool BLOCK_BACK_BUTTON_DEFAULT_VALUE = true;
        public const bool CUSTOM_CANVAS_NAME_DEFAULT_VALUE = false;
        public const bool DESTROY_AFTER_HIDE_DEFAULT_VALUE = true;
        public const bool HIDE_ON_ANY_BUTTON_DEFAULT_VALUE = false;
        public const bool HIDE_ON_BACK_BUTTON_DEFAULT_VALUE = true;
        public const bool HIDE_ON_CLICK_ANYWHERE_DEFAULT_VALUE = false;
        public const bool HIDE_ON_CLICK_CONTAINER_DEFAULT_VALUE = true;
        public const bool HIDE_ON_CLICK_OVERLAY_DEFAULT_VALUE = true;
        public const bool UPDATE_HIDE_PROGRESSOR_ON_SHOW_DEFAULT_VALUE = false;
        public const bool UPDATE_SHOW_PROGRESSOR_ON_HIDE_DEFAULT_VALUE = false;
        public const bool USE_OVERLAY_DEFAULT_VALUE = true;
        public const float AUTO_HIDE_AFTER_SHOW_DELAY_DEFAULT_VALUE = 3f;
        public const float DISABLE_WHEN_HIDDEN_TIME_BUFFER = 0.05f; //after an UIPopup has been hidden, the system will wait for an additional time buffer before it sets the Source's active state to false. This is a failsafe measure and fixes a small bug on iOS
        public const PopupDisplayOn DISPLAY_ON_DEFAULT_VALUE = PopupDisplayOn.PopupCanvas;
        public const VisibilityState VISIBILITY_DEFAULT_VALUE = VisibilityState.Visible;

        public PopupDisplayOn DisplayTarget = DISPLAY_ON_DEFAULT_VALUE;
        public bool AddToPopupQueue = ADD_TO_POPUP_QUEUE_DEFAULT_VALUE;
        public bool AutoHideAfterShow = AUTO_HIDE_AFTER_SHOW_DEFAULT_VALUE;
        public bool AutoSelectButtonAfterShow = AUTO_SELECT_BUTTON_AFTER_SHOW_DEFAULT_VALUE;
        public bool BlockBackButton = BLOCK_BACK_BUTTON_DEFAULT_VALUE;
        public bool CustomCanvasName = CUSTOM_CANVAS_NAME_DEFAULT_VALUE;
        public bool DestroyAfterHide = DESTROY_AFTER_HIDE_DEFAULT_VALUE;
        public bool HideOnAnyButton = HIDE_ON_ANY_BUTTON_DEFAULT_VALUE;
        public bool HideOnBackButton = HIDE_ON_BACK_BUTTON_DEFAULT_VALUE;
        public bool HideOnClickAnywhere = HIDE_ON_CLICK_ANYWHERE_DEFAULT_VALUE;
        public bool HideOnClickContainer = HIDE_ON_CLICK_CONTAINER_DEFAULT_VALUE;
        public bool HideOnClickOverlay = HIDE_ON_CLICK_OVERLAY_DEFAULT_VALUE;
        public bool UpdateHideProgressorOnShow = UPDATE_HIDE_PROGRESSOR_ON_SHOW_DEFAULT_VALUE;
        public bool UpdateShowProgressorOnHide = UPDATE_SHOW_PROGRESSOR_ON_HIDE_DEFAULT_VALUE;
        public bool UseOverlay = USE_OVERLAY_DEFAULT_VALUE;
        public float AutoHideAfterShowDelay = AUTO_HIDE_AFTER_SHOW_DELAY_DEFAULT_VALUE;
        public string CanvasName = UIPopup.DefaultTargetCanvasName;

        private void Reset()
        {
            AddToPopupQueue = ADD_TO_POPUP_QUEUE_DEFAULT_VALUE;
            AutoHideAfterShow = AUTO_HIDE_AFTER_SHOW_DEFAULT_VALUE;
            AutoHideAfterShowDelay = AUTO_HIDE_AFTER_SHOW_DELAY_DEFAULT_VALUE;
            AutoSelectButtonAfterShow = AUTO_SELECT_BUTTON_AFTER_SHOW_DEFAULT_VALUE;
            BlockBackButton = BLOCK_BACK_BUTTON_DEFAULT_VALUE;
            CanvasName = UIPopup.DefaultTargetCanvasName;
            CustomCanvasName = CUSTOM_CANVAS_NAME_DEFAULT_VALUE;
            DestroyAfterHide = DESTROY_AFTER_HIDE_DEFAULT_VALUE;
            DisplayTarget = DISPLAY_ON_DEFAULT_VALUE;
            HideOnAnyButton = HIDE_ON_ANY_BUTTON_DEFAULT_VALUE;
            HideOnBackButton = HIDE_ON_BACK_BUTTON_DEFAULT_VALUE;
            HideOnClickAnywhere = HIDE_ON_CLICK_ANYWHERE_DEFAULT_VALUE;
            HideOnClickContainer = HIDE_ON_CLICK_CONTAINER_DEFAULT_VALUE;
            HideOnClickOverlay = HIDE_ON_CLICK_OVERLAY_DEFAULT_VALUE;
            UpdateHideProgressorOnShow = UPDATE_HIDE_PROGRESSOR_ON_SHOW_DEFAULT_VALUE;
            UpdateShowProgressorOnHide = UPDATE_SHOW_PROGRESSOR_ON_HIDE_DEFAULT_VALUE;
            UseOverlay = USE_OVERLAY_DEFAULT_VALUE;
        }

        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(UIPopup popup)
        {
            popup.AddToPopupQueue = AddToPopupQueue;
            popup.AutoHideAfterShow = AutoHideAfterShow;
            popup.AutoHideAfterShowDelay = AutoHideAfterShowDelay;
            popup.AutoSelectButtonAfterShow = AutoSelectButtonAfterShow;
            popup.BlockBackButton = BlockBackButton;
            popup.CanvasName = CanvasName;
            popup.CustomCanvasName = CustomCanvasName;
            popup.DestroyAfterHide = DestroyAfterHide;
            popup.DisplayTarget = DisplayTarget;
            popup.HideOnAnyButton = HideOnAnyButton;
            popup.HideOnBackButton = HideOnBackButton;
            popup.HideOnClickAnywhere = HideOnClickAnywhere;
            popup.HideOnClickContainer = HideOnClickContainer;
            popup.HideOnClickOverlay = HideOnClickOverlay;
            popup.UpdateHideProgressorOnShow = UpdateHideProgressorOnShow;
            popup.UpdateShowProgressorOnHide = UpdateShowProgressorOnHide;
            popup.UseOverlay = UseOverlay;

            if (popup.Container == null) popup.Container = new UIContainer();
            if (popup.Overlay == null) popup.Overlay = new UIContainer();
            if (popup.Data == null) popup.Data = new UIPopupContentReferences();

            popup.ShowBehavior = new UIPopupBehavior(AnimationType.Show);
            popup.HideBehavior = new UIPopupBehavior(AnimationType.Hide);

            popup.SelectedButton = null;
            popup.Visibility = VISIBILITY_DEFAULT_VALUE;
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}