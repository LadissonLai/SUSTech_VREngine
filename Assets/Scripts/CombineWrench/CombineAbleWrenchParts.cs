using DG.Tweening;
using Framework;
using Framework.Tools;
using Fxb.DA;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.Highlighters;
using VRTKExtensions;
using static Fxb.DA.WrenchConfig;

namespace Fxb.CMSVR
{
    public class CombineAbleWrenchParts : MonoBehaviour
    {
        private Sequence flyBackAnim;

        protected Material PlaceHolderMat
        {
            get
            {
                return Resources.Load<Material>(PathConfig.MAT_PATH_DAPLACEHOLDER);
            }
        }

        protected HandToolCollisionTracker htTrackerCache;

        public Pose poseStartup;

        [HideInInspector]
        public AdvancedInteractableObj interactObj;

        public Item ConfigData { get; protected set; }

        public bool DisplayAsPlaceHolder { get; set; }

        public VRAnimWrenchCtr AttachTo { get; set; }

        private List<Collider> triggerColliders;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (interactObj == null)
            {
                interactObj = GetComponent<AdvancedInteractableObj>();

                interactObj.isGrabbable = true;

                interactObj.holdButtonToGrab = false;

                interactObj.holdButtonToUse = true;

                interactObj.isUsable = true;

                interactObj.useOnlyIfGrabbed = true;
            }

            if (transform.GetComponent<VRTK_BaseGrabAttach>() == null)
            {
                var grabAttach = gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();

                grabAttach.precisionGrab = false;
            }
        }
 
        private void OnDestroy()
        {
            if(triggerColliders != null && triggerColliders.Count > 0)
            {
                foreach (var c in triggerColliders)
                {
                    if(c != null)
                        c.isTrigger = true;
                }
            }

            if(!DisplayAsPlaceHolder)
                Clear();
        }

        private void Start()
        {
            poseStartup = new Pose(transform.position, transform.rotation);

            ConfigData = World.Get<WrenchConfig>().FindRDByModelName(name);

            DebugEx.AssertNotNull(ConfigData, $"ConfigData:{name} 未找到");

            triggerColliders = new List<Collider>();

            GetComponentsInChildren(triggerColliders);

            //trigger为true的碰撞体不会被射线触碰，开始时将trigger关闭，被销毁时重置
            for (int i = triggerColliders.Count - 1; i >= 0; i--)
            {
                if(triggerColliders[i].isTrigger)
                {
                    triggerColliders[i].isTrigger = false;

                    continue;
                }

                triggerColliders.RemoveAt(i);
            }

            if (TryGetComponent<PanelSpawnTooltipTrigger>(out var tooltipTrigger))
            {
                if (string.IsNullOrEmpty(tooltipTrigger.customTooltipSpawnKey))
                    tooltipTrigger.customTooltipSpawnKey = PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_S;
            }

            if (string.IsNullOrEmpty(interactObj.readableName))
            {
                interactObj.readableName = ConfigData.Name;
            }

            if (DisplayAsPlaceHolder)
            {
                GetComponentInChildren<Collider>().isTrigger = true;

                interactObj.isGrabbable = false;

                interactObj.useOnlyIfGrabbed = false;

                var gc = gameObject.AddComponent<GraphicsCache>();

                gc.SwapGraphicsSharedMats(PlaceHolderMat, true);

                tag = TagConst.WrenchPartsPlaceHolder;
            }
            else
            {
                interactObj.InteractableObjectGrabbed += InteractObj_InteractableObjectGrabbed;

                interactObj.InteractableObjectUngrabbed += InteractObj_InteractableObjectUngrabbed;
            }
        }

        private void InteractObj_InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            //被放下后，隔一段时间自动飞回
            if (AttachTo != null)
                return;

            if (flyBackAnim != null)
                flyBackAnim.Kill();

            flyBackAnim = DOTween.Sequence();

            flyBackAnim.Join(transform.DOMove(poseStartup.position, 0.3f));

            flyBackAnim.Join(transform.DORotateQuaternion(poseStartup.rotation, 0.2f));

            flyBackAnim.SetDelay(2.0f);
        }

        private void InteractObj_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (flyBackAnim != null)
            {
                flyBackAnim.Kill();

                flyBackAnim = null;
            }

            if (htTrackerCache == null)
            {
                htTrackerCache = gameObject.AddComponent<HandToolCollisionTracker>();

                htTrackerCache.CustomPredicate = CheckValidCombineTarget;
            }
        }

        /// <summary>
        /// 只允许和扳手部件placeholder交互
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CheckValidCombineTarget(AdvancedInteractableObj obj)
        {
            if (!obj.CompareTag(TagConst.WrenchPartsPlaceHolder))
                return false;

            //if (!obj.TryGetComponent<CombineAbleWrenchParts>(out var otherParts))
            //    return false;

            //配置数据目前是根据名称获取，暂时可以直接比较name是否一致
            if (obj.name != name)
                return false;

            return true;
        }

        public void Clear()
        {
            if (interactObj == null)
                return;

            interactObj.InteractableObjectGrabbed -= InteractObj_InteractableObjectGrabbed;

            interactObj.InteractableObjectUngrabbed -= InteractObj_InteractableObjectUngrabbed;

            transform.DestoryComponents(
                              typeof(VRTK_InteractableListener),
                              typeof(VRTK_BaseHighlighter),
                              typeof(VRTK_BaseGrabAttach),
                              typeof(HandToolCollisionTracker),
                              typeof(VRTK_PolicyList),
                              typeof(Rigidbody),
                              typeof(InteractObjTooltipTriggerBase)
                          );
 
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);

                Destroy(interactObj);

                gameObject.SetActive(true);
            }
            else
            {
                Destroy(interactObj);
            }
        }
    }
}

