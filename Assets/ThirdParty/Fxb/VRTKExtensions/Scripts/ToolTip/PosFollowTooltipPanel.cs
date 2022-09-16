using UnityEngine;
using UnityEngine.UI;
using Framework;
using TMPro;

namespace VRTKExtensions
{
    public class PosFollowTooltipPanel : MonoBehaviour, ISpawnAbleTooltip
    {
        private bool textValInvalid;

        private string textTip;

        public Transform FollowTarget { set; get; }

        public Vector3? FollowPos { set; get; }

        public string Key { get; set; }

        public Vector3 AdjuestPosOffset { set; get; }
         
        public string TextTip
        {
            set
            {
                if (textTip == value)
                    return;

                textTip = value;

                textValInvalid = true;
            }
        }

        public Canvas planeCanvas;

        public Text text;

        public TextMeshProUGUI textPro;

        #region 防止距离过远看不到文字，距离过近文字太大

        [Tooltip("最小高度 摄像机空间 0-1")]
        public float minViewSizeH = 0.05f;

        [Tooltip("最大高度 摄像机空间 0-1")]
        public float maxViewSizeH = 0.3f;

        #endregion
         
        [Tooltip("是否根据观察距离刷新尺寸")]
        public bool autoAdjuestSize;
         
        public CanvasGroup canvasGroup;

        protected Camera RenderCam => VRTKHelper.HeadSetCamera != null ? VRTKHelper.HeadSetCamera : Camera.main;

        public virtual void OnDespawn()
        {
            FollowTarget = null;

            FollowPos = null;

            AdjuestPosOffset = Vector3.zero;

            gameObject.SetActive(false);
        }

        public virtual void OnSpawn()
        {
            gameObject.SetActive(true);
  
            canvasGroup.alpha = 0.0f;
        }

        protected void LookupCam()
        {
            if (RenderCam == null)
                return;

            var dir = RenderCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f)) - RenderCam.transform.position;

            dir.Normalize();

            transform.forward = dir;
        }

        protected virtual void AdjuestSize()
        {
            if (RenderCam == null)
                return;

            //m11 = cot(fovy/2)  
            //m11 = depth/(h / 2)           
            //h = depth / m11 * 2
            //depth = m11 * (h/2)

            var canvasRT = planeCanvas.transform as RectTransform;

            var canvasScaleY = canvasRT.localScale.y * canvasRT.sizeDelta.y;
             
            var m11 = Camera.main.projectionMatrix.m11;
              
            var currentDepth = RenderCam.WorldToViewportPoint(transform.position).z;

            //当前深度的全屏尺寸
            var currentFullWH = currentDepth / m11 * 2 / canvasScaleY;

            //当前限制的最大尺寸
            var currentLimitMaxWH = currentFullWH * maxViewSizeH;

            var currentLimitMinWH = currentFullWH * minViewSizeH;

            //Debug.Log($"currentFullWH:{currentFullWH}  currentLimitMaxWH:{currentLimitMaxWH} currentLimitMinWH:{currentLimitMinWH} canvasScaleY:{canvasScaleY}");

            var scaleY = 1.0f;

            if (currentLimitMaxWH <= 1)
            {
                //尺寸过大 显示距离过近
                scaleY = currentLimitMaxWH;
            }
            else if (currentLimitMinWH > 1)
            {
                //尺寸过大 显示距离过远
                scaleY = currentLimitMinWH;
            }

            transform.localScale = new Vector3(scaleY, scaleY, 1);
        }

        protected virtual void FollowTargetPos()
        {
            var targetPos = FollowPos == null ? FollowTarget.position : FollowPos.Value;

            transform.position = targetPos + AdjuestPosOffset;
        }

        protected virtual void LateUpdate()
        {
            if(textValInvalid)
            {
                textValInvalid = true;

                if (text != null)
                    text.text = textTip;
                else
                    textPro.text = textTip;

                LayoutRebuilder.ForceRebuildLayoutImmediate(planeCanvas.transform as RectTransform);
            }

            //待观察效果
            canvasGroup.alpha = 1;
             
            if (FollowPos != null || FollowTarget != null)
                FollowTargetPos();
             
            LookupCam();

            if (autoAdjuestSize)
                AdjuestSize();
        }
    }
}



