using UnityEngine;

namespace Fxb.Localization
{
    /// <summary>
    /// Mesh相关多语言脚本  允许自动替换网格，材质等
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshRendererLocalize : LocalizeTargetBase
    {
        [System.Serializable]
        public class LocalizeAssetsMat : LocalizeAssets<Material>
        {
            public Mesh customMeshReference;
        }

        public MeshRenderer meshRenderer;

        [Tooltip("材质索引，只替换一个材质")]
        public int materialIndex;

        [Tooltip("是否使用共享材质，默认true")]
        public bool useSharedMaterial = true;

        public LocalizeAssetsMat[] localizedAssets;

        protected MeshFilter meshFilter;

        private Material srcRendererMat;

        private Mesh srcMesh;

        protected override void OnLocalize()
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();

            if(srcRendererMat == null)
                srcRendererMat = useSharedMaterial ? meshRenderer.sharedMaterials[materialIndex] : meshRenderer.materials[materialIndex];

            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            if (srcMesh == null)
                srcMesh = meshFilter.sharedMesh;

            foreach (var lAssets in localizedAssets)
            {
                if (LocalizeMgr.Inst.CurrentLanguage == lAssets.language)
                {
                    SetRendererMesh(lAssets.customMeshReference);
                    SetRendererMaterial(lAssets.assetsReference);

                    return;
                }
            }

            SetRendererMaterial(srcRendererMat);
            SetRendererMesh(srcMesh);
        }

        protected void SetRendererMesh(Mesh mesh)
        {
            if (null == mesh)
                return;
 
            meshFilter.sharedMesh = mesh;
        }

        protected void SetRendererMaterial(Material mat)
        {
            Material[] mats = null;

            if(useSharedMaterial)
            {
                mats = meshRenderer.sharedMaterials;

                mats[materialIndex] = mat;

                meshRenderer.sharedMaterials = mats;
            }
            else
            {
                mats = meshRenderer.materials;

                mats[materialIndex] = mat;

                meshRenderer.materials = mats;
            }
        }
    }
}
