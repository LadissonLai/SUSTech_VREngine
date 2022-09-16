using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.DA
{
    [DisallowMultipleComponent]
    public class DisappearEffectScript : MonoBehaviour
    {
        [Tooltip("移动的距离 为0则表示不移动")]
        public float moveDistancs = 0.05f;

        [Tooltip("匹配的自义定物体方向，如果为空 则参考自身的方向")]
        public Transform customDirMatchObj;

        [Tooltip("是否使用forawrd方向 如果为false则使用forward方向")]
        public bool useUpwardDir = true;

        [Tooltip("移动时间")]
        public float moveDuration = 0.3f;
         
        [Tooltip("消失时移动完之后的延迟时间和显示时移动前的延迟时间")]
        public float moveDelay = 0.2f;

        private Vector3 initPosCache;

        public virtual IEnumerator DisappearWithAnim()
        {
            if (moveDistancs > 0)
            {
                initPosCache = transform.localPosition;

                var matchObj = customDirMatchObj != null ? customDirMatchObj : transform;

                var targetPos = transform.position + (useUpwardDir ? matchObj.up : matchObj.forward) * moveDistancs;

                yield return transform.DOMove(targetPos, moveDuration, false).WaitForCompletion(false);

                if (moveDelay > 0)
                    yield return new WaitForSeconds(moveDelay);
            }

            //TODO 物体消失效果
            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="overrideDelayToMove">增加外部可以改写的延迟时间，方便上层控制多个物体的出现动画效果</param>
        /// <returns></returns>
        public virtual IEnumerator AppearWithAnim(float overrideDelayToMove = 0)
        {
            yield return null;

            if(moveDistancs > 0)
            {
                //TODO 物体显示效果
                if(overrideDelayToMove > 0)
                    yield return new WaitForSeconds(overrideDelayToMove);
                else if (moveDelay > 0)
                    yield return new WaitForSeconds(moveDelay);

                yield return transform.DOLocalMove(initPosCache, moveDuration, false).WaitForCompletion(false);
            }
        }
    }
}
