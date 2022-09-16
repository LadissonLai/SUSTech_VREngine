// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_Playmaker

using Doozy.Engine.Soundy;
using HutongGames.PlayMaker;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Integrations.Playmaker
{
    // ReSharper disable once UnusedMember.Global
    [ActionCategory("Soundy")]
    [Tooltip("Play the specified sound with the given category and name (from Soundy)")]
    public class PlaySound : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        public FsmBool DebugMode;

        [UIHint(UIHint.FsmString)]
        [Tooltip("SoundDatabase database name that contains the sound name")]
        public FsmString DatabaseName;

        [UIHint(UIHint.FsmString)]
        [Tooltip("Sound name of a SoundGroupData that holds settings and references to one or more audio clips")]
        public FsmString SoundName;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE this action will finish immediately")]
        public FsmBool FinishImmediately;
        
        public override void Reset()
        {
            DebugMode = new FsmBool {UseVariable = false, Value = false};
            DatabaseName = new FsmString {UseVariable = false};
            SoundName = new FsmString {UseVariable = false};
            FinishImmediately = new FsmBool { UseVariable = false, Value = true };
        }

        public override void OnEnter()
        {
            if (DebugMode.Value) DDebug.Log("Playmaker - State Name [" + State.Name + "] - Play Sound");
            SoundyManager.Play(DatabaseName.Value, SoundName.Value);
            if (FinishImmediately.Value) Finish();
        }
    }
}

#endif