using UnityEngine;
using System.Collections;
using VRTK;
using System.Collections.Generic;
using Doozy.Engine;
using System;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 自定义的手部模型控制。  弃用 VRTK_AvatarHandController
    /// </summary>
    public class CustomVirtualHandAvatarCtr : MonoBehaviour       
    {
        public Transform pointerRendererPos;

        public Transform colliderRoot;

        public Transform grabAttachPoint;

        public float targetGrabAnimBlendVal = 0.4f;

        public Material matRubber;

        public Material matFade;

        public Renderer handRenderer;

        protected VRTK_UIPointer uiPointer;

        protected VRTK_Pointer pointer;

        protected VRTK_InteractGrab interactGrab;

        protected Animator animator;

        private readonly static int STATE_ID_POINT = Animator.StringToHash("Point");

        private readonly static int STATE_ID_TRIGGER = Animator.StringToHash("Trigger");
 
        private Dictionary<int, float> statesFVals;

        private void OnDestroy()
        {
            Message.RemoveListener<WearEquipmentMessage>(OnWearEquipmentMessage);
        }

        private void Awake()
        {
            statesFVals = new Dictionary<int, float>
            {
                { STATE_ID_POINT, 0.0f },
                { STATE_ID_TRIGGER, 0.0f }
            };

            SetHandSkin(matFade);

            Message.AddListener<WearEquipmentMessage>(OnWearEquipmentMessage);
        }

        private void OnWearEquipmentMessage(WearEquipmentMessage message)
        {
            if (message.isOn && message.equipName == EquipName.RubberGloves)
                SetHandSkin(matRubber);
        }

        protected void OnEnable()
        {
            interactGrab = GetComponentInParent<VRTK_InteractGrab>();

            animator = GetComponentInChildren<Animator>();
 
            uiPointer = GetComponentInParent<VRTK_UIPointer>();

            pointer = GetComponentInParent<VRTK_Pointer>();
        }
 
        protected void Update()
        {
            var indexPose = 
                (uiPointer != null && uiPointer.PointerActive()) 
                || 
                (pointer != null && pointer.IsPointerActive());

            var grabbedObj = interactGrab == null ? null : interactGrab.GetGrabbedObject();

            var grabPose = false;

            if(grabbedObj != null)
            {
                var daTool = grabbedObj.GetComponent<IDAUsingTool>();

                if((daTool as MonoBehaviour) == null || !daTool.IsUsing)
                {
                    grabPose = true;
                }
            }

            statesFVals[STATE_ID_POINT] = indexPose ? 1 : 0;

            statesFVals[STATE_ID_TRIGGER] = grabPose ? targetGrabAnimBlendVal : 0;
             
            foreach (var kv in statesFVals)
            {
                var val = animator.GetFloat(kv.Key);

                if (Mathf.Abs(val - kv.Value) > 0.001f)
                {
                    val = Mathf.Lerp(val, kv.Value, 0.15f);

                    animator.SetFloat(kv.Key, val);
                }
            }
        }

        private void SetHandSkin(Material mat)
        {
            handRenderer.sharedMaterial = mat;
        }
    }
}