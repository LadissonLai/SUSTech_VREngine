using System.Collections;
using System.Collections.Generic;
using Fxb.DA;
using UnityEngine;

namespace Fxb.CMS
{
    /// <summary>
    /// 通过播放动画来进行拆装。
    /// 两种方式：1 使用旧的单独的anim进行播放 2 使用动画状态机进行播放
    /// </summary>
    public class AnimDAScript : AbstractDAScript
    {
        public override bool AutoFix { get => true; }

        private void OnValidate()
        {
            if (animParam.animRoot != null)
            {
                animParam.animRoot.playAutomatically = false;
            }
        }

        private void Reset()
        {
            if(animParam.animRoot == null)
                animParam.animRoot = GetComponentInParent<Animation>();
        }

        [System.Serializable]
        public struct AnimParam
        {
            public string disassembleAnimName;

            public string assembleAnimName;

            public Animation animRoot;

            public Animator animatorRoot;

            [Tooltip("可空")]
            public AnimationClip disassembleAnim;

            [Tooltip("可空")]
            public AnimationClip assembleAnim;
        }

        public AnimParam animParam;

        public override IEnumerator PlayDisassembleAnim(IDAUsingTool usingTool = null)
        {
            yield return null;

            if (animParam.animRoot != null)
            {
                string name = animParam.disassembleAnim ? animParam.disassembleAnim.name : 
                    animParam.disassembleAnimName;

                yield return PlayLegacyAnim(animParam.animRoot, name);
            }
            else
            {
                //todo
            }

            IsAnimSuccess = true;
        }

        public override IEnumerator PlayAssembleAnim(IDAUsingTool usingTool = null)
        {
            yield return null;

            if (animParam.animRoot != null)
            {
                string name = animParam.assembleAnim ? animParam.assembleAnim.name :
                   animParam.assembleAnimName;

                yield return PlayLegacyAnim(animParam.animRoot, name);
            }

            IsAnimSuccess = true;
        }

        private IEnumerator PlayLegacyAnim(Animation anim, string animName)
        {
            anim.Play(animName);

            yield return new WaitUntil(() => anim.IsPlaying(animName));

            yield return new WaitUntil(() => !anim.isPlaying);
        }

        public override IEnumerator PlayFixAnim(IDAUsingTool usingTool = null)
        {
            IsAnimSuccess = true;
            yield break;
        }
    }
}

