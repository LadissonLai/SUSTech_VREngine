using System.Collections;
using Framework;
using UnityEngine;
using VRTKExtensions;
using Doozy.Engine;
using Fxb.Localization;
using Framework.Tools;

namespace Fxb.DA
{
    /// <summary>
    /// 使用扳手的拆卸与安装动画
    /// </summary>
    public class WrenchDAScriptV2 : AbstractDAScript
    {
        private Pose screwPoseCache;

        [Tooltip("一般是螺丝的根部 留空表示使用自身transform")]
        public Transform wrenchUsePos;

        [Tooltip("是否需要匹配目标位置方向, 如果不匹配的话则通过当前视角自动找到合适的方向")]
        public bool matchPosDirection = true;

        public WrenchUseCondition wrenchUseCondition;

        [Tooltip("螺丝拆装时移动的距离")]
        public ScrewOutLevel screwOut = ScrewOutLevel.TwoCM;

        [Tooltip("自定义螺丝的移动距离")]
        public float customOutLevel;

        public override bool AutoFix { get => autoFix; }

        [Tooltip("是否自动固定（拧固）")]
        public bool autoFix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns>单位厘米</returns>
        protected float ScrewOutLevelToCM(ScrewOutLevel level)
        {
            return customOutLevel == 0 ? (int)level * 0.5f : customOutLevel;
        }

        /// <summary>
        /// 拆装后会有误差，恢复到拆下前的位置状态
        /// </summary>
        protected virtual void ResetScrewPose()
        {
            transform.localPosition = screwPoseCache.position;

            transform.localRotation = screwPoseCache.rotation;
        }

        public override IEnumerator PlayAssembleAnim(IDAUsingTool usingTool = null)
        {
            var screwOutRes = screwOut;

            var wrenchCtr = usingTool as AnimWrenchCtr;

            var useAbleRes = CheckWrenchUseAble(false, wrenchCtr);

            IsAnimSuccess = useAbleRes.useAble;

            if (!IsAnimSuccess)
            {
                Message.Send(new DAToolErrorMessage(useAbleRes.errorMsg, daObjID, DAAnimType.Assemble));

                yield break;
            }

            yield return PlayWrenchAnim(AnimWrenchCtr.AnimType.Assembly, screwOut, wrenchCtr);

            if (!AutoFix)
                ResetScrewPose();
        }

        public override IEnumerator PlayDisassembleAnim(IDAUsingTool usingTool = null)
        {
            var screwOutRes = screwOut;

            var wrenchCtr = usingTool as AnimWrenchCtr;
             
            var useAbleRes = CheckWrenchUseAble(false, wrenchCtr);

            IsAnimSuccess = useAbleRes.useAble;

            if (!IsAnimSuccess)
            {
                Message.Send(new DAToolErrorMessage(useAbleRes.errorMsg, daObjID, DAAnimType.Disassemble));

                yield break;
            }

            screwPoseCache = new Pose(transform.localPosition, transform.localRotation);

            yield return PlayWrenchAnim(AnimWrenchCtr.AnimType.Disassembly, screwOutRes, wrenchCtr);
        }

        /// <summary>
        /// 紧固时默认螺丝是不动的
        /// </summary>
        /// <returns></returns>
        public override IEnumerator PlayFixAnim(IDAUsingTool usingTool = null)
        {
            if (AutoFix)
                yield break;

            var wrenchCtr = usingTool as AnimWrenchCtr;
           
            var useAbleRes = CheckWrenchUseAble(true, wrenchCtr);

            IsAnimSuccess = useAbleRes.useAble;

            //是否成功都可以播放对应动画
            if (!IsAnimSuccess)
            {
                Message.Send(new DAToolErrorMessage(useAbleRes.errorMsg,daObjID, DAAnimType.Fix));

                yield break;
            }

            yield return PlayWrenchAnim(AnimWrenchCtr.AnimType.Fix, ScrewOutLevel.NoMove, wrenchCtr);

            ResetScrewPose();
        }

        public override bool CheckAssembleCondition(IDAUsingTool usingTool = null)
        {
            return CheckWrenchUseAble(false, usingTool as AnimWrenchCtr).useAble;
        }

