using System;
using System.Collections.Generic;
using Doozy.Engine;
using Framework.Tools;
using Framework;
using UnityEngine;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 可以放下拆装物体的简易snapZone
    /// </summary>
    public class CloneObjCustomSnapZone : MonoBehaviour, ICloneObjDropAble
    {
        public string[] validPropIDs;

        public Transform placeHolder;

        [HideInInspector]
        public AdvancedInteractableObj interactObj;
         
        private DACloneObjCtr currentDroped;

        private DACloneObjCtr CurrentDroped
        {
            get => currentDroped;
            
            set
            {
                if (currentDroped == value)
                    return;

                currentDroped = value;

                interactObj.isUsable = value == null;

                if(currentDroped != null)
                {
                    currentDroped.transform.position = placeHolder.position;

                    currentDroped.transform.rotation = placeHolder.rotation;
                }
            }
        }

        private List<DACloneObjCtr> checkingObjs;
 
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (interactObj == null)
                interactObj = GetComponent<AdvancedInteractableObj>();

            interactObj.isUsable = true;

            interactObj.holdButtonToUse = true;

            interactObj.isGrabbable = false;
        }

        public bool CheckCloneObjDropAble(DACloneObjCtr target)
        {
            return Array.IndexOf(validPropIDs, target.PropID) > -1;
        }

        private void OnDestroy()
        {
            Message.RemoveListener<ControllerGrabInteractObjMessage>(OnControllerGrabInteractObj);
        }

        private void Start()
        {
            if (validPropIDs == null || validPropIDs.Length == 0)
            {
                enabled = false;

                placeHolder.gameObject.SetActive(false);

                return;
            }

            checkingObjs = new List<DACloneObjCtr>();
             
            placeHolder.gameObject.SetActive(false);

            interactObj.InteractableObjectUnused += InteractObj_InteractableObjectUnused;
 
            Message.AddListener<ControllerGrabInteractObjMessage>(OnControllerGrabInteractObj);
        }
         
        private void InteractObj_InteractableObjectUnused(object sender, VRTK.InteractableObjectEventArgs e)
        {
            var grabObj =
                e.interactingObject == VRTKHelper.LeftHand.gameObject
                ? VRTKHelper.LeftGrab.GetGrabbedObject()
                : VRTKHelper.RightGrab.GetGrabbedObject();

            DACloneObjCtr cloneObj = grabObj == null ? null : grabObj.GetComponent<DACloneObjCtr>();

            if (cloneObj == null)
                return;

            if(this == cloneObj.CurrentValidDropAble as MonoBehaviour)
                cloneObj.interactObj.ForceStopInteracting();
        }

        private void OnControllerGrabInteractObj(ControllerGrabInteractObjMessage msg)
        {
            if (msg.isGrab)
            {
                if (!msg.interactObj.TryGetComponent<DACloneObjCtr>(out var cloneObj))
                    return;

                if (!CheckCloneObjDropAble(cloneObj))
                    return;

                checkingObjs.AddUnique(cloneObj);
            }
        }
 
        private void LateUpdate()
        {
            var hasValidObjGrabed = checkingObjs.Count > 0 && UpdateCheckingObjs() && interactObj.isUsable;

            if (placeHolder.gameObject.activeSelf != hasValidObjGrabed)
                placeHolder.gameObject.SetActive(hasValidObjGrabed);
        }
         
        private bool UpdateCheckingObjs()
        {
            var res = false;

            for (int i = checkingObjs.Count - 1; i >= 0; i--)
            {
                var checkingObj = checkingObjs[i];

                if (checkingObj == null || !checkingObj.IsGrabed)
                {
                    if(!checkingObj.IsGrabed)
                    {
                        if(this == checkingObj.CurrentValidDropAble as MonoBehaviour)
                        {
                            CurrentDroped = checkingObj;

                            CurrentDroped.SetValidDrop(false,this);
                        }
                    }

                    checkingObjs.RemoveAt(i);
                      
                    break;
                }

                if(CurrentDroped == checkingObj)
                {
                    CurrentDroped = null;
                }

                var isGrabedByTouchHand = false;

                var currentTouchings = interactObj.GetTouchingObjects();

                if(currentTouchings.Count > 0)
                {
                    foreach (var byTouching in currentTouchings)
                    {
                        if (checkingObj.interactObj.IsGrabbed(byTouching.gameObject))
                        {
                            isGrabedByTouchHand = true;

                            break;
                        }
                    }
                }

                checkingObj.SetValidDrop(isGrabedByTouchHand, this);

                res = true;
            }

            return res;
        }
    }
}