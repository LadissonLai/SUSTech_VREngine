// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Themes
{
    /// <inheritdoc />
    /// <summary>
    /// Used by the Theme Manager to update the font value of a Text component
    /// </summary>
    [AddComponentMenu(MenuUtils.FontTargetText_AddComponentMenu_MenuName, MenuUtils.FontTargetText_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.FONT_TARGET_TEXT)]
    public class FontTargetText : ThemeTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.FontTargetText_MenuItem_ItemName, false, MenuUtils.FontTargetText_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<FontTargetText>(MenuUtils.FontTargetText_GameObject_Name, false, true); }
#endif

        #endregion
        
        #region Public Variables

        /// <summary> Target Text component </summary>
        public Text Text;

        #endregion

        #region Public Methods

        /// <summary> Method used by the ThemeManager when the active variant or selected theme have changed </summary>
        /// <param name="theme"> Target theme </param>
        public override void UpdateTarget(ThemeData theme)
        {
            if (Text == null) return;
            if (theme == null) return;
            base.UpdateTarget(theme);
            if (ThemeId == Guid.Empty) return;
            if (PropertyId == Guid.Empty) return;
            if (theme.ActiveVariant == null) return;
            if (!theme.ActiveVariant.ContainsFont(PropertyId)) return;
            Text.font = theme.ActiveVariant.GetFont(PropertyId);
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
            if (Text == null)
                Text = GetComponent<Text>();
        }

        #endregion
    }
}