using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.DA
{
    /// <summary>
    /// 简单移动物体的拆装动画  弃用
    /// </summary>
    public class MovementDAScript : AbstractDAScript
    {
        public override bool IsAnimSuccess => true;

        public override bool AutoFix { get => true; }

        [Tooltip("移动的距离")]
        public float moveDistancs = 0.05f;

        [Tooltip("匹配的自义定物体方向，如果为空 则参考自身的方向")]
        public Transform customDirMatchObj;

        [Tooltip("是否使用forawrd方向 如果为false则使用forward方向")]
        public bool useUpwardDir = true;

        public float duration = 0.3f;

        [Tooltip("安装动画开始前及拆下动画完成后的延迟")]
        public float delay = 0.2f;

        private Vector3 initPosCache;
         
        public override IEnumerator PlayAssembleAnim(IDAUsingTool usingTool = null)
        {
            yield return new WaitForSeconds(delay);
            yield return transform.DOLocalMove(initPosCache, duration, false).WaitForCompletion(false);
        }

        public override IEnumerator PlayDisassembleAnim(IDAUsingTool usingTool = null)
        {
            initPosCache = transform.localPosition;

            var matchObj = customDirMatchObj != null ? customDirMatchObj : transform;

            var targetPos = transform.position + (useUpwardDir ? matchObj.up : matchObj.forward) * moveDistancs;

            yield return transform.DOMove(targetPos, duration, false).WaitForCompletion(false);

            yield return new WaitForSeconds(delay);
        }

        public override IEnumerator PlayFixAnim(IDAUsingTool usingTool = null)
        {
            yield break;
        }
    }
}
