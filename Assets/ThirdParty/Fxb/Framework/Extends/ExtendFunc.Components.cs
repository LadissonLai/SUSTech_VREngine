using System;

using UnityEngine;

using System.Collections.Generic;

namespace Framework
{
    public static partial class ExtendFunc
    {
        public static bool TryGetComponentInParent<T>(this Component c1, out T c2)
        {
            c2 = c1.GetComponentInParent<T>();

            return c2 != null;
        }

        /// <summary>
        /// 添加缺少的脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="component"></param>
        /// <returns>是否新添加</returns>
        public static bool AddMissingComponent<T>(this GameObject go, out T component) where T : Component
        {
            component = go.GetComponent<T>();

            var newComponent = false;

            if (component == null)
            {
                component = go.AddComponent<T>();

                newComponent = true;
            }

            return newComponent;
        }

        /// <summary>
        /// 添加缺少的脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T AddMissingComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();

            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        /// <summary>
        /// 添加缺少的脚本
        /// </summary>
        /// <param name="go"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Component AddMissingComponent(this GameObject go, Type type)
        {
            var comp = go.GetComponent(type);

            if (comp == null)
            {
                comp = go.AddComponent(type);
            }

            return comp;
        }

        /// <summary>
        /// 删除一个存在的脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public static void DestroyComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return;

            T comp = go.GetComponent<T>();

            if (comp != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(comp);
                else
                    UnityEngine.Object.DestroyImmediate(comp);
            }
        }

