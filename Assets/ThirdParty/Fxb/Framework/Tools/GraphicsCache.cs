using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Tools
{
    /// <summary>
    /// 缓存当前mesh renderer的材质，默认忽略隐藏物体
    /// </summary>
    public class GraphicsCache : MonoBehaviour
    {
        private Dictionary<Renderer, Material[]> materialsCache;

        private List<GraphicsCache> childs;

        public Renderer[] renderersCache;

        public Renderer[] rendersUnCache;

        public Material CustomSharedMaterial { get; private set; }

        void Start()
        {
            if (materialsCache == null)
                CacheMaterial();

            if (CustomSharedMaterial != null)
                SwapGraphicsSharedMats(CustomSharedMaterial, true);

            var parent = transform.FindComponentInParents<GraphicsCache>(10);

            if (parent != null)
                parent.RegistChild(this);
        }

        //目前逻辑   如果切换了材质然后结构更改，不会自动还原。 不确定是否需要此逻辑。
        void OnTransformParentChanged()
        {
            var parent = transform.FindComponentInParents<GraphicsCache>(10);

            if (parent != null)
            {
                parent.RegistChild(this);
            }
        }

        private bool IsInChildGraphics(Renderer r)
        {
            if (childs == null)
                return false;

            foreach (var c in childs)
            {
                if (r.transform.IsChildOf(c.transform))
                    return true;
            }

            return false;
        }

        private void CacheMaterial()
        {
            var renderers = renderersCache;

            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>(false);

            materialsCache = new Dictionary<Renderer, Material[]>();

            //剔除自定义显示
            HashSet<Renderer> unCaches = rendersUnCache?.Length > 0 ? new HashSet<Renderer>(rendersUnCache) : null;

            foreach (var r in renderers)
            {
                if (unCaches != null && unCaches.Contains(r))
                    continue;

                //嵌套
                if (IsInChildGraphics(r))
                    continue;

                materialsCache.Add(r, r.sharedMaterials);
            }

#if UNITY_EDITOR
            if (materialsCache.Count > 50)
                Debug.LogWarning($"mesh renderer数量过多:{materialsCache.Count}  name:{name}");
#endif
        }

        void RegistChild(GraphicsCache c)
        {
            childs = childs ?? new List<GraphicsCache>();

            childs.AddUnique(c);
        }

        void UnRegistChild(GraphicsCache c)
        {
            if (childs == null)
                return;

            childs.Remove(c);
        }

        public void SwapGraphicsSharedMats(Material mat, bool forceSwapMats = false, bool ignoreChilds = true)
        {
            if (materialsCache == null)
                CacheMaterial();

            if (!forceSwapMats && mat == CustomSharedMaterial)
                return;

            CustomSharedMaterial = mat;

            foreach (var kv in materialsCache)
            {
                kv.Key.ReplaceSharedMats(mat);
            }

            if (!ignoreChilds && childs?.Count > 0)
            {
                foreach (var cG in childs)
                {
                    cG.SwapGraphicsSharedMats(mat, forceSwapMats);
                }
            }
        }

        public void OriginalSharedMats()
        {
            foreach (var kv in materialsCache)
            {
                kv.Key.sharedMaterials = kv.Value;
            }

            if (childs?.Count > 0)
            {
                foreach (var cG in childs)
                {
                    if (CustomSharedMaterial == CustomSharedMaterial)
                        cG.OriginalSharedMats();
                }
            }

            CustomSharedMaterial = null;
        }
    }
}