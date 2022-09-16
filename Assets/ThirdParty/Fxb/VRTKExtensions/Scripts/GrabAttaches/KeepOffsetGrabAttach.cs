using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.GrabAttachMechanics;

namespace VRTKExtensions
{
    public class KeepOffsetGrabAttach : VRTK_ChildOfControllerGrabAttach
    {
        public static float defaultOffsetDistance;

        public float customOffsetDistance;

        protected override void SnapObjectToGrabToController(GameObject obj)
        {
            base.SnapObjectToGrabToController(obj);
            
            obj.transform.localPosition += Vector3.forward * (customOffsetDistance > 0.0f ? customOffsetDistance : defaultOffsetDistance);
        }
    }
}
