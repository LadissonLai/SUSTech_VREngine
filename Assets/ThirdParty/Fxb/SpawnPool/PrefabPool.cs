using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.SpawnPool
{
    public class PrefabPool
    {
        /// <summary>
        /// 可以使用的内容
        /// </summary>
        private List<Transform> despawnedPools;
        /// <summary>
        /// 对应的prefab。 
        /// 目前逻辑是在舞台上存在的一个prefab，考虑使用一个AB中或者Resouce中的一个Assets来作为此prefab
        /// </summary>
        public Transform prefab;

#if UNITY_EDITOR
        private int insNameIndex = 0;
#endif

        public int DespawnedCount
        {
            get
            {
                return despawnedPools.Count;
            }
        }

        public PrefabPool(Transform prefab_)
        {
            prefab = prefab_;

            despawnedPools = new List<Transform>();
        }

        public Transform Spawn(Transform newParent)
        {
            Transform ins = null;

            if (despawnedPools.Count == 0)
            {
                //用完
                ins = CreateInstance(newParent);
            }
            else
            {
                ins = despawnedPools[0];

                if (ins.parent != newParent)
                    ins.SetParent(newParent);
            }

            despawnedPools.RemoveAt(0);

            return ins;
        }

        public void Despawn(Transform ins)
        {
            if (despawnedPools.Contains(ins))
            {
                return;
            }

            if (prefab)
                ins.SetParent(prefab.parent, false);

            despawnedPools.Add(ins);
        }

        /// <summary>
        /// 创建可以使用的池中对象
        /// </summary>
        public Transform CreateInstance(Transform newParent)
        {
            GameObject ins = GameObject.Instantiate(prefab.gameObject, newParent);

            despawnedPools.Add(ins.transform);

#if UNITY_EDITOR
            ins.name = $"{prefab.name}#ins{insNameIndex++}";
#endif

            return ins.transform;
        }

        public IEnumerator CreateInstance(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateInstance(prefab.parent);

                yield return null;
            }
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            insNameIndex = 0;
#endif

            despawnedPools.ForEach((ins) =>
            {
                if (ins != null)
                    GameObject.Destroy(ins.gameObject);
            });

            if (prefab != null)
            {
                GameObject.Destroy(prefab.gameObject);
            }

            despawnedPools = null;
        }
    }
}
