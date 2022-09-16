using UnityEngine;
using UnityEngine.UI;

namespace Fxb.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UguiImageLocalize : LocalizeTargetBase
    {
        [System.Serializable]
        public class LocalizeAssetsSprite : LocalizeAssets<Sprite> { }
         
        public Image img;
         
        [Tooltip("切换sprite时是否自动设置大小")]
        public bool autoSetToNativeSize;

        public LocalizeAssetsSprite[] localizedAssets;

        private Sprite srcAssetSprite;
 
        protected override void OnLocalize()
        {
            if(img == null)
                img = GetComponent<Image>();

            if (srcAssetSprite == null)
                srcAssetSprite = img.sprite;

            foreach (var lAssets in localizedAssets)
            {
                if (LocalizeMgr.Inst.CurrentLanguage == lAssets.language)
                {
                    SetImgSprite(lAssets.assetsReference);
                    return;
                }
            }
            
            SetImgSprite(srcAssetSprite);
        }

        protected void SetImgSprite(Sprite sprite)
        {
            img.sprite = sprite;

            if (autoSetToNativeSize)
                img.SetNativeSize();
        }
    }
}
