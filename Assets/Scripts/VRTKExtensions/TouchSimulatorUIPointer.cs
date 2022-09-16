using System;

using UnityEngine;
using VRTK;

/// <summary>
/// 适配模拟触摸操作。
/// 遗留问题：2.按钮的复层有可交互的背景时，按钮有可能收不到hover状态通知。
/// </summary>
public class TouchSimulatorUIPointer : VRTK_UIPointer
{
    /// <summary>
    /// 震动
    /// </summary>
    public float hapticStrength = 0.3f;

    private VRTK_Pointer pointer;

    private VRTK_InteractGrab grab;

    private bool lastCollisionClick;
     
    protected override void OnEnable()
    {
        base.OnEnable();

        pointer = GetComponent<VRTK_Pointer>();

        grab = GetComponent<VRTK_InteractGrab>();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if(lastCollisionClick != collisionClick)
        {
            lastCollisionClick = collisionClick;

            if(collisionClick && hapticStrength > 0)
            {
                //触摸后震动提示
                VRTK_ControllerHaptics.TriggerHapticPulse(GetControllerReference(), hapticStrength);
            }
        }
    }

    public override bool ValidClick(bool checkLastClick, bool lastClickState = false)
    {
        if (grab?.GetGrabbedObject() != null)
            return false;
 
        return base.ValidClick(checkLastClick, lastClickState);
    }

    /// <summary>
    /// 触碰到ui后开启pressed
    /// </summary>
    /// <returns></returns>
    public override bool IsSelectionButtonPressed()
    {
        return collisionClick;
    }
     
    /// <summary>
    /// 激活canvas后激活射线
    /// </summary>
    /// <returns></returns>
    public override bool PointerActive()
    {
        var hasGrab = grab?.GetGrabbedObject() != null;

        if (hasGrab)
            return false;
         
        return autoActivatingCanvas != null;
    }
     
    //射线起点往后挪，防止手指插入平板后失去焦点
    public override Vector3 GetOriginPosition()
    {
        if (customOrigin == null || !autoActivatingCanvas)
            return base.GetOriginPosition();

        var ajuestOrigin = customOrigin.position - autoActivatingCanvas.transform.forward * 0.05f;
         
        return ajuestOrigin;
    }

    /// <summary>
    /// 方向直接指向canvas
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetOriginForward()
    {
        if(customOrigin == null || !autoActivatingCanvas)
            return base.GetOriginForward();

        //canvas的ui方向为-z
        return autoActivatingCanvas.transform.forward;
    }
}
