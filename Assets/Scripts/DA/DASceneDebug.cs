using Doozy.Engine;
using Framework;
using Fxb.DA;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public class DASceneDebug : MonoBehaviour
    {
        private DAState DaState => World.Get<DAState>();

        private IRecordModel RecordModel => World.Get<IRecordModel>();

        private DATaskGuide taskGuide;

        private void OnGuideTipMessage(GuideTipMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.tip))
                Debug.Log("步骤提示:" + msg.tip);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<GuideTipMessage>(OnGuideTipMessage);
        }

        // Start is called before the first frame update
        void Start()
        {
            Message.AddListener<GuideTipMessage>(OnGuideTipMessage);
        }

        // Update is called once per frame
        void Update()
        {
            DebugAutoProcessObj();

            DebugLogGuideInfo();
        }

        private void DebugLogGuideInfo()
        {
            //测试 输出当前步骤情况
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (taskGuide == null)
                    taskGuide = FindObjectOfType<DATaskGuide>();

                var cutRecord = "无记录项";

                var recordCompleted = false;
                 
                if (!string.IsNullOrEmpty(taskGuide.Current))
                {
                    cutRecord = RecordModel.FindRecord(taskGuide.Current)?.Title;

                    recordCompleted = RecordModel.CheckRecordCompleted(taskGuide.Current);
                }
                 
                Debug.Log($"当前步骤是否完成:{taskGuide.IsStepCompleted}\n当前目标记录项:{cutRecord}\n当前记录项是否完成:{recordCompleted}");
            }
        }

        private bool DebugAutoDA()
        {
            var targetDAObj = DaState.tipsInGuiding.Find((obj) =>
            {
                var daObj = obj as DAObjCtr;

                if (daObj == null || obj.interactObj == null)
                    return false;

                if (!daObj.interactObj.gameObject.activeInHierarchy)
                    return false;

                if (!daObj.interactObj.isTiped)
                    return false;

                return true;
            }) as DAObjCtr;

            if (targetDAObj != null)
            {
                //操作拆装物体
                var useHand = VRTKHelper.RightHand;

                VRTKHelper.FindAllGrabedGameObject(out var res);

                if (targetDAObj.DisplayMode == CmsDisplayMode.PlaceHolder)
                {
                    //找有抓取物的手，优先左手
                    if (res.leftGO != null && res.leftGO.GetComponent<DACloneObjCtr>() != null)
                        useHand = VRTKHelper.LeftHand;
                }
                else
                {
                    var daScript = targetDAObj.GetComponent<AbstractDAScript>();

                    if (daScript != null && (daScript is MixedToolDAScript || daScript is WrenchDAScriptV2))
                    {
                        //工具手
                        if (res.leftGO != null && res.leftGO.GetComponent<DACloneObjCtr>() == null)
                            useHand = VRTKHelper.LeftHand;
                    }
                    else
                    {
                        if (res.leftGO == null)
                            useHand = VRTKHelper.LeftHand;
                    }
                }

                targetDAObj.interactObj.StopUsing(useHand.GetComponent<VRTK_InteractUse>());

                return true;
            }

            return false;
        }

        private bool DebugAutoGrabCloneObj()
        {
            var targetCloneObj = (DaState.tipsInGuiding.Find((obj) => {
                var daObjCtr = obj as DAObjCtr;

                if (daObjCtr.CloneObjToPickup == null || !daObjCtr.CloneObjToPickup.interactObj.isTiped)
                    return false;

                return true;
            }) as DAObjCtr)?.CloneObjToPickup;

            if(targetCloneObj == null)
            {
                //从桌上找一找
                var daSceneState = World.Get<DASceneState>();

                foreach (var kv in daSceneState.cloneObjsInTable)
                {
                    var tmpObj = World.Get<DACloneObjCtr>(kv.Value);

                    if(tmpObj.interactObj.isTiped)
                    {
                        targetCloneObj = tmpObj;

                        break;
                    }
                }
            }

            if (targetCloneObj != null)
            {
                if (targetCloneObj.interactObj.TryGetComponent<VRTK_BaseGrabAttach>(out var grabAttach))
                {
                    //抓取点随意的话会导致抓取物体离手掌距离过远
                    grabAttach.precisionGrab = false;
                }

                VRTKHelper.ForceGrab(SDK_BaseController.ControllerHand.None, targetCloneObj.gameObject);

                return true;
            }

            return false;
        }

        private void DebugAutoProcessObj()
        {
            //测试 直接尝试用右手去拆装提示物体
            if (Input.GetKeyDown(KeyCode.F2))
            {
                //抓取装备后自动穿上
                var equipmentObj = VRTKHelper.FindGrabedObjCom<EquipmentObjCtr>();

                if (equipmentObj != null)
                {
                    equipmentObj.Wear(true);

                    return;
                }

                if (DaState.tipsInGuiding == null || DaState.tipsInGuiding.Count == 0)
                    return;

                if (DebugAutoGrabCloneObj())
                    return;

                if (DebugAutoDA())
                    return;
            }
        }
    }
}
