using UnityEngine;
using UnityEngine.UI;

namespace Fxb.Localization
{
    /// <summary>
    /// 用作ugui文本组件静态内容的自动化切换翻译内容。
    /// 尽量不要用在代码动态赋值的Text上
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class UguiTextLocalize : LocalizeTargetBase
    {
        public Text text;

        private string srcTextString;
         
        protected override void OnLocalize()
        {
            if (text == null)
                text = GetComponent<Text>();

            if (srcTextString == null)
                srcTextString = text.text;

            text.text = LocalizeMgr.Inst.GetLocalizedStr(srcTextString);
        }
    }
}

