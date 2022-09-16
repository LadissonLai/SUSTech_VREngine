using Framework;
using Fxb.DA;
using System.Collections.Generic;
using UnityEngine;
using System;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public class DAGuideStep : AbstractGuideStep
    {
        private DAState DaState => World.Get<DAState>();

        private Dictionary<DAObjCtr, DACloneObjCtr> cloneObjTipMap;

        public override void Setup(string tipInfo, RecordStepType type, string singleParam = null, string[] mutiPrams = null)
        {
            base.Setup(tipInfo, type, singleParam, mutiPrams);

            DebugEx.AssertIsTrue(singleParam != null || mutiPrams != null);

            if (cloneObjTipMap == null)
                cloneObjTipMap = new Dictionary<DAObjCtr, DACloneObjCtr>();

            switch (type)
            {
                case RecordStepType.Dismantle:
                    DaState.guidingProcessTarget = DAProcessTarget.Dismantle;
                    break;
                case RecordStepType.Assemble:
                    DaState.guidingProcessTarget = DAProcessTarget.Assemble;
                    break;
                case RecordStepType.Fix:
                    DaState.guidingProcessTarget = DAProcessTarget.Fix;
                    break;
            }

            if (DaState.tipsInGuiding == null)
                DaState.tipsInGuiding = new List<AbstractDAObjCtr>();

            DaState.tipsInGuiding.Clear();

            if (singleParam != null)
            {
                DebugEx.AssertNotNull(World.Get<DAObjCtr>(singleParam), $"无DAObjCtr. singleParam:{singleParam}");

                DaState.tipsInGuiding.Add(World.Get<DAObjCtr>(singleParam));
            }
            else
            {
                Array.ForEach(mutiPrams, (param) =>
                {
                    //Debug.Log("param:" + param + "  |   " + World.Get<DAObjCtr>(param));

                    DebugEx.AssertNotNull(World.Get<DAObjCtr>(param), $"无DAObjCtr. param:{param}");

                    DaState.tipsInGuiding.Add(World.Get<DAObjCtr>(param));
                });
            }
        }

        public override bool IsCompleted
        {
            get
            {
                return DaState.tipsInGuiding.TrueForAll(CheckDAObjComplete);
            }
        }

        private bool CheckDAObjComplete(AbstractDAObjCtr daObjCtr)
        {
            if (daObjCtr.IsProcessing)
                return false;

            switch (RecordType)
            {
                case RecordStepType.Dismantle:
                    return (daObjCtr.State & CmsObjState.Dismantable) == 0;
                case RecordStepType.Assemble:
                    return (daObjCtr.State & CmsObjState.Dismantable) != 0;
                case RecordStepType.Fix:
                    return (daObjCtr.State == CmsObjState.Fixed || daObjCtr.State == CmsObjState.Default);
                default:
                    return true;
            }
        }

        private (bool tipInteract, DACloneObjCtr cloneObjToTip) CheckDAObjTipAble(DAObjCtr daObjCtr)
        {
            (bool tipInteract, DACloneObjCtr cloneObjToTip) res = (false, null);

            if (daObjCtr.IsProcessing)
                return res;

            switch (RecordType)
            {
                case RecordStepType.Dismantle:
                    if (daObjCtr.State == CmsObjState.WaitForPickup)
                        res.cloneObjToTip = daObjCtr.CloneObjToPickup;
                    else
                        res.tipInteract = (daObjCtr.State & CmsObjState.Dismantable) != 0;
                    break;
                case RecordStepType.Assemble:
                    if (daObjCtr.State == CmsObjState.Dismantled)
                    {
                        if (!daObjCtr.autoDisappear)
                        {
                            res.tipInteract = true;

                            break;
                        }

                        if (VRTKHelper.FindAllGrabedGameObject(out var grabs))
                        {
                            //如果当前已经拿取了有效物体就不需要再高亮提示
                            //考虑将抓取的克隆物体信息直接存到SceneState中，用来快速判断
                            if (
                                grabs.leftGO != null && grabs.leftGO.TryGetComponent<DACloneObjCtr>(out var cloneObj) && cloneObj.PropID == daObjCtr.PropID
                               ||
                                grabs.rightGO != null && grabs.rightGO.TryGetComponent(out cloneObj) && cloneObj.PropID == daObjCtr.PropID
                                )
                            {
                                res.tipInteract = true;

                                break;
                            }
                        }

                        //需要拿取高亮物体安装
                        var daSceneState = World.Get<DASceneState>();

                        if (daSceneState.cloneObjsInTable.TryGetValue(daObjCtr.PropID, out var objID))
                        {
                            res.cloneObjToTip = World.Get<DACloneObjCtr>(objID);
                        }
                    }
                    else if (daObjCtr.State == CmsObjState.Placed)
                    {
                        res.tipInteract = true;
                    }

                    break;
                case RecordStepType.Fix:
                    res.tipInteract = daObjCtr.State == CmsObjState.Assembled;
                    break;

                default:
                    Debug.LogError("Record type error! " + RecordType);
                    break;
            }

            return res;
        }

        protected override void UpdateTipObjs()
        {
            var hasInteractObjTiped = false;

            var tipCloneObjToPickupStep = false;

            foreach (var daObj in DaState.tipsInGuiding)
            {
                DAObjCtr daObjCtr = daObj as DAObjCtr;

                var (tipInteract, cloneObjToTip) = CheckDAObjTipAble(daObjCtr);

                if (daObjCtr.interactObj != null)
                {
                    if (tipInteract)
                    {
                        daObj.interactObj.TipInteractableObj();

                        hasInteractObjTiped = true;
                    }
                    else
                        daObj.interactObj.UnTipInteractableObj();
                }

                if (cloneObjToTip != null && !cloneObjTipMap.ContainsKey(daObjCtr))
                {
                    cloneObjTipMap.Add(daObjCtr, cloneObjToTip);
                }
                else if (cloneObjToTip == null && cloneObjTipMap.ContainsKey(daObjCtr))
                {
                    var cloneObj = cloneObjTipMap[daObjCtr];

                    if (cloneObj != null)
                        cloneObj.interactObj.UnTipInteractableObj();

                    cloneObjTipMap.Remove(daObjCtr);
                }
            }

            //安装步骤允许同时激活克隆部件和原始部件
            if ((hasInteractObjTiped && (RecordType & RecordStepType.Assemble) == 0) ||
                DaState.processingObjs.Count > 0)
                return;

            var placeToPartsTable = false;

            foreach (var kv in cloneObjTipMap)
            {
                if (kv.Value != null)
                {
                    if (!kv.Value.interactObj.isTiped)
                    {
                        kv.Value.interactObj.TipInteractableObj();

                        if (kv.Key.CloneObjToPickup != kv.Value)
                            continue;

                        if (kv.Value.dropGridPlane != null)
                        {
                            placeToPartsTable = true;
                        }

                        tipCloneObjToPickupStep = true;
                    }
                }
            }

            if (tipCloneObjToPickupStep)
            {
                var msg = placeToPartsTable ? "零件桌" : "指定地点";

                SendDAObjTipMessage($"请将拆下的物体放置到{msg}");
            }
        }

        public override void Clear()
        {
            DebugEx.AssertNotNull(DaState.tipsInGuiding);

            foreach (var daObj in DaState.tipsInGuiding)
            {
                if (daObj.interactObj != null)
                    daObj.interactObj.UnTipInteractableObj();
            }

            DaState.tipsInGuiding.Clear();

            foreach (var kv in cloneObjTipMap)
            {
                if (kv.Value != null)
                {
                    kv.Value.interactObj.UnTipInteractableObj();
                }
            }

            cloneObjTipMap = null;

            DaState.guidingProcessTarget = DAProcessTarget.None;

            //Debug.Log("Clear----");

            base.Clear();
        }
    }
}
