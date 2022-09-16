// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        /// <summary> Updates the cursor to reflect current context state </summary>
        private void UpdateCursorIcon()
        {
            //if not panning or Space key down -> return
            if (m_mode != GraphMode.Pan && !m_spaceKeyDown) return;

            //calculate the custom cursor rect
            var cursorRect = new Rect(CurrentMousePosition.x, CurrentMousePosition.y, 32f, 32f);

            //show a custom cursor for PANNING
            EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.Pan);
        }
    }
}