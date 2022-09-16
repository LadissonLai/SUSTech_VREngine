// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private static void DrawDefaultViewHorizontalSpacing(float multiplier = 1) { GUILayout.Space(DGUI.Properties.Space() * multiplier); }

        private static bool WindowTabButton(GUIStyle icon, string label, ColorName colorName, bool active)
        {
            return DGUI.Button.Dynamic.DrawIconButton(icon, label, WINDOW_TAB_BUTTON_TEXT_SIZE, WINDOW_TAB_BUTTON_TEXT_ALIGN,
                                                      DGUI.Colors.GetBackgroundColorName(active, colorName),
                                                      DGUI.Colors.GetTextColorName(active, colorName),
                                                      WindowTabButtonHeight, false);
        }

        private static bool TopButton(GUIStyle icon, string label, ColorName colorName, bool active, float height)
        {
            return DGUI.Button.Dynamic.DrawIconButton(icon, label, TOP_BUTTON_TEXT_SIZE, TOP_BUTTON_TEXT_ALIGN,
                                                      DGUI.Colors.GetBackgroundColorName(active, colorName),
                                                      DGUI.Colors.GetTextColorName(active, colorName),
                                                      height, false);
        }

        private static bool TopButton(GUIStyle icon, string label, ColorName colorName, bool active) { return TopButton(icon, label, colorName, active, TopButtonHeight); }

        private static bool ButtonNew(bool active, string label)
        {
            return TopButton(DGUI.Icon.New, label, ColorName.Green, active); // || !active && DGUI.KeyMapper.DetectKeyUp(Event.current, KeyCode.N) && !EditorGUIUtility.editingTextField;
        }

        private static bool ButtonEditMode(bool active) { return TopButton(DGUI.Icon.Edit, UILabels.EditMode, ColorName.Green, active); }
        private static bool ButtonRefreshDatabase() { return TopButton(DGUI.Icon.Refresh, UILabels.Refresh, ColorName.Green, false); }
        private static bool ButtonRemoveDuplicates() { return TopButton(DGUI.Icon.Duplicate, UILabels.RemoveDuplicates, ColorName.Green, false); }
        private static bool ButtonResetDatabase() { return TopButton(DGUI.Icon.Reset, UILabels.Reset, ColorName.Red, true); }
        private static bool ButtonSaveDatabase() { return TopButton(DGUI.Icon.Save, UILabels.Save, ColorName.Green, false); }
        private static bool ButtonSearchFor(string label) { return TopButton(DGUI.Icon.Search, label, ColorName.Green, false); }
        private static bool ButtonSortDatabase() { return TopButton(DGUI.Icon.SortAlphaDown, UILabels.Sort, ColorName.Green, false); }
        private static bool ButtonAutoSave(bool active) { return TopButton(DGUI.Icon.Save, active ? UILabels.AutoSaveEnabled : UILabels.AutoSaveDisabled, ColorName.Green, active); }

        private static bool ButtonClearSearch(AnimBool searchEnabled)
        {
            var clearSearchContent = new GUIContent(UILabels.ClearSearch);
            Vector2 clearSearchContentSize = DGUI.Button.LabelStyle(Size.S).CalcSize(clearSearchContent);
            return DGUI.Button.Draw(clearSearchContent,
                                    Size.S, TextAlign.Center, ColorName.Red,
                                    true,
                                    SearchRowHeight + DGUI.Properties.Space(2),
                                    clearSearchContentSize.x * searchEnabled.faded)
                   ||
                   Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Escape || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
        }
    }
}