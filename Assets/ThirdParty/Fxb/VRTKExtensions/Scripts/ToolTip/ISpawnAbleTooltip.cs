
using Fxb.SpawnPool;
using UnityEngine;

namespace VRTKExtensions
{
    public interface ISpawnAbleTooltip : ISpawnAble 
    {
        /// <summary>
        /// FollowTarget �� FollowPos��ѡһ
        /// </summary>
        Transform FollowTarget {set;}

        Vector3? FollowPos { set; }
         
        string TextTip {set;}

        /// <summary>
        /// ����ƫ��  ����ռ䡣
        /// </summary>
        Vector3 AdjuestPosOffset { set; }
    }
}