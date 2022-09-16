using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Framework;

namespace VRTKExtensions
{
    public static class VRTKHelper
    {
        private static Camera headSetCamera;
        private static GameObject leftHand;
        private static GameObject rightHand;
        private static VRTK_InteractGrab leftGrab;
        private static VRTK_InteractGrab rightGrab;
        private static VRTK_InteractTouch leftTouch;
        private static VRTK_InteractTouch rightTouch;
        private static VRTK_BodyPhysics bodyPhysics;

        //不要改成 ?? 形式。
        public static Camera HeadSetCamera
        {
            get
            {
                headSetCamera = headSetCamera != null ? headSetCamera : (VRTK_DeviceFinder.HeadsetCamera()?.GetComponent<Camera>());

                if (headSetCamera == null)
                    headSetCamera = Camera.main;

                return headSetCamera;
            }
        }

        public static GameObject LeftHand
            => leftHand = leftHand != null ? leftHand : VRTK_DeviceFinder.GetControllerLeftHand();

        public static GameObject RightHand
            => rightHand = rightHand != null ? rightHand : VRTK_DeviceFinder.GetControllerRightHand();

        public static VRTK_InteractGrab LeftGrab
            => leftGrab = leftGrab != null ? leftGrab : LeftHand.GetComponent<VRTK_InteractGrab>();

        public static VRTK_InteractGrab RightGrab
            => rightGrab = rightGrab != null ? rightGrab : RightHand.GetComponent<VRTK_InteractGrab>();

        public static VRTK_InteractTouch LeftTouch
            => leftTouch = leftTouch != null ? leftTouch : LeftHand.GetComponent<VRTK_InteractTouch>();

        public static VRTK_InteractTouch RightTouch
            => rightTouch = rightTouch != null ? rightTouch : RightHand.GetComponent<VRTK_InteractTouch>();

        public static VRTK_BodyPhysics BodyPhysics
        {
            get
            {
                if (bodyPhysics == null)
                {
                    var allMarkers = VRTK_ObjectCache.registeredDestinationMarkers;

                    if (allMarkers.Count > 0)
                    {
                        bodyPhysics = allMarkers.Find((marker) =>
                        {
                            return marker is VRTK_BodyPhysics;
                        }) as VRTK_BodyPhysics;
                    }
                }

                return bodyPhysics;
            }
        }

        public static void ForceGrab(VRTK_InteractableObject obj)
        {
            switch (obj.allowedGrabControllers)
            {
                case VRTK_InteractableObject.AllowedController.Both:
                    ForceGrab(SDK_BaseController.ControllerHand.None, obj.gameObject);
                    break;
                case VRTK_InteractableObject.AllowedController.LeftOnly:
                    ForceGrab(SDK_BaseController.ControllerHand.Left, obj.gameObject);
                    break;
                case VRTK_InteractableObject.AllowedController.RightOnly:
                    ForceGrab(SDK_BaseController.ControllerHand.Right, obj.gameObject);
                    break;
            }
        }

