using Fxb.SpawnPool;
using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 生成一个平面作为tips展示
    /// </summary>
    public class PanelSpawnTooltipTrigger : InteractObjTooltipTriggerBase
    {
        public static string defaultTooltipSpawnKey;

        [Tooltip("是否跟随交互的碰撞点 只支持Focus")]
        public bool followMarkerPoint = true;

        protected ISpawnAbleTooltip tooltip;

        private VRTK_DestinationMarker targetMarker;

        /// <summary>
        /// 自定义目标
        /// </summary>
        [Tooltip("自定义目标 可空")]
        public Transform customOrigin;

        [Tooltip("位置偏移 世界坐标系 后期待改成基于观察位置偏移")]
        public Vector3 AdjuestPosOffset;
         
        public string customTooltipSpawnKey;

        protected override void OnObjFocusEnter(object sender, InteractableObjectEventArgs e)
        {
            if (followMarkerPoint && customOrigin == null && e.interactingObject != null)
            {
                if (e.interactingObject.TryGetComponent<VRTK_DestinationMarker>(out var marker))
                {
                    marker.DestinationMarkerHover -= Marker_DestinationMarkerHover;
                    marker.DestinationMarkerHover += Marker_DestinationMarkerHover;

                    targetMarker = marker;
                }
            }

            base.OnObjFocusEnter(sender, e);
        }

        private void Marker_DestinationMarkerHover(object sender, DestinationMarkerEventArgs e)
        {
            if (tooltip != null)
            {
                tooltip.FollowPos = e.raycastHit.point;
            }
        }

        public override void RefTipMsg()
        {
            base.RefTipMsg();

            if(tooltip as Component != null)
                tooltip.TextTip = TipMsg;
        }

        public override void ShowTooltip()
        {
            base.ShowTooltip();

            if (tooltip == null)
            {
                tooltip = SpawnPoolMgr.Inst.Spawn(!string.IsNullOrEmpty(customTooltipSpawnKey) ? customTooltipSpawnKey : defaultTooltipSpawnKey) as ISpawnAbleTooltip;

                if (tooltip as Component != null)
                {
                    tooltip.FollowTarget = customOrigin == null ? transform : customOrigin;

                    tooltip.AdjuestPosOffset = AdjuestPosOffset;
                     
                    tooltip.TextTip = TipMsg;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("tooltip 为空:" + customTooltipSpawnKey);
#endif
                }
            }
        }

        public override void HideTooltip()
        {
            base.HideTooltip();

            if (targetMarker != null)
            {
                targetMarker.DestinationMarkerHover -= Marker_DestinationMarkerHover;

                targetMarker = null;
            }

            if ((tooltip as Component) == null)
                return;
             
            SpawnPoolMgr.Inst.Despawn(tooltip);

            tooltip = null;
        }
    }
}