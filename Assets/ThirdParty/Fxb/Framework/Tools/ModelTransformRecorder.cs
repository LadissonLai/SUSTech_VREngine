using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Tools
{
    public struct TransformCache
    {
        public Vector3 postion;

        public Vector3 localScale;

        public Quaternion rotation;

        public Transform parent;
    }

    /// <summary>
    /// 缓存transform各项原始数据
    /// </summary>
    public class ModelTransformRecorder : MonoBehaviour
    {
        protected LinkedList<TransformCache> records;
         
        public TransformCache PreRecored => records.Last.Value;

        public Vector3 PreLocalPostion => PreRecored.postion;

        public Vector3 PreLocalScale => PreRecored.localScale;

        public Quaternion PreLocalRotation => PreRecored.rotation;

        public Transform PreParent => PreRecored.parent;

        private void Awake()
        {
            records = new LinkedList<TransformCache>();
        }

        /// <summary>
        /// 增加一项当前transform的状态记录
        /// </summary>
        public virtual void Record()
        {
            var cache = new TransformCache()
            {
                parent = transform.parent,
                postion = transform.localPosition,
                rotation = transform.localRotation,
                localScale = transform.localScale,
            };

            records.AddLast(cache);
        }

        /// <summary>
        /// 回到最近一次记录的内容
        /// </summary>
        /// <param name="autoLose"></param>
        public virtual void Back(bool autoLose = false)
        {
            if (records.Count == 0)
                return;

            TransReset(records.Last.Value);

            if (autoLose)
                records.RemoveLast();
        }

        public virtual void Entry()
        {
            if (records.Count == 0)
                return;

            TransReset(records.First.Value);
        }

        /// <summary>
        /// 直接通过缓存的数据reset
        /// </summary>
        /// <param name="cache"></param>
        public virtual void TransReset(TransformCache cache)
        {
            transform.SetParent(cache.parent);

            transform.localPosition = cache.postion;

            transform.localRotation = cache.rotation;

            transform.localScale = cache.localScale;
        }

        /// <summary>
        /// 丢弃最近一次记录的内容
        /// </summary>
        /// <param name="loseAllRecords">是否丢弃所有记录</param>
        public virtual void Lose(bool loseAllRecords = false)
        {
            if (records.Count == 0)
                return;

            if (loseAllRecords)
                records.Clear();
            else
                records.RemoveLast();
        }
    }
}