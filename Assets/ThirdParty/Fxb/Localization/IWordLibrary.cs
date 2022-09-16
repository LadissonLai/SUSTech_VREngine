 
namespace Fxb.Localization
{
    public interface IWordLibrary
    {
        /// <summary>
        /// 搜索对应语言的文字
        /// </summary>
        /// <param name="language"></param>
        /// <param name="srcWord"></param>
        /// <returns></returns>
        bool SearchLanguageWords(string language, string srcWord, out string destWord);
    }

    public class EmptyWordLibrary : IWordLibrary
    {
        public bool SearchLanguageWords(string language, string srcWord, out string destWord)
        {
            destWord = srcWord;

            return false;
        }
    }
}

