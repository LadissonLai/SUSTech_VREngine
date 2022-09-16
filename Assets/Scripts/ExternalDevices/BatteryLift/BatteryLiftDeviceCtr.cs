using DG.Tweening;
using Doozy.Engine;
using Framework;
using Fxb.DA;
using System;
using System.Collections;
using UnityEngine;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    [Flags]
    public enum BatteryLiftDeviceState
    {
        Default = 0,
        Lifted = 1,
        AtLocation = 2,
    }

    public enum BatteryLiftDeviceAction
    {
        None,
        
        /// <summary>
        /// 放置装置。（将装置移到车辆下方）
        /// </summary>
        Place = 1,

        /// <summary>
        /// 举升装置
        /// </summary>
        Lift,

        /// <summary>
        /// 降下装置
        /// </summary>
        Dorp,

        /// <summary>
        /// 将装置放回。
        /// </summary>
        Back,
    }

    /// <summary>
    /// 电池举升装置。
    /// </summary>
    public class BatteryLiftDeviceCtr : MonoBehaviour
    {
        private Pose testInitPose;
         
        public BatteryLiftDeviceState State { get; private set; }

        private VRDaPartsCtr targetBatteryDAObj;

        private BatteryLiftDeviceStateChangeMessage stateChangeMessage;

        private DATipMessage daTipMessage;

        private DAObjCtr batteryAttached;

        private Transform batteryDAObjParentCache;

        public string targetBatteryDAObjID;

        public Transform testPos;

        public Animation anim;

        public AdvancedInteractableObj buttonLift;

        public AdvancedInteractableObj buttonDrop;

        public AdvancedInteractableObj buttonMove;

        public string animLift;

        public string animDrop;

        /// <summary>
        /// 拆下动力电池后，动力电池被放入此root下
        /// </summary>
        public Transform connect;

        public Transform[] wheels;

        [Tooltip("轮子方向参考位置")]
        public Transform wheelDirReference;

        public DOTweenPath dotweenPath;
   
        private void ButtonUp_InteractableObjectUnused(object sender, VRTK.InteractableObjectEventArgs e)
        {
            if(!CheckLiftAble(true, out string errorMsg))
            {
                DATipMessage.Send(errorMsg, ref daTipMessage);

                return;
            }
             
            StartCoroutine(Lift());
        }

        private void ButtonDown_InteractableObjectUnused(object sender, VRTK.InteractableObjectEventArgs e)
        {
            if (!CheckLiftAble(false, out string errorMsg))
            {
                DATipMessage.Send(errorMsg, ref daTipMessage);

                return;
            }

            StartCoroutine(Drop());
        }

        private void ButtonMove_InteractableObjectUnused(object sender, VRTK.InteractableObjectEventArgs e)
        {
            if (!CheckMoveAble(out var errorMsg))
            {
                DATipMessage.Send(errorMsg, ref daTipMessage);

                return;
            }

            StartCoroutine(Move());
        }

        private void UpdateInteracts(bool interactAble)
        {
            buttonLift.isUsable = interactAble;

            buttonDrop.isUsable = interactAble;

            buttonMove.isUsable = interactAble;
        }

        private bool CheckLiftAble(bool liftUp, out string errorMsg)
        {
            errorMsg = null;

            if (!CheckTraniningModeInteractAble(liftUp ? buttonLift : buttonDrop,  ref errorMsg) || CheckIfBatteryIsProcessing(ref errorMsg))
            {
                return false;
            }

            var enableLift = (State & BatteryLiftDeviceState.Lifted) == 0;

            //目前不能中途暂停举升过程
            return liftUp ? enableLift : !enableLift;
        }

        private bool CheckMoveAble(out string errorMsg)
        {
            errorMsg = null;

            if (!CheckTraniningModeInteractAble(buttonMove, ref errorMsg) || CheckIfBatteryIsProcessing(ref errorMsg) || !CheckValidCarLiftLocation(ref errorMsg))
            {
                return false;
            }

            if (World.Get<CmsCarState>().liftLocation < 1.9f)
            {
                errorMsg = "举升车辆到最高位";

                return false;
            }

            return true;
        }

        private bool CheckValidCarLiftLocation(ref string errorMsg)
        {
            if (World.Get<CmsCarState>().liftLocation < 1.9f)
            {
                errorMsg = "举升车辆到最高位";

                return false;
            }

            return true;
        }

        private bool CheckIfBatteryIsProcessing(ref string errorMsg)
        {
            if ((targetBatteryDAObj.State & (CmsObjState.Dismantled | CmsObjState.Default)) == 0)
            {
                //电池完整与被拆下时才可以操作举升装置。（中途不能降下或者举升或者移动）
                errorMsg = "请先完成电池拆装步骤";

                return true;
            }
 
            return false;
        }

        private bool CheckTraniningModeInteractAble(AdvancedInteractableObj targetButton, ref string errorMsg)
        {
            if (World.Get<DASceneState>().taskMode == DaTaskMode.Training)
            {
                if(!targetButton.isTiped)
                {
                    errorMsg = "实训模式请按照指引步骤操作";

                    return false;
                }
            }

            return true;
        }
  
        private IEnumerator Drop()
        {
            UpdateInteracts(false);

            yield return PlayLegacyAnim(anim, animDrop);

            UpdateInteracts(true);
            
            State &= ~BatteryLiftDeviceState.Lifted;

            BatteryLiftDeviceStateChangeMessage.Send(State, BatteryLiftDeviceAction.Dorp, ref stateChangeMessage);
        }

        private IEnumerator Lift()
        {
            UpdateInteracts(false);

            yield return PlayLegacyAnim(anim, animLift);

            UpdateInteracts(true);

            State |= BatteryLiftDeviceState.Lifted;

            BatteryLiftDeviceStateChangeMessage.Send(State, BatteryLiftDeviceAction.Lift, ref stateChangeMessage);
        }

        private IEnumerator WaitForPathAnim(bool forward)
        {
            if (forward)
                dotweenPath.DOPlayForward();
            else
                dotweenPath.DOPlayBackwards();

            var waitForEndOfFrame = new WaitForEndOfFrame();

            yield return null;

            Vector3 wheelPostionCache = wheelDirReference.position;

            while (dotweenPath.tween.IsPlaying() || Vector3.Dot(transform.right, -dotweenPath.transform.forward) < 0.9999f)
            {
                //Debug.Log(Vector3.Dot(transform.right, -dotweenPath.transform.forward));

                yield return waitForEndOfFrame;

                transform.position = dotweenPath.transform.position;
                 
                transform.right = Vector3.Lerp(transform.right, -dotweenPath.transform.forward, 0.1f);

                var moveDir = (wheelDirReference.position - wheelPostionCache).normalized;

                if (forward)
                    moveDir *= -1f;

                wheelPostionCache = wheelDirReference.position;

                //Debug.Log(moveDir.magnitude);

                if(moveDir.magnitude > 0.01f)
                {
                    foreach (var wheel in wheels)
                    {
                        wheel.right = Vector3.Lerp(wheel.right, moveDir, 0.05f);
                    }
                }
            }

            transform.right = -dotweenPath.transform.forward;
        }
 
        private IEnumerator Move()
        {
            UpdateInteracts(false);

            if ((State & BatteryLiftDeviceState.AtLocation) == 0)
            {
                yield return WaitForPathAnim(true);

                State |= BatteryLiftDeviceState.AtLocation;

                BatteryLiftDeviceStateChangeMessage.Send(State, BatteryLiftDeviceAction.Place, ref stateChangeMessage);
            }
            else
            {
                yield return WaitForPathAnim(false);

                State &= ~BatteryLiftDeviceState.AtLocation;

                BatteryLiftDeviceStateChangeMessage.Send(State, BatteryLiftDeviceAction.Back, ref stateChangeMessage);
            }

            UpdateInteracts(true);
        }

        private IEnumerator PlayLegacyAnim(Animation anim, string animName)
        {
            anim.Play(animName);

            yield return new WaitUntil(() => anim.IsPlaying(animName));

            yield return new WaitUntil(() => !anim.isPlaying);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<DAObjStateChangeMessage>(OnDAObjStateChanged);
        }

        private void Start()
        {
            buttonLift.InteractableObjectUnused += ButtonUp_InteractableObjectUnused;

            buttonDrop.InteractableObjectUnused += ButtonDown_InteractableObjectUnused;

            buttonMove.InteractableObjectUnused += ButtonMove_InteractableObjectUnused;

            targetBatteryDAObj = string.IsNullOrEmpty(targetBatteryDAObjID) 
                ? null 
                : World.Get<DAObjCtr>(targetBatteryDAObjID) as VRDaPartsCtr;

            Debug.Assert(targetBatteryDAObj != null, "电池举升装置未设置电池拆装物体id");

            testInitPose = new Pose(transform.position, transform.rotation);

            Message.AddListener<DAObjStateChangeMessage>(OnDAObjStateChanged);
        }
 
        protected void TipInteractButton(bool tip, AdvancedInteractableObj interactButton)
        {
            if (tip)
                interactButton.TipInteractableObj();
            else
                interactButton.UnTipInteractableObj();
        }

        /// <summary>
        /// 提示举升装置交互
        /// </summary>
        /// <param name="isLiftUp">是否是举升，为false表示降下</param>
        public void TipInteract(bool liftUp = false, bool dorp = false, bool move = false)
        {
            TipInteractButton(liftUp, buttonLift);

            TipInteractButton(dorp, buttonDrop);

            TipInteractButton(move, buttonMove);
        }
         
        public void UntipInteracts()
        {
            TipInteract(false, false, false);
        }

        private void OnDAObjStateChanged(DAObjStateChangeMessage msg)
        {
            if (msg.objCtr.ID != targetBatteryDAObjID)
                return;

            Debug.Assert(State.HasFlag(BatteryLiftDeviceState.AtLocation | BatteryLiftDeviceState.Lifted));

            if (msg.objCtr.State == CmsObjState.Dismantled)
            {
                Debug.Assert(batteryAttached == null);

                batteryAttached = msg.objCtr as DAObjCtr;

                batteryDAObjParentCache = batteryAttached.transform.parent;

                batteryAttached.transform.SetParent(connect);
            }
            else if(msg.objCtr.State == CmsObjState.Default)
            {
                Debug.Assert(batteryAttached != null);

                batteryAttached.transform.SetParent(batteryDAObjParentCache);

                batteryAttached = null;

                batteryDAObjParentCache = null;
            }
        }

    }
}


