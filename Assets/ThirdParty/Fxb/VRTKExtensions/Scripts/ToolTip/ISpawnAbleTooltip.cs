
using Fxb.SpawnPool;
using UnityEngine;

namespace VRTKExtensions
{
    public interface ISpawnAbleTooltip : ISpawnAble 
    {
        /// <summary>
        /// FollowTarget 与 FollowPos二选一
        /// </summary>
        Transform FollowTarget {set;}

        Vector3? FollowPos { set; }
         
        string TextTip {set;}

        /// <summary>
        /// 坐标偏移  世界空间。
        /// </summary>
        Vector3 AdjuestPosOffset { set; }
    }
}