using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.Localization
{
    public interface IWordRecorder
    {
        void Record(string word);

        void Dispose();
    }

    public class DebugWordRecorder : IWordRecorder
    {
        public void Dispose()
        {
            
        }

        public void Record(string word)
        {
            //DebugEx.Log($"未翻译文字:{word}");
        }
    }
}
