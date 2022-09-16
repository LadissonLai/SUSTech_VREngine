using Framework;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using VRTKExtensions;

namespace Fxb.DA
{
    public class AnimWrenchCtr : CombineAbleWrench, IDAUsingTool
    {
        protected static WrenchConfig WrenchConfig => World.Get<WrenchConfig>();

        public static Transform partsPreviewPrefab;

        protected AdvancedInteractableObj interactObj;

        public AdvancedInteractableObj InteractableObj
        {
            get
            {
                if (interactObj == null)
                    interactObj = GetComponent<AdvancedInteractableObj>();

                return interactObj;
            }
        }

        public enum AnimType
        {
            Disassembly = 1,
            Assembly,
            Fix,
        }

        private AnimatorStateInfo CutAnimStateInfo => animator.GetCurrentAnimatorStateInfo(0);

        private static readonly int ANIM_ID_SCREW_OUT = Animator.StringToHash("ScrewOut");

        private static readonly int ANIM_ID_ANIM_TYPE = Animator.StringToHash("AnimType");

        private static readonly int ANIM_ID_TORQUE = Animator.StringToHash("torque");

        private static readonly int ANIM_ID_PLAY = Animator.StringToHash("Play");

        private static readonly int ANIM_ID_RESET = Animator.StringToHash("Reset");

        public Animator animator;

        protected bool isUsing;

        public virtual bool IsUsing
        {
            get => isUsing;

            set
            {
                if (value != isUsing)
                {
                    isUsing = value;

                    if (isUsing)
                        isModifyBtnPressed = false;
                }
            }
        }

        public bool FixUsingAble => wrenchInfo.torsion > -1;

        #region 修改参数

        public int modifyParamStep = 1;

        public float modifyParamDelayTime = 0.3f;

        [Tooltip("秒")]
        public float modifyParamInterval = 0.12f;

        protected TextMeshProUGUI torsion;

        float preModifyTime;

        bool isFirstModify = true;

        int modifyParamStepCache;

        int minParam = 40;

        int maxParam = 200;

        bool isModifyBtnPressed;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screwT">螺丝transform</param>
        /// <param name="screwOut">螺丝拧出的距离 单位厘米</param>
        /// <param name="isClockwise">方向</param>
        /// <param name="torque">力矩</param>
        /// <returns></returns>
        public virtual IEnumerator PlayWrenchAnim(Transform screwT, float screwOut, AnimType animType)
        {
            IsUsing = true;

            //拆装物体移动时 保持工具位置同步
            transform.SetParent(screwT.parent);

            var defaultAnim = CutAnimStateInfo.shortNameHash;

            animator.SetFloat(ANIM_ID_SCREW_OUT, screwOut);
            animator.SetInteger(ANIM_ID_ANIM_TYPE, (int)animType);
            animator.SetInteger(ANIM_ID_TORQUE, wrenchInfo.torsion);
            animator.SetTrigger(ANIM_ID_PLAY);

            var initInterfacePos = ScrewConnect.position;

            var initScrewPos = screwT.position;

            while (true)
            {
                yield return null;

                screwT.position = initScrewPos + (ScrewConnect.position - initInterfacePos);

                if (defaultAnim != CutAnimStateInfo.shortNameHash && CutAnimStateInfo.normalizedTime >= 0.999f)
                {
                    break;
                }
            }

            yield return null;

            animator.SetTrigger(ANIM_ID_RESET);

            yield return null;

            IsUsing = false;
        }

        public override Transform GenWrenchParts(string partID)
        {
            var configRD = WrenchConfig.FindRowDatas(partID);

            DebugEx.AssertNotNull(configRD);
            
            return GenWrenchPartsByPrefabName(configRD.PrefabName);
        }

        public Transform GenWrenchPartsByPrefabName(string partsName)
        {
            var prefab = partsPreviewPrefab.Find(partsName);

            var inst = Instantiate(prefab);

            inst.name = partsName;

            return inst;
        }

        #region 扭力设置

        public virtual bool DoModifyTorsion(bool isIncrease)
        {
            if (!isModifyBtnPressed)
                return false;

            modifyParamStepCache = isIncrease ? modifyParamStep : -modifyParamStep;

            if (isFirstModify && Time.realtimeSinceStartup - preModifyTime < modifyParamDelayTime)
                return false;

            //Debug.Log(modifyParamDelayTime);

            if (Time.realtimeSinceStartup - preModifyTime < modifyParamInterval)
                return false;

            DoSetTorsion();

            preModifyTime = Time.realtimeSinceStartup;

            isFirstModify = false;

            return true;
        }

        void DoSetTorsion()
        {
            if (wrenchInfo.torsion == -1)
                return;

            wrenchInfo.torsion += modifyParamStepCache;

            wrenchInfo.torsion = Mathf.Clamp(wrenchInfo.torsion, minParam, maxParam);

            torsion.text = wrenchInfo.torsion.ToString();
        }

        void CheckTorsionType()
        {
            switch (wrenchInfo.banshou)
            {
                case "4":
                    minParam = 25;

                    maxParam = 200;
                    break;

                case "5":
                    minParam = 5;

                    maxParam = 45;
                    break;

                case "6":
                    minParam = 5;

                    maxParam = 10;
                    break;

                default:
                    break;
            }
        }

        void ShowTorsion(bool isShow)
        {
            if (wrenchInfo.torsion == -1)
                return;

            torsion.transform.parent.gameObject.SetActive(isShow);
        }

        #endregion

        /// <summary>
        /// 设置第一次修改
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="isModify">是否需要修改值</param>
        /// <param name="isIncrease"></param>
        public void SetFirstModify(bool flag, bool isModify, bool isIncrease)
        {
            if (isUsing)
                return;

            if (flag)
            {
                preModifyTime = Time.realtimeSinceStartup;

                if (isModify)
                    modifyParamStepCache = isIncrease ? modifyParamStep : -modifyParamStep;

                isFirstModify = flag;
            }
            else
            {
                if (!isFirstModify || !isModifyBtnPressed)
                    return;

                DoSetTorsion();
            }

            isModifyBtnPressed = flag;
        }

        void OnAnimWrenchGrabbed(object sender, InteractableObjectEventArgs e)
        {
            ShowTorsion(true);
        }

        void OnAnimWrenchUnGrabbed(object sender, InteractableObjectEventArgs e)
        {
            ShowTorsion(false);

            isModifyBtnPressed = false;
        }
         
        protected virtual void Awake()
        {
            
        }

        protected override void Start()
        {
            base.Start();
 
            if (InteractableObj)
            {
                InteractableObj.InteractableObjectGrabbed += OnAnimWrenchGrabbed;

                InteractableObj.InteractableObjectUngrabbed += OnAnimWrenchUnGrabbed;
            }

            if (wrenchInfo.torsion != -1)
            {
                CheckTorsionType();

                var canvas = handle.Find("ParamCanvas");

                if (canvas != null)
                {
                    torsion = canvas.Find("Num_TMP").GetComponent<TextMeshProUGUI>();

                    wrenchInfo.torsion = Mathf.Clamp(wrenchInfo.torsion, minParam, maxParam);

                    torsion.text = wrenchInfo.torsion.ToString();

                    canvas.gameObject.SetActive(InteractableObj.IsGrabbed());
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (InteractableObj)
            {
                InteractableObj.InteractableObjectGrabbed -= OnAnimWrenchGrabbed;

                InteractableObj.InteractableObjectUngrabbed -= OnAnimWrenchUnGrabbed;
            }
        }
    }
}
