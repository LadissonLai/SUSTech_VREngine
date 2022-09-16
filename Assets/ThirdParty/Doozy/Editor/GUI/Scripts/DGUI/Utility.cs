// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

// ReSharper disable ConvertToConstant.Global
// ReSharper disable InconsistentNaming

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Utility
        {
            public static bool IsProSkin { get { return EditorGUIUtility.isProSkin; } }
            public static void PingObjectInProjectView(Object target) { EditorGUIUtility.PingObject(target); }
            public static void SelectObjectInInspector(Object target) { Selection.activeObject = target; }
            public static void SelectObjectsInInspector(Object[] targets) { Selection.objects = targets; }
        }
    }
}