using DG.Tweening;
using Doozy.Engine;
using Framework;
using Framework.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTKExtensions;

namespace Fxb.DA
{
    public abstract class AbstractDAObjCtr : MonoBehaviour
    {
        private DAObjStateChangeMessage stateChangeMessage;

        private DAObjProcessingMessage processingMessage;

        private DAObjProcessCompleteMessage processCompleteMessage;

        protected GraphicsCache graphicsCache;

        protected virtual Material PlaceHolderMat => null;

        protected static DAProcessObjMessage daProccessMessage;

        /// <summary>
        /// 事件触发的顺序在抛出对应消息之前
        /// </summary>
        public event Action<AbstractDAObjCtr> OnStateChanged;

        /// <summary>
        /// 允许为null 为空表示此拆装物体只有逻辑没有自身的交互
        /// </summary>
        public AdvancedInteractableObj interactObj;

        [Tooltip("是否自动消失")]
        public bool autoDisappear;
         
        [Header("拆装脚本")]
        public DOTweenAnimation daDoTweenAnim;

        public AbstractDAScript customDAScript;
        
        public DisappearEffectScript disappearEffect;

        public virtual string ID { get => name; }

        public abstract CmsObjType Type { get; }

        public abstract List<AbstractDAObjCtr> DependParts { get; }

        public abstract List<AbstractDAObjCtr> DependSnapFits { get; }

        /// <summary>
        /// 内部包含的所有部件，拆装时会同步激活。（不会通过依赖找到到上层部件）
        /// 拆下时可以不拆除内部部件，安装时需要保证内部部件完整性。
        /// 一般只有模块才有，所有内部部件一定要在模块内部，需要一起消失。
        /// </summary>
        public virtual List<AbstractDAObjCtr> InternalParts { get; protected set; }
 
        [Tooltip("所属拆装物体，模块为空")]
        public AbstractDAObjCtr attachTo;

        public CmsDisplayMode DisplayMode { get; protected set; } = CmsDisplayMode.Default;

        public virtual string Name { get => interactObj.readableName; }

        public bool IsActive { get; protected set; } = true;
 
