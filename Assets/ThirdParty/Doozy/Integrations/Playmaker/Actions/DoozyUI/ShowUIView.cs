// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_Playmaker

using System.Collections;
using Doozy.Engine;
using Doozy.Engine.UI;
using HutongGames.PlayMaker;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Integrations.Playmaker
{
    // ReSharper disable once UnusedMember.Global
    [ActionCategory("DoozyUI")]
    [Tooltip("Show an UIView")]
    public class ShowUIView : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        public FsmBool DebugMode;

        [UIHint(UIHint.FsmString)]
        [Tooltip("UIView category")]
        public FsmString ViewCategory;

        [UIHint(UIHint.FsmString)]
        [Tooltip("UIView name (found in the view category)")]
        public FsmString ViewName;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("Determines if the SHOW animation should happen instantly (in zero seconds)")]
        public FsmBool InstantAction;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE this action will finish immediately")]
        public FsmBool FinishImmediately;

        public override void Reset()
        {
            DebugMode = new FsmBool {UseVariable = false, Value = false};
            ViewCategory = new FsmString {UseVariable = false};
            ViewName = new FsmString {UseVariable = false};
            InstantAction = new FsmBool {UseVariable = false, Value = false};
            FinishImmediately = new FsmBool {UseVariable = false, Value = true};
        }

        public override void OnEnter()
        {
            if (DebugMode.Value) DDebug.Log("Playmaker - State Name [" + State.Name + "] - Show UIView");
            Coroutiner.Start(ShowView());
            if (FinishImmediately.Value) Finish();
        }

        private IEnumerator ShowView()
        {
            yield return null;
            UIView.ShowView(ViewCategory.Value, ViewName.Value, InstantAction.Value);
        }
    }
}

#endif