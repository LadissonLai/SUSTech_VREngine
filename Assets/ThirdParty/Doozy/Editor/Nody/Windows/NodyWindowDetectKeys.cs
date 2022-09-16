// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private void DetectKeys(Event current)
        {
            //Save Graph
            if (WindowSettings.SaveCurrentGraphWithControlS && DGUI.KeyMapper.DetectKeyUpCombo(current, EventModifiers.Control, KeyCode.S))
            {
                SaveGraph();
                current.Use();
            }

            if (!HasFocus) return;

            if (current.type == EventType.KeyUp && current.keyCode == KeyCode.Escape) DGUI.Properties.ResetKeyboardFocus();      //reset keyboard focus on key Escape
            if (current.type == EventType.KeyUp && current.keyCode == KeyCode.Return) DGUI.Properties.ResetKeyboardFocus();      //reset keyboard focus on key Return
            if (current.type == EventType.KeyUp && current.keyCode == KeyCode.KeypadEnter) DGUI.Properties.ResetKeyboardFocus(); //reset keyboard focus on key KeypadEnter
        }
    }
}