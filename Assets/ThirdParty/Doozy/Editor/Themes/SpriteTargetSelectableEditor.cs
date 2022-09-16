// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

namespace Doozy.Editor.Themes
{
	[CustomEditor(typeof(SpriteTargetSelectable))]
	[CanEditMultipleObjects]
	public class SpriteTargetSelectableEditor : BaseEditor
	{
		protected override ColorName ComponentColorName
		{
			get { return DGUI.Colors.ThemesColorName; }
		}

		private SpriteTargetSelectable m_target;

		private SpriteTargetSelectable Target
		{
			get
			{
				if (m_target != null) return m_target;
				m_target = (SpriteTargetSelectable) target;
				return m_target;
			}
		}

		private static ThemesDatabase Database
		{
			get { return ThemesSettings.Database; }
		}

		private string[] ThemesNames;
		private string[] VariantsNames;

		private SerializedProperty m_selectable;

		private AnimBool
			m_highlightedAnimBool,
			m_pressedAnimBool,
#if UNITY_2019_3_OR_NEWER
			m_selectedAnimBool,
#endif
			m_disabledAnimBool;

		private ThemeData m_theme;
		private LabelId m_Property;
		private int m_selectedThemeIndex;
		private int m_selectedVariantIndex;
		private int m_selectedPropertyIndex;
		private int m_selectedHighlightedPropertyIndex;
		private int m_selectedPressedPropertyIndex;
#if UNITY_2019_3_OR_NEWER
		private int m_selectedSelectedPropertyIndex;
#endif
		private int m_selectedDisabledPropertyIndex;

		private bool HasReference
		{
			get { return m_selectable.objectReferenceValue != null; }
		}

		protected override void LoadSerializedProperty()
		{
			base.LoadSerializedProperty();

			m_selectable = GetProperty(PropertyName.Selectable);
		}

		protected override void InitAnimBool()
		{
			base.InitAnimBool();

			m_highlightedAnimBool = new AnimBool(false, Repaint);
			m_pressedAnimBool = new AnimBool(false, Repaint);
#if UNITY_2019_3_OR_NEWER
			m_selectedAnimBool = new AnimBool(false, Repaint);
#endif
			m_disabledAnimBool = new AnimBool(false, Repaint);
		}

		public override void OnInspectorGUI()
		{
			UpdateIds();
			UpdateLists();
			base.OnInspectorGUI();
			serializedObject.Update();
			DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSpriteTargetSelectable), MenuUtils.SpriteTargetSelectable_Manual, MenuUtils.SpriteTargetSelectable_YouTube);
			GUILayout.Space(DGUI.Properties.Space(2));
			DGUI.Property.Draw(m_selectable, UILabels.Selectable, HasReference ? ComponentColorName : ColorName.Red);
			GUILayout.Space(DGUI.Properties.Space(4));
			int themeIndex = Database.GetThemeIndex(Target.ThemeId);
			if (themeIndex != -1)
			{
				ThemeTargetEditorUtils.DrawThemePopup(Database, m_theme, ThemesNames, themeIndex, ComponentColorName,
					serializedObject, targets, Target, InitialGUIColor, UpdateIds, UpdateLists);
				GUILayout.Space(DGUI.Properties.Space());
				ThemeTargetEditorUtils.DrawActiveVariant(m_theme, ComponentColorName);
			}

			DrawPropertyIndex(Target.HighlightedSpritePropertyId, ThemeTargetEditorUtils.SelectionState.Highlighted, m_highlightedAnimBool);
			DrawPropertyIndex(Target.PressedSpritePropertyId, ThemeTargetEditorUtils.SelectionState.Pressed, m_pressedAnimBool);
#if UNITY_2019_3_OR_NEWER
			DrawPropertyIndex(Target.SelectedSpritePropertyId, ThemeTargetEditorUtils.SelectionState.Selected, m_selectedAnimBool);
#endif
			DrawPropertyIndex(Target.DisabledSpritePropertyId, ThemeTargetEditorUtils.SelectionState.Disabled, m_disabledAnimBool);

			GUILayout.Space(DGUI.Properties.Space(4));
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawPropertyIndex(Guid id, ThemeTargetEditorUtils.SelectionState selectionState, AnimBool animBool)
		{
			GUILayout.Space(DGUI.Properties.Space(2));
			int propertyIndex = m_theme.GetSpritePropertyIndex(id);
			bool propertyNotFound = id == Guid.Empty || propertyIndex == -1;

			DGUI.Bar.Draw(selectionState + (propertyNotFound
				              ? ""
				              : ": " + m_theme.SpriteLabels[propertyIndex].Label),
				Size.L, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, animBool);

			if (DGUI.FoldOut.Begin(animBool))
			{
				GUILayout.Space(DGUI.Properties.Space(2));
				if (propertyNotFound) ThemeTargetEditorUtils.DrawLabelNoPropertyFound();
				else DrawSpriteProperties(m_theme, propertyIndex, selectionState);
			}

			DGUI.FoldOut.End(animBool);
		}

