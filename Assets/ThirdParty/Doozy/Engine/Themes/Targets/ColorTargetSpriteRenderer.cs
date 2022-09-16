// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Themes
{
    /// <inheritdoc />
    /// <summary>
    /// Used by the Theme Manager to update the color value of a SpriteRenderer component
    /// </summary>
    [AddComponentMenu(MenuUtils.ColorTargetSpriteRenderer_AddComponentMenu_MenuName, MenuUtils.ColorTargetSpriteRenderer_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.COLOR_TARGET_SPRITE_RENDERER)]
    public class ColorTargetSpriteRenderer : ThemeTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ColorTargetSpriteRenderer_MenuItem_ItemName, false, MenuUtils.ColorTargetSpriteRenderer_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ColorTargetSpriteRenderer>(MenuUtils.ColorTargetSpriteRenderer_GameObject_Name, false, true); }
#endif

        #endregion
        
        #region Public Variables

        /// <summary> Target SpriteRenderer component </summary>
        public SpriteRenderer SpriteRenderer;

        /// <summary> Determines if the target color preserves its alpha value when the theme variant changes </summary>
        public bool OverrideAlpha;

        /// <summary> Alpha value for the target color when the theme variant changes (when OverrideAlpha is true) </summary>
        public float Alpha;

        #endregion
        
        #region Private Variables

        private float m_previousAlphaValue = -1;
        
        #endregion
        
        #region Unity Methods

        private void Update()
        {
            if(!OverrideAlpha) return;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Alpha == m_previousAlphaValue) return;
            SetAlpha(Alpha);
            m_previousAlphaValue = Alpha;
        }

        #endregion
        
        #region Public Methods

        /// <summary> Method used by the ThemeManager when the active variant or selected theme have changed </summary>
        /// <param name="theme"> Target theme </param>
        public override void UpdateTarget(ThemeData theme)
        {
            if (SpriteRenderer == null) return;
            if (theme == null) return;
            base.UpdateTarget(theme);
            if (ThemeId == Guid.Empty) return;
            if (PropertyId == Guid.Empty) return;
            if (theme.ActiveVariant == null) return;
            SpriteRenderer.color = theme.ActiveVariant.GetColor(PropertyId);
            if (!OverrideAlpha) return;
            SetAlpha(Alpha);
        }

        /// <summary> Sets the Alpha value for the target component </summary>
        /// <param name="value"> Alpha value </param>
        public void SetAlpha(float value)
        {
            if (SpriteRenderer == null) return;
            Alpha = value;
            Color color = SpriteRenderer.color;
            SpriteRenderer.color = new Color()
                          {
                              r = color.r,
                              g = color.g,
                              b = color.b,
                              a = Alpha
                          };;
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
            if (SpriteRenderer == null)
                SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        #endregion
    }
}