using Framework;
using System.Collections.Generic;

using UnityEngine;

namespace Fxb.SpawnPool
{
    public class SpawnPoolMgr : Singleton<SpawnPoolMgr> , IDispose
    {
        private static readonly string GOPOOL_ROOT = nameof(GOPOOL_ROOT);

        private static readonly string DYNAMIC_GOROOT = nameof(DYNAMIC_GOROOT);

        /// <summary>
        /// 不会销毁，存放所有对象池物体
        /// </summary>
        private Transform root;

        /// <summary>
        /// 动态节点
        /// </summary>
        private Transform dynamicRoot;

        private Dictionary<string, PrefabPool> prefabPools;
         
        public void Clear()
        {
            foreach (var item in prefabPools)
            {
                item.Value.Dispose();
            }

            prefabPools.Clear();
        }

        public void Dispose()
        {
            foreach (var item in prefabPools)
            {
                item.Value.Dispose();
            }

            prefabPools = null;

            if (root != null)
            {
                Object.Destroy(root.gameObject);
            }
        }

        public void Init()
        {
            DebugEx.AssertIsTrue(root == null);

            root = new GameObject(GOPOOL_ROOT).transform;

            GameObject.DontDestroyOnLoad(root.gameObject);
            
            prefabPools = new Dictionary<string, PrefabPool>();
        }

        public bool IsPreload(string path)
        {
            return prefabPools.ContainsKey(path);
        }

        /// <summary>
        /// 目前会根据prefab创建一个gameobj挂上脚本来作为对象池的prefab。后期看看要不要更改。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefabGO"></param>
        /// <returns></returns>
        public PrefabPool AddPool(string key, GameObject prefabAsset)
        {
            if (IsPreload(key))
                return null;

            var prefabGO = GameObject.Instantiate(prefabAsset, root);
 
            var spawnObj = prefabGO.GetComponent(typeof(ISpawnAble)) as ISpawnAble 
                ?? prefabGO.AddComponent<InActiveSpawnObj>();
              
            PrefabPool pool = new PrefabPool(prefabGO.transform);

            prefabPools.Add(key, pool);

            //待修改 直接使用asset作为源，不手动隐藏 TODO
            prefabGO.gameObject.SetActive(false);

#if UNITY_EDITOR
            prefabGO.gameObject.name = string.Concat(key, "#Prefab");
#endif

            return pool;
        }

        public bool RemovePool(string key)
        {
            if (prefabPools == null || !prefabPools.ContainsKey(key))
                return false;

            prefabPools[key].Dispose();
            
            prefabPools.Remove(key);

            return true;
        }
         
        /// <summary>
        /// 创建一个GameObject，可以自行销毁，也可以通过DesPawn返回对象池。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ISpawnAble Spawn(string key, Transform newParent = null)
        {
            if (!IsPreload(key))
                return null;

            //没有显示指定parent，统存放到dynamicRoot下
            newParent = newParent ?? (dynamicRoot = dynamicRoot ?? new GameObject(DYNAMIC_GOROOT).transform);

            Transform ins = null;

            var pool = prefabPools[key];

            ins = pool.Spawn(newParent);
              
            ins.gameObject.SetActive(true);

            ISpawnAble spawnAble = ins.GetComponent(typeof(ISpawnAble)) as ISpawnAble;

            spawnAble.Key = key;

            spawnAble.OnSpawn();
             
            return spawnAble;
        }

        /// <summary>
        /// 将一个物体返回对象池。
        /// 必须已经preload，且挂在ISpawnAble接口脚本。
        /// </summary>
        /// <param name="ins"></param>
        public bool Despawn(Transform ins)
        {
            //无挂载脚本或者key值为空表示ins不是通过对象池创建
            if (!(ins.GetComponent(typeof(ISpawnAble)) is ISpawnAble spawnAble))
                return false;
             
            return Despawn(ins, spawnAble);
        }
 
        public bool Despawn(ISpawnAble spawnAble)
        {
            var componentInst = spawnAble as Component;

            if (componentInst == null)
                return false;

            return Despawn(componentInst.transform, spawnAble);
        }
 
        public bool Despawn(Transform ins, ISpawnAble spawnAble)
        {
            if (string.IsNullOrEmpty(spawnAble.Key) || !IsPreload(spawnAble.Key))
                return false;
             
            if (ins == null)
            {
                return false;
            }

            PrefabPool pool = prefabPools[spawnAble.Key];

            spawnAble.OnDespawn();

            pool.Despawn(ins);
             
            return true;
        }
    }
}
