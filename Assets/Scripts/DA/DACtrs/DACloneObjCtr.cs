using Doozy.Engine;
using Framework;
using System;
using System.Collections;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTKExtensions;
using DG.Tweening;


namespace Fxb.CMSVR
{
    /// <summary>
    /// 拆裝物体被拆下后生成。
    /// </summary>
    public class DACloneObjCtr : MergeAbleObj, IDAUsingTool
    {
        private Rigidbody rigidBody;

        private InteractObjTooltipTriggerBase tooltipTrigger;

        //暂时屏蔽
        //private Vector2Int initGridSize;

        public AdvancedInteractableObj interactObj;

        public HandToolCollisionTracker collisionTracker;

        public DAGridPlane dropGridPlane;

        [NonSerialized]
        public bool isGenerateByModelCopy;

        [Tooltip("可以留空，拆下物体后会自动赋值")]
        public string PropID;

        /// <summary>
        /// 自身id 方便统一管理
        /// </summary>
        public string ID { get; protected set; }

        public bool FixUsingAble => false;

        public bool IsGrabed => interactObj.IsGrabbed();

        public bool IsAnimPlaying { get; protected set; }

        public bool WaitForFistPick { get; protected set; } = true;

        /// <summary>
        /// 是否等待被放置。（没有放置过）
        /// </summary>
        public bool WaitForFistDrop { get; private set; } = true;

        public bool IsUsing => false;

        /// <summary>
        /// 可以放置到的目标  零件桌暂时没用此逻辑
        /// </summary>
        public ICloneObjDropAble CurrentValidDropAble { get; protected set; }
         