        protected ModelTransformRecorder transRecorder;
         
        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (autoDisappear && disappearEffect == null || !autoDisappear && disappearEffect != null)
            {
#if UNITY_EDITOR
                Debug.Log("Onvalidate  " + name);

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    Debug.Log("update disappearEffect scripts");

                    if (autoDisappear)
                    {
                        disappearEffect = gameObject.AddMissingComponent<DisappearEffectScript>();
                    }
                    else
                    {
                        if (disappearEffect != null)
                        {
                            DestroyImmediate(disappearEffect, true);

                            disappearEffect = null;
                        }
                    }
                };
#endif
            }
        }
      
        /// <summary>
        /// 是否自动固定物体
        /// 逻辑调整：
        /// 1.如果不自动固定，说明需要手动去操作执行固定（比如螺丝通过扭力扳手固定），或者需要将snapfit安装完成后才固定（比如有螺丝的部件）
        /// 2.目前部件不支持在有snapfit需要安装的情况下又需要通过工具去固定。
        /// </summary>
        /// <value></value>
        public virtual bool AutoFix
        {
            get
            {
                if (DependSnapFits?.Count > 0)
                    return false;

                if (customDAScript != null)
                    return customDAScript.AutoFix;

                return true;
            }
        }

        /// <summary>
        /// 固定物体后是否自动切换到default状态 
        /// </summary>
        public virtual bool AutoDefault => true;
 
        protected CmsObjState state = CmsObjState.Default;

        /// <summary>
        /// Dismantling 与 Assembling 移除，通过
        /// </summary>
        protected bool isProcessing;

        /// <summary>
        /// false => 去除此物体 true => 加入此物体 (DaState.processingObjs)
        public virtual bool IsProcessing
        {
            get => isProcessing;
            
            set
            {
                if(value != isProcessing)
                {
                    isProcessing = value;

                    processingMessage = processingMessage ?? new DAObjProcessingMessage();

                    processingMessage.target = this;

                    Message.Send(processingMessage);
                }
            }
        }
        
        /// <summary>
        /// 代表当前状态，拆装时，又可能因为工具使用错误的情况下频繁切换状态
        /// </summary>
        public virtual CmsObjState State
        {
            get => state;

            protected set
            {
                if (value != state)
                {
                    stateChangeMessage = stateChangeMessage ?? new DAObjStateChangeMessage();

                    stateChangeMessage.objCtr = this;

                    state = value;

                    OnStateChanged?.Invoke(this);

                    Message.Send(stateChangeMessage);
                }
            }
        }

        protected virtual void Awake()
        {
            if (interactObj == null)
                interactObj = GetComponent<AdvancedInteractableObj>();

            if(customDAScript == null)
                customDAScript = GetComponent<AbstractDAScript>();


            if (customDAScript != null)
            {
                customDAScript.daObjID = ID;
            }

            if (disappearEffect == null)
                disappearEffect = GetComponent<DisappearEffectScript>();

            InitActiveds();
        }
 
        protected virtual void InitActiveds()
        {
            if (Type == CmsObjType.Parts || Type == CmsObjType.SnapFit)
            {
                SetActived(false);
            }
        }

        [System.Obsolete("弃用")]
        protected virtual GameObject CloneGO()
        {
            var newGO = new GameObject();

            newGO.transform.SetParent(null);

            newGO.transform.position = transform.position;

            newGO.transform.localScale = transform.lossyScale;

            newGO.transform.rotation = transform.rotation;

            VRTKHelper.CopyModel(target: transform, givenParent: newGO.transform);

            return newGO;
        }
         
        public virtual void SetActived(bool actived)
        {
            if (IsActive == actived)
                return;

            IsActive = actived;

            UpdateObjInteractAble(actived);

            if (actived)
            {
                if (State == CmsObjState.Dismantled && autoDisappear)
                    SetDisplayMode(CmsDisplayMode.PlaceHolder);
            }
            else
            {
                if (State == CmsObjState.Dismantled && autoDisappear)
                    SetDisplayMode(CmsDisplayMode.Hide);
            }
        }

        protected virtual void UpdateObjInteractAble(bool enable)
        {
            if(interactObj != null)
            interactObj.SetInteractAble(enable, false);
        }
        
        protected virtual void DoProcess(IDAUsingTool usingTool = null)
        {
            if (!World.Get<DAState>().isRunning)
                return;

            //需要抓取模型后安装
            daProccessMessage = daProccessMessage ?? new DAProcessObjMessage();

            daProccessMessage.target = this;

            daProccessMessage.usingTool = usingTool;

            Message.Send(daProccessMessage);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public virtual void DoUnFix()
        {
            if (State != CmsObjState.Default && State != CmsObjState.Fixed)
                return;

            State = CmsObjState.Assembled;
        }

        /// <summary>
        /// 固定
        /// </summary>
        public virtual void DoFix(IDAUsingTool usingTool = null)
        {
            if (State != CmsObjState.Assembled)
                return;

            if(customDAScript != null && !customDAScript.AutoFix)
                World.current.StartCoroutine(DoFixWithAnim(usingTool));
            else
            {
                State = CmsObjState.Fixed;
 
                if (AutoDefault)
                    ForceSwitchState(CmsObjState.Default);
            }
        }
         
        /// <summary>
        /// 目前只用在 AutoDefault为false的情况下手动切换Fixed与Default状态（模块拆装）。
        /// </summary>
        /// <param name="newState"></param>
        public virtual void ForceSwitchState(CmsObjState newState)
        {
            if(
                (newState == CmsObjState.Default && State == CmsObjState.Fixed) 
                ||
                (newState == CmsObjState.Fixed && State == CmsObjState.Default)
               )
            {
                State = newState;
            }
        }
         
        //拆卸
        public virtual void DoDisassemble(IDAUsingTool usingTool = null)
        {
            if ((State & CmsObjState.Dismantable) == 0)
                return;

            //防止因为脚本关闭造成协程中断，使用world执行协程
            World.current.StartCoroutine(DoDisassembleWithAnim(usingTool));
        }

        public virtual void DoAssemble(IDAUsingTool usingTool = null)
        {
            if ((State & CmsObjState.Installable) == 0)
                return;

            World.current.StartCoroutine(DoAssembleWithAnim(usingTool));
        }

        protected IEnumerator DoFixWithAnim(IDAUsingTool usingTool)
        {
            DebugEx.AssertNotNull(customDAScript);
              
            IsProcessing = true;

            yield return customDAScript.PlayFixAnim(usingTool);

            IsProcessing = false;

            if (customDAScript.IsAnimSuccess)
                State = CmsObjState.Fixed;
             
            if (AutoDefault)
                ForceSwitchState(CmsObjState.Default);
        }

        protected virtual IEnumerator DoAssembleWithAnim(IDAUsingTool usingTool)
        {
            IsProcessing = true;

            var success = false;

            //有可能已经播放过出现动画
            if (autoDisappear && DisplayMode != CmsDisplayMode.Default)
            {
                if(transRecorder != null)
                {
                    //回到动画播放后的位置（最后一次记录的位置）
                    transRecorder.Back();
                }
                
                yield return AppearWithAnim(usingTool);
            }

            if (customDAScript != null)
            {
                yield return customDAScript.PlayAssembleAnim(usingTool);

                success = customDAScript.IsAnimSuccess;
            }
            else
            {
                if (daDoTweenAnim != null)
                {
                    //内置dotween动画支持
                    yield return PlayDoTweenAnim(false);
                }

                success = true;
            }

            IsProcessing = false;

            if (success)
                State = CmsObjState.Assembled;
            
            OnAssembled(success);
             
            if (success && AutoFix)
            {
                DoFix();
            }
        }

        protected virtual IEnumerator DoDisassembleWithAnim(IDAUsingTool usingTool)
        {
            IsProcessing = true;
 
            var success = false;
  
            if(autoDisappear)
            {
                //拆装动作前缓存当前位置
                transRecorder = gameObject.AddMissingComponent<ModelTransformRecorder>();

                transRecorder.Lose(true);

                //默认状态
                transRecorder.Record(); 
            }

            if (customDAScript != null)
            {
                yield return customDAScript.PlayDisassembleAnim(usingTool);

                //播放完拆卸动画后位置状态会发生改变，记录
                if(transRecorder != null)
                    transRecorder.Record();

                success = customDAScript.IsAnimSuccess;
            }
            else
            {
                if (daDoTweenAnim != null)
                {
                    //内置dotween动画支持
                    yield return PlayDoTweenAnim(true);
                }

                success = true;
            }

            if (success && autoDisappear)
            {
                yield return DisappearWithAnim();

                //消失动画播放后  状态位置会发生改变，记录，appear时回到此位置并播放出现动画
                if(transRecorder != null)
                    transRecorder.Record();
            }

            IsProcessing = false;

            if (success)
            {
                //如果自动消失，需要拾取后才算拆下
                if (autoDisappear)
                    State = CmsObjState.WaitForPickup;
                else
                    State = CmsObjState.Dismantled;
            }
            
            OnDisassembled(success);
        }
         
        /// <summary>
        /// 拆卸完成
        /// </summary>
        /// <param name="success">是否成功</param>
        protected virtual void OnDisassembled(bool success)
        {
            
        }

        /// <summary>
        /// 安装完成
        /// </summary>
        /// <param name="success">是否成功</param>
        protected virtual void OnAssembled(bool success)
        {
            
        }

        protected IEnumerator PlayDoTweenAnim(bool dir)
        {
            if (dir)
            {
                daDoTweenAnim.tween.PlayForward();
            }
            else
            {
                daDoTweenAnim.tween.PlayBackwards();
            }

            yield return daDoTweenAnim.tween.WaitForCompletion(false);
        }

        protected virtual IEnumerator AppearWithAnim(IDAUsingTool usingTool)
        {
            SetDisplayMode(CmsDisplayMode.Default);

            if (disappearEffect != null)
               yield return disappearEffect.AppearWithAnim();
        }
        
        /// <summary>
        /// 消失出现动画用一套效果应该就可以了  待整理
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator DisappearWithAnim()
        {
            if (disappearEffect != null)
                yield return disappearEffect.DisappearWithAnim();

            //已经重复隐藏被操作的物体多次
            SetDisplayMode(CmsDisplayMode.Hide);
        }

        public void SetDisplayMode(CmsDisplayMode displayMode)
        {
            if (DisplayMode == displayMode)
                return;

            //Debug.Log("SetDisplayMode:" + displayMode);

            switch (displayMode)
            {
                case CmsDisplayMode.Default:
                    ChangeToDefaultDisplay();
                    break;
                case CmsDisplayMode.PlaceHolder:
                    if (transRecorder != null)
                    {
                        transRecorder.Entry();
                    }

                    ChangeToDisplayPlaceHolder();
                    break;
                case CmsDisplayMode.Hide:
                    ChangeToDisplayHide();
                    break;
            }

            DisplayMode = displayMode;
        }

        protected virtual void ChangeToDefaultDisplay()
        {
            gameObject.SetActive(true);

            if (graphicsCache != null)
            {
                graphicsCache.OriginalSharedMats();
            }
        }

        protected virtual void ChangeToDisplayPlaceHolder()
        {
            //Debug.Log("ChangeToDisplayPlaceHolder");

            gameObject.SetActive(true);

            if(graphicsCache == null)
                graphicsCache = interactObj.gameObject.AddMissingComponent<GraphicsCache>();

            graphicsCache.SwapGraphicsSharedMats(PlaceHolderMat, true);
        }
 
        protected virtual void ChangeToDisplayHide()
        {
            gameObject.SetActive(false);
        }

        //private void SendProcessCompleteMessage(bool success, CmsObjState targetState)
        //{
        //    processCompleteMessage = processCompleteMessage ?? new DAObjProcessCompleteMessage();

        //    processCompleteMessage.objCtr = this;

        //    processCompleteMessage.sucess = success;

        //    processCompleteMessage.targetState = targetState;

        //    Message.Send(processCompleteMessage);
        //}
    }
}