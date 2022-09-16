// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Utils
{
    public static class DefineSymbolsUtils
    {
        private static readonly StringBuilder StringBuilderContainer = new StringBuilder();

        /// <summary> Add the passed global define if it's not already present </summary>
        public static void AddGlobalDefine(string id)
        {
            bool flag = false;
            int num = 0;
            foreach (BuildTargetGroup buildTargetGroup in (BuildTargetGroup[]) Enum.GetValues(typeof(BuildTargetGroup)))
                if (IsValidBuildTargetGroup(buildTargetGroup))
                {
                    string defineSymbolsForGroup = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                    if (Array.IndexOf(defineSymbolsForGroup.Split(';'), id) != -1) continue;
                    flag = true;
                    ++num;
                    string defines = defineSymbolsForGroup + (defineSymbolsForGroup.Length > 0 ? ";" + id : id);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                }

            if (!flag) return;
            DDebug.Log(string.Format("Added global define \"{0}\" to {1} BuildTargetGroups", id, num));
        }

        /// <summary> Remove the passed global define if it's present</summary>
        public static void RemoveGlobalDefine(string id)
        {
            bool flag = false;
            int num = 0;
            foreach (BuildTargetGroup buildTargetGroup in (BuildTargetGroup[]) Enum.GetValues(typeof(BuildTargetGroup)))
                if (IsValidBuildTargetGroup(buildTargetGroup))
                {
                    string[] array = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';');
                    if (Array.IndexOf(array, id) == -1) continue;
                    flag = true;
                    ++num;
                    StringBuilderContainer.Length = 0;
                    foreach (string t in array)
                        if (t != id)
                        {
                            if (StringBuilderContainer.Length > 0) StringBuilderContainer.Append(';');
                            StringBuilderContainer.Append(t);
                        }

                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, StringBuilderContainer.ToString());
                }

            StringBuilderContainer.Length = 0;
            if (!flag) return;
            DDebug.Log(string.Format("Removed global define \"{0}\" from {1} BuildTargetGroups", id, num));
        }

        /// <summary>
        ///     Returns TRUE if the given global define is present in all the <see cref="T:UnityEditor.BuildTargetGroup" />
        ///     or only in the given <see cref="T:UnityEditor.BuildTargetGroup" />, depending on passed parameters.
        ///     <para />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildTargetGroup"><see cref="T:UnityEditor.BuildTargetGroup" />to use. Leave NULL to check in all of them.</param>
        public static bool HasGlobalDefine(string id, BuildTargetGroup? buildTargetGroup = null)
        {
            BuildTargetGroup[] buildTargetGroupArray = buildTargetGroup.HasValue ? new[] {buildTargetGroup.Value} : (BuildTargetGroup[]) Enum.GetValues(typeof(BuildTargetGroup));
            return buildTargetGroupArray.Where(IsValidBuildTargetGroup).Any(buildTargetGroup1 => Array.IndexOf(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup1).Split(';'), id) != -1);
        }

        private static bool IsValidBuildTargetGroup(BuildTargetGroup group)
        {
            if (group == BuildTargetGroup.Unknown) return false;
            Type unityEditorModuleManagerType = Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
            if (unityEditorModuleManagerType == null) return true;
            MethodInfo method1 = unityEditorModuleManagerType.GetMethod("GetTargetStringFromBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo method2 = typeof(PlayerSettings).GetMethod("GetPlatformName", BindingFlags.Static | BindingFlags.NonPublic);
            var parameters = new object[] {group};
            if (method1 == null) return true;
            string str1 = (string) method1.Invoke(null, parameters);
            if (method2 == null) return true;
            string str2 = (string) method2.Invoke(null, new object[] {group});
            if (string.IsNullOrEmpty(str1)) return !string.IsNullOrEmpty(str2);
            return true;
        }
    }
}