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
using SelectionState = Doozy.Editor.Themes.ThemeTargetEditorUtils.SelectionState;

namespace Doozy.Editor.Themes
{
	[CustomEditor((typeof(ColorTargetSelectable)))]
	[CanEditMultipleObjects]
	public class ColorTargetSelectableEditor : BaseEditor
	{
		protected override ColorName ComponentColorName
		{
			get { return DGUI.Colors.ThemesColorName; }
		}

		private ColorTargetSelectable m_target;

		private ColorTargetSelectable Target
		{
			get
			{
				if (m_target != null) return m_target;
				m_target = (ColorTargetSelectable) target;
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
			m_normalAnimBool,
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
		private int m_selectedNormalPropertyIndex;
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

			m_normalAnimBool = new AnimBool(false, Repaint);
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
			DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderColorTargetSelectable),
				MenuUtils.ColorTargetSelectable_Manual, MenuUtils.ColorTargetSelectable_YouTube);
			GUILayout.Space(DGUI.Properties.Space(2));
			DGUI.Property.Draw(m_selectable, UILabels.Selectable, HasReference ? ComponentColorName : ColorName.Red);
			GUILayout.Space(DGUI.Properties.Space(4));
			var themeIndex = Database.GetThemeIndex(Target.ThemeId);
			if (themeIndex != -1)
			{
				ThemeTargetEditorUtils.DrawThemePopup(Database, m_theme, ThemesNames, themeIndex, ComponentColorName,
					serializedObject, targets, Target, InitialGUIColor, UpdateIds, UpdateLists);
				GUILayout.Space(DGUI.Properties.Space());
				ThemeTargetEditorUtils.DrawActiveVariant(m_theme, ComponentColorName);
			}

			DrawPropertyIndex(Target.NormalColorPropertyId, SelectionState.Normal, m_normalAnimBool);
			DrawPropertyIndex(Target.HighlightedColorPropertyId, SelectionState.Highlighted, m_highlightedAnimBool);
			DrawPropertyIndex(Target.PressedColorPropertyId, SelectionState.Pressed, m_pressedAnimBool);
#if UNITY_2019_3_OR_NEWER
			DrawPropertyIndex(Target.SelectedColorPropertyId, SelectionState.Selected, m_selectedAnimBool);
#endif
			DrawPropertyIndex(Target.DisabledColorPropertyId, SelectionState.Disabled, m_disabledAnimBool);

			GUILayout.Space(DGUI.Properties.Space(4));
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawPropertyIndex(Guid id, SelectionState selectionState, AnimBool animBool)
		{
			GUILayout.Space(DGUI.Properties.Space(2));
			var propertyIndex = m_theme.GetColorPropertyIndex(id);
			var propertyNotFound = id == Guid.Empty || propertyIndex == -1;
			GUILayout.BeginHorizontal();
			{
				if (!propertyNotFound && !animBool.target)
				{
					GUI.color = m_theme.ActiveVariant.Colors[propertyIndex].Color;
					GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.ColorButtonSelected));
					GUI.color = InitialGUIColor;
					GUILayout.Space(2);
				}

				DGUI.Bar.Draw(selectionState.ToString(), Size.L, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, animBool);
			}
			GUILayout.EndHorizontal();

			if (DGUI.FoldOut.Begin(animBool))
			{
				GUILayout.Space(DGUI.Properties.Space(2));
				if (propertyNotFound) ThemeTargetEditorUtils.DrawLabelNoPropertyFound();
				else DrawColorProperties(m_theme, propertyIndex, selectionState);
			}

