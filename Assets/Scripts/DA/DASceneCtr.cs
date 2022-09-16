using Doozy.Engine;
using Doozy.Engine.UI;
using Framework;
using Fxb.DA;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 塞入了大部分逻辑， 待拆
    /// </summary>
    public class DASceneCtr : SceneScript
    {
        public Transform padPos;

        public Transform carPos;

        private DASceneState SceneState => World.Get<DASceneState>();

        private IRecordModel RecordModel => World.Get<IRecordModel>();

        [Header("Debug")]

        public bool ignoreWrenchConditionCheck;

        public bool skipDAToolAnimation;

        //  public bool skipSafetyEquip;

        public string taskID;

        public DaTaskMode taskState;
        private GameObject padPosObj;

        ITaskModel TaskModel => World.Get<ITaskModel>();

        private DAState DaState => World.Get<DAState>();

        IEnumerator Start()
        {
            if (Debug.isDebugBuild)
                gameObject.AddComponent<DASceneDebug>();

            Application.targetFrameRate = 120;

            Message.AddListener<PartsTableDropObjChangeMessage>(OnPartsTableDropObjChangeMessage);

            Message.AddListener<DAObjStateChangeMessage>(OnObjStateChanged);

            Message.AddListener<CarLiftLocationChangedMessages>(OnLiftLocationChanged);

            Message.AddListener<DAToolErrorMessage>(OnDAToolError);
            Message.AddListener<WearEquipmentMessage>(OnWearEquipment);
            Message.AddListener<ReloadDaSceneMessage>(OnReloadDaScene);

            Message.AddListener<BatteryLiftDeviceStateChangeMessage>(OnBatteryLiftStateChanged);

            yield return new WaitForSeconds(1);

            gameObject.AddComponent<DASystem>();

            gameObject.AddComponent<DATaskGuide>();

            yield return null;

            Message.Send(new StartDAModeMessage()
            {
                mode = DAMode.DisassemblyAssembly,

                rootCtrs = new List<AbstractDAObjCtr>()
                {
                    World.Get<DAObjCtr>("204"),
                    World.Get<DAObjCtr>("203"),
                    World.Get<DAObjCtr>("201"),
                    World.Get<DAObjCtr>("8"),
                    World.Get<DAObjCtr>("4"),
                    World.Get<DAObjCtr>("2"),
                    World.Get<DAObjCtr>("1"),
                     World.Get<DAObjCtr>("3"),
                      World.Get<DAObjCtr>("5"),
                       World.Get<DAObjCtr>("6"),
                        World.Get<DAObjCtr>("7"),
                         World.Get<DAObjCtr>("9"),
                          World.Get<DAObjCtr>("5"),
                           World.Get<DAObjCtr>("10"),
                            World.Get<DAObjCtr>("11"),
                              World.Get<DAObjCtr>("205"),
                                World.Get<DAObjCtr>("206"),
                                  World.Get<DAObjCtr>("301"),
                                    World.Get<DAObjCtr>("307"),
                                      World.Get<DAObjCtr>("204"),
                                        World.Get<DAObjCtr>("401"),
                                          World.Get<DAObjCtr>("501"),
                                            World.Get<DAObjCtr>("502"),
                                              World.Get<DAObjCtr>("503"),
                                                World.Get<DAObjCtr>("601"),
                                                  World.Get<DAObjCtr>("701"),
                                                  World.Get<DAObjCtr>("702"),
                                                  World.Get<DAObjCtr>("703"),


                }
            });

            StartCoroutine(CheckTaskCompleted());

            TryInitWithTask();
        }

        private void OnBatteryLiftStateChanged(BatteryLiftDeviceStateChangeMessage msg)
        {
            SceneState.batteryLiftDeviceState = msg.newState;

            RecordModel.Record(RecordStepType.BatteryLift, ((int)msg.byAction).ToString());
        }

        private void OnDestroy()
        {
            Message.RemoveListener<DAObjStateChangeMessage>(OnObjStateChanged);

            Message.RemoveListener<DAToolErrorMessage>(OnDAToolError);

            Message.RemoveListener<PartsTableDropObjChangeMessage>(OnPartsTableDropObjChangeMessage);

            Message.RemoveListener<CarLiftLocationChangedMessages>(OnLiftLocationChanged);

            Message.RemoveListener<WearEquipmentMessage>(OnWearEquipment);

            Message.RemoveListener<ReloadDaSceneMessage>(OnReloadDaScene);

            Message.RemoveListener<BatteryLiftDeviceStateChangeMessage>(OnBatteryLiftStateChanged);

            World.current.Injecter.UnRegist<ITaskModel>();

            var sceneState = World.Get<DASceneState>();

            if (sceneState != null)
            {
                var targetMode = sceneState.taskMode;
                var targetTasklD = sceneState.taskID2Init;
                var taskIDGroupingModeInit = sceneState.taskIDGroupingModeInit;

                World.current.Injecter.UnRegist<DASceneState>();

                var newSceneState = World.current.Injecter.Regist<DASceneState>();
                newSceneState.taskMode = targetMode;
                newSceneState.taskID2Init = targetTasklD;
                newSceneState.taskIDGroupingModeInit = taskIDGroupingModeInit;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            AnimWrenchCtr.partsPreviewPrefab = Resources.Load<GameObject>(PathConfig.PREFAB_PATH_COMBINE_WRENCH_PREVIEW).transform;

            padPosObj = Instantiate(Resources.Load<GameObject>(PathConfig.PREFAB_PATH_PAD), padPos);

            padPosObj.transform.ResetLocalMatrix();

            Instantiate(Resources.Load<GameObject>(PathConfig.PREFAB_PATH_CAR), carPos).transform.ResetLocalMatrix();

            if (SceneState == null)
                World.current.Injecter.Regist<DASceneState>();
#if UNITY_EDITOR
            padPosObj.transform.localScale = new Vector3(2f, 2f, 2f);
            padPosObj.GetComponentInChildren<AdvancedInteractableObj>().autoGrabOnFocusSetIfEnable = false;
            //padPosObj.AddMissingComponent<ModelTransformRecorder>(out var modelTransformRecorder);
            //modelTransformRecorder.Record();
#endif
        }

        private void OnPartsTableDropObjChangeMessage(PartsTableDropObjChangeMessage message)
        {
            if (message.objOnTable == null)
            {
                //被取走
                SceneState.cloneObjsInTable.Remove(message.propId);
            }
            else
            {
                if (SceneState.cloneObjsInTable.ContainsKey(message.propId))
                {
                    SceneState.cloneObjsInTable[message.propId] = message.objOnTable.ID;
                }
                else
                {
                    SceneState.cloneObjsInTable.Add(message.propId, message.objOnTable.ID);
                }
            }
        }

        private void OnWearEquipment(WearEquipmentMessage msg)
        {
            RecordModel.Record(RecordStepType.Equip, ((int)msg.equipName).ToString());
        }

        private void OnObjStateChanged(DAObjStateChangeMessage msg)
        {
            if (TaskModel != null && TaskModel.IsSubmitAllTask)
                return;

            var cutState = msg.objCtr.State;

            var preState = msg.preState;

            var recordType = RecordStepType.None;

            switch (cutState)
            {
                case CmsObjState.Dismantled:
                    recordType = RecordStepType.Dismantle;
                    break;
                case CmsObjState.Assembled:
                    recordType = RecordStepType.Assemble;
                    break;
                case CmsObjState.Fixed:
                    recordType = RecordStepType.Fix;
                    break;
            }

            RecordModel.Record(recordType, msg.objCtr.ID);
        }

        private void OnDAToolError(DAToolErrorMessage message)
        {
            //Debug.LogWarning("Wrench use error tip:" + message.tipInfo);

            switch (message.daAnimType)
            {
                case AbstractDAScript.DAAnimType.Disassemble:
                    RecordModel.RecordError(RecordStepType.Dismantle, message.daObjID, ErrorRecordType.InvalidTools);
                    break;
                case AbstractDAScript.DAAnimType.Assemble:
                    RecordModel.RecordError(RecordStepType.Assemble, message.daObjID, ErrorRecordType.InvalidTools);
                    break;
                case AbstractDAScript.DAAnimType.Fix:
                    RecordModel.RecordError(RecordStepType.Fix, message.daObjID, ErrorRecordType.InvalidTools);
                    break;
            }
        }

        private IEnumerator CheckTaskCompleted()
        {
            var completedDelay = new WaitForSeconds(1.0f);

            while (TaskModel == null)
                yield return completedDelay;

            while (!TaskModel.CheckAllTaskCompleted())
            {
                yield return completedDelay;
            }

            TaskModel.SubmitTask();

            if (SceneState.taskMode != DaTaskMode.GroupingMode)
                UIView.ShowView(DoozyNamesDB.VIEW_CATEGORY_PAD, DoozyNamesDB.VIEW_PAD_RECORD);

            var comepletedString = SceneState.taskMode == DaTaskMode.GroupingMode ? "分组任务已完成" : "所有任务完成！任务已提交！";

            Popup_Tips.Show(comepletedString, null, true);

            var ComepletedMsg = new GuideTipMessage
            {
                tip = comepletedString
            };
            Message.Send(ComepletedMsg);

            if (SceneState.taskMode == DaTaskMode.GroupingMode)
            {
                SceneState.taskIDGroupingModeInit.Remove(TaskModel.GetTaskIDs()[0]);

                if (SceneState.taskIDGroupingModeInit.Count > 0)
                    StartCoroutine(SetGroupingModeChildTaskStart());
                else
                    SceneState.taskIDGroupingModeInit = null;

            }
        }

        IEnumerator SetGroupingModeChildTaskStart()
        {
            yield return new WaitForSeconds(2f);

            OnReloadDaScene(new ReloadDaSceneMessage { });
        }

        void TryInitWithTask()
        {
            string newID;

            var daState = World.Get<DASceneState>();

            newID = daState.taskID2Init;

#if UNITY_EDITOR
            //safetyEquip = !skipSafetyEquip;

            newID = string.IsNullOrWhiteSpace(newID) ? taskID : newID;

            if (daState.taskMode == DaTaskMode.None)
                daState.taskMode = taskState;

            DAConfig.ignoreWrenchConditionCheck = ignoreWrenchConditionCheck;

            DAConfig.skipToolAnimation = skipDAToolAnimation;

#endif

            string[] taskIDs = null;

            taskIDs = daState.taskMode == DaTaskMode.GroupingMode ? new string[] { daState.taskIDGroupingModeInit.ToArray()[0] } : new string[] { newID };

            if (taskIDs == null || taskIDs.Length == 0 || string.IsNullOrWhiteSpace(taskIDs[0]))
                return;

            if (daState.taskMode == DaTaskMode.None)
                daState.taskMode = taskState;

            daState.taskID2Init = null;
            //Debug.Log($"{taskIDs}COn---------------{taskIDs[0]}");


            World.current.Injecter.Regist<ITaskModel>(new TaskModel(taskIDs)).Init();

            if (SceneState.taskMode != DaTaskMode.GroupingMode)
                UIView.ShowView(DoozyNamesDB.VIEW_CATEGORY_PAD, DoozyNamesDB.VIEW_PAD_TASKDETAIL);

            Message.Send(new PrepareTaskMessage());
        }

        void OnReloadDaScene(ReloadDaSceneMessage msg)
        {
            SceneManager.LoadScene(0);
        }

        private void OnLiftLocationChanged(CarLiftLocationChangedMessages msg)
        {
            if (TaskModel != null && TaskModel.IsSubmitAllTask)
                return;

            var liftLocation = World.Get<CmsCarState>().liftLocation;

            //print(liftLocation);

            //小于3的整数
            if (int.TryParse(liftLocation.ToString(), out int result) && liftLocation < 3)
            {
                if (!World.Get<CmsCarState>().liftUp && liftLocation != 0)
                    liftLocation += 0.5f;

                RecordModel.Record(RecordStepType.LiftCar, liftLocation.ToString());
            }
        }
    }

    public class StructureAniStateMessage : Message
    {
        public bool isAniExpansion;
    }
}

