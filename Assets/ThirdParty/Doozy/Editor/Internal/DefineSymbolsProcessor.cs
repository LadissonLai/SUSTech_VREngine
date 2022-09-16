// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Doozy.Editor.Utils;
using Doozy.Engine.Settings;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Doozy.Editor.Internal
{
    [InitializeOnLoad]
    public static class DefineSymbolsProcessor
    {
        /// <summary> Define Symbol for Doozy UI Manager </summary>
        public const string DEFINE_DOOZY_MANAGER = "dUI_MANAGER";
        
        /// <summary> Define Symbol for Doozy UI Designer </summary>
        public const string DEFINE_DOOZY_DESIGNER = "dUI_DESIGNER";            

        /// <summary> Define Symbol for Master Audio </summary>
        public const string DEFINE_MASTER_AUDIO = "dUI_MasterAudio";

        /// <summary> Define Symbol for Playmaker </summary>
        public const string DEFINE_PLAYMAKER = "dUI_Playmaker";

        /// <summary> Define Symbol for TextMeshPro </summary>
        public const string DEFINE_TEXT_MESH_PRO = "dUI_TextMeshPro";

        /// <summary> Namespace for Master Audio </summary>
        private const string NAMESPACE_DARK_TONIC_MASTER_AUDIO = "DarkTonic.MasterAudio";

        /// <summary> Namespace for DOTween </summary>
        private const string NAMESPACE_DG_TWEENING = "DG.Tweening";

        /// <summary> Namespace for DoozyUI version 2 </summary>
        private const string NAMESPACE_DOOZYUI = "DoozyUI";

        /// <summary> Namespace for DoozyUI version 3 </summary>
        private const string NAMESPACE_DOOZY_ENGINE = "Doozy.Engine";

        /// <summary> Namespace for Playmaker </summary>
        private const string NAMESPACE_HUTONG_GAMES_PLAY_MAKER = "HutongGames.PlayMaker";

        /// <summary> Namespace for TextMeshPro </summary>
        private const string NAMESPACE_TMPRO = "TMPro";

        private static DoozySettings Settings { get { return DoozySettings.Instance; } }
        private static bool s_saveAssets;
        private static List<Assembly> s_assemblies = new List<Assembly>();

        static DefineSymbolsProcessor() { ExecuteProcessor(); }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            //Do nothing... for now ;)
        }

        private static void ExecuteProcessor()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            DelayedCall.Run(0.1f, Run);
        }

        public static void Run()
        {
            s_saveAssets = false;
            UpdateAssemblies();
            UpdateInstalledPluginsInDoozySettings();
            UpdateActivatedPluginsInDoozySettings();
            if (s_saveAssets) AssetDatabase.SaveAssets();
            UpdateScriptingDefineSymbols();
        }

        private static IEnumerable<Assembly> GetAssemblies() { return AppDomain.CurrentDomain.GetAssemblies(); }

        //https://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
        private static bool NamespaceExists(string targetNamespace)
        {
            if (s_assemblies == null || s_assemblies.Count == 0)
            {
                UpdateAssemblies();
                if (s_assemblies == null || s_assemblies.Count == 0)
                    return false;
            }

            foreach (Assembly assembly in s_assemblies)
            {
                if (assembly == null) continue;
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types.Where(t => t != null).ToArray();
                }

                if (typesInAsm.Any(type => type.Namespace == targetNamespace))
                    return true;
            }

            return false;
        }

        private static void UpdateAssemblies()
        {
            s_assemblies.Clear();
            s_assemblies = GetAssemblies().ToList();
        }

        private static void UpdateInstalledPluginsInDoozySettings()
        {
            bool saveAssets = false;

            //MasterAudio - DarkTonic.MasterAudio
            bool hasMasterAudio = NamespaceExists(NAMESPACE_DARK_TONIC_MASTER_AUDIO);
            if (Settings.MasterAudioDetected != hasMasterAudio)
            {
                Settings.MasterAudioDetected = hasMasterAudio;
                saveAssets = true;
            }

            //DOTween - DG.Tweening
            bool hasDOTween = NamespaceExists(NAMESPACE_DG_TWEENING);
            if (Settings.DOTweenDetected != hasDOTween)
            {
                Settings.DOTweenDetected = hasDOTween;
                saveAssets = true;
            }

            //Previous DoozyUI version - DoozyUI
            bool hasDoozyUI = NamespaceExists(NAMESPACE_DOOZYUI);
            if (Settings.DoozyUIVersion2Detected != hasDoozyUI)
            {
                Settings.DoozyUIVersion2Detected = hasDoozyUI;
                saveAssets = true;
            }

            //Current DoozyUI version - Doozy.Engine
            bool hasDoozyEngine = NamespaceExists(NAMESPACE_DOOZY_ENGINE);
            if (Settings.DoozyUIVersion3Detected != hasDoozyEngine)
            {
                Settings.DoozyUIVersion3Detected = hasDoozyEngine;
                saveAssets = true;
            }

            //Playmaker - HutongGames.PlayMaker
            bool hasPlaymaker = NamespaceExists(NAMESPACE_HUTONG_GAMES_PLAY_MAKER);
            if (Settings.PlaymakerDetected != hasPlaymaker)
            {
                Settings.PlaymakerDetected = hasPlaymaker;
                saveAssets = true;
            }

            //TextMeshPro - TMPro
            bool hasTextMeshPro = NamespaceExists(NAMESPACE_TMPRO);
            if (Settings.TextMeshProDetected != hasTextMeshPro)
            {
                Settings.TextMeshProDetected = hasTextMeshPro;
                saveAssets = true;
            }

            if (!saveAssets) return;
            Settings.SetDirty(false);
            s_saveAssets = true;
        }

        private static void UpdateActivatedPluginsInDoozySettings()
        {
            bool saveAssets = false;

            //Playmaker
            if (Settings.UsePlaymaker && !Settings.PlaymakerDetected)
            {
                Settings.UsePlaymaker = false;
                saveAssets = true;
            }

            //MasterAudio
            if (Settings.UseMasterAudio && !Settings.MasterAudioDetected)
            {
                Settings.UseMasterAudio = false;
                saveAssets = true;
            }

            //TextMeshPro
            if (Settings.UseTextMeshPro && !Settings.TextMeshProDetected)
            {
                Settings.UseTextMeshPro = false;
                saveAssets = true;
            }

            if (!saveAssets) return;
            Settings.SetDirty(false);
            s_saveAssets = true;
        }

        public static void UpdateScriptingDefineSymbols()
        {
            DefineSymbolsUtils.AddGlobalDefine(DEFINE_DOOZY_MANAGER);
            
            if (Settings.UsePlaymaker) DefineSymbolsUtils.AddGlobalDefine(DEFINE_PLAYMAKER);
            else DefineSymbolsUtils.RemoveGlobalDefine(DEFINE_PLAYMAKER);

            if (Settings.UseMasterAudio) DefineSymbolsUtils.AddGlobalDefine(DEFINE_MASTER_AUDIO);
            else DefineSymbolsUtils.RemoveGlobalDefine(DEFINE_MASTER_AUDIO);

            if (Settings.UseTextMeshPro) DefineSymbolsUtils.AddGlobalDefine(DEFINE_TEXT_MESH_PRO);
            else DefineSymbolsUtils.RemoveGlobalDefine(DEFINE_TEXT_MESH_PRO);
        }
    }
}