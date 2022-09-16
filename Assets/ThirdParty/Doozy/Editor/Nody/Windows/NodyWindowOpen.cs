// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        [MenuItem(MenuUtils.NodyWindow_MenuItem_ItemName, false, MenuUtils.NodyWindow_MenuItem_Order)]
        public static void Open() { GetWindow<NodyWindow>(); }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var graph = EditorUtility.InstanceIDToObject(instanceId) as Graph; //cast the asset
            if (graph == null) return false;                                   //if the clicked asset is not a Graph -> return
            Instance.LoadGraph(graph);                                         //load the Graph
            return true;
        }
    }
}