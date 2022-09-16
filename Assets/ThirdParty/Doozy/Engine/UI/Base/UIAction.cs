// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Events;
using Doozy.Engine.Soundy;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Base
{
    /// <summary>
    ///     Versatile event-like class to that allows for the execution of several 'actions' at once.
    ///     <para />
    ///     With a single method call it can invoke an Action, invoke a list of AnimatorEvent, play or stop an UIEffect (ParticleSystem), invoke an UnityEvent,
    ///     send a list of game events and play a sound.
    /// </summary>
    [Serializable]
    public class UIAction
    {
        #region Properties

        /// <summary> Returns the number of entries in the AnimatorEvents list </summary>
        public int AnimatorEventsCount
        {
            get { return AnimatorEvents.Count; }
        }

        /// <summary> Returns the number of entries in the GameEvents list </summary>
        public int GameEventsCount
        {
            get { return GameEvents.Count; }
        }

        /// <summary> Returns TRUE if the AnimatorEvents list has at least one entry </summary>
        public bool HasAnimatorEvents
        {
            get { return AnimatorEvents != null && AnimatorEventsCount > 0; }
        }

        /// <summary> Returns TRUE if the Effect (UIEffect) has a ParticleSystem referenced </summary>
        public bool HasEffect
        {
            get { return Effect != null && Effect.ParticleSystem != null; }
        }

        /// <summary> Returns TRUE if the GameEvents list has at least one entry </summary>
        public bool HasGameEvents
        {
            get { return GameEvents != null && GameEventsCount > 0; }
        }

        /// <summary> Returns TRUE if SoundData has valid sound settings </summary>
        public bool HasSound
        {
            get
            {
                switch (SoundData.SoundSource)
                {
                    case SoundSource.Soundy:      return SoundData.SoundName != SoundyManager.NO_SOUND && !string.IsNullOrEmpty(SoundData.SoundName);
                    case SoundSource.AudioClip:   return SoundData.AudioClip != null;
                    case SoundSource.MasterAudio: return !string.IsNullOrEmpty(SoundData.SoundName);
                    default:                      return false;
                }
            }
        }


        /// <summary> Returns TRUE if the Event (UnityEvent) has at least one registered persistent listener </summary>
        public bool HasUnityEvent
        {
            get { return Event != null && UnityEventListenerCount > 0; }
        }

        /// <summary> Returns the number of registered persistent listeners </summary>
        public int UnityEventListenerCount
        {
            get { return Event.GetPersistentEventCount(); }
        }

        #endregion

        #region Public Variables

        /// <summary> Callback executed when this UIAction is executed </summary>
        public Action<GameObject> Action = delegate { };

        /// <summary> Animator Events invoked when this UIAction is executed </summary>
        public List<AnimatorEvent> AnimatorEvents = new List<AnimatorEvent>();

        /// <summary> UIEffect invoked when this UIAction is executed </summary>
        public UIEffect Effect;

        /// <summary> Callback executed when this UIAction is executed </summary>
        public UnityEvent Event;

        /// <summary> Game Events sent when this UIAction is executed </summary>
        public List<string> GameEvents = new List<string>();

        /// <summary> SoundyData used by Soundy to play a sound when this UIAction is executed </summary>
        public SoundyData SoundData;

        #endregion

        #region Private Variables

        /// <summary> Internal variable to the Canvas reference used by the Effect </summary>
        private Canvas m_canvasForEffect;

        #endregion

        #region Constructor

        /// <summary> Initializes a new instance of the class </summary>
        public UIAction() { Reset(); }

        #endregion

        #region Public Methods

        /// <summary> Add and AnimatorEvent, to the AnimatorEvents list, that will get invoked when this UIAction is invoked </summary>
        /// <param name="animatorEvent"> AnimatorEvent that gets invoked when this UIAction is invoked </param>
        public UIAction AddAnimatorEvent(AnimatorEvent animatorEvent)
        {
            if (animatorEvent == null) return this;
            if (AnimatorEvents.Contains(animatorEvent)) return this;
            AnimatorEvents.Add(animatorEvent);
            return this;
        }
        
        /// <summary> Add a list of AnimatorEvent that will get invoked when this UIAction is invoked </summary>
        /// <param name="animatorEvents"> List of AnimatorEvents </param>
        public UIAction AddAnimatorEvents(List<AnimatorEvent> animatorEvents)
        {
            if (animatorEvents == null) return this;
            animatorEvents = animatorEvents.Where(x => x != null).ToList();
            foreach (AnimatorEvent animatorEvent in animatorEvents)
               if(!AnimatorEvents.Contains(animatorEvent))
                   AnimatorEvents.Add(animatorEvent);
            return this;
        }

        /// <summary> Add a game event, to the GameEvents list, that will get sent when this UIAction is invoked </summary>
        /// <param name="gameEvent"> Game event string </param>
        /// <param name="clearGameEventsList"> Clears the GameEvents list before adding this game event string to it</param>
        public UIAction AddGameEvent(string gameEvent, bool clearGameEventsList = false)
        {
            gameEvent = gameEvent.Trim();
            if (string.IsNullOrEmpty(gameEvent)) return this;
            if(clearGameEventsList) GameEvents.Clear();
            if (GameEvents.Contains(gameEvent)) return this;
            GameEvents.Add(gameEvent);
            return this;
        }

        /// <summary> Add a list of game events, that will get sent when this UIAction is invoked </summary>
        /// <param name="gameEvents"> List of game events </param>
        /// <param name="clearGameEventsList"> Clears the GameEvents list before adding this game event string to it</param>
        public UIAction AddGameEvents(List<string> gameEvents, bool clearGameEventsList = false)
        {
            if (gameEvents == null) return this;
            for (int i = 0; i < gameEvents.Count; i++)
                gameEvents[i] = gameEvents[i].Trim();
            gameEvents = gameEvents.Where(s => !string.IsNullOrEmpty(s)).ToList();
            if(clearGameEventsList) GameEvents.Clear();
            foreach (string gameEvent in gameEvents)
                if(!GameEvents.Contains(gameEvent))
                    GameEvents.Add(gameEvent);
            return this;
        }

        /// <summary> Returns the first Canvas component attached to the source GameObject. If no Canvas is found, it returns null. This method is needed mostly used by the UIEffect </summary>
        /// <param name="source"> Source GameObject that should have a Canvas attached </param>
        /// <param name="refresh"> Because this method also stores the Canvas reference for future use (for efficiency), refresh forces another GetComponent to be triggered on the source GameObject </param>
        public Canvas GetCanvas(GameObject source, bool refresh = false)
        {
            if (!refresh && m_canvasForEffect != null && (m_canvasForEffect.isRootCanvas || m_canvasForEffect.overrideSorting)) return m_canvasForEffect;
            m_canvasForEffect = null;
            Canvas[] canvases = source.GetComponentsInParent<Canvas>();
            if (canvases == null || canvases.Length == 0) return m_canvasForEffect;
            foreach (Canvas canvas in canvases)
            {
                if (!canvas.isRootCanvas && !canvas.overrideSorting) continue;
                m_canvasForEffect = canvas;
                break;
            }

            return m_canvasForEffect;
        }

        /// <summary> Invokes all the enabled settings </summary>
        /// <param name="source"> Target GameObject that triggered this invoke </param>
        /// <param name="playSound"> Enable to play sound </param>
        /// <param name="playEffect"> Enable to play effect </param>
        /// <param name="playAnimatorEvents"> Enable to invoke animator events </param>
        /// <param name="sendGameEvents"> Enable to send game events </param>
        /// <param name="invokeUnityEvent"> Enable to invoke the Event (UnityEvent) </param>
        /// <param name="invokeAction"> Enable to invoke the Action (System.Action) </param>
        public void Invoke(GameObject source,
                           bool playSound = true,
                           bool playEffect = true,
                           bool playAnimatorEvents = true,
                           bool sendGameEvents = true,
                           bool invokeUnityEvent = true,
                           bool invokeAction = true)
        {
            if (playSound) PlaySound();
            if (playEffect) ExecuteEffect(GetCanvas(source));
            if (playAnimatorEvents) InvokeAnimatorEvents();
            if (sendGameEvents) SendGameEvents(source);
            if (invokeUnityEvent) InvokeUnityEvent();
            if (invokeAction) InvokeAction(source);
        }

        /// <summary> Invokes the Action of this UIAction, if it's not null </summary>
        /// <param name="source"></param>
        public void InvokeAction(GameObject source)
        {
            if (Action == null) return;
            Action.Invoke(source);
        }

        /// <summary> Invokes the Event (UnityEvent) of this UIAction, if it's not null </summary>
        public void InvokeUnityEvent()
        {
            if (Event == null) return;
            Event.Invoke();
        }

        /// <summary> Invokes all the AnimatorEvent entries in the AnimatorEvents list </summary>
        public void InvokeAnimatorEvents()
        {
            if (!HasAnimatorEvents) return;
            foreach (AnimatorEvent animatorEvent in AnimatorEvents)
                animatorEvent.Invoke();
        }

        /// <summary> Plays the UIEffect (the target ParticleSystem) of this UIAction </summary>
        public void ExecuteEffect(Canvas canvas)
        {
            if (!HasEffect) return;
            if (canvas == null)
            {
                Effect.Execute();
                return;
            }

            Effect.Execute(canvas.sortingLayerName, canvas.sortingOrder);
        }

        /// <summary> Plays the sound (if enabled) of this UIAction </summary>
        public void PlaySound()
        {
            if (!HasSound) return;
            SoundyManager.Play(SoundData);
        }
        
        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            SoundData = new SoundyData();
            AnimatorEvents = new List<AnimatorEvent>();
            GameEvents = new List<string>();
            Event = new UnityEvent();
            Action = delegate { };
        }

        /// <summary> Sends any registered game events this UIAction has </summary>
        /// <param name="source"> Track back reference </param>
        public void SendGameEvents(GameObject source)
        {
            if (!HasGameEvents) return;
            GameEventMessage.SendEvents(GameEvents, source.gameObject);
        }
        
        /// <summary> Set the Action callback executed when this UIAction is invoked </summary>
        /// <param name="action"> Callback executed when this UIAction is invoked </param>
        public UIAction SetAction(Action<GameObject> action)
        {
            Action = action;
            return this;
        }
        
        /// <summary> Set and UIEffect that will get invoked when this UIAction is invoked </summary>
        /// <param name="effect"> Target UIEffect </param>
        public UIAction SetEffect(UIEffect effect)
        {
            if (effect == null) return this;
            Effect = effect;
            return this;
        }
        
        /// <summary> Set a new SoundyData that will be used by Soundy to play a sound when this UIAction is invoked </summary>
        /// <param name="soundyData"> SoundyData used by Soundy to play a sound when this UIAction is invoked </param>
        public UIAction SetSoundyData(SoundyData soundyData)
        {
            if (soundyData == null) return this;
            SoundData = soundyData;
            return this;
        }

        #endregion
    }
}