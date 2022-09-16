using DG.Tweening;
using Doozy.Engine;
using Framework;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public class DAHandCtr : MonoBehaviour
    {
        public enum HandDirection
        {
            UnKnown, Left, Right
        }

        public HandDirection Direction
        {
            get
            {
                if (VRTK_SDKManager.instance.scriptAliasLeftController == gameObject)
                    return HandDirection.Left;
                else if (VRTK_SDKManager.instance.scriptAliasRightController == gameObject)
                    return HandDirection.Right;
                
                return HandDirection.UnKnown;
            }
        }

        private bool handModelInvalid;

        private VRTK_ControllerEvents evtCtr;

        private VRTK_Pointer pointer;

        private VRTK_InteractTouch touch;

        private VRTK_InteractGrab grab;

        private VRTK_UIPointer uiPointer;

        private CustomVirtualHandAvatarCtr avatarCtr;

        private string handAvataAssets;

        //待确定是否需要
        private bool sdkHandVisible;

        private List<DACloneObjCtr> cloneObjHelpList;

        private ControllerGrabInteractObjMessage grabMessageCache;

        private List<Collider> disabledTouchCollidersCache;

        private IGrabableTool cutGrabedTool;

        private VRTK_ControllerReference controllerReference;

        private bool hasInit;
 
        protected void Init()
        {
            hasInit = true;

            controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);

            cloneObjHelpList = new List<DACloneObjCtr>();

            uiPointer = GetComponent<VRTK_UIPointer>();

            pointer = GetComponent<VRTK_Pointer>();

            pointer.DestinationMarkerSet += Pointer_DestinationMarkerSet;

            touch = GetComponent<VRTK_InteractTouch>();
             
            InitGrab();

            InitEvtController();

            if (Direction == HandDirection.Left)
                SetHandAvatar(PathConfig.PREFAB_VIRTUAL_HAND_L);
            else if (Direction == HandDirection.Right)
                SetHandAvatar(PathConfig.PREFAB_VIRTUAL_HAND_R);
        }

        protected void InitGrab()
        {
            grab = GetComponent<VRTK_InteractGrab>();

            grab.ControllerGrabInteractableObject += Grab_ControllerGrabInteractableObject;

            grab.ControllerUngrabInteractableObject += Grab_ControllerUngrabInteractableObject;
        }

        private void Pointer_DestinationMarkerSet(object sender, DestinationMarkerEventArgs e)
        {
            //射线抓取桌上的物体会有问题。
 
            if (grab.GetGrabbedObject() != null)
                return;

            var interactObj = e.target.GetComponentInParent<AdvancedInteractableObj>();

            if (interactObj == null || !interactObj.isGrabbable || !interactObj.autoGrabOnFocusSetIfEnable || interactObj.IsGrabbed())
                return;

            var grabAttach = interactObj.grabAttachMechanicScript;

            if (grabAttach == null)
                return;

            if(!grabAttach.precisionGrab)
            {
                ForceGrab(interactObj.gameObject);

                return;
            }

            var origin = (pointer as VRTK_Pointer).customOrigin.position;

            var dir = (origin - e.raycastHit.point).normalized;

            var dis = e.raycastHit.distance;

            var targetPos = interactObj.transform.position + dir * (dis + 0.02f);

            interactObj.transform.DOMove(targetPos, 0.1f).OnComplete(()=> {
                if(grab.GetGrabbedObject() == null)
                    ForceGrab(interactObj.gameObject);
            });
        }

        private void ForceGrab(GameObject go)
        {
            touch.ForceStopTouching();
            touch.ForceTouch(go);
            grab.AttemptGrab();
            touch.ForceStopTouching();
        }

        private void Grab_ControllerUngrabInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            //放下物体后恢复touch的碰撞
            if(World.current.IsAppQuitting)
            {
                return;
            }

            cutGrabedTool = null;

            EnableHandCollision();

            grabMessageCache = grabMessageCache ?? new ControllerGrabInteractObjMessage();

            grabMessageCache.FromGrabEvts(sender, e);

            grabMessageCache.isGrab = false;

            Message.Send(grabMessageCache);
        }
        
        private void Grab_ControllerGrabInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if(e.target.TryGetComponent<IGrabableTool>(out var tool))
            {
                cutGrabedTool = tool;
            }

            //抓取物体后关闭手部交互。
            //ToggleControllerRigidBody只是将手部碰撞的isTrigger关闭，有可能其它物体身上也存在trigger。改为直接将碰撞体关闭。
            //touch.ToggleControllerRigidBody(true, true);
            DisableHandCollision();

            grabMessageCache = grabMessageCache ?? new ControllerGrabInteractObjMessage();

            grabMessageCache.FromGrabEvts(sender, e);

            grabMessageCache.isGrab = true;

            Message.Send(grabMessageCache);

            //TODO 如果是手持工具，那么在工具使用过程中不允许被另一只手交互.
        }

        protected void InitEvtController()
        {
            //pointer的开关由自身控制
            Debug.Assert(pointer.activationButton == VRTK_ControllerEvents.ButtonAlias.Undefined);

            Debug.Assert(pointer.selectionButton == VRTK_ControllerEvents.ButtonAlias.Undefined);

            evtCtr = GetComponent<VRTK_ControllerEvents>();
 
            evtCtr.SubscribeToButtonAliasEvent(VRTK_ControllerEvents.ButtonAlias.TouchpadPress, true, OnTouchpadPressed);

            evtCtr.SubscribeToButtonAliasEvent(VRTK_ControllerEvents.ButtonAlias.TouchpadPress, false, pointer.SelectionButtonAction);

            evtCtr.SubscribeToButtonAliasEvent(VRTK_ControllerEvents.ButtonAlias.TouchpadPress, false, OnTouchpadReleased);
        }

        private void OnTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (TryGetParamModifyGrabedTools(out var tool))
            {
                tool.PressModifyBtn(false, e.touchpadAxis);
            }

            if (pointer.IsPointerActive())
                pointer.Toggle(false);
        }

        private void OnTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (TryGetParamModifyGrabedTools(out var tool))
            {
                tool.PressModifyBtn(true, e.touchpadAxis);
            }
            else
            {
                pointer.Toggle(true);
            }
        }
 
        private bool TryGetParamModifyGrabedTools(out IGrabableTool tool)
        {
            tool = cutGrabedTool;

            var grabedTool = tool as MonoBehaviour;

            //脚本被关闭也算不生效  
            //待确定 组合哪怕没有接杆和套筒，应该也是可以调整扭力的
            if (grabedTool == null || !grabedTool.enabled || grab.GetGrabbedObject() != grabedTool.gameObject || !tool.CheckModifyParamValid())
                return false;

            return true;
        }
         
        /// <summary>
        /// 开启手部碰撞
        /// </summary>
        private void EnableHandCollision()
        {
            if (disabledTouchCollidersCache == null || disabledTouchCollidersCache.Count == 0)
                return;

            foreach (var c in disabledTouchCollidersCache)
            {
                c.enabled = true;
            }

            disabledTouchCollidersCache.Clear();
        }

        /// <summary>
        /// 关闭手部碰撞
        /// </summary>
        private void DisableHandCollision()
        {
            disabledTouchCollidersCache = disabledTouchCollidersCache ?? new List<Collider>();

            if (disabledTouchCollidersCache.Count > 0)
                return;

            var colliders = touch.ControllerColliders(); 

            foreach (var c in colliders)
            {
                if (c.attachedRigidbody == null || c.attachedRigidbody.transform != transform)
                    continue;
                 
                c.enabled = false;

                disabledTouchCollidersCache.Add(c);
            }

            //uiPointer.autoActivatingCanvas 在VRTK_UIPointerAutoActivator中会监听物理事件去赋值和清理。直接关闭手部碰撞会监听不到对应事件。
            //手动处理
            if (uiPointer != null && uiPointer.autoActivatingCanvas != null)
                uiPointer.autoActivatingCanvas = null;
        }
    
        public CustomVirtualHandAvatarCtr SetHandAvatar(string assets)
        {
            if (avatarCtr != null && handAvataAssets == assets)
                return avatarCtr;

            handAvataAssets = assets;

            var prefab = Resources.Load<GameObject>(assets);

            avatarCtr = Instantiate(prefab, transform).GetComponent<CustomVirtualHandAvatarCtr>();

            avatarCtr.transform.ResetLocalMatrix();

            handModelInvalid = true;

            return avatarCtr;
        }
         
        /// <summary>
        /// 显示隐藏手柄与avatar 手模型
        /// </summary>
        protected void UpdateHandModel()
        {
            var controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
            
            if (controllerReference.model == null)
                return;

            Transform pointerCustomOrigin = null;

            Transform colliderContainer = null;

            Rigidbody grabAttachPoint = null;

            controllerReference.model.SetActive(sdkHandVisible);

            if (avatarCtr != null)
            {
                avatarCtr.gameObject.SetActive(!sdkHandVisible);

                //待调整，从同一的接口获取各种位置点信息
                pointerCustomOrigin = avatarCtr.pointerRendererPos;

                colliderContainer = avatarCtr.colliderRoot.transform;

                grabAttachPoint = avatarCtr.grabAttachPoint.GetComponent<Rigidbody>();
            }

            if (pointer != null && pointer is VRTK_Pointer)
            {
                (pointer as VRTK_Pointer).customOrigin = pointerCustomOrigin;
            }

            var uiPointer = GetComponent<VRTK_UIPointer>();

            if (uiPointer != null)
            {
                uiPointer.customOrigin = pointerCustomOrigin;
            }

            if (touch != null)
            {
                touch.enabled = false;

                touch.customColliderContainer = colliderContainer == null ? null : colliderContainer.gameObject;

                touch.enabled = true;
            }

            if (grab != null)
            {
                grab.enabled = false;

                grab.ForceControllerAttachPoint(grabAttachPoint);

                grab.enabled = true;
            }
        }

        /// <summary>
        /// 自动合并克隆物体到抓取的目标上
        /// </summary>
        private void CheckAutoCombineCloneObjs()
        {
            var grabedObj = grab.GetGrabbedObject();

            if (grabedObj == null || !grabedObj.TryGetComponent<DACloneObjCtr>(out var cloneObj))
                return;
 
            cloneObjHelpList.Clear();

            World.current.Injecter.GetAll(cloneObjHelpList);

            if (cloneObjHelpList.Count == 0)
                return;

            foreach (var checkObj in cloneObjHelpList)
            {
                if (checkObj == cloneObj || checkObj.PropID != cloneObj.PropID || !checkObj.WaitForFistPick || checkObj.IsAnimPlaying)
                    continue;

                //一定距离之内的才允许合并
                if (Vector3.Distance(checkObj.transform.position, cloneObj.transform.position) > 2.5f)
                    continue;

                cloneObj.AddCloneObj(checkObj, true);

                //错开动画  待测试看效果
                break;
            }
        }

