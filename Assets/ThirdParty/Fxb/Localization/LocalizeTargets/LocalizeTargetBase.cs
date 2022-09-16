using UnityEngine;
 
namespace Fxb.Localization
{
    public abstract class LocalizeTargetBase : MonoBehaviour
    {
        public bool NeedLocalize { get; protected set; } = true;

        /// <summary>
        /// 当前已经本地化对应语言，默认为 DefaultLanguage
        /// </summary>
        private string localizedLanguage;

        protected virtual void OnEnable()
        {
            LocalizeMgr.Inst.OnLanguageChanged += OnLanguageChanged;

            if (localizedLanguage == null)
                localizedLanguage = LocalizeMgr.Inst.DefaultLanguage;

            if(localizedLanguage != LocalizeMgr.Inst.CurrentLanguage)
            {
                OnLocalize();

                localizedLanguage = LocalizeMgr.Inst.CurrentLanguage;
            }
        }

        protected virtual void OnDisable()
        {
            LocalizeMgr.Inst.OnLanguageChanged -= OnLanguageChanged;
        }
 
        private void OnLanguageChanged(string cutLanguage)
        {
            OnLocalize();

            localizedLanguage = cutLanguage;
        }

        protected abstract void OnLocalize();
    }
    
    /// <summary>
    /// 泛型无法被编辑器序列化，需要继承到一个子类上
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LocalizeAssets<T> where T : Object
    {
        public string language;

        public T assetsReference;
    }
}