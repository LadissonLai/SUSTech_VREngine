using Doozy.Engine;
using Framework;
using Fxb.DA;
using HighlightPlus;
using System;
using System.Collections;
using UnityEngine;
using VRTK;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 控制扳手组合的相关逻辑。
    /// 理论上可以提炼一部分通用的逻辑，用在其它可以组装的地方
    /// </summary>
    [DisallowMultipleComponent]     //防止重复批量添加脚本
    public class WrenchCombineCtr : MonoBehaviour
    {
        protected VRAnimWrenchCtr wrenchCtr;

        public VRAnimWrenchCtr WrenchCtr
        {
            get
            {
                if(wrenchCtr == null)
                    wrenchCtr = GetComponent<VRAnimWrenchCtr>();

                return wrenchCtr;
            }
        }

        protected WrenchConfig WrenchConfig => World.Get<WrenchConfig>();

        public WrenchConfig.Item HandleConfigRD { get; protected set; }

        protected CombineAbleWrenchParts cutPlaceHolder;

        private Pose poseStartup;
 
        private void OnDisable()
        {
            Message.RemoveListener<ControllerGrabInteractObjMessage>(OnControllerGrabInteractObjMessage);
        }

        private void OnEnable()
        {
            Message.AddListener<ControllerGrabInteractObjMessage>(OnControllerGrabInteractObjMessage);
        }

        private void Start()
        {
            poseStartup = new Pose(transform.position, transform.rotation);

            //扳手脚本默认关闭 拼接完成后再打开
            WrenchCtr.enabled = false;
 
            if(wrenchCtr.handle == null)
                GenHandleById(wrenchCtr.WrenchInfo.banshou);

            HandleConfigRD = WrenchConfig.FindRowDatas(wrenchCtr.WrenchInfo.banshou);

            if (TryGetComponent<PanelSpawnTooltipTrigger>(out var tooltipTrigger))
            {
                if (string.IsNullOrEmpty(tooltipTrigger.customTooltipSpawnKey))
                    tooltipTrigger.customTooltipSpawnKey = PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_S;
            }
        }
         
        //抓取/放下物体后刷新相关内容。 实际的逻辑中，一只手需要拿取扳手，另一只手才能抓取扳手的零件
        private void OnControllerGrabInteractObjMessage(ControllerGrabInteractObjMessage msg)
        {
            if (msg.interactObj.gameObject == gameObject)
            {
                //考虑将接杆的碰撞默认隐藏
                return;
            }

            if (!msg.interactObj.CompareTag(TagConst.WrenchParts))
                return;

            var targetParts = msg.interactObj.GetComponent<CombineAbleWrenchParts>();

            if (msg.isGrab)
            {
                //抓起
                if (targetParts.AttachTo == wrenchCtr)
                {
                    //从扳手身上抓取
                    WrenchCtr.RemoveCombinedParts(targetParts.transform);

                    targetParts.AttachTo = null;
                }
                
                if(cutPlaceHolder == null && CheckPartsCombineAble(targetParts))
                    ShowPlaceHolderFromTarget(targetParts);
            }
            else
            {
                //默认放下物体后会将物体返回到之前的父节点下。会导致从扳手身上拆下来的接杆又回到扳手的父节点下。
                if(msg.interactObj != null && msg.interactObj.gameObject.activeInHierarchy)
                    msg.interactObj.transform.parent = null;

                //放下
                if (cutPlaceHolder == null)
                    return;

                HideCurrentPlaceHolder();
            }
        }

        /// <summary>
        /// 检查目标部件可否安装到扳手
        /// </summary>
        /// <param name="partsGO"></param>
        /// <returns></returns>
        private bool CheckPartsCombineAble(CombineAbleWrenchParts checkWrenchParts)
        {
            //是否需要抓取后才能安装待确定
            //if (!wrenchCtr.InteractObj.IsGrabbed())
            //    return false;
             
            if (Array.IndexOf(HandleConfigRD.KitArray, checkWrenchParts.ConfigData.Id) == -1)
            {
                return false;
            }
             
            return true;
        }

        private void HideCurrentPlaceHolder()
        {
            if (cutPlaceHolder == null)
                return;

            cutPlaceHolder.interactObj.InteractableObjectUnused -= OnPlaceHolderPartsUnused;

            Destroy(cutPlaceHolder.gameObject);
        }
         
        /// <summary>
        /// 生成placeholder
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isExtension">是否是接杆</param>
        private void ShowPlaceHolderFromTarget(CombineAbleWrenchParts target)
        {
            Debug.Assert(cutPlaceHolder == null);

            var configData = target.ConfigData;

            //根据目标的模型名称，从preview里面的对应原始套筒,接杆等
            var isExtension = configData.Type == WrenchPartsType.Extension;

            var parent = WrenchCtr.sleeveRoot;

            if (!isExtension && WrenchCtr.extension != null)
                parent = WrenchCtr.extension.Find("Connect");
              
            var parts = wrenchCtr.GenWrenchPartsByPrefabName(configData.PrefabName);

            parts.SetParent(parent);

            parts.ResetLocalMatrix();

            cutPlaceHolder = parts.GetComponent<CombineAbleWrenchParts>();

            cutPlaceHolder.DisplayAsPlaceHolder = true;

            cutPlaceHolder.interactObj.InteractableObjectUnused -= OnPlaceHolderPartsUnused;

            cutPlaceHolder.interactObj.InteractableObjectUnused += OnPlaceHolderPartsUnused;
        }

        private void OnPlaceHolderPartsUnused(object sender, InteractableObjectEventArgs e)
        {
            var grabObj =
                e.interactingObject == VRTKHelper.LeftHand.gameObject
                ? VRTKHelper.LeftGrab.GetGrabbedObject()
                : VRTKHelper.RightGrab.GetGrabbedObject();

            if (grabObj == null || !grabObj.CompareTag(TagConst.WrenchParts))
                return;

            var grabParts = grabObj.GetComponent<CombineAbleWrenchParts>();
             
            Debug.Assert(cutPlaceHolder != null);

            HideCurrentPlaceHolder();

            var grabInteractObj = grabObj.GetComponent<AdvancedInteractableObj>();

            grabParts.AttachTo = wrenchCtr;

            grabInteractObj.gameObject.SetActive(false);

            grabInteractObj.ForceStopInteracting();

            grabInteractObj.gameObject.SetActive(true);
 
            WrenchCtr.AddCombineParts(grabParts.ConfigData, grabParts.transform);

            UpdateWrenchCombineStates();
        }
         
        private void UpdateWrenchCombineStates()
        {
            //有套筒就表示安装完成
            if(WrenchCtr.sleeve == null)
            {
                return;
            }

            HideCurrentPlaceHolder();

            WrenchCtr.enabled = true;

            Message.RemoveListener<ControllerGrabInteractObjMessage>(OnControllerGrabInteractObjMessage);

            //高亮效果需要重新刷新，让高亮的物体包括新组合的物体
            var hlEffect = GetComponent<HighlightEffect>();

            hlEffect.enabled = false;
            hlEffect.enabled = true;

            CreateNewHandleCopy();

            if (WrenchCtr.extension != null)
                CreateNewCombineParts(WrenchCtr.extension.GetComponent<CombineAbleWrenchParts>());

            CreateNewCombineParts(WrenchCtr.sleeve.GetComponent<CombineAbleWrenchParts>());

            StartCoroutine(DelayClear());
        }

        //一定要在帧尾执行清理 等待其它逻辑执行完毕
        private IEnumerator DelayClear()
        {
            yield return new WaitForEndOfFrame();

            Clear();
        }
 
        private void Clear()
        {
            if (WrenchCtr.extension != null)
                Destroy(WrenchCtr.extension.GetComponent<CombineAbleWrenchParts>());

            Destroy(WrenchCtr.sleeve.GetComponent<CombineAbleWrenchParts>());
 
            Destroy(this);
        }

        private void CreateNewHandleCopy()
        {
            var wrenchContainerPrefab = Resources.Load<GameObject>(PathConfig.PREFAB_PATH_WRENCH_CONTAINER);

            var newWrench = Instantiate(wrenchContainerPrefab);

            var newWrenchCtr = newWrench.GetComponent<VRAnimWrenchCtr>();

            newWrenchCtr.AddCombineParts(HandleConfigRD);

            newWrenchCtr.transform.position = poseStartup.position;

            newWrenchCtr.transform.rotation = poseStartup.rotation;
        }

        private void CreateNewCombineParts(CombineAbleWrenchParts parts)
        {
            if (parts == null)
                return;

            //组装好一个扳手后  重新创建被用掉的这些零件
            var newParts = wrenchCtr.GenWrenchPartsByPrefabName(parts.ConfigData.PrefabName);

            newParts.position = parts.poseStartup.position;

            newParts.rotation = parts.poseStartup.rotation;
        }
         
        /// <summary>
        /// 通过id生成扳手
        /// </summary>
        /// <param name="id"></param>
        public void GenHandleById(string id)
        {
            Debug.Assert(wrenchCtr.handle == null);
            
            var configRD = WrenchConfig.FindRowDatas(id);

            wrenchCtr.AddCombineParts(configRD);
        }
    }
}

