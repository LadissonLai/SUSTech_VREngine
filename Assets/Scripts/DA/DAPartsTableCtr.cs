using Framework;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using static Fxb.CMSVR.DADorpAblePlane;

namespace Fxb.CMSVR
{
    public class DAPartsTableCtr : MonoBehaviour
    {
        /// <summary>
        /// 默认被忽略的空间
        /// </summary>
        public DAGridPlane[] ingoreZones;

        public DADorpAblePlane dropAblePlane;

        public VRTK_CollisionTracker collisionTracker;

        private List<DACloneObjCtr> objsChecking;

        /// <summary>
        /// propid->target
        /// </summary>
        private Dictionary<string, DACloneObjCtr> objsDroped;

        /// <summary>
        /// 当前帧还在碰撞体区域内的物体
        /// </summary>
        private HashSet<GameObject> collisionStays;

        private PartsTableDropObjChangeMessage placeMessageCache;

        private void Awake()
        {
            collisionTracker.TriggerStay += CollisionTracker_TriggerStay;

            collisionTracker.TriggerEnter += CollisionTracker_TriggerEnter;

            objsChecking = new List<DACloneObjCtr>();

            collisionStays = new HashSet<GameObject>();

            objsDroped = new Dictionary<string, DACloneObjCtr>();

            placeMessageCache = new PartsTableDropObjChangeMessage();
        }

        private void CollisionTracker_TriggerEnter(object sender, CollisionTrackerEventArgs e)
        {
            if (e.collider.isTrigger)
                return;

            var rigidBody = e.collider.attachedRigidbody;

            if (rigidBody == null)
                return;

            if (rigidBody.TryGetComponent<DACloneObjCtr>(out var cloneObjCtr))
            {
                AddCheckingObj(cloneObjCtr);
            }
        }

        private void CollisionTracker_TriggerStay(object sender, CollisionTrackerEventArgs e)
        {
            if (e.collider.isTrigger)
                return;

            var rigidBody = e.collider.attachedRigidbody;

            if (rigidBody == null)
                return;

            collisionStays.AddUnique(rigidBody.gameObject);
        }

        private void FixedUpdate()
        {
            for (int i = objsChecking.Count - 1; i >= 0; i--)
            {
                var objCtr = objsChecking[i];

                if (!collisionStays.Contains(objCtr.gameObject))
                {
                    RemoveCheckingObj(objCtr);
                }
            }

            //Debug.Log(collisionStays.Count + "|" + objInCollisions.Count);

            collisionStays.Clear();
        }

        private void InteractObj_InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            //Debug.Log($"InteractObj_InteractableObjectUngrabbed  sender:{sender}  interactingObj:{e.interactingObject}");

            if (World.current.IsAppQuitting)
                return;

            var cloneObj = (sender as Component).GetComponent<DACloneObjCtr>();

            Debug.Assert(cloneObj != null);
             
            if (cloneObj.gameObject.activeInHierarchy)
                AddDropedObj(cloneObj);
            
            RemoveCheckingObj(cloneObj);
        }

        private void InteractObj_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            var cloneObj = (sender as Component).GetComponentInParent<DACloneObjCtr>();

            Debug.Assert(cloneObj != null);

            RemoveDropedObj(cloneObj);

