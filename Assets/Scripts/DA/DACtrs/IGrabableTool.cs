using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 待完善  
    /// 1.标志此物体是一个手持类工具。
    /// 2.可以外部允许抓取后通过圆盘等按键调整参数。（需要参数指定是否可以调整）
    /// </summary>
    public interface IGrabableTool
    {
        /// <summary>
        /// 目前不是TouchpadPress都是无效的
        /// </summary>
        /// <param name="modifyBtn"></param>
        /// <returns></returns>
        bool CheckModifyParamValid(VRTK_ControllerEvents.ButtonAlias modifyBtn = VRTK_ControllerEvents.ButtonAlias.TouchpadPress);

        /// <summary>
        /// 按下刷新alis
        /// </summary>
        /// <param name="alis"></param>
        /// <returns></returns>
        bool UpdateAlis(Vector2 alis);

        /// <summary>
        /// 按键按下与抬起
        /// </summary>
        /// <param name="isPressed"></param>
        /// <param name="btnAlis"></param>
        void PressModifyBtn(bool isPressed, Vector2 btnAlis);
    }
}
 