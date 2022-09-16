using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 将多个可交互物体分组，同组的可交互物体会同步高亮与FocusSet事件
    /// 暂时不可用
    /// 
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class InteractObjGroup : MonoBehaviour
    {
        public AdvancedInteractableObj[] interactObjs;

        private void Awake()
        {
            foreach (var obj in interactObjs)
            {
                obj.InteractableObjectFocusSet += InteractObjGroup_InteractableObjectFocusSet;
            }
        }

        private void OnDestroy()
        {
            foreach (var obj in interactObjs)
            {
                if(obj != null)
                    obj.InteractableObjectFocusSet -= InteractObjGroup_InteractableObjectFocusSet;
            }
        }

        private void MonitorInteractObj_InteractableObjectFocusEnter(object sender, InteractableObjectEventArgs e)
        {
            
        }

        private void InteractObjGroup_InteractableObjectFocusSet(object sender, InteractableObjectEventArgs e)
        {
            if (e.interactingObject == gameObject)
                return;

            Debug.Log("OnInteractableObjectFocusSet  obj group:" + sender);

            var senderInteractObj = sender as AdvancedInteractableObj;

            foreach (var obj in interactObjs)
            {
                if (senderInteractObj == obj)
                    continue;

                obj.OnInteractableObjectFocusSet(new InteractableObjectEventArgs() { interactingObject = gameObject });
            }
        }
    }
}