            AddCheckingObj(cloneObj);
        }

        private void AddCheckingObj(DACloneObjCtr cloneObj)
        {
            if (objsDroped.ContainsValue(cloneObj))
                return;

            if (objsChecking.Contains(cloneObj))
                return;

            objsChecking.Add(cloneObj);

            cloneObj.interactObj.InteractableObjectUngrabbed -= InteractObj_InteractableObjectUngrabbed;

            cloneObj.interactObj.InteractableObjectUngrabbed += InteractObj_InteractableObjectUngrabbed;
        }

        private void RemoveCheckingObj(DACloneObjCtr cloneObj)
        {
            if (!objsChecking.Contains(cloneObj))
                return;

            objsChecking.Remove(cloneObj);

            if (cloneObj.dropGridPlane != null)
            {
                dropAblePlane.RemoveProjectPlane(cloneObj.dropGridPlane);
            }

            cloneObj.SetValidDrop(false);

            cloneObj.interactObj.InteractableObjectUngrabbed -= InteractObj_InteractableObjectUngrabbed;
        }

        private bool CheckObjDropEnabel(DACloneObjCtr cloneObj, out ProjectChecker checker)
        {
            checker = dropAblePlane.GetProjectChecker(cloneObj.dropGridPlane);

            return checker == null ? false : checker.canDrop;
        }

        /// <summary>
        /// 添加已经放置的物体。
        /// </summary>
        /// <param name="cloneObj"></param>
        private void AddDropedObj(DACloneObjCtr cloneObj)
        {
            if (objsDroped.TryGetValue(cloneObj.PropID, out var existObj))
            {
                //已存在物体数量增加
                existObj.AddCloneObj(cloneObj);
            }
            else
            {
                //新物体
                //根据grid plane的检测结果存放物体

                var dropEnable = CheckObjDropEnabel(cloneObj, out var projectChecker);

                if (!dropEnable)
                    return;

                dropAblePlane.PlaceToProjectPose(cloneObj.gameObject, cloneObj.dropGridPlane, projectChecker.projectResults);

                dropAblePlane.SetGridStateUsed(cloneObj.PropID, projectChecker.projectResults);

                cloneObj.interactObj.InteractableObjectGrabbed -= InteractObj_InteractableObjectGrabbed;

                cloneObj.interactObj.InteractableObjectGrabbed += InteractObj_InteractableObjectGrabbed;

                cloneObj.OnPlaced -= CloneObj_OnPlaced;

                cloneObj.OnPlaced += CloneObj_OnPlaced;

                objsDroped.Add(cloneObj.PropID, cloneObj);

                placeMessageCache.Send(cloneObj.PropID, cloneObj);
            }
        }

        private void RemoveDropedObj(DACloneObjCtr cloneObj)
        {
            if (!objsDroped.TryGetValue(cloneObj.PropID, out var existObj))
                return;

            var amount = existObj.GetAmount();

            if (amount == 1)
            {
                objsDroped.Remove(cloneObj.PropID);

                if (cloneObj.dropGridPlane != null)
                    dropAblePlane.SetGridStateUnUsed(cloneObj.PropID);

                placeMessageCache.Send(cloneObj.PropID, null);
            }
            else
            {
                var separateObj = cloneObj.SeparateCurrent() as DACloneObjCtr;   //自身被抓起，next留在原地

                //数量不只1个 保留一个物体在原地
                objsDroped[cloneObj.PropID] = separateObj;

                placeMessageCache.Send(cloneObj.PropID, separateObj);

                separateObj.interactObj.InteractableObjectGrabbed += InteractObj_InteractableObjectGrabbed;

                separateObj.OnPlaced += CloneObj_OnPlaced;
            }

            cloneObj.OnPlaced -= CloneObj_OnPlaced;

            cloneObj.interactObj.InteractableObjectGrabbed -= InteractObj_InteractableObjectGrabbed;
        }

        private void CloneObj_OnPlaced(DACloneObjCtr cloneObj)
        {
            objsDroped.Remove(cloneObj.PropID);

            placeMessageCache.Send(cloneObj.PropID, null);

            dropAblePlane.SetGridStateUnUsed(cloneObj.PropID);

            if (cloneObj.dropGridPlane != null)
            {
                dropAblePlane.RemoveProjectPlane(cloneObj.dropGridPlane);
            }
        }

        private void Update()
        {
            for (int i = objsChecking.Count - 1; i >= 0; i--)
            {
                var cloneObj = objsChecking[i];

                if (cloneObj == null)
                {
                    objsChecking.RemoveAt(i);

                    continue;
                }

                if (objsDroped.TryGetValue(cloneObj.PropID, out var existObj))
                {
                    //桌上存在相同物体，直接叠加
                    cloneObj.SetValidDrop(true);

                    if (cloneObj.dropGridPlane != null)
                        dropAblePlane.RemoveProjectPlane(cloneObj.dropGridPlane);

                    if (dropAblePlane.GetUsedGridById(existObj.PropID, out var rect))
                    {
                        //测试看看效果 已存在相同物品，直接在其下方绘制rect
                        dropAblePlane.drawer.DrawRect(rect, true);
                    }
                }
                else
                {
                    //新物体
                    if (cloneObj.dropGridPlane != null)
                    {
                        dropAblePlane.AddProjectPlane(cloneObj.dropGridPlane);

                        cloneObj.SetValidDrop(CheckObjDropEnabel(cloneObj, out _));

                        //var projectChecker = dropAblePlane.GetProjectChecker(cloneObj.dropGridPlane);

                        //cloneObj.SetValidDrop(projectChecker == null ? false : projectChecker.canDrop);
                    }
                }
            }
        }

        private bool TryGetExistObjByPropID(IReadOnlyCollection<DACloneObjCtr> container, string propID, out DACloneObjCtr res)
        {
            res = null;

            foreach (var findObj in container)
            {
                if (findObj.PropID == propID)
                {
                    res = findObj;

                    return true;
                }
            }

            return false;
        }
    }
}
