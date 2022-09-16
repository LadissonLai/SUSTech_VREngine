using Doozy.Engine;
using UnityEngine;
using VRTK;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public class AutoWearCollisionTracker : MonoBehaviour
    {
        public VRTK_BodyPhysics BodyPhysics;

        // Start is called before the first frame update
        void Start()
        {
            //简单监听 StartColliding 触发碰撞穿戴
            //StartColliding会在每个碰撞体OnTrigger事件中触发一次
            BodyPhysics.StartColliding += BodyPhysics_StartColliding;
        }

        private void BodyPhysics_StartColliding(object sender, BodyPhysicsEventArgs e)
        {
            var rigidbody = e.collider.attachedRigidbody;

            if(rigidbody == null || e.collider.isTrigger || e.collider.gameObject.layer == LayerConst.Floor)
            {
                return;
            }

            if (VRTKHelper.LeftGrab.GetGrabbedObject() != rigidbody.gameObject && VRTKHelper.RightGrab.GetGrabbedObject() != rigidbody.gameObject)
            {
                return;
            }

            //Debug.Log("BodyPhysics_StartColliding1:" + e.target);

            if (CheckWearable(rigidbody.gameObject, out var validWearableObj))
            {
                //Debug.Log("BodyPhysics_StartColliding2:" + validWearableObj);

                validWearableObj.Wear(true);
            }
        }

        private bool CheckWearable(GameObject checker, out IWearable validWearableObj)
        {
            validWearableObj = null;

            if (checker == null || !checker.TryGetComponent(out validWearableObj) || (validWearableObj as MonoBehaviour) == null)
                return false;

            return true;
        }
    }
}