        public override bool CheckDisassembleCondition(IDAUsingTool usingTool = null)
        {
            return CheckWrenchUseAble(false, usingTool as AnimWrenchCtr).useAble;
        }

        public override bool CheckFixCondition(IDAUsingTool usingTool = null)
        {
            return CheckWrenchUseAble(true, usingTool as AnimWrenchCtr).useAble;
        }

        protected virtual (bool useAble, string errorMsg) CheckWrenchUseAble(bool isFixed, AnimWrenchCtr wrench)
        {
            (bool useAble, string errorMsg) res = (true, null);
             
            if (wrench == null)
            {
                res.useAble = false;

                res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("请使用正确的工具进行操作");

                return res;
            }

#if UNITY_EDITOR
            if (DAConfig.ignoreWrenchConditionCheck)
                return res;
#endif

            return CheckUseCondition(wrench, wrenchUseCondition, isFixed);
        }

        protected virtual (bool useAble, string errorMsg) CheckUseCondition(AnimWrenchCtr wrench, WrenchUseCondition checkCondition, bool isFixed)
        {
            (bool useAble, string errorMsg) res = (true, null);

            var wrenchInfo = wrench.WrenchInfo;

            if (
                (!string.IsNullOrEmpty(checkCondition.taotongID) && wrenchInfo.taotong != checkCondition.taotongID)
                ||
                (!isFixed && !string.IsNullOrEmpty(checkCondition.banshouID) && wrenchInfo.banshou != checkCondition.banshouID)
                ||
                (isFixed && !string.IsNullOrEmpty(checkCondition.fixBanshouID) && wrenchInfo.banshou != checkCondition.fixBanshouID)
                )
            {
                res.useAble = false;

                res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("请使用正确的套筒组合工具操作");

                return res;
            }

            if (checkCondition.needJiegan)
            {
                if (string.IsNullOrEmpty(wrenchInfo.jiegan))
                {
                    res.useAble = false;

                    res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("需要接杆");

                    return res;
                }
            }

            if (!isFixed && wrenchInfo.torsion > -1)
            {
                res.useAble = false;

                res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("请使用棘轮扳手操作");

                return res;
            }

            if (isFixed
                && (wrenchInfo.torsion < checkCondition.minFixTorsionRange || wrenchInfo.torsion > checkCondition.maxFixTorsionRange))
            {
                res.useAble = false;

                if (wrenchInfo.torsion < 0)
                {
                    res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("使用扭力扳手操作");
                }
                else
                {
                    res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("扭力设置错误");
                }

                return res;
            }

            if (!res.useAble && res.errorMsg == null)
                res.errorMsg = LocalizeMgr.Inst.GetLocalizedStr("请使用正确的工具进行操作");

            return res;
        }

        protected virtual IEnumerator PlayWrenchAnim(AnimWrenchCtr.AnimType animType, ScrewOutLevel screwOut, AnimWrenchCtr wrench)
        {
#if UNITY_EDITOR
            if (DAConfig.skipToolAnimation)
                yield break;
#endif

            wrench.gameObject.AddMissingComponent<ModelTransformRecorder>(out var transCache);

            transCache.Record();

            wrench.transform.SetParent(null);

            MatchToScrewPose(wrench);

            yield return wrench.PlayWrenchAnim(transform, ScrewOutLevelToCM(screwOut), animType);

            transCache.Back(true);

            yield return new WaitForSeconds(0.2f);
        }

        protected void MatchToScrewPose(AnimWrenchCtr wrench)
        {
            var attachPos = wrenchUsePos == null ? transform : wrenchUsePos;

            var lookForward = Vector3.zero;

            if (matchPosDirection)
            {
                lookForward = attachPos.forward;
            }
            else
            {
                var headDir = (attachPos.position - VRTKHelper.HeadSetCamera.transform.position).normalized;

                lookForward = Vector3.ProjectOnPlane(headDir, attachPos.up).normalized;

                lookForward = Vector3.Cross(lookForward, attachPos.up);
            }

            wrench.transform.rotation = Quaternion.LookRotation(lookForward, attachPos.up);

            var connectPos = wrench.ScrewConnect == null ? wrench.transform.position : wrench.ScrewConnect.position;

            wrench.transform.position += (attachPos.position - connectPos);
        }
    }
}
