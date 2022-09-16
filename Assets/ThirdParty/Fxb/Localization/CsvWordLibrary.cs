using System;
using UnityEngine;
using System.IO;
using Framework;

namespace Fxb.Localization
{
    public class CsvWordLibrary : IWordLibrary
    {
        //private LanguageConfig languageConfig;

        public bool SearchLanguageWords(string language, string srcWord, out string destWord)
        {
            destWord = srcWord;

            //if (languageConfig == null)
            //    languageConfig = World.Get<LanguageConfig>();

            //if (Enum.TryParse<LanguageConfig.Key>(language, true, out var configCol))
            //{
            //    //var word = LanguageConfig.Inst.Get((int)configCol, srcWord);

            //    var word = languageConfig.Find((int)configCol, srcWord);

            //    destWord = string.IsNullOrWhiteSpace(word) ? srcWord : word;

            //    return word != null;
            //}

            return false;
        }
    }
}
 