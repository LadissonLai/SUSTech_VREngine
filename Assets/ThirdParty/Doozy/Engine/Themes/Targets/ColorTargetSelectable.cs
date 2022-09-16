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
	/// Used by the Theme Manager to update the color values of a Selectable component
	/// </summary>
	[AddComponentMenu(MenuUtils.ColorTargetSelectable_AddComponentMenu_MenuName, MenuUtils.ColorTargetSelectable_AddComponentMenu_Order)]
	[DefaultExecutionOrder(DoozyExecutionOrder.COLOR_TARGET_SELECTABLE)]
	public class ColorTargetSelectable : ThemeTarget
	{
		#region UNITY_EDITOR

#if UNITY_EDITOR
		[MenuItem(MenuUtils.ColorTargetSelectable_MenuItem_ItemName, false, MenuUtils.ColorTargetImage_MenuItem_Priority)]
		private static void CreateComponent(MenuCommand menuCommand)
		{
			DoozyUtils.AddToScene<ColorTargetSelectable>(MenuUtils.ColorTargetSelectable_GameObject_Name, false, true);
		}
#endif

		#endregion

		#region Public Variables

		/// <summary> System.Guid unique id for the variant property </summary>
		public Guid NormalColorPropertyId = Guid.Empty;

		/// <summary> System.Guid unique id for the variant property </summary>
		public Guid HighlightedColorPropertyId = Guid.Empty;

		/// <summary> System.Guid unique id for the variant property </summary>
		public Guid PressedColorPropertyId = Guid.Empty;

#if UNITY_2019_3_OR_NEWER
		/// <summary> System.Guid unique id for the variant property </summary>
		public Guid SelectedColorPropertyId = Guid.Empty;
#endif

		/// <summary> System.Guid unique id for the variant property </summary>
		public Guid DisabledColorPropertyId = Guid.Empty;

		/// <summary> Target Selectable component </summary>
		public Selectable Selectable;

		#endregion

		#region Private Variables

		/// <summary> Vector array describing the System.Guid for the variant property </summary>
		[SerializeField] private byte[] NormalPropertyIdSerializedGuid;

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
			if (NormalColorPropertyId == Guid.Empty) return;
			if (HighlightedColorPropertyId == Guid.Empty) return;
			if (PressedColorPropertyId == Guid.Empty) return;
#if UNITY_2019_3_OR_NEWER
			if (SelectedColorPropertyId == Guid.Empty) return;
#endif
			if (DisabledColorPropertyId == Guid.Empty) return;
			UpdateTarget(ThemeManager.Database.GetThemeData(ThemeId));
		}

		public override void OnBeforeSerialize()
		{
			base.OnBeforeSerialize();
			NormalPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(NormalColorPropertyId);
			HighlightedPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(HighlightedColorPropertyId);
			PressedPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(PressedColorPropertyId);
#if UNITY_2019_3_OR_NEWER
			SelectedPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(SelectedColorPropertyId);
#endif
			DisabledPropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(DisabledColorPropertyId);
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			NormalColorPropertyId = GuidUtils.SerializedGuidToGuid(NormalPropertyIdSerializedGuid);
			HighlightedColorPropertyId = GuidUtils.SerializedGuidToGuid(HighlightedPropertyIdSerializedGuid);
			PressedColorPropertyId = GuidUtils.SerializedGuidToGuid(PressedPropertyIdSerializedGuid);
#if UNITY_2019_3_OR_NEWER
			SelectedColorPropertyId = GuidUtils.SerializedGuidToGuid(SelectedPropertyIdSerializedGuid);
#endif
			DisabledColorPropertyId = GuidUtils.SerializedGuidToGuid(DisabledPropertyIdSerializedGuid);
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

			ColorBlock colors = Selectable.colors;
			Selectable.colors = new ColorBlock
			{
				normalColor = NormalColorPropertyId != Guid.Empty
					? theme.ActiveVariant.GetColor(NormalColorPropertyId)
					: colors.normalColor,

				highlightedColor = HighlightedColorPropertyId != Guid.Empty
					? theme.ActiveVariant.GetColor(HighlightedColorPropertyId)
					: colors.highlightedColor,

				pressedColor = PressedColorPropertyId != Guid.Empty
					? theme.ActiveVariant.GetColor(PressedColorPropertyId)
					: colors.pressedColor,

#if UNITY_2019_3_OR_NEWER
				selectedColor = SelectedColorPropertyId != Guid.Empty
					? theme.ActiveVariant.GetColor(SelectedColorPropertyId)
					: colors.selectedColor,
#endif
				
				disabledColor = DisabledColorPropertyId != Guid.Empty
					? theme.ActiveVariant.GetColor(DisabledColorPropertyId)
					: colors.disabledColor,

				colorMultiplier = colors.colorMultiplier,
				fadeDuration = colors.fadeDuration
			};
		}

		#endregion

		#region Private Methods

		private void Reset()
		{
			ThemeId = Guid.Empty;
			VariantId = Guid.Empty;
			PropertyId = Guid.Empty;

			NormalColorPropertyId = Guid.Empty;
			HighlightedColorPropertyId = Guid.Empty;
			PressedColorPropertyId = Guid.Empty;
#if UNITY_2019_3_OR_NEWER
			SelectedColorPropertyId = Guid.Empty;
#endif
			DisabledColorPropertyId = Guid.Empty;

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