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
    /// Used by the Theme Manager to update the Sprite references of a Selectable component
    /// </summary>
    [AddComponentMenu(MenuUtils.SpriteTargetSelectable_AddComponentMenu_MenuName, MenuUtils.SpriteTargetSelectable_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.SPRITE_TARGET_SELECTABLE)]
    public class SpriteTargetSelectable : ThemeTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.SpriteTargetSelectable_MenuItem_ItemName, false, MenuUtils.SpriteTargetSelectable_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            DoozyUtils.AddToScene<SpriteTargetSelectable>(MenuUtils.SpriteTargetSelectable_GameObject_Name, false, true);
        }
#endif

        #endregion

        #region Public Variables

        /// <summary> System.Guid unique id for the variant property </summary>
        public Guid HighlightedSpritePropertyId = Guid.Empty;

        /// <summary> System.Guid unique id for the variant property </summary>
        public Guid PressedSpritePropertyId = Guid.Empty;

#if UNITY_2019_3_OR_NEWER
        /// <summary> System.Guid unique id for the variant property </summary>
        public Guid SelectedSpritePropertyId = Guid.Empty;
#endif
        
        /// <summary> System.Guid unique id for the variant property </summary>
        public Guid DisabledSpritePropertyId = Guid.Empty;

        /// <summary> Target Selectable component </summary>
        public Selectable Selectable;

        #endregion

        #region Private Variables

        /// <summary> Vector array describing the System.Guid for the variant property </summary>
        [SerializeField] private byte[] HighlightedPropertyIdSerializedGuid;

        /// <summary> Vector array describing the System.Guid for the variant property </summary>
        [SerializeField] private byte[] PressedPropertyIdSerializedGuid;

#if UNITY_2019_3_OR_NEWER
        /// <summary> Vector array describing the System.Guid for the variant property </summary>
        [SerializeField] private byte[] SelectedPropertyIdSerializedGuid;
#endif
        
        /// <summary> Vector array describing the System.Guid for the variant property </summary>
        [SerializeField] private byte[] DisabledPropertyIdSerializedGuid;

        #endregion

        #region Unity Methods

        protected override void OnValidate()
        {
            if (ThemeId == Guid.Empty) return;
            if (HighlightedSpritePropertyId == Guid.Empty) return;
            if (PressedSpritePropertyId == Guid.Empty) return;
#if UNITY_2019_3_OR_NEWER
            if (SelectedSpritePropertyId == Guid.Empty) return;
#endif
            if (DisabledSpritePropertyId == Guid.Empty) return;
            UpdateTarget(ThemeManager.Database.GetThemeData(ThemeId));
        }
        
        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            HighlightedPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(HighlightedSpritePropertyId);
            PressedPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(PressedSpritePropertyId);
#if UNITY_2019_3_OR_NEWER
            SelectedPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(SelectedSpritePropertyId);
#endif
            DisabledPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(DisabledSpritePropertyId);
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            HighlightedSpritePropertyId = GuidUtils.SerializedGuidToGuid(HighlightedPropertyIdSerializedGuid);
            PressedSpritePropertyId = GuidUtils.SerializedGuidToGuid(PressedPropertyIdSerializedGuid);
#if UNITY_2019_3_OR_NEWER
            SelectedSpritePropertyId = GuidUtils.SerializedGuidToGuid(SelectedPropertyIdSerializedGuid);
#endif
            DisabledSpritePropertyId = GuidUtils.SerializedGuidToGuid(DisabledPropertyIdSerializedGuid);
        }

        #endregion
        
        #region Public Methods

        /// <summary> Method used by the ThemeManager when the active variant or selected theme have changed </summary>
        /// <param name="theme"> Target theme </param>
        public override void UpdateTarget(ThemeData theme)
        {
            if (Selectable == null) return;
            if (theme == null) return;
            base.UpdateTarget(theme);
            if (ThemeId == Guid.Empty) return;
            if (theme.ActiveVariant == null) return;

            SpriteState spriteState = Selectable.spriteState;

            Selectable.spriteState = new SpriteState
            {
                highlightedSprite = HighlightedSpritePropertyId != Guid.Empty
                    ? theme.ActiveVariant.GetSprite(HighlightedSpritePropertyId)
                    : spriteState.highlightedSprite,

                pressedSprite = PressedSpritePropertyId != Guid.Empty
                    ? theme.ActiveVariant.GetSprite(PressedSpritePropertyId)
                    : spriteState.pressedSprite,

#if UNITY_2019_3_OR_NEWER
                selectedSprite = SelectedSpritePropertyId != Guid.Empty
                    ? theme.ActiveVariant.GetSprite(SelectedSpritePropertyId)
                    : spriteState.selectedSprite,
#endif
                
                disabledSprite = DisabledSpritePropertyId != Guid.Empty
                    ? theme.ActiveVariant.GetSprite(DisabledSpritePropertyId)
                    : spriteState.disabledSprite
            };
        }

        #endregion

        #region Private Methods

        private void Reset()
        {
            ThemeId = Guid.Empty;
            VariantId = Guid.Empty;
            PropertyId = Guid.Empty;

            HighlightedSpritePropertyId = Guid.Empty;
            PressedSpritePropertyId = Guid.Empty;
#if UNITY_2019_3_OR_NEWER
            SelectedSpritePropertyId = Guid.Empty;
#endif
            DisabledSpritePropertyId = Guid.Empty;

            UpdateReference();
        }

        private void UpdateReference()
        {
            if (Selectable == null)
                Selectable = GetComponent<Selectable>();
        }

        #endregion
    }
}