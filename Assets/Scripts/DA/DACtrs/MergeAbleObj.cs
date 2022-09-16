using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fxb.CMSVR
{
    public abstract class MergeAbleObj : MonoBehaviour
    {
        protected MergeAbleObj nextNodeObj;

        public int GetAmount()
        {
            if (nextNodeObj == null)
                return 1;

            //检测链表内循环的问题
            var slow = this;
            var fast = this;

            var amount = 0;

            while (slow.nextNodeObj != null)
            {
                slow = slow.nextNodeObj;

                amount++;

                if (fast.nextNodeObj != null && fast.nextNodeObj.nextNodeObj != null)
                {
                    fast = fast.nextNodeObj.nextNodeObj;

                    if (fast == slow)
                    {
                        //内循环
                        Debug.LogError("内循环");
                        return 1;
                    }
                }
            }

            return amount + 1;
        }

        public virtual void AddCloneObj(MergeAbleObj other)
        {
            if (other.transform != null)
                other.transform.SetParent(null);

            other.gameObject.SetActive(false);

            //新增物体放到最后
            var lastNodeObj = GetLastNodeObj();

            lastNodeObj.nextNodeObj = other;

            UpdateAmountState();
        }

        protected MergeAbleObj GetLastNodeObj(int loopCount = 0)
        {
            var tmpNextObj = nextNodeObj;

            if (++loopCount > 20)
            {
                Debug.LogError($"LastNodeObj Get error:{name}");

                return this;
            }

            if (nextNodeObj == null)
                return this;

            return nextNodeObj.GetLastNodeObj(loopCount);
        }

        /// <summary>
        /// next保留 自身分离
        /// </summary>
        /// <returns></returns>
        public MergeAbleObj SeparateCurrent()
        {
            var next = nextNodeObj;

            if (next == null)
                return null;

            next.transform.position = transform.position;

            next.transform.rotation = transform.rotation;

            next.gameObject.SetActive(true);

            nextNodeObj = null;

            UpdateAmountState();

            next.UpdateAmountState();

            return next;
        }

        /// <summary>
        /// 分离出nextObj，自身保留
        /// </summary>
        public MergeAbleObj SeparateNext()
        {
            var next = nextNodeObj;

            if (next == null)
                return null;

            nextNodeObj = next.nextNodeObj;

            next.nextNodeObj = null;

            next.transform.position = transform.position;

            next.transform.rotation = transform.rotation;

            next.gameObject.SetActive(true);

            UpdateAmountState();

            next.UpdateAmountState();

            return next;
        }

        protected virtual void UpdateAmountState()
        {
            //var amount = GetAmount();
            
        }
    }
}
