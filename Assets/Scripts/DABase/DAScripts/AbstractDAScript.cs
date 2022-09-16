using System;
using System.Collections;
using UnityEngine;

namespace Fxb.DA
{
    public abstract class AbstractDAScript : MonoBehaviour
    {
        public enum DAAnimType
        {
            Disassemble,
            Assemble,
            Fix
        }

        /// <summary>
        /// 对应的拆装物体id
        /// </summary>
        [NonSerialized]
        public string daObjID;

        public abstract IEnumerator PlayDisassembleAnim(IDAUsingTool usingTool = null);
         
        public abstract IEnumerator PlayAssembleAnim(IDAUsingTool usingTool = null);

        public abstract IEnumerator PlayFixAnim(IDAUsingTool usingTool = null);
 
        public virtual bool CheckDisassembleCondition(IDAUsingTool usingTool = null) => true;

        public virtual bool CheckAssembleCondition(IDAUsingTool usingTool = null) => true;

        public virtual bool CheckFixCondition(IDAUsingTool usingTool = null) => true;

        public virtual bool AutoFix {get;}
 
        public virtual bool IsAnimSuccess { get; protected set; } = false;
    }
}

