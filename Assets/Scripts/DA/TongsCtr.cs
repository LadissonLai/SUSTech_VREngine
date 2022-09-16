using DG.Tweening;
using Framework;
using Fxb.DA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 钳子
    /// </summary>
    public class TongsCtr : AnimMixedToolCtr
    {
        public override IEnumerator PlayAnim(Transform processedObj, MixedToolDAScript.DAAnimType DAAnimType, float outLevel)
        {
            IsUsing = true;

            var defaultAnim = CutAnimStateInfo.shortNameHash;

            var srcLayer = gameObject.layer;

            gameObject.ApplyLayer("Default");

            //拆装物体移动时 保持工具位置同步
            transform.SetParent(processedObj.parent);

            animator.SetFloat("PartsOut", outLevel);
            animator.SetTrigger("Play");

            while (true)
            {
                yield return null;

                if (defaultAnim != CutAnimStateInfo.shortNameHash && CutAnimStateInfo.normalizedTime >= 0.999f)
                {
                    break;
                }
            }

            //卡箍结尾动画
            yield return PlayEndAnimation(processedObj, DAAnimType);

            animator.SetTrigger("Reset");

            transform.SetParent(null);

            gameObject.ApplyLayer(srcLayer);

            yield return null;

            IsUsing = false;
        }

        IEnumerator PlayEndAnimation(Transform processedObj, AbstractDAScript.DAAnimType DAAnimType)
        {
            //外部自动复位
            transform.SetParent(processedObj);

            Vector3 endPos = Vector3.zero;

            float x = 0.01f;

            Vector3 moveDir = processedObj.right;

            //使用自定义方向
            var daScript = processedObj.GetComponent<MixedToolDAScript>();

            if (daScript != null && daScript.mixedAttachedPos)
                moveDir = daScript.mixedAttachedPos.up;

            if (DAAnimType == AbstractDAScript.DAAnimType.Disassemble)
                endPos = moveDir * -x;
            else if (DAAnimType == AbstractDAScript.DAAnimType.Assemble)
                endPos = moveDir * x;

            yield return processedObj.DOBlendableMoveBy(endPos, 0.5f).WaitForCompletion();
        }
    }
}