        public static void ForceDropGrab()
        {
            var leftGrabObj = LeftGrab.GetGrabbedObject();
            var rightGrabObj = RightGrab.GetGrabbedObject();

            if (leftGrabObj != null)
                LeftGrab.ForceRelease();

            if (rightGrabObj != null)
                RightGrab.ForceRelease();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hand">如果hand==None 则使用空闲的手抓取</param>
        /// <returns></returns>
        public static void ForceGrab(SDK_BaseController.ControllerHand hand, GameObject obj)
        {
            VRTK_InteractGrab targetGrab = null;
            VRTK_InteractTouch targetTouch = null;

            if (hand == SDK_BaseController.ControllerHand.None)
            {
                hand = SDK_BaseController.ControllerHand.Right;

                if (RightGrab.GetGrabbedObject() != null && LeftGrab.GetGrabbedObject() == null)
                    hand = SDK_BaseController.ControllerHand.Left;
            }

            switch (hand)
            {
                case SDK_BaseController.ControllerHand.Left:
                    targetGrab = LeftGrab;
                    targetTouch = LeftTouch;

                    break;
                case SDK_BaseController.ControllerHand.Right:
                    targetGrab = RightGrab;
                    targetTouch = RightTouch;

                    break;
                default:
                    Debug.LogError("未知控制器");
                    break;
            }

            targetGrab.ForceRelease();
            targetTouch.ForceTouch(obj.gameObject);
            targetGrab.AttemptGrab();
            targetTouch.ForceStopTouching();
        }

        public static bool FindAllGrabedObj(out (VRTK_InteractableObject leftObj, VRTK_InteractableObject rightObj) result)
        {
            result.leftObj = LeftGrab.GetGrabbedObject()?.GetComponent<VRTK_InteractableObject>();

            result.rightObj = RightGrab.GetGrabbedObject()?.GetComponent<VRTK_InteractableObject>();

            return result.leftObj != null || result.rightObj != null;
        }

        /// <summary>
        /// FindAllGrabedGameObject
        /// </summary>
        /// <param name="res">leftGO左手抓取 rightGO右手抓取</param>
        /// <returns></returns>
        public static bool FindAllGrabedGameObject(out (GameObject leftGO, GameObject rightGO) res)
        {
            res.leftGO = LeftGrab.GetGrabbedObject();

            res.rightGO = RightGrab.GetGrabbedObject();

            return res.leftGO != null || res.rightGO != null;
        }

        public static T FindGrabedObjCom<T>() where T : Component
        {
            T com = null;

            var obj = LeftGrab.GetGrabbedObject()?.GetComponent<VRTK_InteractableObject>();

            if (obj != null && obj.TryGetComponent<T>(out com))
            {
                return com;
            }

            obj = RightGrab.GetGrabbedObject()?.GetComponent<VRTK_InteractableObject>();

            if (obj != null && obj.TryGetComponent<T>(out com))
            {
                return com;
            }

            return com;
        }
         
        public static bool FindGrabedObj(out VRTK_InteractableObject obj, Predicate<VRTK_InteractableObject> match = null)
        {
            obj = LeftGrab.GetGrabbedObject()?.GetComponent<VRTK_InteractableObject>();

            if (IsObjValid(obj, match))
                return true;

            obj = RightGrab.GetGrabbedObject()?.GetComponent<VRTK_InteractableObject>();

            if (IsObjValid(obj, match))
                return true;

            return false;
        }

        private static bool IsObjValid(VRTK_InteractableObject obj, Predicate<VRTK_InteractableObject> match = null)
        {
            return obj != null && (match == null || match(obj));
        }

        private static List<Type> DefaultModelCopyComsFilter = new List<Type> { typeof(MeshFilter), typeof(Renderer), typeof(Collider) };

        public static GameObject CloneGameObj(GameObject root, Transform givenParent = null, List<Type> holdMonoComsFilter = null)
        {
            var newGO = GameObject.Instantiate(root, givenParent, true);

            var coms = newGO.GetComponentsInChildren<MonoBehaviour>();

            foreach (var com in coms)
            {
                var comType = com.GetType();

                var holdCom = false;

                if (holdMonoComsFilter != null)
                {
                    foreach (var holdType in holdMonoComsFilter)
                    {
                        //Debug.Log($"{holdType} is instanceoftype {com}  {holdType.IsInstanceOfType(com)}");

                        if (holdType.IsInstanceOfType(com))
                        {
                            holdCom = true;

                            break;
                        }
                    }
                }

                if (!holdCom)
                    UnityEngine.Object.Destroy(com);
            }

            return newGO;
        }

        /// <summary>
        /// 拷贝模型
        /// </summary>
        /// <param name="target"></param>
        /// <param name="givenRenderers">为null则从target下获取所有renderer</param>
        /// <param name="comsFilter">需要拷贝的所有com</param>
        /// <param name="givenParent">拷贝出来的模型根目录</param>
        /// <param name="givenMats">替换的材质</param>
        public static void CopyModel(
            Transform target,
            IList<Renderer> givenRenderers = null,
            List<Type> comsFilter = null,
            bool includeColider = false,
            Transform givenParent = null,
            Material givenMats = null)
        {
            comsFilter = comsFilter ?? DefaultModelCopyComsFilter;

            givenRenderers = givenRenderers ?? target.GetComponentsInChildren<Renderer>();

            var offsetPos = givenParent.position - target.position;
 
            foreach (var srcRenderer in givenRenderers)
            {
                GameObject newRendererGO = new GameObject(srcRenderer.name);

                newRendererGO.transform.position = srcRenderer.transform.position + offsetPos;
                newRendererGO.transform.rotation = srcRenderer.transform.rotation;
                newRendererGO.transform.SetGlobalScale(srcRenderer.transform.lossyScale);
                newRendererGO.transform.SetParent(givenParent);

                var newRenderer = VRTK_SharedMethods.CloneComponent(srcRenderer, newRendererGO, true) as Renderer;

                if (srcRenderer.TryGetComponent<MeshFilter>(out var copyMesh))
                {
                    VRTK_SharedMethods.CloneComponent(copyMesh, newRendererGO, true);
                }

                if (givenMats != null)
                {
                    Material[] shardMaterials = new Material[newRenderer.sharedMaterials.Length];
                    newRenderer.ReplaceSharedMats(givenMats, shardMaterials);
                }
            }
        }

        public static void CopyModelNew(Transform target,
            Transform givenParent,
            IList<Renderer> givenRenderers = null,
            bool includeColider = false,
            IList<Collider> givenColliders = null,
            Material givenMats = null)
        {
            givenRenderers = givenRenderers ?? target.GetComponentsInChildren<Renderer>(false);

            var transComMap = new Dictionary<Transform, List<Component>>();
             
            foreach (var renderer in givenRenderers)
            {
                var coms = new List<Component>() { renderer };

                if (renderer.TryGetComponent<MeshFilter>(out var meshFilter))
                    coms.Add(meshFilter);
                
                transComMap.Add(renderer.transform, new List<Component>() { renderer, renderer.GetComponent<MeshFilter>()});
            }
 
            if (includeColider)
            {
                givenColliders = givenColliders ?? target.GetComponentsInChildren<Collider>(false);

                foreach (var colider in givenColliders)
                {
                    if (!transComMap.TryGetValue(colider.transform, out var coms))
                    {
                        coms = new List<Component>();

                        transComMap.Add(colider.transform, coms);
                    }

                    coms.AddUnique(colider);

                    //多个colider会对应1个rigidbody  如果colider被关闭attachedRigidbody为null
                    if (colider.attachedRigidbody != null && colider.attachedRigidbody.transform.IsChildOf(target))
                        coms.AddUnique(colider.attachedRigidbody);
                }
            }

            var offsetPos = givenParent.position - target.position;

            foreach (var kv in transComMap)
            {
                GameObject copyGO = new GameObject(kv.Key.name);

                copyGO.transform.position = kv.Key.position + offsetPos;
                copyGO.transform.rotation = kv.Key.rotation;
                copyGO.transform.SetGlobalScale(kv.Key.lossyScale);
                copyGO.transform.SetParent(givenParent);

                foreach (var com in kv.Value)
                {
                    if (com == null)
                        continue;

                    //Clone component太耗，待重构
                    var newCOm = VRTK_SharedMethods.CloneComponent(com, copyGO, true);

                    if(com is Renderer renderer && givenMats != null)
                    {
                        Material[] shardMaterials = new Material[renderer.sharedMaterials.Length];

                        (newCOm as Renderer).ReplaceSharedMats(givenMats, shardMaterials);
                    }
                }
            }
        }
    }
}

