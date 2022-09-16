using DG.Tweening;
using UnityEngine;
using VRTK;
using Doozy.Engine.Soundy;
using VRTKExtensions;
using Doozy.Engine.UI;
using Framework;
using System;
using Doozy.Engine;
using Fxb.Localization;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 可改成预制动画 或者抛弃dotween Update手动刷新位置
    /// </summary>
    public class CarLiftCtr : MonoBehaviour
    {
        private DOTweenAnimation upBtnAni, downBtnAni;

        private float initYPos;

        private float targetInitYPos;

        private float liftLocation;

        private CarLiftLocationChangedMessages liftMessageCache;

        private SoundyController soundyController;

        public AdvancedInteractableObj up_InteractableObj, down_InteractableObj;

        public Transform jushengji;

        public Transform target;

        [Tooltip("举升机最高位置x和花费时间y")]
        public Vector2[] topPosAndDuration;

        /// <summary>
        /// 举升机与目标在不同动画阶段的初始Y
        /// </summary>
        Vector2[] initYPosLiftAndTargetWithLevel;

        /// <summary>
        /// 举升机当前位置的阶段 0为初始位置 递增+1
        /// </summary>
        int curTargetPosLevel;

        float originPos;

        Tween jushengjiTween;

        /// <summary>
        /// 播放中的动画方向
        /// </summary>
        bool isUp;

        /// <summary>
        /// 从静止状态开始的未播完动画 剩下的时间 处理频繁切换动画状态
        /// </summary>
        float remainedTime;

        /// <summary>
        /// 从静止状态开始的动画 0 down 1 up -1 none
        /// </summary>
        int upOrDownAnimating;

        /// <summary>
        /// 举升机举起目标所需的高度
        /// </summary>
        const float LIFTRAISETARGETTHRESHOLD = 0.01f;

        /// <summary>
        /// 每个动画阶段的实际距离
        /// </summary>
        float distance;

        protected void Start()
        {
            up_InteractableObj.InteractableObjectUnused += Up_InteractableObj_OnHandUsed;
            down_InteractableObj.InteractableObjectUnused += Down_InteractableObj_OnHandUsed;

            upBtnAni = up_InteractableObj.GetComponent<DOTweenAnimation>();
            downBtnAni = down_InteractableObj.GetComponent<DOTweenAnimation>();

            originPos = jushengji.localPosition.y;

            initYPosLiftAndTargetWithLevel = new Vector2[topPosAndDuration.Length];

            initYPosLiftAndTargetWithLevel[0] = new Vector2(jushengji.position.y, target.transform.position.y);

            for (int i = 0; i < topPosAndDuration.Length - 1; i++)
            {
                initYPosLiftAndTargetWithLevel[i + 1] = new Vector2(topPosAndDuration[i].x,
                    target.transform.position.y + topPosAndDuration[i].x - jushengji.position.y);
            }
        }

        private void OnJuShengJiAnimUpdate()
        {
            var animPos = jushengji.position.y - initYPos;

            liftLocation = Mathf.Clamp(animPos / distance, 0.0f, 1.0f);

            //模拟真实的举升
            if (jushengji.localPosition.y > LIFTRAISETARGETTHRESHOLD)
            {
                target.transform.position = new Vector3(target.transform.position.x
                    , targetInitYPos + animPos, target.transform.position.z);
            }

            //Debug.Log($"liftLocation:{liftLocation}");

            //处理传出去的liftLocation的变化
            var msgLocation = isUp ? liftLocation + curTargetPosLevel - 1 : liftLocation + curTargetPosLevel;

            World.Get<CmsCarState>().liftLocation = msgLocation;

            liftMessageCache = liftMessageCache ?? new CarLiftLocationChangedMessages();

            Message.Send(liftMessageCache);

            //完成状态
            if (liftLocation != 1 && liftLocation != 0)
                return;

            if (soundyController != null)
            {
                soundyController.Stop();
                soundyController = null;
            }

            ResetLiftBtn();

            Clear();
        }

        private void Down_InteractableObj_OnHandUsed(object sender, InteractableObjectEventArgs e)
        {
            //Debug.Log("Down----used");

            if (!CheckLiftValid(false))
                return;

            //没有手动中断 && 底部
            if (remainedTime == 0 && curTargetPosLevel == 0)
                return;

            if (!AnimateLift(false))
                return;

            downBtnAni.DOPlayForward();
            upBtnAni.DOPlayBackwards();

            if (liftLocation >= 1)
            {
                if (soundyController)
                    soundyController.Stop();

                ResetLiftBtn();
            }
            //else if (liftLocation > 0.0f)
            //{

            //    if (soundyController == null)
            //    {
            //        soundyController = SoundyManager.Play(DoozyNamesDB.SOUND_CATEGORY_CARSOUND, DoozyNamesDB.SOUND_CARSOUND_LIFTINGMACHINE);
            //    }
            //    else
            //    {
            //        soundyController.Unpause();
            //    }
            //}
        }

        private void Up_InteractableObj_OnHandUsed(object sender, InteractableObjectEventArgs e)
        {
            //Debug.Log(e.interactingObject.name);

            if (!CheckLiftValid(true))
                return;

            //没有手动中断 && 顶点
            if (remainedTime == 0 && curTargetPosLevel >= topPosAndDuration.Length)
                return;

            if (!AnimateLift(true))
                return;

            downBtnAni.DOPlayBackwards();
            upBtnAni.DOPlayForward();

            if (liftLocation >= 1.0f)
            {
                if (soundyController)
                    soundyController.Stop();

                ResetLiftBtn();
            }
            else if (liftLocation < 1.0f)
            {

                //if (soundyController == null)
                //{
                //    soundyController = SoundyManager.Play(DoozyNamesDB.SOUND_CATEGORY_CARSOUND, DoozyNamesDB.SOUND_CARSOUND_LIFTINGMACHINE);
                //}
                //else
                //{
                //    soundyController.Unpause();
                //}
            }
        }

        void ResetLiftBtn()
        {
            downBtnAni.DOPlayBackwards();
            upBtnAni.DOPlayBackwards();
        }

        /// <summary>
        /// 举升机动画
        /// </summary>
        /// <param name="ascend">是否升高</param>
        bool AnimateLift(bool ascend)
        {
            //未播完的动画已经流逝的时间 动画完成position=0
            float elapsedTime = jushengjiTween == null ? 0 : jushengjiTween.position;

            if (TryStopAnim(ascend, elapsedTime))
                return false;

            if (TryResumeAnim(ascend, elapsedTime))
                return true;

            DOTween.Kill(jushengji);

            float animTime;

            if (ascend)
            {
                curTargetPosLevel += 1;

                initYPos = initYPosLiftAndTargetWithLevel[curTargetPosLevel - 1].x;

                targetInitYPos = initYPosLiftAndTargetWithLevel[curTargetPosLevel - 1].y;

                if (elapsedTime == 0)
                {
                    upOrDownAnimating = 1;

                    remainedTime = topPosAndDuration[curTargetPosLevel - 1].y;
                }

                remainedTime = upOrDownAnimating == 0 ? remainedTime - elapsedTime : remainedTime + elapsedTime;

                animTime = upOrDownAnimating == 0 ? topPosAndDuration[curTargetPosLevel - 1].y - remainedTime : remainedTime;
            }
            else
            {
                curTargetPosLevel -= 1;

                initYPos = initYPosLiftAndTargetWithLevel[curTargetPosLevel].x;

                targetInitYPos = initYPosLiftAndTargetWithLevel[curTargetPosLevel].y;

                if (elapsedTime == 0)
                {
                    upOrDownAnimating = 0;

                    remainedTime = topPosAndDuration[curTargetPosLevel].y;
                }

                remainedTime = upOrDownAnimating == 1 ? remainedTime - elapsedTime : remainedTime + elapsedTime;

                animTime = upOrDownAnimating == 1 ? topPosAndDuration[curTargetPosLevel].y - remainedTime : remainedTime;
            }

            //Debug.Log("curTargetPosLevel" + curTargetPosLevel);

            float targetPos = curTargetPosLevel == 0 ? originPos : topPosAndDuration[curTargetPosLevel - 1].x;

            DoAnimateLift(ascend, targetPos, animTime);

            return true;
        }

        bool CheckLiftValid(bool ascend)
        {
            if (World.Get<DASceneState>().isTaskPreparing)
            {
                Popup_Tips.Show(LocalizeMgr.Inst.GetLocalizedStr("请先选择一个任务"));

                return false;
            }

            if (World.Get<DASceneState>().batteryLiftDeviceState !=BatteryLiftDeviceState.Default)
            {
                Popup_Tips.Show(LocalizeMgr.Inst.GetLocalizedStr("请不要在操作举升装置时移动举升机"));

                return false;
            }

            return true;
        }

        bool TryStopAnim(bool ascend, float elapsedTime)
        {
            if (jushengjiTween != null && jushengjiTween.active && jushengjiTween.IsPlaying() && ascend == isUp)
            {
                //Debug.Log("Kill");

                jushengjiTween.Kill();

                //同向
                if (upOrDownAnimating == 1 && ascend || (upOrDownAnimating == 0 && !ascend))
                    remainedTime -= elapsedTime;
                else
                    remainedTime += elapsedTime;

                ResetLiftBtn();
                //todo 可能存在停止时刚好达到挡位的情况 待观察
                return true;
            }

            return false;
        }

        //状态 时间 liftlocation
        bool TryResumeAnim(bool ascend, float elapsedTime)
        {
            if (elapsedTime == 0 && remainedTime != 0)
            {
                //Debug.Log($"resume：{curTargetPosLevel}");

                //反向切换目标点
                if (!ascend && isUp)
                    curTargetPosLevel -= 1;
                else if (ascend && !isUp)
                    curTargetPosLevel += 1;

                switch (upOrDownAnimating)
                {
                    case 0:
                        remainedTime = ascend ? topPosAndDuration[curTargetPosLevel - 1].y - remainedTime : remainedTime;

                        break;
                    case 1:
                        remainedTime = ascend ? remainedTime :
                            topPosAndDuration[curTargetPosLevel].y - remainedTime;

                        break;
                    default:
                        Debug.LogError("upOrDownAnimating 错误的状态");

                        return true;
                }

                upOrDownAnimating = ascend ? 1 : 0;

                float targetPos;

                targetPos = curTargetPosLevel == 0 ? originPos : topPosAndDuration[curTargetPosLevel - 1].x;

                DoAnimateLift(ascend, targetPos, remainedTime);

                return true;
            }

            return false;
        }


        void DoAnimateLift(bool ascend, float targetPos, float time)
        {
            jushengjiTween = jushengji.DOLocalMoveY(targetPos, time).SetEase(Ease.Linear);

            jushengjiTween.onUpdate += OnJuShengJiAnimUpdate;

            isUp = ascend;

            World.Get<CmsCarState>().liftUp = isUp;

            if (isUp)
                distance = topPosAndDuration[curTargetPosLevel - 1].x - initYPos;
            else
                distance = topPosAndDuration[curTargetPosLevel].x - initYPos;
        }

        void Clear()
        {
            liftLocation = 0;

            remainedTime = 0;

            upOrDownAnimating = -1;

            distance = 0;
        }
    }
}
