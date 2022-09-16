// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_Playmaker

using Doozy.Engine.SceneManagement;
using HutongGames.PlayMaker;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Integrations.Playmaker
{
    // ReSharper disable once UnusedMember.Global
    [ActionCategory("DoozyUI")]
    [Tooltip("Unload a scene via the SceneDirector")]
    public class UnloadScene : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        public FsmBool DebugMode;

        [UIHint(UIHint.FsmEnum)]
        public GetSceneBy GetSceneBy;

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
            SceneBuildIndex = new FsmInt {UseVariable = false, Value = SceneLoader.DEFAULT_BUILD_INDEX};
            SceneName = new FsmString {UseVariable = false, Value = SceneLoader.DEFAULT_SCENE_NAME};
            FinishImmediately = new FsmBool {UseVariable = false, Value = true};
        }

        public override void OnEnter()
        {
            if (DebugMode.Value) DDebug.Log("Playmaker - State Name [" + State.Name + "] - Unload Scene");
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    SceneDirector.UnloadSceneAsync(SceneName.Value);
                    break;
                case GetSceneBy.BuildIndex:
                    SceneDirector.UnloadSceneAsync(SceneBuildIndex.Value);
                    break;
            }

            if (FinishImmediately.Value) Finish();
        }
    }
}

#endif