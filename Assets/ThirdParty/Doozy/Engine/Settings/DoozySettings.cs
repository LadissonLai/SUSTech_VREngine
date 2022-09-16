// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.Settings
{
    [Serializable]
    public class DoozySettings : ScriptableObject
    {
        public const string FILE_NAME = "DoozySettings";
        private static string ResourcesPath { get { return DoozyPath.ENGINE_RESOURCES_PATH; } }

        private static DoozySettings s_instance;

        public static DoozySettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<DoozySettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }


        public Engine.Language SelectedLanguage = LanguagePack.DEFAULT_LANGUAGE;

        public bool AutoDisableUIInteractions = true;
        public bool DebugBackButton;
        public bool DebugGameEventListener;
        public bool DebugGameEventManager;
        public bool DebugGestureListener;
        public bool DebugGraphController;
        public bool DebugKeyToAction;
        public bool DebugKeyToGameEvent;
        public bool DebugOrientationDetector;
        public bool DebugProgressor;
        public bool DebugProgressorGroup;
        public bool DebugSceneDirector;
        public bool DebugSceneLoader;
        public bool DebugSoundyController;
        public bool DebugSoundyManager;
        public bool DebugSoundyPooler;
        public bool DebugTouchDetector;
        public bool DebugUIButton;
        public bool DebugUIButtonListener;
        public bool DebugUICanvas;
        public bool DebugUIDrawer;
        public bool DebugUIDrawerListener;
        public bool DebugUIPopup;
        public bool DebugUIPopupManager;
        public bool DebugUIToggle;
        public bool DebugUIView;
        public bool DebugUIViewListener;
        public bool DOTweenDetected;
        public bool DoozyUIVersion3Detected;
        public bool DoozyUIVersion2Detected;
        public bool MasterAudioDetected;
        public bool PlaymakerDetected;
        public bool TextMeshProDetected;
        public bool IgnoreUnityTimescale = true; //If TRUE, the UI animations will ignore Unity's Time.Timescale. By default, this value is set to TRUE
        public bool SpeedBasedAnimations;        //If TRUE, then all the tweens will be set as speed based (the duration will represent the number of units the  tween moves x second). Has no effect on Sequences, nested tweens, or if the tween has already started
        public bool UseBackButton = true;
        public bool UseMasterAudio;
        public bool UseOrientationDetector;
        public bool UsePlaymaker;
        public bool UseTextMeshPro;
        public bool AssetDatabaseSaveAssetsNeeded;
        public bool AssetDatabaseRefreshNeeded;

        /// <summary> Executed if a Refresh was in order (due to the creation of assets) or a SaveAssets needed, but NOT performed </summary>
        public void SaveAndRefreshAssetDatabase()
        {
#if UNITY_EDITOR
            if (AssetDatabaseRefreshNeeded)
            {
                AssetDatabaseSaveAssetsNeeded = false;
                AssetDatabaseRefreshNeeded = false;
                SetDirty(true);
                UnityEditor.AssetDatabase.Refresh();
                return;
            }
            
            if (AssetDatabaseSaveAssetsNeeded)
            {
                AssetDatabaseSaveAssetsNeeded = false;
                SetDirty(true);
            }
#endif
        }


        #region Help

        public const string MAILTO = "mailto:";
        public const string SUPPORT_EMAIL_ADDRESS = "support@doozyui.com";


        public const string LINK_DISCORD_INVITE = "https://discord.gg/y9Axq7b";
        public const string LINK_FACEBOOK = "https://www.facebook.com/doozyentertainment";
        public const string LINK_TWITTER = "https://twitter.com/doozyplay";
        public const string LINK_WEBSITE_DOOZYUI = "http://doozyui.com/";
        public const string LINK_WEBSITE_DOOZYUI_DOCUMENTATION = "http://doozyui.com/learn/documentation/";
        public const string LINK_WEBSITE_DOOZYUI_DOCUMENTATION_GETTING_STARTED = "http://doozyui.com/getting-started/";
        public const string LINK_WEBSITE_DOOZYUI_FAQ = "http://doozyui.com/learn/faq/";
        public const string LINK_WEBSITE_DOOZYUI_LEARN = "http://doozyui.com/learn/";
        public const string LINK_WEBSITE_DOOZYUI_SUPPORT_REQUEST = "http://doozyui.com/support-request/";
        public const string LINK_WEBSITE_DOTWEEN = "http://dotween.demigiant.com/";
        public const string LINK_WEBSITE_MICROSOFT_DOT_NET_API = "https://docs.microsoft.com/en-us/dotnet/api/";
        public const string LINK_WEBSITE_UNITY_MANUAL = "https://docs.unity3d.com/Manual/index.html";
        public const string LINK_WEBSITE_UNITY_SCRIPTING_API = "https://docs.unity3d.com/ScriptReference/index.html";
        public const string LINK_YOUTUBE_CHANNEL = "http://www.youtube.com/c/DoozyEntertainment";
        public const string LINK_YOUTUBE_VIDEOS = "https://www.youtube.com/user/doozyplay/videos";
        public const string LINK_ZENDESK_TICKET = "https://doozyentertainment.zendesk.com/hc/en-us/requests/new";

        #endregion

        #region About
      
        public const string DOOZYUI_ABOUT = "DoozyUI is a complete User Interface (UI) management system for Unity. It does that by manipulating native Unity components and taking full advantage of their intended usage." +
                                            "\n\n" +
                                            "This approach assures maximum compatibility with uGUI, best performance and makes the entire system behave in a predictable manner." +
                                            "\n\n" +
                                            "Also, by working only with native components, the system will be compatible with any other asset that uses uGUI correctly." +
                                            "\n\n" +
                                            "Easy to use and understand, given the user has some basic knowledge of how Unity’s native UI solution (uGUI) works, DoozyUI has flexible components that can be configured in a lot of ways. Functionality and design go hand in hand in order to offer a pleasant User Experience (UX) while using the system.";

        public const string SOUNDY_VERSION = "1.0.0";

        public const string SOUNDY_ABOUT = "Soundy is a complex sound management system that works side by side with DoozyUI. It can play sounds from one of its sound databases, or directly by using AudioClip references, or through MasterAudio (a third-party plugin)." +
                                           "\n\n" +
                                           "An automated sound pooling system (Soundy Pooler) manages memory consumption while playing sounds in a dynamic way by reusing Soundy Controllers. The sounds pool is dynamic, Soundy Pooler being able to grow and shrink it as needed.";

        public const string TOUCHY_VERSION = "1.0.0";

        public const string TOUCHY_ABOUT = "Touchy is a touch detection system that comes with DoozyUI. It captures touches (and clicks) from Unity's native Input solution and passes it the system with a few extra bits of info." +
                                           "\n\n" +
                                           "A Gesture Listener is also included, and it is responsible of interpreting the touch and can identify taps, long taps and swipes.";

        public const string NODY_VERSION = "1.0.0";

        public const string NODY_ABOUT = "Nody is a node graph engine built around modular node components which can be connected to form Graphs. Being an important part of DoozyUI, it was specifically designed to help create, visualize and manage UI navigation flows." +
                                         "\n\n" +
                                         "Typical node graphs are intended to be drawn (and work) from left to right or from top to bottom. An UI can flow in any direction and Nody was designed to accommodate that requirement. Each selected node shows its input and output connections in a visual way." +
                                         "\n\n" +
                                         "The node graph architecture implemented in Nody is robust and reliable enough to handle very complex UI flows to create deep UI interactions. Graph UI flows can be set up to work together or separately all together.";

        #endregion

      
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}