		private void DrawSpriteProperties(ThemeData themeData, int propertyIndex, ThemeTargetEditorUtils.SelectionState selectionState)
		{
			GUIStyle buttonStyleDisabled = Styles.GetStyle(Styles.StyleName.CheckBoxDisabled);
			GUIStyle buttonStyleEnabled = Styles.GetStyle(Styles.StyleName.CheckBoxEnabled);
			for (var i = 0; i < themeData.SpriteLabels.Count; i++)
			{
				LabelId spriteProperty = themeData.SpriteLabels[i];
				int index = i;
				bool selected = i == propertyIndex;
				GUILayout.BeginHorizontal();
				{
					GUI.color = DGUI.Colors.PropertyColor(ComponentColorName);
					if (GUILayout.Button(GUIContent.none, selected ? buttonStyleEnabled : buttonStyleDisabled))
					{
						if (serializedObject.isEditingMultipleObjects)
						{
							DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
							foreach (Object o in targets)
							{
								var themeTarget = (SpriteTargetSelectable) o;
								if (themeTarget == null) continue;
								switch (selectionState)
								{
									case ThemeTargetEditorUtils.SelectionState.Highlighted:
										themeTarget.HighlightedSpritePropertyId = themeData.SpriteLabels[index].Id;
										break;
									case ThemeTargetEditorUtils.SelectionState.Pressed:
										themeTarget.PressedSpritePropertyId = themeData.SpriteLabels[index].Id;
										break;
#if UNITY_2019_3_OR_NEWER
									case ThemeTargetEditorUtils.SelectionState.Selected:
										themeTarget.SelectedSpritePropertyId = themeData.SpriteLabels[index].Id;
										break;
#endif
									case ThemeTargetEditorUtils.SelectionState.Disabled:
										themeTarget.DisabledSpritePropertyId = themeData.SpriteLabels[index].Id;
										break;
								}

								themeTarget.UpdateTarget(themeData);
							}
						}
						else
						{
							DoozyUtils.UndoRecordObject(target, UILabels.UpdateValue);
							switch (selectionState)
							{
								case ThemeTargetEditorUtils.SelectionState.Highlighted:
									Target.HighlightedSpritePropertyId = themeData.SpriteLabels[index].Id;
									break;
								case ThemeTargetEditorUtils.SelectionState.Pressed:
									Target.PressedSpritePropertyId = themeData.SpriteLabels[index].Id;
									break;
#if UNITY_2019_3_OR_NEWER
								case ThemeTargetEditorUtils.SelectionState.Selected:
									Target.SelectedSpritePropertyId = themeData.SpriteLabels[index].Id;
									break;
#endif
								case ThemeTargetEditorUtils.SelectionState.Disabled:
									Target.DisabledSpritePropertyId = themeData.SpriteLabels[index].Id;
									break;
							}

							Target.UpdateTarget(themeData);
						}
					}

					GUI.color = InitialGUIColor;
					GUILayout.Space(DGUI.Properties.Space(2));
					GUI.enabled = selected;
					DGUI.Label.Draw(spriteProperty.Label, selected ? Size.L : Size.M);
					GUI.enabled = true;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(DGUI.Properties.Space());
			}
		}

		private void UpdateIds()
		{
			if (!Database.Contains(Target.ThemeId))
				Target.ThemeId = Database.Themes[0].Id;
			m_theme = Database.GetThemeData(Target.ThemeId);

			//HighlightedSpritePropertyId
			if (!m_theme.ContainsSpriteProperty(Target.HighlightedSpritePropertyId))
				Target.HighlightedSpritePropertyId = m_theme.SpriteLabels.Count > 0
					? m_theme.SpriteLabels[0].Id
					: Guid.Empty;

			//PressedSpritePropertyId
			if (!m_theme.ContainsSpriteProperty(Target.PressedSpritePropertyId))
				Target.PressedSpritePropertyId = m_theme.SpriteLabels.Count > 0
					? m_theme.SpriteLabels[0].Id
					: Guid.Empty;

#if UNITY_2019_3_OR_NEWER
			//SelectedSpritePropertyId
			if (!m_theme.ContainsSpriteProperty(Target.SelectedSpritePropertyId))
				Target.SelectedSpritePropertyId = m_theme.SpriteLabels.Count > 0
					? m_theme.SpriteLabels[0].Id
					: Guid.Empty;
#endif
			
			//DisabledSpritePropertyId
			if (!m_theme.ContainsSpriteProperty(Target.DisabledSpritePropertyId))
				Target.DisabledSpritePropertyId = m_theme.SpriteLabels.Count > 0
					? m_theme.SpriteLabels[0].Id
					: Guid.Empty;
		}


		private void UpdateLists()
		{
			ThemesNames = ThemesDatabase.GetThemesNames(Database);
		}
	}
}