using DG.Tweening;
using Doozy.Engine.Soundy;
using System;
using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 具有正反两种状态的动画脚本
    /// </summary>
    public class InteractStateAnim : MonoBehaviour
    {
        [Serializable]
        public struct SoundDatas
        {
            public SoundyData startSound;

            public SoundyData completeSound;
        }

        public VRTK_InteractableObject interactableObj;

        /// <summary>
        /// 是否自动倒播
        /// </summary>
        public bool autoRewind;
         
        protected DOTweenAnimation doTweenAnim;

        /// <summary>
        /// param:bool->方向
        /// </summary>
        public event Action<InteractStateAnim, bool> OnAnimComplete;

        /// <summary>
        /// param:bool->方向
        /// </summary>
        public event Action<InteractStateAnim, bool> OnAnimStart;

        public SoundDatas forwardSoundDatas;

        public SoundDatas backwardsSoundDatas;

        /// <summary>
        /// 默认false，播放一次后变为true
        /// </summary>
        public bool AnimStateFlag { get; private set; }

        public bool IsPlaying { get; private set; }
 
        private void Awake()
        {
            interactableObj = interactableObj == null ? GetComponent<VRTK_InteractableObject>() : interactableObj;
        }

        private void OnDestroy()
        {
            OnAnimComplete = null;
            OnAnimStart = null;
        }

        private void Start()
        {
            if(interactableObj != null)
                interactableObj.InteractableObjectUsed += InteractableObj_InteractableObjectUsed;

            doTweenAnim = doTweenAnim ?? GetComponent<DOTweenAnimation>();

            if(doTweenAnim != null)
            {
                doTweenAnim.tween.onComplete += OnDOTweenAnimComplete;
                doTweenAnim.tween.onRewind += OnDOTweenAnimComplete;
            }
        }

        private void PlaySoundOnComplete(bool flag)
        {
            if (flag && forwardSoundDatas.completeSound?.SoundName != SoundyManager.NO_SOUND)
                SoundyManager.Play(forwardSoundDatas.completeSound);
            else if (!flag && backwardsSoundDatas.completeSound?.SoundName != SoundyManager.NO_SOUND)
                SoundyManager.Play(backwardsSoundDatas.completeSound);
        }

        private void PlaySoundOnStart(bool flag)
        {
            if (flag && forwardSoundDatas.startSound?.SoundName != SoundyManager.NO_SOUND)
                SoundyManager.Play(forwardSoundDatas.startSound);
            else if (!flag && backwardsSoundDatas.startSound?.SoundName != SoundyManager.NO_SOUND)
                SoundyManager.Play(backwardsSoundDatas.startSound);
        }

        private void InteractableObj_InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
        {
            PlayStateAnim(!AnimStateFlag);
        }
         
        public void PlayStateAnim(bool stateFlag)
        {
            if (stateFlag == AnimStateFlag)
                return;

            IsPlaying = true;

            if (doTweenAnim != null)
            {
                PlayDotweenAnim(stateFlag);
            }

            AnimStateFlag = stateFlag;
 
            OnAnimStart?.Invoke(this,stateFlag);

            PlaySoundOnStart(stateFlag);
        }
         
        protected void PlayDotweenAnim(bool flag)
        {
            Debug.Assert(doTweenAnim.tween != null);

            if (flag)
            {
                doTweenAnim.tween.PlayForward();
            }
            else
            {
                doTweenAnim.tween.PlayBackwards();
            }
        }

        private void OnDOTweenAnimComplete()
        {
            IsPlaying = false;
            OnAnimComplete?.Invoke(this,AnimStateFlag);

            PlaySoundOnComplete(AnimStateFlag);

            if (AnimStateFlag && autoRewind)
                PlayStateAnim(false);
        }
    }
}