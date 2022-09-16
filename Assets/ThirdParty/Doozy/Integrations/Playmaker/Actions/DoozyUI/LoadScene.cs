// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_Playmaker

using Doozy.Engine.SceneManagement;
using HutongGames.PlayMaker;
using UnityEngine.SceneManagement;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Integrations.Playmaker
{
    // ReSharper disable once UnusedMember.Global
    [ActionCategory("DoozyUI")]
    [Tooltip("Loads a scene by using a SceneLoader")]
    public class LoadScene : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        public FsmBool DebugMode;

        [UIHint(UIHint.FsmEnum)]
        public GetSceneBy GetSceneBy;

        [UIHint(UIHint.FsmEnum)]
        public LoadSceneMode LoadSceneMode;

        [UIHint(UIHint.FsmBool)]
        public FsmBool AllowSceneActivation;

        [UIHint(UIHint.FsmFloat)]
        public FsmFloat SceneActivationDelay;

        [UIHint(UIHint.FsmInt)]
        public FsmInt SceneBuildIndex;

        [UIHint(UIHint.FsmString)]
        public FsmString SceneName;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE this action will finish immediately")]
        public FsmBool FinishImmediately;

        public override void Reset()
        {
            DebugMode = new FsmBool {UseVariable = false, Value = false};
            GetSceneBy = SceneLoader.DEFAULT_GET_SCENE_BY;
            LoadSceneMode = SceneLoader.DEFAULT_LOAD_SCENE_MODE;
            AllowSceneActivation = new FsmBool {UseVariable = false, Value = SceneLoader.DEFAULT_AUTO_SCENE_ACTIVATION};
            SceneActivationDelay = new FsmFloat {UseVariable = false, Value = SceneLoader.DEFAULT_SCENE_ACTIVATION_DELAY};
            SceneBuildIndex = new FsmInt {UseVariable = false, Value = SceneLoader.DEFAULT_BUILD_INDEX};
            SceneName = new FsmString {UseVariable = false, Value = SceneLoader.DEFAULT_SCENE_NAME};
            FinishImmediately = new FsmBool {UseVariable = false, Value = true};
        }

        public override void OnEnter()
        {
            if (DebugMode.Value) DDebug.Log("Playmaker - State Name [" + State.Name + "] - Load Scene");
            SceneLoader.GetLoader()
                       .SetLoadSceneMode(LoadSceneMode)
                       .SetLoadSceneBy(GetSceneBy)
                       .SetSceneName(SceneName.Value)
                       .SetSceneBuildIndex(SceneBuildIndex.Value)
                       .SetAllowSceneActivation(AllowSceneActivation.Value)
                       .SetSceneActivationDelay(SceneActivationDelay.Value)
                       .SetSelfDestructAfterSceneLoaded(true)
                       .LoadSceneAsync();
            if (FinishImmediately.Value) Finish();
        }
    }
}

#endif