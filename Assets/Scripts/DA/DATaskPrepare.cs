using Framework;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using Doozy.Engine;
using System;
using VRTKExtensions;
using VRTK.GrabAttachMechanics;
using Framework.Tools;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 处理任务开始前的一些准备流程 目前只是播放一系列动画
    /// </summary>
    public class DATaskPrepare : MonoBehaviour
    {
        public string carName, structureObjName;

        //[Tooltip("前舱盖")]
        //public string carHoodName;
      
        private readonly string taskObjectsContainer= "TaskObjectsContainer{0}";

        //[Tooltip("升降机")]
        //public Transform lift;

        /// <summary>
        /// 升降机支架A
        /// </summary>
        Transform liftHolderA;

        Transform liftHolderB;

        Transform liftHolderC;

        Transform liftHolderD;

        List<string> animateTargetNames;

        Transform car, structureObj;

        Transform carHood;

        /// <summary>
        /// 前舱盖支撑杆
        /// </summary>
        Transform carHoodHolder;

        private List<GameObject> TaskObjectsContainers;

        private List<AdvancedInteractableObj> structureChildsInteracts;

        private Animation ani_structureObj;

        private AudioSource audioSource;

        private void Awake()
        {
            Message.AddListener<PrepareTaskMessage>(StartPrepare);
            Message.AddListener<StructureAniStateMessage>(OnStructureAniStateChange);
            audioSource = gameObject.AddMissingComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0;
        }

        private void OnStructureAniStateChange(StructureAniStateMessage obj)
        {
            var state= obj.isAniExpansion;

            World.Get<DASceneState>().isStructureAniCompleted = state;

            StructureObjHandle(state);
        }

        private void StructureObjHandle(bool isPlay)
        {
            if (isPlay)
            {
                structureObj.gameObject.SetActive(true);

                ani_structureObj.CrossFade("Zha");

                if (IsInvoking("HideStructureObj"))
                {
                    CancelInvoke("HideStructureObj");
                }
            }
            else
            {
                ani_structureObj.CrossFade("He");

                Invoke("HideStructureObj", 0.8333f);
            }
        }

        void HideStructureObj()
        {
            structureObj.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            //GatherCarTrans();

            StructureObjInit();

            GetEnginTaskObjectsContainerObj();
        }

        private void StructureObjInit()
        {
            structureObj = GameObject.Find(structureObjName).transform;

            ani_structureObj = structureObj.GetComponent<Animation>();

            structureChildsInteracts = new List<AdvancedInteractableObj>();

            structureObj.GetComponentsInChildren<AdvancedInteractableObj>(structureChildsInteracts);

            if (structureChildsInteracts == null || structureChildsInteracts.Count == 0)
            {
                DebugEx.Log($"{structureObj}---子物体没有交互代码，请注意查看");
                return;
            }

            foreach (var item in structureChildsInteracts)
            {
                item.isUsable = true;

                item.isGrabbable = true;

                item.holdButtonToGrab = false;

                item.stayGrabbedOnTeleport = true;

                var grabAttach=item.gameObject.AddMissingComponent<VRTK_ChildOfControllerGrabAttach>();

                grabAttach.precisionGrab = true;

                item.TryGetComponent<PanelSpawnTooltipTrigger>(out var panelSpawnTooltipTrigger);

                if (panelSpawnTooltipTrigger)
                    panelSpawnTooltipTrigger.customTooltipSpawnKey = PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_L;
                //var modelTransRecord=item.gameObject.AddMissingComponent<ModelTransformRecorder>();

                    //modelTransRecord.Record();
            }

            structureObj.gameObject.SetActive(false);
        }

        private void GetEnginTaskObjectsContainerObj()
        {
            TaskObjectsContainers = new List<GameObject>();

            car = GameObject.Find(carName).transform;

            for (int i = 0; i < 7; i++)
            {
                var obj = car.Find<Transform>(string.Format(taskObjectsContainer, i+1)).gameObject;

                if (obj&& !TaskObjectsContainers.Contains(obj))
                    TaskObjectsContainers.Add(obj);
            }
        }

        public void StartPrepare(PrepareTaskMessage msg)
        {
            //    Sequence sequence = DOTween.Sequence();

            //    //顺序 举升机支架 - 车舱盖 - 支撑杆 -  任务开始
            //    sequence.Append(
            //        liftHolderA.DOLocalRotate(new Vector3(0, 50, 0), 1f, RotateMode.LocalAxisAdd)).Join(
            //        liftHolderB.DOLocalRotate(new Vector3(0, -50, 0), 1f, RotateMode.LocalAxisAdd)).Join(
            //        liftHolderC.DOLocalRotate(new Vector3(0, -50, 0), 1f, RotateMode.LocalAxisAdd)).Join(
            //        liftHolderD.DOLocalRotate(new Vector3(0, 50, 0), 1f, RotateMode.LocalAxisAdd))
            //        //.AppendInterval(0.5f).Append(
            //        //lift.DOLocalMoveY(0.13f, 2f)).Insert(1.8f, car.DOLocalMoveY(0.11f, 1.7f)).AppendInterval(0.5f)
            //        .Append(
            //        carHood.DOLocalRotate(new Vector3(55, 0, 0), 1f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce)).
            //        Append(carHoodHolder.DOLocalRotate(new Vector3(-1.5f, 9.5f, -87.5f), 0.5f, RotateMode.LocalAxisAdd).
            //        SetEase(Ease.OutQuart)).AppendCallback(() => World.Get<DASceneState>().isTaskPreparing = false);
            World.Get<DASceneState>().isTaskPreparing = false;

            if (TaskObjectsContainers == null || TaskObjectsContainers.Count == 0)
                return;

            var taskmodel = World.Get<ITaskModel>();

            var taskid = int.Parse(taskmodel.GetTaskIDs()[0]);

            for (int i = 0; i < TaskObjectsContainers.Count; i++)
            {
                var index = taskid - 1;

                TaskObjectsContainers[i].SetActive(i >= index);
            }


            var ids = taskmodel.GetTaskIDs();
            var audioClip = Resources.Load<AudioClip>(string.Format(PathConfig.Task_AudioClip_Path, ids[0]));

            if (audioClip)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }

        private void OnDestroy()
        {
            Message.RemoveListener<PrepareTaskMessage>(StartPrepare);
            Message.RemoveListener<StructureAniStateMessage>(OnStructureAniStateChange);
        }

        #region Helper

        void FindChild(Transform parent, List<string> names, ref Dictionary<int, Transform> prepareTargets)
        {
            for (int i = 0; i < names.Count; i++)
            {
                if (prepareTargets.ContainsKey(i))
                    continue;

                var child = parent.Find(names[i]);

                if (child)
                    prepareTargets[i] = child;
            }

            if (prepareTargets.Count == names.Count)
                return;

            for (int i = 0; i < parent.childCount; i++)
            {
                FindChild(parent.GetChild(i), names, ref prepareTargets);

                if (prepareTargets.Count == names.Count)
                    return;
            }
        }

        Transform FindChild(Transform parent, string name)
        {
            var child = parent.Find(name);

            if (child)
                return child;

            for (int i = 0; i < parent.childCount; i++)
            {
                child = FindChild(parent.GetChild(i), name);

                if (child)
                    return child;
            }

            return null;
        }

        #endregion
    }


    public class PrepareTaskMessage : Message { }
}

#region old logic
//void GatherCarTrans()
//{
//    car = GameObject.Find(carName).transform;

//    animateTargetNames = new List<string>() { carHoodName };

//    Dictionary<int, Transform> targets = new Dictionary<int, Transform>();

//    FindChild(car, animateTargetNames, ref targets);

//    carHood = targets[0];

//    carHoodHolder = carHood.GetChild(0).GetChild(0);
//}

//void GatherLiftTrans()
//{
//    liftHolderA = lift.GetChild(1);

//    liftHolderB = lift.GetChild(2);

//    liftHolderC = lift.GetChild(3);

//    liftHolderD = lift.GetChild(4);
//}
#endregion