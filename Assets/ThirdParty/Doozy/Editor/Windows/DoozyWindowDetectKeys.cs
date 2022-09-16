// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private void DetectKeys(Event current)
        {
            if (!HasFocus) return;

            //Close Window
            if (DGUI.KeyMapper.DetectKeyUpCombo(current, EventModifiers.Alt, KeyCode.X)) Close();

            //Collapse or Expand the Left Toolbar Menu
            if (DGUI.KeyMapper.DetectKeyUpCombo(current, EventModifiers.Alt, KeyCode.BackQuote)) ToggleToolbarMenuExpandOrCollapse();

            //Toolbar Quick Tab Selector
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha1)) SetView(View.General);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha2)) SetView(View.Buttons);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha3)) SetView(View.Views);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha4)) SetView(View.Canvases);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha5)) SetView(View.Drawers);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha6)) SetView(View.Nody);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha7)) SetView(View.Soundy);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha8)) SetView(View.Touchy);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha9)) SetView(View.Animations);
//            if (DGUI.KeyMapper.DetectKeyDownCombo(current, EventModifiers.Alt, KeyCode.Alpha0)) SetView(View.Templates);
        }
    }
}