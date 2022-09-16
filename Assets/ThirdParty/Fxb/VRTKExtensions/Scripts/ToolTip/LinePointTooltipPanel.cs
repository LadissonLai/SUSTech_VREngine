using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTKExtensions;

namespace VRTKExtensions
{
    //目前有问题  暂不使用
    public class LinePointTooltipPanel : PosFollowTooltipPanel
    {
        public LineRenderer lineRenderer;

        private void Awake()
        {
            //lineRenderer.startWidth = 0.003f;
            //lineRenderer.endWidth = lineRenderer.startWidth;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
             
            lineRenderer.SetPosition(0, FollowTarget.position);

            lineRenderer.SetPosition(1, FollowPos.Value);

            UpdateTooltipDir();
        }

        protected void UpdateTooltipDir()
        {
            if (RenderCam == null)
                return;

            var originViewPos = RenderCam.WorldToViewportPoint(FollowTarget.position);

            var tooltipViewPos = RenderCam.WorldToViewportPoint(FollowPos.Value);

            var canvasRT = planeCanvas.transform as RectTransform;

            if (tooltipViewPos.x > originViewPos.x)
            {
                canvasRT.pivot = Vector2.zero;
            }
            else if(tooltipViewPos.x < originViewPos.x)
            {
                canvasRT.pivot = Vector2.right;
            }

            LayoutRebuilder.MarkLayoutForRebuild(canvasRT);
        }
    }
}
