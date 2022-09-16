using System;
using System.IO;
using UnityEngine;

namespace Fxb.Localization
{
    public class TxtFileOutputRecorder : IWordRecorder
    {
        private StreamWriter wordSW;
         
        public StreamWriter CreateUnTransWordFile()
        {
            var dir = Directory.GetParent(Application.dataPath).Parent;
            
            var path = Path.Combine(dir.ToString(), "LocalizeTool/UnTransWords/");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += $"WordRecord_{DateTime.Now.Ticks}.txt";

            Debug.Log($"TxtFileOutputRecorder   path:{path}");

            return File.CreateText(path);
        }

        public void Dispose()
        {
            if (wordSW != null)
            {
                Debug.Log("TxtFileOutputRecorder  dispose.-------");

                wordSW.Close();

                wordSW.Dispose();
            }
        }

        public void Record(string word)
        {
            if (!Debug.isDebugBuild)
                return;

            if (string.IsNullOrWhiteSpace(word))
                return;
             
            if (wordSW == null)
                wordSW = CreateUnTransWordFile();

            Debug.Log("记录未翻译文字:" + word);

            wordSW.WriteLine(word);

            wordSW.Flush();
        }
    }
}