        /// <summary>
        /// 安装的时候有可能会批量直接销毁，外部需要能收到对应消息
        /// </summary>
        public event Action<DACloneObjCtr> OnPlaced;

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(ID))
                World.current.Injecter.Regist(this, ID);
        }

        private void OnDisable()
        {
            if (!string.IsNullOrEmpty(ID))
                World.current.Injecter.UnRegist<DACloneObjCtr>(ID);
        }

        private void Awake()
        {
            SetValidDrop(false);

            interactObj.InteractableObjectUngrabbed += InteractObj_InteractableObjectUngrabbed;

            interactObj.InteractableObjectGrabbed += InteractObj_InteractableObjectGrabbed;

            interactObj.InteractableObjectTryDrop += InteractObj_InteractableObjectTryDrop;

            interactObj.isUsable = true;

            interactObj.useOnlyIfGrabbed = false;

            interactObj.holdButtonToUse = true;
        }

        private void InteractObj_InteractableObjectTryDrop(AdvancedInteractableObj arg1, bool isDropable)
        {
            if(!isDropable)
            {
                var errorMsg = "请将拆下的物体放到零件桌的合适位置";

                if (dropGridPlane == null)
                {
                    //需要放到桌上
                    errorMsg = "请将拆下的物体放到指定位置";
                }

                Popup_Tips.Show(errorMsg);
            }
        }

        private void Start()
        {
            if (interactObj.grabAttachMechanicScript == null)
            {
                if(!interactObj.TryGetComponent<VRTK_BaseGrabAttach>(out var grabAttach))
                {
                    grabAttach = interactObj.gameObject.AddComponent<VRTK_ChildOfControllerGrabAttach>();

                    grabAttach.precisionGrab = true;
                }

                interactObj.grabAttachMechanicScript = grabAttach;
            }

            ID = $"{PropID}_{DateTime.Now.Ticks}";

            name = ID;

            World.current.Injecter.Regist(this, ID);

            collisionTracker.CustomPredicate = CollisionPredicate;

            if (dropGridPlane == null)
                dropGridPlane = GetComponentInChildren<DAGridPlane>();

            //暂时屏蔽
            //if (dropGridPlane != null)
            //    initGridSize = dropGridPlane.size;

            rigidBody = GetComponent<Rigidbody>();

            if (rigidBody == null)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();

                rigidBody.isKinematic = true;

                rigidBody.useGravity = false;
            }

            if (!isGenerateByModelCopy)
                return;

            var colliders = GetComponentsInChildren<Collider>();

            for (int i = colliders.Length - 1; i >= 0; i--)
            {
                var c = colliders[i];

                c.enabled = true;

                //拷贝出来的模型，有可能会带上拆装物体单独创建的trigger碰撞体。
                if (c.isTrigger)
                    Destroy(c);
            }
        }
 
        private void InteractObj_InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            Drop();
        }

        private void InteractObj_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            Pickup();
        }

        /// <summary>
        /// 不通过此条件不会触发目标物体高亮，也无法使用
        /// TODO 零件桌的放置逻辑没有走此机制，待修改
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CollisionPredicate(AdvancedInteractableObj obj)
        {
            if(obj.TryGetComponent<ICloneObjDropAble>(out var dropAbleTarget))
            {
                if (dropAbleTarget.CheckCloneObjDropAble(this))
                    return true;
            }
             
            return false;
        }
         
        /// <summary>
        /// 由外部调用  放置到拆装物体身上
        /// 期望在销毁前能够通知外部做清理
        /// </summary>
        public void Place()
        {
            Debug.Assert(GetAmount() == 1);

            OnPlaced?.Invoke(this);

            gameObject.SetActive(false);

            //先关闭再调用ForceStopInteracting，否则会延迟一帧处理stop interacting相关逻辑
            interactObj.ForceStopInteracting();

            Destroy(gameObject);
        }
          
        /// <summary>
        /// 设置物体是否可允许被放下
        /// </summary>
        /// <param name="state"></param>
        /// <param name="targetDropAble"></param>
        public void SetValidDrop(bool state, ICloneObjDropAble targetDropAble = null)
        {
            if (CurrentValidDropAble != null && CurrentValidDropAble != targetDropAble && !state )
                return;

            if (state)
                CurrentValidDropAble = targetDropAble;
            else if (targetDropAble == CurrentValidDropAble)
                CurrentValidDropAble = null;

            //有可能需要放置到指定位置 TODO
            interactObj.validDrop = state ? VRTK_InteractableObject.ValidDropTypes.DropAnywhere : VRTK_InteractableObject.ValidDropTypes.NoDrop;
        }

        protected virtual void Drop()
        {
            WaitForFistDrop = false;

            if (nextNodeObj as DACloneObjCtr != null)
                (nextNodeObj as DACloneObjCtr).Drop();
        }

        /// <summary>
        /// 被抓取。 会发送消息通知外部。 被合并到被抓取物体身上时也会被调用。
        /// </summary>
        protected virtual void Pickup()
        {
            //消息在 WaitForFistPick 赋值前面还是后面暂不确定
            Message.Send(new CloneObjPickupMessage() { target = this });

            WaitForFistPick = false;
        }
 
        private IEnumerator PlayFlyAnim(DACloneObjCtr other)
        {
            IsAnimPlaying = other.IsAnimPlaying = true;

            var duration = 0.0;

            while (true)
            {
                if (duration > 2.0f)
                    break;

                //中途被抓取或者自身不再被抓取则取消动画
                if (other.IsGrabed || !IsGrabed)
                    break;

                var fromPos = other.transform.position;

                var fromRot = other.transform.rotation;

                var toPos = transform.position;

                var toRot = transform.rotation;

                var distance = Vector3.Distance(fromPos, toPos);

                var angle = Quaternion.Angle(fromRot, toRot);

                //if (distance < 0.02f && angle < 2f)
                //    break;

                //Debug.Log($"dis:{distance}  angle:{angle}");

                var move = (toPos - fromPos).normalized * Time.deltaTime * 3.0f;

                if (move.magnitude >= distance)
                    break;

                other.transform.position += move;

                //other.transform.position = Vector3.Lerp(fromPos, toPos, 0.02f);

                other.transform.rotation = Quaternion.Lerp(fromRot, toRot, 0.1f);

                duration += Time.deltaTime;

                yield return null;
            }

            IsAnimPlaying = other.IsAnimPlaying = false;

            if (!other.IsGrabed)
            {
                AddCloneObj(other, false);
            }
        }

        public override void AddCloneObj(MergeAbleObj other)
        {
            base.AddCloneObj(other);

            if (IsGrabed)
            {
                //自身被抓取
                (other as DACloneObjCtr).Pickup();
            }
        }

        public void AddCloneObj(DACloneObjCtr other, bool playFlyAnim)
        {
            if (playFlyAnim)
            {
                if (other.transform != null)
                    other.transform.SetParent(null);

                World.current.StartCoroutine(PlayFlyAnim(other));
            }
            else
            {
                AddCloneObj(other);
            }
        }

        protected override void UpdateAmountState()
        {
            base.UpdateAmountState();

            var amount = GetAmount();
             
            if (tooltipTrigger == null)
                tooltipTrigger = interactObj.GetComponent<InteractObjTooltipTriggerBase>();

            if (tooltipTrigger == null)
                return;

            tooltipTrigger.overrideTipMsg = amount < 2 ? null : $"{interactObj.readableName}x{amount}";

            if (tooltipTrigger.isToolTipActived)
                tooltipTrigger.RefTipMsg();
            else if(amount > 1 && gameObject.activeInHierarchy && tooltipTrigger.enableTooltipOnTouch)
            {
                StartCoroutine(ShowTooltipForAmount());
            }
        }

        //暂时屏蔽
        //protected void UpdateGridSizeByAmount(int amount)
        //{
        //    dropGridPlane.size = new Vector2Int(initGridSize.x * amount, initGridSize.y);
        //}

        protected IEnumerator ShowTooltipForAmount()
        {
            tooltipTrigger.ShowTooltip();

            yield return new WaitForSeconds(2.0f);

            if(!interactObj.IsTouched() && !interactObj.isInFocused)
                tooltipTrigger.HideTooltip();
        }
    }
}



