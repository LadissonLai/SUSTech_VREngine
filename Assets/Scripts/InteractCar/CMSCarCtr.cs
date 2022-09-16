using Doozy.Engine;
using Framework;
using System;
using UnityEngine;
using VRTKExtensions;
using static Fxb.CMSVR.CmsCarState;

namespace Fxb.CMSVR
{
    public class CMSCarCtr : MonoBehaviour
    {
        [Serializable]
        public class AnimFlagTrigger
        {
            public SwitchState targetSwitch;

            /// <summary>
            /// true:正向  false:反向 
            /// </summary>
            public bool animFlag;

            /// <summary>
            /// 控制是开启还是关闭switch
            /// </summary>
            public bool switchOn;

            public LockState targetLocks;

            /// <summary>
            /// 控制是锁定还是解锁 true:解锁  false:锁定
            /// </summary>
            public bool unLock;
        }

        [Serializable]
        public class AnimTrigger
        {
            public InteractStateAnim interactAnim;

            public AnimFlagTrigger[] flagTriggers;
        }

        private CarStateChangedMessage carStateChangedMessage;

        private CmsCarState carState;

        public InteractStateAnim lfDoorAnim;

        public AnimTrigger[] switchTriggers;

        protected virtual void InitCarState()
        {

        }

        protected virtual void AddStateAnimEvents()
        {
            if (switchTriggers == null)
                return;

            for (int i = 0; i < switchTriggers.Length; i++)
            {
                var st = switchTriggers[i];

                st.interactAnim.OnAnimComplete += InteractAnim_OnAnimComplete;
            }
        }

        private void InteractAnim_OnAnimComplete(InteractStateAnim anim, bool flag)
        {
            var targetTrigger = Array.Find(switchTriggers, (trigger)=> { return trigger.interactAnim == anim; });

            if (targetTrigger == null)
                return;

            foreach (var flagTrigger in targetTrigger.flagTriggers)
            {
                if (flagTrigger.animFlag != flag)
                    continue;

                if(flagTrigger.targetSwitch != SwitchState.Undefined)
                {
                    var switchStateChanged = false;

                    carStateChangedMessage.stateType = CarStateChangedMessage.StateType.SwitchState;

                    //控制开关
                    if (flagTrigger.switchOn && carState.switchStates.AddUnique(flagTrigger.targetSwitch))
                        switchStateChanged = true;
                    else if (carState.switchStates.Remove(flagTrigger.targetSwitch))
                        switchStateChanged = true;

                    if(switchStateChanged)
                    {
                        carStateChangedMessage.stateType = CarStateChangedMessage.StateType.SwitchState;

                        carStateChangedMessage.intNewState = (int)flagTrigger.targetSwitch;

                        Message.Send(carStateChangedMessage);
                    }
                }

                if(flagTrigger.targetLocks != LockState.Undefined)
                {
                    var lockStateChanged = false;

                    if (flagTrigger.unLock && carState.unLockerStates.AddUnique(flagTrigger.targetLocks))
                        lockStateChanged = true;
                    else if(carState.unLockerStates.Remove(flagTrigger.targetLocks))
                        lockStateChanged = true;

                    if(lockStateChanged)
                    {
                        carStateChangedMessage.stateType = CarStateChangedMessage.StateType.LockState;

                        carStateChangedMessage.intNewState = (int)flagTrigger.targetLocks;

                        Message.Send(carStateChangedMessage);
                    }
                }
            }
        }
 
        private void Awake()
        {
            carStateChangedMessage = new CarStateChangedMessage();

            carState = World.current.Injecter.Regist<CmsCarState>();

            InitCarState();

            AddStateAnimEvents();
        }

        private void OnDestroy() {
            World.current.Injecter.UnRegist<CmsCarState>();
        }
    }
}
