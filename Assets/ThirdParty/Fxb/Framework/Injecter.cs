using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Injecter
    {
        public sealed class InstContainer<T> where T : class
        {
            private Dictionary<string, T> keyMap;

            private T defaultInst;

            public void Set(T inst, string customKey = null)
            {
                DebugEx.AssertIsTrue(customKey != string.Empty);

                var instReplaced = false;

                if(customKey == null)
                {
                    instReplaced = defaultInst != null;

                    defaultInst = inst;
                }
                else
                {
                    keyMap = keyMap ?? new Dictionary<string, T>();

                    instReplaced = !keyMap.AddUnique(customKey, inst);
                }

                if (instReplaced)
                    Debug.LogError($"注入内容被覆盖:{inst}-{customKey}");
            }

            public T Get(string customKey = null)
            {
                DebugEx.AssertIsTrue(customKey != string.Empty);

                return customKey == null ? defaultInst : keyMap.GetSafely(customKey);
            }

            public void GetAll(List<T> results)
            {
                if (defaultInst != null)
                    results.Add(defaultInst);

                results.AddRange(keyMap.Values);
            }

            /// <summary>
            /// 清理
            /// </summary>
            /// <param name="customKey"></param>
            /// <returns>if empty</returns>
            public bool Remove(string customKey = null)
            {
                if(customKey == null)
                    defaultInst = null;
                else if(keyMap != null)
                    keyMap.Remove(customKey);

                return defaultInst == null && keyMap?.Count == 0;
            }
        }//InstContainer end


        private Dictionary<Type, object> map;

        public Injecter()
        {
            map = new Dictionary<Type, object>();
        }

        public virtual T Regist<T>(string customKey = null) where T : class, new()
        {
            var inst = new T();

            return Regist(inst, customKey);
        }

        public virtual T Regist<T>(T inst , string customKey = null) where T : class
        {
            Debug.Assert(inst != null);

            var type = typeof(T);

            if(!map.TryGetValue(type, out var instContainer))
            {
                instContainer = new InstContainer<T>();

                map.Add(type,instContainer);
            }

            (instContainer as InstContainer<T>).Set(inst, customKey);
 
            return inst;
        }

        public virtual bool RegistAsList<T>(T inst) where T : class
        {
            var exist = Get<List<T>>();

            if (exist == null)
                exist = Regist<List<T>>();

            return exist.AddUnique(inst);
        }

        public virtual void UnRegist<T>(string customKey = null) where T : class 
        {
            var type = typeof(T);
 
            if(!map.TryGetValue(type, out var instContainer))
                return;

            if((instContainer as InstContainer<T>).Remove(customKey))
                map.Remove(type);
        }
 
        public virtual void UnRegistFromList<T>() where T : class
        {
            UnRegist<List<T>>();
        }

        public virtual void UnRegistFromList<T>(T inst) where T : class
        {
            var exist = Get<List<T>>();

            if (exist == null)
                return;

            exist.Remove(inst);

            if (exist.Count == 0)
                UnRegistFromList<T>();
        }

        public virtual T Get<T>(string customKey = null) where T : class
        {
            var type = typeof(T);

            if(!map.TryGetValue(type, out var instContainer))
                return null;

            return (instContainer as InstContainer<T>).Get(customKey) as T;
        }

        public virtual void GetAll<T>(List<T> results) where T : class
        {
            var type = typeof(T);

            if (!map.TryGetValue(type, out var instContainer))
                return;

            (instContainer as InstContainer<T>).GetAll(results);
        }

        public virtual void Clear()
        {
            map.Clear();
        }
    }
}