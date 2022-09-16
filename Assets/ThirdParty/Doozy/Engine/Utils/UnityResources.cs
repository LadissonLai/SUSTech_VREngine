// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Doozy.Engine.Utils
{
    public static class UnityResources
    {
#if UNITY_EDITOR

        #region UI built-in sprites

        private const string STANDARD_SPRITE_PATH = "UI/Skin/UISprite.psd";
        private const string BACKGROUND_SPRITE_RESOURCE_PATH = "UI/Skin/Background.psd";
        private const string INPUT_FIELD_BACKGROUND_PATH = "UI/Skin/InputFieldBackground.psd";
        private const string KNOB_PATH = "UI/Skin/Knob.psd";
        private const string CHECKMARK_PATH = "UI/Skin/Checkmark.psd";

        public static Sprite UISprite { get { return AssetDatabase.GetBuiltinExtraResource<Sprite>(STANDARD_SPRITE_PATH); } }
        public static Sprite Background { get { return AssetDatabase.GetBuiltinExtraResource<Sprite>(BACKGROUND_SPRITE_RESOURCE_PATH); } }
        public static Sprite FieldBackground { get { return AssetDatabase.GetBuiltinExtraResource<Sprite>(INPUT_FIELD_BACKGROUND_PATH); } }
        public static Sprite Knob { get { return AssetDatabase.GetBuiltinExtraResource<Sprite>(KNOB_PATH); } }
        public static Sprite Checkmark { get { return AssetDatabase.GetBuiltinExtraResource<Sprite>(CHECKMARK_PATH); } }

        #endregion

#endif
    }
}