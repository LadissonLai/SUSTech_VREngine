using UnityEngine;
using UnityEngine.UI;

namespace Fxb.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class UguiTextStyleLocalize : LocalizeTargetBase
    {
        [System.Serializable]
        public class LocalizeAssetsTextStyle : LocalizeAssets<Font>
        {
            public int fontSize = 12;
            public float lineSpacing = 1.0f;
            public FontStyle fontStyle;

            //不确定是否需要
            //public bool bestFit;
            //public TextAnchor alignment;
            //public bool alignByGeometry;
            //public bool richText;
            //public HorizontalWrapMode horizontalOverflow;
            //public VerticalWrapMode verticalOverflow;
        }

        public Text text;

        public LocalizeAssetsTextStyle[] localizedAssets;

        private LocalizeAssetsTextStyle defaultLocalizedAssets;

        protected override void OnLocalize()
        {
            if (text == null)
                text = GetComponent<Text>();

            defaultLocalizedAssets = defaultLocalizedAssets ?? new LocalizeAssetsTextStyle()
            {
                assetsReference = text.font,
                fontSize = text.fontSize,
                lineSpacing = text.lineSpacing,
                fontStyle = text.fontStyle
            };

            foreach (var lAssets in localizedAssets)
            {
                if (LocalizeMgr.Inst.CurrentLanguage == lAssets.language)
                {
                    SetLocalizedAssets(lAssets);
                    return;
                }
            }

            SetLocalizedAssets(defaultLocalizedAssets);
        }

        protected void SetLocalizedAssets(LocalizeAssetsTextStyle lAssets)
        {
            text.font = lAssets.assetsReference != null ? lAssets.assetsReference : text.font;

            text.fontSize = lAssets.fontSize != 0 ? lAssets.fontSize : text.fontSize;

            text.lineSpacing = lAssets.lineSpacing != 0 ? lAssets.lineSpacing : text.lineSpacing;

            text.fontStyle = lAssets.fontStyle;
        }
    }
}

