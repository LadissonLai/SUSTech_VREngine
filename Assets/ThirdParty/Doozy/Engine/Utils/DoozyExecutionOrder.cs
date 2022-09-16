// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable IdentifierTypo
namespace Doozy.Engine.Utils
{
    /// <summary> Execution order for all the components inside DoozyUI </summary>
    public static class DoozyExecutionOrder
    {
        private const int COMPONENT = -100;
        private const int MANAGER = -200;
        
        public const int BACK_BUTTON = MANAGER;
        public const int COLOR_TARGET_IMAGE = COMPONENT;
        public const int COLOR_TARGET_PARTICLE_SYSTEM = COMPONENT;
        public const int COLOR_TARGET_RAW_IMAGE = COMPONENT;
        public const int COLOR_TARGET_SPRITE_RENDERER = COMPONENT;
        public const int COLOR_TARGET_TEXT = COMPONENT;
        public const int COLOR_TARGET_TEXTMESHPRO = COMPONENT;
        public const int COLOR_TARGET_SELECTABLE = COMPONENT;
        public const int COLOR_TARGET_UNITY_EVENT = COMPONENT;
        public const int FONT_TARGET_TEXT = COMPONENT;
        public const int FONT_TARGET_TEXTMESHPRO = COMPONENT;
        public const int GAME_EVENT_LISTENER = COMPONENT;
        public const int GAME_EVENT_MANAGER = MANAGER;
        public const int GESTURE_LISTENER = COMPONENT;
        public const int GRAPH_CONTROLLER = COMPONENT;
        public const int KEY_TO_ACTION = COMPONENT;
        public const int KEY_TO_GAME_EVENT = COMPONENT;
        public const int ORIENTATION_DETECTOR = COMPONENT;
        public const int PROGRESS_TARGET_ACTION = COMPONENT + 1;
        public const int PROGRESS_TARGET_ANIMATOR = COMPONENT + 1;
        public const int PROGRESS_TARGET_AUDIOMIXERGROUP = COMPONENT + 1;
        public const int PROGRESS_TARGET_IMAGE = COMPONENT + 1;
        public const int PROGRESS_TARGET_TEXT = COMPONENT + 1;
        public const int PROGRESS_TARGET_TEXTMESHPRO = COMPONENT + 1;
        public const int PROGRESSOR = COMPONENT;
        public const int PROGRESSOR_GROUP = COMPONENT;
        public const int RADIAL_LAYOUT = COMPONENT + 2;
        public const int SCENE_DIRECTOR = COMPONENT;
        public const int SCENE_LOADER = COMPONENT;
        public const int SOUNDY_CONTROLLER = COMPONENT;
        public const int SOUNDY_MANAGER = MANAGER;
        public const int SOUNDY_POOLER = COMPONENT;
        public const int SPRITE_TARGET_IMAGE = COMPONENT;
        public const int SPRITE_TARGET_SELECTABLE = COMPONENT;
        public const int SPRITE_TARGET_SPRITE_RENDERER = COMPONENT;
        public const int SPRITE_TARGET_UNITY_EVENT = COMPONENT;
        public const int TEXTURE_TARGET_RAW_IMAGE = COMPONENT;
        public const int TEXTURE_TARGET_UNITY_EVENT = COMPONENT;
        public const int THEME_MANAGER = MANAGER;
        public const int TOUCH_DETECTOR = MANAGER;
        public const int UIBUTTON = COMPONENT;
        public const int UIBUTTON_LISTENER = COMPONENT;
        public const int UICANVAS = COMPONENT;
        public const int UIDRAWER = COMPONENT;
        public const int UIDRAWER_LISTENER = COMPONENT;
        public const int UIIMAGE = COMPONENT;
        public const int UIPOPUP = COMPONENT;
        public const int UIPOPUP_MANAGER = MANAGER;
        public const int UITOGGLE = COMPONENT;
        public const int UIVIEW = COMPONENT;
        public const int UIVIEW_LISTENER = COMPONENT;
    }
}