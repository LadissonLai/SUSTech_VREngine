using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    [ExecuteInEditMode]
    public class TextCutOffLayout : UIBehaviour, ILayoutSelfController
    {
        RectTransform rect;

        [Tooltip("文本需要保持的AnchorPosition.x")]
        public float customStayPosX = 215;

        [Tooltip("文本初始(最大)宽度")]
        public float maxWidth = 734;

        protected override void Start()
        {
            rect = transform as RectTransform;
        }

        public void SetLayoutHorizontal()
        {
            //Debug.Log(rect.sizeDelta.x);

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth - rect.
                anchoredPosition.x + customStayPosX);
        }

        public void SetLayoutVertical()
        {

        }
    }

}