        /// <summary>
        /// 删除自身所有指定类型的脚本
        /// </summary>
        /// <param name="selfCom"></param>
        /// <param name="comTypes"></param>
        public static void DestoryComponents(this Component selfCom, params Type[] comTypes)
        {
            var coms = new List<Component>();

            foreach (Type type in comTypes)
            {
                coms.AddRange(selfCom.GetComponents(type));
            }

            foreach (var c in coms)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(c);
                else
                    UnityEngine.Object.DestroyImmediate(c);
            }
        }

        /// <summary>
        /// 删除child所有指定类型的脚本
        /// </summary>
        /// <param name="selfCom"></param>
        /// <param name="comTypes"></param>
        public static void DestoryComponentsInChildren(this Component selfCom, params Type[] comTypes)
        {
            var coms = new List<Component>();

            foreach (Type type in comTypes)
            {
                coms.AddRange(selfCom.GetComponentsInChildren(type));
            }

            foreach (var c in coms)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(c);
                else
                    UnityEngine.Object.DestroyImmediate(c);
            }
        }

        /// <summary>
        /// 重置本地矩阵
        /// </summary>
        /// <param name="trans"></param>
        public static void ResetLocalMatrix(this Transform trans)
        {
            trans.localPosition = Vector3.zero;

            trans.localRotation = Quaternion.identity;

            trans.localScale = Vector3.one;
        }

        /// <summary>
        /// 父层搜索脚本. 会忽略自身。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T FindComponentInParents<T>(this Component c, int depth = 5) where T : Component
        {
            if (depth-- <= 0)
                return null;

            var pT = c.transform.parent;

            if (pT == null)
                return null;

            if (pT.TryGetComponent<T>(out var rC))
            {
                return rC;
            }

            return FindComponentInParents<T>(c.transform.parent, depth);
        }

        /// <summary>
        /// 通过子物体路径搜索脚本
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path">子物体相对路径</param>
        /// <param name="type"></param>
        /// <param name="ignoreError">如果为false，寻找失败时log error</param>
        /// <returns></returns>
        public static Component Find(this Transform t, string path, Type type, bool ignoreError = false)
        {
            var target = t.Find(path);

            if (target != null)
            {
                var component = target.GetComponent(type);

                if (component != null)
                {
                    return component;
                }
            }

            if (!ignoreError)
            {
                DebugEx.Error("{0}-{1} not find", t, path);
            }

            return null;
        }

        /// <summary>
        /// 通过子物体路径搜索脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <param name="path">子物体相对路径</param>
        /// <param name="ignoreError">如果为false，寻找失败时log error</param>
        /// <returns></returns>
        public static T Find<T>(this Transform trans, string path, bool ignoreError = false) where T : Component
        {
            var target = trans.Find(path);

            if (target != null)
            {
                var component = target.GetComponent<T>();

                if (component != null)
                {
                    return component;
                }
            }

            if (!ignoreError)
            {
                DebugEx.Error("{0}-{1} not find", trans, path);
            }

            return null;
        }

        public static void DestroyChildren(this Transform trans, int startIndex = 0)
        {
            while (trans.childCount > startIndex)
            {
                var c = trans.GetChild(startIndex);

                if (Application.isPlaying)
                    GameObject.DestroyImmediate(c.gameObject);
                else
                    GameObject.Destroy(c.gameObject);
            }
        }

        public static void ForEach(this Transform trans, Action<Transform> action, int depth)
        {
            depth--;

            if (depth < 0)
                return;

            for (int i = 0; i < trans.childCount; i++)
            {
                var c = trans.GetChild(i);

                action.Invoke(c);

                c.ForEach(action, depth);
            }
        }


        //ForEach
        public static void ForEach(this Transform trans, Predicate<Transform> match, Action<Transform> action, int depth)
        {
            depth--;

            if (depth < 0)
                return;

            for (int i = 0; i < trans.childCount; i++)
            {
                var c = trans.GetChild(i);

                if (match.Invoke(c))
                    action?.Invoke(c);
                else
                    c.ForEach(match, action, depth);
            }
        }

        public static void ApplyLayer(this GameObject rootGO, string layerName)
        {
            rootGO.ApplyLayer(LayerMask.NameToLayer(layerName));
        }

        public static void ApplyLayer(this GameObject rootGO, int newLayer)
        {
            rootGO.layer = newLayer;

            for (int i = 0, len = rootGO.transform.childCount; i < len; i++)
            {
                var cGO = rootGO.transform.GetChild(i).gameObject;

                cGO.ApplyLayer(newLayer);
            }
        }

        public static Material[] ReplaceSharedMats(this Renderer renderer, Material mat, Material[] givenMaterials = null)
        {
            var matAmount = renderer.sharedMaterials.Length;

            if (matAmount == 0)
                return null;

            var oldMats = renderer.sharedMaterials;

            givenMaterials = givenMaterials ?? new Material[matAmount];

            for (int i = 0; i < givenMaterials.Length; i++)
            {
                givenMaterials[i] = mat;
            }

            renderer.sharedMaterials = givenMaterials;

            return oldMats;
        }
        
        /// <summary>
        /// 以脚本为基础，设置碰撞体的isTrigger属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behaviour"></param>
        /// <param name="triggerState"></param>
        /// <param name="filterDirectChild"></param>
        public static void SetChildCollisionTrigger<T>(this T behaviour, bool triggerState, bool filterDirectChild = true) where T : MonoBehaviour
        {
            var allColliders = behaviour.GetComponentsInChildren<Collider>(false);

            foreach (var c in allColliders)
            {
                //有嵌套的子节点
                if (filterDirectChild && c.GetComponentInParent<T>() != behaviour)
                    continue;

                if (c is MeshCollider meshCollider)
                {
                    if (meshCollider.convex)
                    {
                        meshCollider.isTrigger = triggerState;
                    }
#if UNITY_EDITOR
                    else if(!triggerState)
                    {
                        Debug.LogWarning("非凸面网格碰撞体不能开启 trigger  " + c.name);
                    }
#endif                    
                    continue;
                }
                    
                c.isTrigger = triggerState;
            }
        }
        
        /// <summary>
        /// 注意：获取不到被关闭的collider
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="results"></param>
        public static void GetChildCollisions(this Rigidbody rigidbody,ref List<Collider> results)
        {
            if (results == null)
                results = new List<Collider>();

            rigidbody.GetComponentsInChildren(results);

            for (int i = results.Count - 1; i >= 0; i--)
            {
                var c = results[i];

                if (c.attachedRigidbody != rigidbody)
                    results.RemoveAt(i);
            }
        }

        /// <summary>
        /// 以脚本为基础，关闭下方所有碰撞体。 
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="isEnable"></param>
        /// <param name="filterDirectChild">是否筛选出嵌套节点并返回</param>
        /// <typeparam name="T"></typeparam>
        public static void SetChildCollisionEnable<T>
            (this T behaviour, bool isEnable, bool filterDirectChild = true) 
            where T : MonoBehaviour
        {
            var allColliders = behaviour.GetComponentsInChildren<Collider>();

            foreach (var c in allColliders)
            {
                //有嵌套的子节点
                if (filterDirectChild && c.GetComponentInParent<T>() != behaviour)
                    continue;
                
                c.enabled = isEnable;
            }
        }
    }
}