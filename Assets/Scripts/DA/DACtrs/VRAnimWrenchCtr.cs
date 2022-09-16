using Fxb.DA;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using VRTKExtensions;
using Framework;
using static Fxb.DA.WrenchConfig;
using VRTK.GrabAttachMechanics;
using System.Text;

namespace Fxb.CMSVR
{
    public class VRAnimWrenchCtr : AnimWrenchCtr, IGrabableTool
    {
        protected HandToolCollisionTracker collisionTracker;

        public override bool IsUsing
        {
            get => base.IsUsing;
            set
            {
                base.IsUsing = value;

                InteractableObj.validDrop =
                    value
                    ? VRTK_InteractableObject.ValidDropTypes.NoDrop
                    : VRTK_InteractableObject.ValidDropTypes.DropAnywhere;

                if (isUsing)
                {
                    collisionTracker.enabled = false;
                }
                else
                {
                    if (InteractableObj.IsGrabbed())
                        collisionTracker.enabled = true;
                }
            }
        }
         
        public VRTK_ControllerEvents.ButtonAlias ModifyParamButton { get; protected set; }

        protected override void Awake()
        {
            base.Awake();

            UpdateWrenchInfo();
        }

        protected override void Start()
        {
            base.Start();

            collisionTracker = gameObject.AddMissingComponent<HandToolCollisionTracker>();

            if (wrenchInfo.torsion != -1)
                ModifyParamButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
        }

        public bool UpdateAlis(Vector2 controllerValue)
        {
            if (ModifyParamButton == VRTK_ControllerEvents.ButtonAlias.TouchpadPress)
                return ModifyTorsion(controllerValue);

            return false;
        }

        bool ModifyTorsion(Vector2 controllerValue)
        {
            if (controllerValue.x == 0)
                return false;

            return DoModifyTorsion(controllerValue.x > 0);
        }

        public bool CheckModifyParamValid(VRTK_ControllerEvents.ButtonAlias modifyBtn = VRTK_ControllerEvents.ButtonAlias.TouchpadPress)
        {
            return ModifyParamButton == modifyBtn;
        }

        public void PressModifyBtn(bool isPressed, Vector2 btnAlis)
        {
            SetFirstModify(isPressed, btnAlis.x != 0, btnAlis.x > 0);
        }

        public void RemoveCombinedParts(Transform parts)
        {
            if(parts == extension)
            {
                wrenchInfo.jiegan = null;

                extension = null;
            }
            else if(parts == sleeve)
            {
                wrenchInfo.taotong = null;

                sleeve = null;
            }

            UpdateWrenchInfo();
        }

        public void AddCombineParts(Item configData, Transform parts = null)
        {
            if (null == parts)
                parts = GenWrenchParts(configData.Id);

            parts.gameObject.SetActive(true);

            if (configData.Type == WrenchPartsType.RatchetWrench || configData.Type == WrenchPartsType.TorqueWrench)
            {
                CheckAndClearExistParts(ref handle, ref parts);

                wrenchInfo.banshou = configData.Id;

                parts.SetParent(handleRoot);

                wrenchInfo.torsion = configData.Type == WrenchPartsType.RatchetWrench ? -1 : 0;

                var grabAttach = GetComponent<VRTK_BaseGrabAttach>();

                grabAttach.leftSnapHandle = handle.Find("LeftGrabPos");

                grabAttach.rightSnapHandle = handle.Find("RightGrabPos");
            }
            else if(configData.Type == WrenchPartsType.Extension)
            {
                CheckAndClearExistParts(ref extension, ref parts);

                wrenchInfo.jiegan = configData.Id;

                parts.SetParent(sleeveRoot);
            }
            else if (configData.Type == WrenchPartsType.Sleeve)
            {
                CheckAndClearExistParts(ref sleeve, ref parts);

                wrenchInfo.taotong = configData.Id;

                parts.SetParent(extension == null ? sleeveRoot : extension.Find("Connect"));
            }

            parts.ResetLocalMatrix();

            UpdateWrenchInfo();
        }
        
        private void CheckAndClearExistParts(ref Transform existParts, ref Transform newParts)
        {
            if (existParts != null)
                Destroy(existParts.gameObject);

            existParts = newParts;
        }

        private void UpdateWrenchInfo()
        {
            var sb = new StringBuilder();

            AppendNameById(wrenchInfo.banshou, sb);

            AppendNameById(wrenchInfo.jiegan, sb);

            AppendNameById(wrenchInfo.taotong, sb);

            if(sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);    //最后一个+号

                InteractableObj.readableName = sb.ToString();
            }
        }

        private bool AppendNameById(string id, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (WrenchConfig == null)
                {
                    Debug.Log($"WrenchConfig is Null---id:{id}--name{name}");

                    return false;
                }

                sb.Append(WrenchConfig.FindRowDatas(id).Name);

                sb.Append("+");

                return true;
            }

            return false;
        }
    }
}

