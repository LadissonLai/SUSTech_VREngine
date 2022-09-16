using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

namespace Fxb.CPTTS
{
    public class HelpView : PadViewBase
    {
        public RectTransform helpListRect, powerListTipGroup;

        bool isRefreshList;

        public Image helpIntroductionDiagram;
    
        public SpriteAtlas spriteAtlas;

        private readonly string spriteName = "help";
        [SerializeField]
        private TextMeshProUGUI textMeshProDiscOrRocker;

        protected override void Start()
        {
            base.Start();

            isRefreshList = true;

#if IS_HTCVIVE
            IsHTCHandleIntroduction();
#else
           IsHTCHandleIntroduction(false);
#endif
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSeconds(0.1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(helpListRect);
        }

        private void LateUpdate()
        {
            if(isRefreshList)
            {
                StartCoroutine(RebuildLayout());

                isRefreshList = false;
            }
        }

        /// <summary>
        /// 是否是HTC手柄介绍
        /// </summary>
        /// <param name="isHtc"></param>
        private void IsHTCHandleIntroduction(bool isHtc = true)
        {
            powerListTipGroup.gameObject.SetActive(!isHtc);

            textMeshProDiscOrRocker.text = isHtc ? "1 -圆盘键:" : "1 - 摇杆:";

            var spritestring = isHtc ? spriteName : string.Concat(spriteName, "2");

            var sprite = spriteAtlas.GetSprite(spritestring);

            if (helpIntroductionDiagram == null)
                return;

            if (sprite)
            {
                helpIntroductionDiagram.sprite = sprite;
            }
        }
    }
}
