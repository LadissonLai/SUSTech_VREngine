// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Engine.Utils
{
    [Serializable]
    public class LanguagePack : ScriptableObject
    {
        private const string CURRENT_LANGUAGE_PREFS_KEY = "Doozy.CurrentLanguage";
        public const Engine.Language DEFAULT_LANGUAGE = Engine.Language.English;

        private static Engine.Language s_currentLanguage = Engine.Language.Unknown;

        public static Engine.Language CurrentLanguage
        {
            get
            {
                if (s_currentLanguage != Engine.Language.Unknown) return s_currentLanguage;
                CurrentLanguage = (Engine.Language) PlayerPrefs.GetInt(CURRENT_LANGUAGE_PREFS_KEY, (int) DEFAULT_LANGUAGE);
                return s_currentLanguage;
            }
            set
            {
                SaveLanguagePreference(value);
                s_currentLanguage = value;
            }
        }

        private static void SaveLanguagePreference(Engine.Language language) { SaveLanguagePreference(CURRENT_LANGUAGE_PREFS_KEY, language); }

        private static void SaveLanguagePreference(string prefsKey, Engine.Language language)
        {
            PlayerPrefs.SetInt(prefsKey, (int) language);
            PlayerPrefs.Save();
        }
    }
}