using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 视线焦点marker
    /// </summary>
    public class FocusDestinationMarker : VRTK_DestinationMarker
    {
        public float focusDistance = 1.0f;

        public VRTK_CustomRaycast customRaycast;

        public Camera customCamera;
 
        /// <summary>
        /// 是否显示碰撞相关的调试信息
        /// </summary>
        public bool showRaycastDebugInfo;

        protected RaycastHit currentHit;
         
        protected virtual Ray GenDestinationRay()
        {
            if (customCamera == null)
                customCamera = VRTKHelper.HeadSetCamera;

            if (VRTKHelper.HeadSetCamera != null)
                return VRTKHelper.HeadSetCamera.ScreenPointToRay(Input.mousePosition);

            return new Ray();
        }

        private void Update()
        {
            CheckDestinationMark();
        }

        private void CheckDestinationMark()
        {
            RaycastHit pointerCollidedWith = new RaycastHit();
 
            var ray = GenDestinationRay();

            var rayHit = customRaycast.CustomRaycast(GenDestinationRay(), out pointerCollidedWith, focusDistance);

            CheckRayMiss(rayHit, pointerCollidedWith);

            CheckRayHit(rayHit, pointerCollidedWith);

            if (rayHit && Input.GetMouseButtonUp(0))
            {
                OnDestinationMarkerSet(SetDestinationMarkerEvent(pointerCollidedWith.distance, pointerCollidedWith.transform, pointerCollidedWith, pointerCollidedWith.point, null, false, null));
            }

            #region raycast debug

#if UNITY_EDITOR
            if(showRaycastDebugInfo)
            {
                if (rayHit)
                {
                    Debug.DrawLine(ray.origin, pointerCollidedWith.point, Color.blue);

                    Debug.Log("Focus Marker raycast   collider:" + pointerCollidedWith.collider + "     transform:" + pointerCollidedWith.transform);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * focusDistance, Color.red);
                }
            }
#endif


            #endregion
        }

        private void CheckRayMiss(bool rayHit, RaycastHit pointerCollidedWith)
        {
            if (!rayHit || pointerCollidedWith.transform != currentHit.transform)
            {
                OnDestinationMarkerExit(SetDestinationMarkerEvent(currentHit.distance, currentHit.transform, currentHit, currentHit.point, null, false, null));

                currentHit = default;
            }
        }

        private void CheckRayHit(bool rayHit, RaycastHit pointerCollidedWith)
        {
            if (rayHit || currentHit.transform != pointerCollidedWith.transform)
            {
                currentHit = pointerCollidedWith;

                DestinationMarkerEventArgs destinationEventArgs = SetDestinationMarkerEvent(currentHit.distance, currentHit.transform, currentHit, currentHit.point, null, false, null);

                OnDestinationMarkerEnter(destinationEventArgs);
            }
        }
    }
}