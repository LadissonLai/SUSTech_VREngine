using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Fxb.Localization
{
    public class LocalizeMgr : Singleton<LocalizeMgr>, IDispose
    {
        private List<string> allLanguages = new List<string>();

        private string currentLanguage;

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }

            set
            {
                if (value == currentLanguage)
                    return;

                if (!allLanguages.Contains(value))
                    return;

                if (value != currentLanguage)
                {
                    currentLanguage = value;

                    OnLanguageChanged?.Invoke(currentLanguage);
                }
            }
        }

        public IReadOnlyList<string> AllLanguages
        {
            get => allLanguages.AsReadOnly();
        }

        public string DefaultLanguage { get; private set; }

        private IWordLibrary wordLibrary;

        private IWordRecorder wordRecorder;

        public event Action<string> OnLanguageChanged;

        public void Dispose()
        {
            wordRecorder?.Dispose();

            wordRecorder = null;
        }

        public void Init(IWordLibrary wordLibrary_ = null, IWordRecorder wordRecorder_ = null)
        {
            wordLibrary = wordLibrary_ ?? new EmptyWordLibrary();

            wordRecorder = wordRecorder_ ?? new DebugWordRecorder();
        }

        public void AddLanguage(string languageID, bool isDefaultLanguage = false)
        {
            if (!allLanguages.Contains(languageID))
                allLanguages.Add(languageID);

            if (isDefaultLanguage)
            {
                currentLanguage = DefaultLanguage = languageID;
            }
        }

        public string GetLocalizedStr(string src)
        {
            if(!wordLibrary.SearchLanguageWords(currentLanguage, src, out var dest))
            {
                //写入missing word
                wordRecorder.Record(src);
            }

            return dest;
        }

        public string GetLocalizedStr(string srcFormat, params string[] srcArgs)
        {
            if(!wordLibrary.SearchLanguageWords(currentLanguage, srcFormat, out var resFormat))
            {
                wordRecorder.Record(srcFormat);
            }
 
            for (int i = 0; i < srcArgs.Length; i++)
            {
                if(!wordLibrary.SearchLanguageWords(currentLanguage, srcArgs[i], out var destWord))
                {
                    wordRecorder.Record(srcArgs[i]);
                }

                srcArgs[i] = destWord;
            }

            return string.Format(resFormat, srcArgs);
        }
    }
}
 