			DGUI.FoldOut.End(animBool);
		}


		private void DrawColorProperties(ThemeData themeData, int propertyIndex, SelectionState selectionState)
		{
			GUIStyle colorButtonStyle = Styles.GetStyle(Styles.StyleName.ColorButton);
			GUIStyle colorButtonSelectedStyle = Styles.GetStyle(Styles.StyleName.ColorButtonSelected);
			for (int i = 0; i < themeData.ColorLabels.Count; i++)
			{
				LabelId colorProperty = themeData.ColorLabels[i];
				int index = i;
				bool selected = i == propertyIndex;
				GUILayout.BeginHorizontal();
				{
					if (!selected)
						GUILayout.Space((colorButtonSelectedStyle.fixedWidth - colorButtonStyle.fixedWidth) / 2);
					GUI.color = themeData.ActiveVariant.Colors[i].Color;
					{
						if (GUILayout.Button(GUIContent.none, selected ? colorButtonSelectedStyle : colorButtonStyle))
						{
							if (serializedObject.isEditingMultipleObjects)
							{
								DoozyUtils.UndoRecordObjects(targets, UILabels.UpdateValue);
								foreach (Object o in targets)
								{
									var themeTarget = (ColorTargetSelectable) o;
									if (themeTarget == null) continue;
									switch (selectionState)
									{
										case SelectionState.Normal:
											themeTarget.NormalColorPropertyId = themeData.ColorLabels[index].Id;
											break;
										case SelectionState.Highlighted:
											themeTarget.HighlightedColorPropertyId = themeData.ColorLabels[index].Id;
											break;
										case SelectionState.Pressed:
											themeTarget.PressedColorPropertyId = themeData.ColorLabels[index].Id;
											break;
#if UNITY_2019_3_OR_NEWER
										case SelectionState.Selected:
											themeTarget.SelectedColorPropertyId = themeData.ColorLabels[index].Id;
											break;
#endif
										case SelectionState.Disabled:
											themeTarget.DisabledColorPropertyId = themeData.ColorLabels[index].Id;
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
									case SelectionState.Normal:
										Target.NormalColorPropertyId = themeData.ColorLabels[index].Id;
										break;
									case SelectionState.Highlighted:
										Target.HighlightedColorPropertyId = themeData.ColorLabels[index].Id;
										break;
									case SelectionState.Pressed:
										Target.PressedColorPropertyId = themeData.ColorLabels[index].Id;
										break;
#if UNITY_2019_3_OR_NEWER
									case SelectionState.Selected:
										Target.SelectedColorPropertyId = themeData.ColorLabels[index].Id;
										break;
#endif
									case SelectionState.Disabled:
										Target.DisabledColorPropertyId = themeData.ColorLabels[index].Id;
										break;
								}

								Target.UpdateTarget(themeData);
							}
						}
					}
					GUI.color = InitialGUIColor;
					GUILayout.Space(DGUI.Properties.Space(2));
					GUI.enabled = selected;
					DGUI.Label.Draw(colorProperty.Label, selected ? Size.L : Size.M,
						selected ? colorButtonSelectedStyle.fixedHeight : colorButtonStyle.fixedHeight);
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

			//NormalColorPropertyId
			if (!m_theme.ContainsColorProperty(Target.NormalColorPropertyId))
				Target.NormalColorPropertyId = m_theme.ColorLabels.Count > 0
					? m_theme.ColorLabels[0].Id
					: Guid.Empty;

			//HighlightedColorPropertyId
			if (!m_theme.ContainsColorProperty(Target.HighlightedColorPropertyId))
				Target.HighlightedColorPropertyId = m_theme.ColorLabels.Count > 0
					? m_theme.ColorLabels[0].Id
					: Guid.Empty;

			//PressedColorPropertyId
			if (!m_theme.ContainsColorProperty(Target.PressedColorPropertyId))
				Target.PressedColorPropertyId = m_theme.ColorLabels.Count > 0
					? m_theme.ColorLabels[0].Id
					: Guid.Empty;

#if UNITY_2019_3_OR_NEWER
			//SelectedColorPropertyId
			if (!m_theme.ContainsColorProperty(Target.SelectedColorPropertyId))
				Target.SelectedColorPropertyId = m_theme.ColorLabels.Count > 0
					? m_theme.ColorLabels[0].Id
					: Guid.Empty;
#endif
			
			//DisabledColorPropertyId
			if (!m_theme.ContainsColorProperty(Target.DisabledColorPropertyId))
				Target.DisabledColorPropertyId = m_theme.ColorLabels.Count > 0
					? m_theme.ColorLabels[0].Id
					: Guid.Empty;
		}


		private void UpdateLists()
		{
			ThemesNames = ThemesDatabase.GetThemesNames(Database);
		}
	}
}