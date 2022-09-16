using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fxb.CMSVR
{
    public class DASceneState
    {
        /// <summary>
        /// 放置在桌上的克隆物体。每种只保留一个
        /// PropId -> clone obj id
        /// </summary>
        public Dictionary<string, string> cloneObjsInTable = new Dictionary<string, string>();

        public bool isGuiding;

        /// <summary>
        /// 用于初始化的任务ID
        /// </summary>
        public string taskID2Init;

        /// <summary>
        /// 实训/考核
        /// </summary>
        public DaTaskMode taskMode;

        /// <summary>
        /// 指引准备中
        /// </summary>
        public bool isTaskPreparing = true;

        public BatteryLiftDeviceState batteryLiftDeviceState;

       public List<string> taskIDGroupingModeInit;

        public bool isStructureAniCompleted;
    }
}
