
namespace Framework.Tools
{
    using System;
    using System.Collections.Generic;
 
    /// <summary>
    /// 简单obj对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : class, new()
    {
        #region Member variables
        
        private Queue<T> objQueue;

        private Action<T> OnSpawn;

        private Action<T> OnDespawn;
 
        #endregion

        #region Public Method
       
        public ObjectPool(int capacity, Action<T> spawnHandle = null, Action<T> despawnHandle = null)
        {
            objQueue = new Queue<T>(capacity);

            OnSpawn = spawnHandle;

            OnDespawn = despawnHandle;
        }
       
        public T Spawn()
        {
            T o = objQueue.Count > 0 ? objQueue.Dequeue() :  new T();

            OnSpawn?.Invoke(o);

            return o;
        }
      
        public void Despawn(T o)
        {

#if UNITY_EDITOR
            DebugEx.AssertIsTrue(!objQueue.Contains(o));
#endif
            objQueue.Enqueue(o);

            OnDespawn?.Invoke(o);
        }

        public void Dispose()
        {
            objQueue.Clear();

            OnSpawn = OnDespawn = null;
        }
      
        #endregion

    }
}
