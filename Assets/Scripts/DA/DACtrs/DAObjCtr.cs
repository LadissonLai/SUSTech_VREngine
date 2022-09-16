/*
 碰撞体细节：
 1.如果物体可以被拆下，在拆下后激活成PlaceHolder状态时会将碰撞体开启trigger来供抓取拷贝的物体来安装.
 如果此物体身上的碰撞体大部分或者全都是meshCollider，需要单独挂一个trigger碰撞体，此碰撞体只作为拷贝物体的交互，会忽略手部及射线交互。
 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine;
using Framework;
using Fxb.DA;
using Fxb.Localization;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public abstract class DAObjCtr : AbstractDAObjCtr, ICloneObjDropAble
    {
        private List<AbstractDAObjCtr> dependPartsCache;

        private List<AbstractDAObjCtr> dependSnapFitsCache;

        protected DAState DaState => World.Get<DAState>();

        private DACsvConfig DAConfig => World.Get<DACsvConfig>();

        private PropCsvConfig PropConfig => World.Get<PropCsvConfig>();

        private DACsvConfig.Item configData;

        /// <summary>
        /// 调用UpdateObjInteractAble后缓存
        /// </summary>
        protected List<Collider> collidersCache;

        /// <summary>
        /// 默认情况下开启了isTrigger的碰撞体用作PlaceHolder时交互用。
        /// 自动缓存
        /// </summary>
        protected HashSet<Collider> placeHolderTriggerColliders;

        /// <summary>
        /// 指定用来安装的克隆物体的路径，替代自动拷贝mesh生成克隆物体的操作
        /// </summary>
        public string customDaCloneObjPath;

        /// <summary>
        /// 用作放置在桌面上的姿态确定
        /// </summary>
        public DAGridPlane dropGridPlane;

        public Transform leftSnapHandleToCopy;

        public Transform rightSnapHandleToCopy;

        private IDAProcessAssertAble[] customProcessConditions;

        public override List<AbstractDAObjCtr> DependParts
        {
            get
            {
                GenDependDAObjsByIDStr(configData.DependParts, ref dependPartsCache);

                return dependPartsCache;
            }
        }

        public override List<AbstractDAObjCtr> DependSnapFits
        {
            get
            {
                GenDependDAObjsByIDStr(configData.DependSnapFits, ref dependSnapFitsCache);

                return dependSnapFitsCache;
            }
        }

        private string idfromConfig;

        private bool waitForFirstActived;

        public override string ID => idfromConfig;

        public override string Name => configData.Name;

        /// <summary>
        /// 物品id  待改成从配置中获取
        /// </summary>
        public string PropID => configData.PropID;

        DATipMessage tipMessageCache;

        public DACloneObjCtr CloneObjToPickup
        {
            get;
            protected set;
        }

        protected override Material PlaceHolderMat
        {
            get
            {
                return Resources.Load<Material>(PathConfig.MAT_PATH_DAPLACEHOLDER);
            }
        }

        protected void GenDependDAObjsByIDStr(string idStr, ref List<AbstractDAObjCtr> list)
        {
            if (list != null || string.IsNullOrWhiteSpace(idStr))
                return;

            list = new List<AbstractDAObjCtr>();

            var idArr = idStr.Split(',');

            foreach (var depandId in idArr)
            {
                if (string.IsNullOrWhiteSpace(depandId))
                    continue;

                var depandObj = World.Get<DAObjCtr>(depandId);

                DebugEx.AssertNotNull(depandObj,
                  $"id:{depandId} 未找到");

                //DebugEx.AssertNotNull(depandObj,
                //    $"id:{depandId} 未找到  gameobj:{World.Get<DACsvConfig>().FindRowDatas(depandId).ModelName}");

                list.Add(depandObj);
            }
        }

        protected override void Awake()
        {
            configData = World.Get<DACsvConfig>().FindRDByModelName(name);

            Debug.Assert(configData != null, $"dacsvconfig搜索失败. {name}");

            idfromConfig = configData.Id;

            base.Awake();

            //会拆下的物体，如果没有指定外部的克隆物体则需要添加平面网格来确定放到桌上的姿态
            //Debug.Assert(!autoDisappear || !string.IsNullOrEmpty(customDaCloneObjPath) || dropGridPlane != null);

            //网格尺寸暂时写死2厘米
            Debug.Assert(dropGridPlane == null || dropGridPlane.gridSize == 0.02f);

            Debug.Assert(dropGridPlane == null || dropGridPlane.transform == transform || dropGridPlane.transform.parent == transform, "嵌套的plane不方便克隆");

            World.current.Injecter.Regist(this, ID);

            if (interactObj == null)
                return;

            var propData = PropConfig.FindRowDatas(PropID);

            if (string.IsNullOrEmpty(customDaCloneObjPath))
            {            
                if (propData != null && !string.IsNullOrEmpty(propData.CustomClonePath))
                {
                    customDaCloneObjPath = $"{PathConfig.CLONE_OBJS_PATH_ROOT_}{propData.CustomClonePath}";
                }
            }

            //冒泡功能在vr下目前没有用上
            interactObj.enableBubbling = false;

            interactObj.readableName = Name;

            if (string.IsNullOrEmpty(interactObj.readableName) && propData != null)
            {
                interactObj.readableName = propData.Name;
            }
        }

        protected virtual IEnumerator Start()
        {
            SetupDependObjs();

            yield return new WaitForEndOfFrame();

            SetupInteractParents();

            if (interactObj != null && interactObj.isGrabbable)
            {
                Debug.LogError("拆装物体默认不能允许被抓取");

                interactObj.isGrabbable = false;
            }
        }

        protected virtual void OnDestroy()
        {
            World.current.Injecter.UnRegist<DAObjCtr>(ID);

            Message.RemoveListener<CloneObjPickupMessage>(OnCloneObjPickupMessage);

            Message.RemoveListener<ControllerGrabInteractObjMessage>(OnControllerGrabObjMessage);
        }

        protected virtual void OnEnable()
        {
            if (interactObj == null)
                return;

            interactObj.InteractableObjectUsed += InteractObj_InteractableObjectUsed;

            interactObj.InteractableObjectUnused += InteractObj_InteractableObjectUnused;
        }

        private void OnDisable()
        {
            if (interactObj == null)
                return;

            interactObj.InteractableObjectUsed -= InteractObj_InteractableObjectUsed;

            interactObj.InteractableObjectUnused -= InteractObj_InteractableObjectUnused;
        }

        /// <summary>
        /// 每次process被交互的手
        /// </summary>
        private GameObject interactProcessHandCache;

        //射线很容易误触碰  关闭此功能
        //private void InteractObj_InteractableObjectFocusSet(object sender, InteractableObjectEventArgs e)
        //{
        //    if (DaState?.processingObjs?.Count > 0)
        //        return;

        //    interactProcessHandCache = e.interactingObject;

        //    var grabObj =
        //    interactProcessHandCache == VRTKHelper.LeftHand.gameObject
        //    ? VRTKHelper.LeftGrab.GetGrabbedObject()
        //    : VRTKHelper.RightGrab.GetGrabbedObject();

        //    IDAUsingTool usingTool = grabObj == null ? null : grabObj.GetComponent<IDAUsingTool>();

        //    DoProcess(usingTool);
        //}

        private void InteractObj_InteractableObjectUnused(object sender, VRTK.InteractableObjectEventArgs e)
        {
            if (DaState?.processingObjs?.Count > 0)
                return;

            //Debug.Log("InteractObj_InteractableObjectUnused:" + sender + "|" + e.interactingObject);

            interactProcessHandCache = e.interactingObject;

            var grabObj =
                interactProcessHandCache == VRTKHelper.LeftHand.gameObject
                ? VRTKHelper.LeftGrab.GetGrabbedObject()
                : VRTKHelper.RightGrab.GetGrabbedObject();

            IDAUsingTool usingTool = grabObj == null ? null : grabObj.GetComponent<IDAUsingTool>();

            DoProcess(usingTool);
        }

        private void InteractObj_InteractableObjectUsed(object sender, VRTK.InteractableObjectEventArgs e)
        {
            //有可能因为物品使用时没有调用stopusing导致下一次无法正常交互  待关注
            //Debug.Log("ToolInteractObj_InteractableObjectUsed:" + sender + "|" + e.interactingObject);
        }

        protected override IEnumerator AppearWithAnim(IDAUsingTool usingTool)
        {
            var daCloneObj = usingTool as DACloneObjCtr;

            Debug.Assert(daCloneObj != null);

            var cloneObjAmount = daCloneObj.GetAmount();

            DACloneObjCtr targetCloneObj = null;

            if (cloneObjAmount > 1)
            {
                targetCloneObj = daCloneObj.SeparateNext() as DACloneObjCtr;
            }
            else
            {
                targetCloneObj = daCloneObj;
            }

            targetCloneObj.Place();

            return base.AppearWithAnim(usingTool);
        }

        public override void DoAssemble(IDAUsingTool usingTool = null)
        {
            if (autoDisappear && DisplayMode != CmsDisplayMode.Default)
            {
                //会消失的物体需要抓取物体后安装
                var daCloneObj = usingTool as DACloneObjCtr;

                if (daCloneObj == null || daCloneObj.PropID != PropID)
                    return;
            }

            base.DoAssemble(usingTool);
        }

        protected virtual bool CheckProcessCondition(DAProcessTarget processTarget)
        {
            var GUID_ERROR_MSG = "实训模式下请按照指引进行操作！";

            if (World.Get<DASceneState>().isGuiding)
            {
                //实训模式只能处理当前正在提示的物体. 
                //TODO 逻辑待更改 依赖数据，不依赖物体的显示状态
                if (!interactObj.isTiped)
                {
                    DATipMessage.Send(GUID_ERROR_MSG, ref tipMessageCache);

                    return false;
                }
            }

            if (DaState.guidingProcessTarget != DAProcessTarget.None && DaState.guidingProcessTarget != processTarget)
            {
                if (DaState.guidingProcessTarget != DAProcessTarget.Assemble || processTarget != DAProcessTarget.Place)
                {
                    //指引没有放置操作。 允许安装时也允许放置
                    DATipMessage.Send(GUID_ERROR_MSG, ref tipMessageCache);

                    return false;
                }
            }

            if (customProcessConditions != null && customProcessConditions.Length > 0)
            {
                foreach (var condition in customProcessConditions)
                {
                    if ((condition.ProcessTarget & processTarget) == 0)
                        continue;

                    if (!condition.Check(this, out var msg))
                    {
                        DATipMessage.Send(msg, ref tipMessageCache);

                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 根据工具来判断具体是安装还是拆卸等
        /// 如：
        /// 1.使用棘轮扳手时可以拆下也可以装上。使用扭力扳手时只能紧固。
        /// 2.使用手直接操作时，可以拆下也可以装上。
        /// </summary>
        /// <param name="usingTool"></param>
        protected override void DoProcess(IDAUsingTool usingTool = null)
        {
            if (DaState?.processingObjs?.Count > 0)
                return;

            //???  此处逻辑是否有用待确定
            //if (CmsDAUtils.CheckDepandObjExist(this, CmsObjType.Parts, CmsObjState.Dismantable, out var _))
            //{
            //    tipMessageCache = tipMessageCache ?? new DATipMessage();

            //    tipMessageCache.tipInfo = "有依赖部件未拆除";

            //    Message.Send(tipMessageCache);

            //    return;
            //}

            if (!World.Get<DAState>().isRunning)
                return;

            if (State == CmsObjState.Dismantled || State == CmsObjState.Placed)
            {
                //可以安装优先安装
                if (CheckProcessCondition(DAProcessTarget.Assemble))
                    DoAssemble(usingTool);
            }
            else if ((State & CmsObjState.Dismantable) != 0)
            {
                //可以拆也可以装
                if (State == CmsObjState.Assembled && (usingTool as Component) != null && usingTool.FixUsingAble)
                {
                    if (CheckProcessCondition(DAProcessTarget.Fix))
                        DoFix(usingTool);
                }
                else
                {
                    if (CheckProcessCondition(DAProcessTarget.Dismantle))
                        DoDisassemble(usingTool);
                }
            }
        }

        protected void SetupInteractParents()
        {
            DebugEx.AssertIsTrue(attachTo != null || Type == CmsObjType.ModelGroup, $"attachTo null:{name}");

        }

        protected virtual void SetupDependObjs()
        {

        }

        protected override void InitActiveds()
        {
            SetActived(false);
        }

        protected override void ChangeToDisplayPlaceHolder()
        {
            base.ChangeToDisplayPlaceHolder();

            foreach (var c in collidersCache)
            {

                if (c is MeshCollider)
                    continue;

                c.isTrigger = true;

                if (placeHolderTriggerColliders != null && placeHolderTriggerColliders.Contains(c))
                {
                    c.enabled = true;
                }
            }
        }

        protected override void ChangeToDefaultDisplay()
        {
            base.ChangeToDefaultDisplay();

            foreach (var c in collidersCache)
            {
                if (placeHolderTriggerColliders != null && placeHolderTriggerColliders.Contains(c))
                {
                    c.enabled = false;
                }
                else
                    c.isTrigger = false;
            }
        }

        public override void SetActived(bool actived)
        {
            if (IsActive == actived)
                return;

            IsActive = actived;

            UpdateObjInteractAble(actived);

            if (actived)
            {
                if (!waitForFirstActived)
                {
                    waitForFirstActived = true;

                    customProcessConditions = GetComponents<IDAProcessAssertAble>();
                }

                if (State == CmsObjState.Dismantled && autoDisappear)
                {
                    //需要抓取CloneObj进行安装
                    if (CheckValidPropGrabed())
                    {
                        SetDisplayMode(CmsDisplayMode.PlaceHolder);
                    }

                    Message.RemoveListener<ControllerGrabInteractObjMessage>(OnControllerGrabObjMessage);
                    Message.AddListener<ControllerGrabInteractObjMessage>(OnControllerGrabObjMessage);
                }
            }
            else
            {
                Message.RemoveListener<ControllerGrabInteractObjMessage>(OnControllerGrabObjMessage);

                if (State == CmsObjState.Dismantled && autoDisappear)
                    SetDisplayMode(CmsDisplayMode.Hide);
            }

            if (interactObj != null)
            {
                interactObj.isUsable = actived;
            }
        }

        private void OnControllerGrabObjMessage(ControllerGrabInteractObjMessage message)
        {
            //正常情况下只有被拆下的状态会监听
            Debug.Assert(state == CmsObjState.Dismantled);

            //未激活时理论上会清理此监听
            Debug.Assert(IsActive);

            //有可能已经提前放置（螺丝等）
            if (DisplayMode == CmsDisplayMode.Default)
                return;

            if (CheckValidPropGrabed())
                SetDisplayMode(CmsDisplayMode.PlaceHolder);
            else
                SetDisplayMode(CmsDisplayMode.Hide);
        }

        private bool CheckValidPropGrabed(GameObject objGrabed)
        {
            if (objGrabed != null && objGrabed.TryGetComponent<DACloneObjCtr>(out var cloneObj))
            {
                #region 此逻辑不合适  暂时屏蔽
                //grab的UnGrab事件触发时机在清理grabobj之前，需要判断是否被抓取状态  cloneObj.IsEnableAssemble &&

                #endregion

                if (cloneObj.interactObj.IsGrabbed() && cloneObj.PropID == PropID && !cloneObj.WaitForFistDrop)
                    return true;
            }

            return false;
        }

        private bool CheckValidPropGrabed()
        {
            if (VRTKHelper.FindAllGrabedGameObject(out var res))
            {
                if (CheckValidPropGrabed(res.leftGO) || CheckValidPropGrabed(res.rightGO))
                    return true;
            }

            return false;
        }

        protected override void UpdateObjInteractAble(bool enable)
        {
            //base.UpdateObjInteractAble(enable);       碰撞体的开关改由自身控制

            //Debug.Log($"name:{name} UpdateObjInteractAble:{enable}  displayMode:{DisplayMode}  State:{State}");

            if (interactObj != null)
            {
                interactObj.SetInteractAble(enable, false);

                collidersCache = collidersCache ?? new List<Collider>();

                var firstCacheColliders = false;

                if (collidersCache.Count == 0)
                {
                    interactObj.GetComponentsInChildren(true, collidersCache);

                    for (int i = collidersCache.Count - 1; i >= 0; i--)
                    {
                        if (collidersCache[i].GetComponentInParent<AdvancedInteractableObj>() != interactObj)
                            collidersCache.RemoveAt(i);
                    }

                    firstCacheColliders = true;
                }

                foreach (var c in collidersCache)
                {
                    if (firstCacheColliders && autoDisappear)
                    {
                        if (c.isTrigger)
                        {
                            placeHolderTriggerColliders = placeHolderTriggerColliders ?? new HashSet<Collider>();

                            placeHolderTriggerColliders.Add(c);
                        }
                    }

                    //默认的trigger碰撞器控制交给DisplayChanged里面实现
                    if (placeHolderTriggerColliders != null && placeHolderTriggerColliders.Contains(c))
                        c.enabled = false;
                    else
                        c.enabled = enable;
                }
            }
        }

        protected override void OnDisassembled(bool success)
        {
            base.OnDisassembled(success);

            if (success && state == CmsObjState.WaitForPickup)
            {
                //拆卸成功 并且当前状态为Assembled 表示需要等待取走克隆物体完成最终拆下流程
                //后续通过监听克隆物体被取走事件完成切换到Dismantled状态
                Debug.Assert(autoDisappear);

                Debug.Assert(CloneObjToPickup == null);

                Message.RemoveListener<CloneObjPickupMessage>(OnCloneObjPickupMessage);

                Message.AddListener<CloneObjPickupMessage>(OnCloneObjPickupMessage);

                CloneObjToPickup = CreateCopyModelInteractObj();

                if (disappearEffect.moveDistancs < 0.001f)
                {
                    //没有移动动画，默认自动抓取
                    var grab =
                        interactProcessHandCache == VRTKHelper.LeftHand.gameObject
                        ? VRTKHelper.LeftGrab
                        : VRTKHelper.RightGrab;

                    if (grab.GetGrabbedObject() != null)
                        return;

                    var hand = interactProcessHandCache == VRTKHelper.LeftHand.gameObject
                        ? SDK_BaseController.ControllerHand.Left
                        : SDK_BaseController.ControllerHand.Right;

                    VRTKHelper.ForceGrab(hand, CloneObjToPickup.gameObject);
                }
            }
        }

        private void OnCloneObjPickupMessage(CloneObjPickupMessage message)
        {
            Debug.Assert(CloneObjToPickup != null);

            if (message.target != CloneObjToPickup)
                return;

            Debug.Assert(state == CmsObjState.WaitForPickup);

            Message.RemoveListener<CloneObjPickupMessage>(OnCloneObjPickupMessage);

            CloneObjToPickup = null;

            DoPickup();
        }

        protected void DoPickup()
        {
            Debug.Assert(state == CmsObjState.WaitForPickup);

            //克隆物体被取走后切换到 Dismantled状态
            State = CmsObjState.Dismantled;
        }

        protected virtual GameObject CopyModel(Transform tRoot)
        {
            var cloneGO = new GameObject
            {
                name = "model root"
            };

            //拷贝出来的物体身上也会带有rigidbody
            VRTKHelper.CopyModelNew(target: transform, givenParent: cloneGO.transform, includeColider: true);

            cloneGO.transform.SetParent(tRoot);

            cloneGO.transform.localPosition = Vector3.zero;

            cloneGO.transform.rotation = Quaternion.identity;

            return cloneGO;
        }

        protected virtual void CopyDropGridPlane(DACloneObjCtr cloneObjCtr)
        {
            GameObject cloneGridGO = null;

            if (dropGridPlane.transform == transform)
            {
                cloneGridGO = cloneObjCtr.gameObject;
            }
            else
            {
                cloneGridGO = new GameObject(dropGridPlane.name);

                cloneGridGO.transform.SetParent(cloneObjCtr.transform);

                cloneGridGO.transform.localPosition = dropGridPlane.transform.localPosition;

                //修改为世界旋转
                cloneGridGO.transform.rotation = dropGridPlane.transform.rotation;
            }

            var cloneGridPlane = VRTK_SharedMethods.CloneComponent(dropGridPlane, cloneGridGO, false) as DAGridPlane;

            cloneObjCtr.dropGridPlane = cloneGridPlane;
        }

        protected virtual void CopySnapHandle(DACloneObjCtr cloneObjCtr)
        {
            if (cloneObjCtr.TryGetComponent<VRTK_BaseGrabAttach>(out var grabAttach))
            {
                grabAttach.precisionGrab = false;

                if (leftSnapHandleToCopy != null)
                {
                    grabAttach.leftSnapHandle = CreateNewTempGO(leftSnapHandleToCopy, "leftSnapHandle", grabAttach.transform).transform;
                }

                //如果左右handle一样，只需要赋值一个即可
                if (rightSnapHandleToCopy != null && rightSnapHandleToCopy != leftSnapHandleToCopy)
                {
                    grabAttach.rightSnapHandle = CreateNewTempGO(rightSnapHandleToCopy, "RightSnapHandle", grabAttach.transform).transform;
                }
            }
        }

        protected virtual void CopyOrUpdateTooltipTrigger(DACloneObjCtr cloneObjCtr)
        {
            var srcTrigger = GetComponent<PanelSpawnTooltipTrigger>();

            if (srcTrigger == null)
                return;

            if (cloneObjCtr.TryGetComponent<PanelSpawnTooltipTrigger>(out var targetTrigger))
            {
                targetTrigger.customTooltipSpawnKey = srcTrigger.customTooltipSpawnKey;
                return;
            }

            targetTrigger = VRTK_SharedMethods.CloneComponent(srcTrigger, cloneObjCtr.gameObject, false) as PanelSpawnTooltipTrigger;

            if (targetTrigger.customOrigin != null)
            {
                var newCustomOrigin = new GameObject("CustomOrigin");

                newCustomOrigin.transform.position = targetTrigger.customOrigin.position;

                newCustomOrigin.transform.rotation = targetTrigger.customOrigin.rotation;

                newCustomOrigin.transform.SetParent(cloneObjCtr.transform);

                targetTrigger.customOrigin = newCustomOrigin.transform;
            }
        }

        /// <summary>
        /// 生成拷贝模型物体 带碰撞体
        /// </summary>
        protected DACloneObjCtr CreateCopyModelInteractObj()
        {
            string prefabPath = customDaCloneObjPath;

            bool needCopyModel = false;

            if (string.IsNullOrEmpty(prefabPath))
            {
                prefabPath = "Prefabs/EmptyCloneObjContainer";

                needCopyModel = true;
            }

            var cloneObjCtr = Instantiate(Resources.Load<GameObject>(prefabPath)).GetComponent<DACloneObjCtr>();

            Debug.Assert(cloneObjCtr != null, $"克隆物体身上需要挂载{typeof(DACloneObjCtr)}脚本");

            cloneObjCtr.transform.position = transform.position;

            cloneObjCtr.transform.rotation = transform.rotation;

            cloneObjCtr.PropID = PropID;

            cloneObjCtr.interactObj.readableName = Name;

            if (needCopyModel)
            {
                var cloneGO = CopyModel(cloneObjCtr.transform);

                if (dropGridPlane != null)
                {
                    CopyDropGridPlane(cloneObjCtr);
                }

                if (leftSnapHandleToCopy != null || rightSnapHandleToCopy != null)
                {
                    CopySnapHandle(cloneObjCtr);
                }

                cloneObjCtr.isGenerateByModelCopy = true;

                //目前高亮相关脚本在新添加子节点后需要强刷一次onEnable
                cloneObjCtr.gameObject.SetActive(false);
                cloneObjCtr.gameObject.SetActive(true);
            }

            CopyOrUpdateTooltipTrigger(cloneObjCtr);

            //螺丝默认的层级会忽略手部交互，待确定是否需要此逻辑
            if (interactObj != null && interactObj.gameObject.layer == LayerConst.IgnoreHandTouch)
                cloneObjCtr.gameObject.ApplyLayer(LayerConst.Default);

            return cloneObjCtr;
        }

        private GameObject CreateNewTempGO(Transform template, string name, Transform newParent)
        {
            var newGO = new GameObject(name);

            newGO.transform.position = template.position;

            newGO.transform.rotation = template.rotation;

            newGO.transform.localScale = template.lossyScale;

            newGO.transform.SetParent(newParent);

            return newGO;
        }

        public virtual bool CheckCloneObjDropAble(DACloneObjCtr target)
        {
            return target.PropID == PropID;
        }
    }
}
