using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 场景脚本 可以用来处理对应场景中的交互逻辑，自动拉起world prefab。 场景切换时销毁
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-99)]
    public class SceneScript : MonoBehaviour
    {
        public GameObject worldPrefab;

        protected virtual void Awake()
        {
            DebugEx.AssertIsTrue(CheckSingletonScriptInscene(), "场景中找到多个 SceneScript. {0}", gameObject.name);

            if (FindObjectsOfType<World>().Length == 0)
            {
                DebugEx.AssertNotNull(worldPrefab, "world prefab 为空");

                Instantiate(worldPrefab);
            }
        }

        protected bool CheckSingletonScriptInscene()
        {
#if UNITY_EDITOR
            return FindObjectsOfType<SceneScript>().Length == 1;
#else
            return true;
#endif
        }

    }
}