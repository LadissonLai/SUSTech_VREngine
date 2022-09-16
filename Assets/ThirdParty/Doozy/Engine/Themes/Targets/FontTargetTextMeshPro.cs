// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System;
using Doozy.Engine.Utils;
using UnityEngine.UI;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if dUI_TextMeshPro
using TMPro;
#endif


namespace Doozy.Engine.Themes
{
    /// <inheritdoc />
    /// <summary>
    /// Used by the Theme Manager to update the font value of a TextMeshPro component
    /// </summary>
    [AddComponentMenu(MenuUtils.FontTargetTextMeshPro_AddComponentMenu_MenuName, MenuUtils.FontTargetTextMeshPro_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.FONT_TARGET_TEXTMESHPRO)]
    public class FontTargetTextMeshPro : ThemeTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.FontTargetTextMeshPro_MenuItem_ItemName, false, MenuUtils.FontTargetTextMeshPro_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<FontTargetTextMeshPro>(MenuUtils.FontTargetTextMeshPro_GameObject_Name, false, true); }
#endif

        #endregion
        
        #region Public Variables

#if dUI_TextMeshPro
        /// <summary> Target TextMeshPro component</summary>
        public TMP_Text TextMeshPro;
#endif

        #endregion

        #region Public Methods

        /// <summary> Method used by the ThemeManager when the active variant or selected theme have changed </summary>
        /// <param name="theme"> Target theme </param>
        public override void UpdateTarget(ThemeData theme)
        {
#if dUI_TextMeshPro
            if (TextMeshPro == null) return;
            if (theme == null) return;
            base.UpdateTarget(theme);
            if (ThemeId == Guid.Empty) return;
            if (PropertyId == Guid.Empty) return;
            if (theme.ActiveVariant == null) return;
            if (!theme.ActiveVariant.ContainsFontAsset(PropertyId)) return;
            TextMeshPro.font = theme.ActiveVariant.GetFontAsset(PropertyId);
#endif
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
#if dUI_TextMeshPro
            if (TextMeshPro == null)
                TextMeshPro = GetComponent<TMP_Text>();
#endif
        }

        #endregion
    }
}