// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Events;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Themes
{
    /// <inheritdoc />
    /// <summary>
    /// Used by the Theme Manager to trigger an UnityEvent, with the Sprite reference, when the active variant changes
    /// </summary>
    [AddComponentMenu(MenuUtils.SpriteTargetUnityEvent_AddComponentMenu_MenuName, MenuUtils.SpriteTargetUnityEvent_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.SPRITE_TARGET_UNITY_EVENT)]
    public class SpriteTargetUnityEvent : ThemeTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.SpriteTargetUnityEvent_MenuItem_ItemName, false, MenuUtils.SpriteTargetUnityEvent_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<SpriteTargetUnityEvent>(MenuUtils.SpriteTargetUnityEvent_GameObject_Name, false, true); }
#endif

        #endregion
        
        #region Public Variables

        /// <summary> Target Event </summary>
        public SpriteEvent Event;

        #endregion
        
        #region Public Methods

        /// <summary> Method used by the ThemeManager when the active variant or selected theme have changed </summary>
        /// <param name="theme"> Target theme </param>
        public override void UpdateTarget(ThemeData theme)
        {
            if (Event == null) return;
            if (theme == null) return;
            base.UpdateTarget(theme);
            if (ThemeId == Guid.Empty) return;
            if (PropertyId == Guid.Empty) return;
            if (theme.ActiveVariant == null) return;
            Event.Invoke(theme.ActiveVariant.GetSprite(PropertyId));
        }

        #endregion

        #region Private Methods

        private void Reset()
        {
            ThemeId = Guid.Empty;
            VariantId = Guid.Empty;
            PropertyId = Guid.Empty;

            UpdateReference();
        }

        private void UpdateReference()
        {
            if (Event == null)
                Event = new SpriteEvent();
        }

        #endregion
    }
}