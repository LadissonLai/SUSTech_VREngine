// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_Playmaker

using Doozy.Engine;
using HutongGames.PlayMaker;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Integrations.Playmaker
{
    // ReSharper disable once UnusedMember.Global
    [ActionCategory("DoozyUI")]
    [Tooltip("Send a Game Event")]
    public class SendGameEvent : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        public FsmBool DebugMode;

        [UIHint(UIHint.FsmString)]
        public FsmString GameEvent;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE this action will finish immediately")]
        public FsmBool FinishImmediately;

        public override void Reset()
        {
            DebugMode = new FsmBool {UseVariable = false, Value = false};
            GameEvent = new FsmString {UseVariable = false};
            FinishImmediately = new FsmBool {UseVariable = false, Value = true};
        }

        public override void OnEnter()
        {
            if (DebugMode.Value) DDebug.Log("Playmaker - State Name [" + State.Name + "] - Send Game Event: " + GameEvent.Value);
            GameEventMessage.SendEvent(GameEvent.Value);
            if (FinishImmediately.Value) Finish();
        }
    }
}

#endif