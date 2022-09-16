
using UnityEngine;

namespace Fxb.SpawnPool
{
    /// <summary>
    /// 默认控制显示隐藏的对象池物体
    /// </summary>
    public class InActiveSpawnObj : MonoBehaviour, ISpawnAble
    {
        public string Key { get; set; }
 
        public void OnDespawn()
        {
            gameObject.SetActive(false);
        }

        public void OnSpawn()
        {
            gameObject.SetActive(true);
        }
    }
}
