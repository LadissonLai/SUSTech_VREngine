using Doozy.Engine;
using Framework;
using Framework.Tools;
using Fxb.DA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTKExtensions;

//public class MultiEnumAttribute : PropertyAttribute { }

//[CustomPropertyDrawer(typeof(MultiEnumAttribute))]
//public class MultiEnumDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);
//    }
//}

public class MixedToolDAScript : AbstractDAScript
{
    public string toolID;

    [Tooltip("是否自动固定（拧固）")]
    public bool autoFix = true;

    public override bool AutoFix { get => autoFix; }

    private Pose screwPoseCache;

    [Tooltip("其他工具附着的位置 留空表示使用自身transform")]
    public Transform mixedAttachedPos;

    [Tooltip("是否需要匹配目标位置方向, 如果不匹配的话则通过当前视角自动找到合适的方向")]
    public bool matchPosDirection = true;

    /// <summary>
    /// 跳过某些阶段
    /// </summary>
    protected List<DAAnimType> validAnimState = new List<DAAnimType>() {
        DAAnimType.Assemble, DAAnimType.Disassemble, DAAnimType.Fix };

    [Tooltip("拆装时部件移动的距离 暂时根据扳手的规则移动")]
    public ScrewOutLevel daPartsOut = ScrewOutLevel.OneCM;

    //[Tooltip("是否播放失败的动画")]
    //public bool isPlayFailedAnimation = true;

    //[MultiEnum]
    //public DAAnimType DAAnimType;

    protected virtual void ResetScrewPose()
    {
        transform.localPosition = screwPoseCache.position;

        transform.localRotation = screwPoseCache.rotation;
    }

    public override IEnumerator PlayAssembleAnim(IDAUsingTool usingTool = null)
    {
        var screwoutLevel = daPartsOut;

        IsAnimSuccess = CheckAssembleCondition(usingTool);

        if (!validAnimState.Contains(DAAnimType.Assemble))
            yield break;

        if (!IsAnimSuccess)
        {     
            Message.Send(new DAToolErrorMessage("请使用正确的工具进行操作", daObjID, DAAnimType.Assemble));

            //if (isPlayFailedAnimation)
            //    screwoutLevel = ScrewOutLevel.NoMove;
            //else
            yield break;
        }

        yield return PlayMixedToolAnim(DAAnimType.Assemble, screwoutLevel);

        if (!AutoFix)
            ResetScrewPose();
    }

    public override IEnumerator PlayDisassembleAnim(IDAUsingTool usingTool = null)
    {
        var screwoutLevel = daPartsOut;

        IsAnimSuccess = CheckDisassembleCondition(usingTool);

        if (!validAnimState.Contains(DAAnimType.Disassemble))
            yield break;

        if (!IsAnimSuccess)
        {
            Message.Send(new DAToolErrorMessage("请使用正确的工具进行操作", daObjID, DAAnimType.Disassemble));

            //if (isPlayFailedAnimation)
            //    screwoutLevel = ScrewOutLevel.NoMove;
            //else
            yield break;
        }

        screwPoseCache = new Pose(transform.localPosition, transform.localRotation);

        yield return PlayMixedToolAnim(DAAnimType.Disassemble, screwoutLevel);
    }

    public override IEnumerator PlayFixAnim(IDAUsingTool usingTool = null)
    {
        if (AutoFix)
            yield break;

        var screwoutLevel = daPartsOut;

        IsAnimSuccess = CheckFixCondition(usingTool);

        if (!validAnimState.Contains(DAAnimType.Fix))
            yield break;

        if (!IsAnimSuccess)
        {
            Message.Send(new DAToolErrorMessage("请使用正确的工具进行操作", daObjID, DAAnimType.Fix));

            //if (isPlayFailedAnimation)
            //    screwoutLevel = ScrewOutLevel.NoMove;
            //else
            yield break;
        }

        yield return PlayMixedToolAnim(DAAnimType.Fix, screwoutLevel);

        ResetScrewPose();
    }

    /// <summary>
    /// 如果同一类型DAAnimType有多个动画 可全部覆写该方法
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual IEnumerator PlayMixedToolAnim(DAAnimType type, ScrewOutLevel outLevel)
    {
#if UNITY_EDITOR
        if (DAConfig.skipToolAnimation)
            yield break;
#endif

        IsAnimSuccess = false;

        var mixedTool = VRTKHelper.FindGrabedObjCom<AnimMixedToolCtr>();

        mixedTool.gameObject.AddMissingComponent<ModelTransformRecorder>(out var transCache);

        transCache.Record();

        mixedTool.transform.SetParent(null);

        var attachPos = mixedAttachedPos == null ? transform : mixedAttachedPos;

        mixedTool.transform.position = attachPos.position;

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

        mixedTool.transform.rotation = Quaternion.LookRotation(lookForward, attachPos.up);

        yield return mixedTool.PlayAnim(transform, type, (int)outLevel * 0.5f);

        transCache.Back(true);

        yield return new WaitForSeconds(0.2f);

        IsAnimSuccess = true;
    }

    public override bool CheckAssembleCondition(IDAUsingTool usingTool = null)
    {
        return CheckMixedToolValid(usingTool);
    }

    public override bool CheckDisassembleCondition(IDAUsingTool usingTool = null)
    {
        return CheckMixedToolValid(usingTool);
    }

    public override bool CheckFixCondition(IDAUsingTool usingTool = null)
    {
        return CheckMixedToolValid(usingTool);
    }

    public virtual bool CheckMixedToolValid(IDAUsingTool usingTool = null)
    {
        var toolCtr = usingTool as AnimMixedToolCtr;

        if (!toolCtr || toolID != toolCtr.ToolID)
        {
            return false;
        }

        return true;
    }
}


