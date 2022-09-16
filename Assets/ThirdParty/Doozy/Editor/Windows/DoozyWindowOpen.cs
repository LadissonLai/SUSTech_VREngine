// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine.Events;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        [MenuItem(MenuUtils.DoozyWindow_MenuItem_ItemName, false, MenuUtils.DoozyWindow_MenuItem_Order)]
        public static void Open() { GetWindow<DoozyWindow>(); }

        public static void Open(View view)
        {
            Open();
            Instance.SetView(view);
        }

        public static void Open(View view, UnityAction callback)
        {
            Open(view);
            if (callback != null) callback.Invoke();
        }

        [MenuItem(MenuUtils.Refresh_MenuItem_ItemName, false, MenuUtils.Refresh_MenuItem_Order)]
        public static void Refresh()
        {
            DoozyAssetsProcessor.Run();
        }
    }
}