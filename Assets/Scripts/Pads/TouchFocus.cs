using UnityEngine;
using UnityEngine.UI;
using VRTK;

namespace Fxb.CPTTS
{
    /// <summary>
    /// VRTK_EventSystem中并没有把vrInputModule设置成当前的currentInputModule,
    /// 而是新添加了一个VRTK_VRInputModule作为脚本添加到go上。
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class TouchFocus : MonoBehaviour
    {
        private VRTK_VRInputModule vrInputModule;

        private Canvas canvas;

        public Graphic targetFocusMark;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        // Update is called once per frame
        void Update()
        {
            targetFocusMark.enabled = false;

            if (vrInputModule == null)
            {
                vrInputModule = FindObjectOfType<VRTK_VRInputModule>();

                return;
            }

            if (canvas.worldCamera == null)
            {
                canvas.worldCamera = VRTK_DeviceFinder.HeadsetTransform()?.GetComponent<Camera>();
            }

            for (int i = 0, len = vrInputModule.pointers.Count; i < len; i++)
            {
                var uiPointer = vrInputModule.pointers[i];

                var pointerEventData = uiPointer.pointerEventData;
                
                if (uiPointer.PointerActive() && pointerEventData.pointerPress == null && !pointerEventData.dragging)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        targetFocusMark.transform.parent as RectTransform,
                        pointerEventData.position,
                        canvas.worldCamera, out var t
                        );

                    targetFocusMark.rectTransform.anchoredPosition = t;

                    targetFocusMark.enabled = true;
                }
            }
        }
    }
}