#if UNITY_EDITOR
        private Vector2 lastTouchpadAlis = Vector2.up;

        private void SimlateTouchpadAlis()
        {
            var grabObj = grab.GetGrabbedObject();

            if (grabObj == null || !grabObj.TryGetComponent<IGrabableTool>(out var grableTool) || !grableTool.CheckModifyParamValid())
                return;

            var sw = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(sw) > 0.0001f)
            {
                lastTouchpadAlis = Quaternion.AngleAxis(100.0f * sw, Vector3.forward) * lastTouchpadAlis;

                lastTouchpadAlis.Normalize();
                
                Debug.DrawRay(transform.position, transform.rotation * lastTouchpadAlis * 0.1f , Color.red, 0.2f);
            }

            if (Input.GetMouseButtonDown(2))
            {
                grableTool.PressModifyBtn(true, lastTouchpadAlis);
            }
            else if (Input.GetMouseButtonUp(2))
            {
                grableTool.PressModifyBtn(false, lastTouchpadAlis);
            }
            else if (Input.GetMouseButton(2))
            {
                grableTool.UpdateAlis(lastTouchpadAlis);
            }
        }
#endif

        private void LateUpdate()
        {
            if(!hasInit)
            {
                if (VRTK_SDKManager.instance.loadedSetup == null)
                    return;
 
                Init();
            }

            var isTouchpadPressed = VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Press, controllerReference);

            if (isTouchpadPressed && TryGetParamModifyGrabedTools(out var validGrabedTool))
            {
                validGrabedTool.UpdateAlis(evtCtr.GetTouchpadAxis());
            }

            if (handModelInvalid)
            {
                UpdateHandModel();

                handModelInvalid = false;
            }

            CheckAutoCombineCloneObjs();

#if UNITY_EDITOR
            SimlateTouchpadAlis();
#endif
        }
    }
}

