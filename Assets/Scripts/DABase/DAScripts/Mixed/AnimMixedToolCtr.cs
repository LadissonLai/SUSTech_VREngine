using Framework;
using System.Collections;
using UnityEngine;
using VRTK;
using VRTKExtensions;

public class AnimMixedToolCtr : MonoBehaviour, IDAUsingTool
{
    protected Animator animator;

    protected Transform toolTrans;

    protected AdvancedInteractableObj interactObj;

    protected HandToolCollisionTracker collisionTracker;

    protected bool isUsing;

    public bool IsUsing
    {
        get => isUsing;
        set
        {
            isUsing = value;

            interactObj.validDrop =
                value
                ? VRTK_InteractableObject.ValidDropTypes.NoDrop
                : VRTK_InteractableObject.ValidDropTypes.DropAnywhere;

            if(isUsing)
            {
                collisionTracker.enabled = false;
            }
            else
            {
                if (interactObj.IsGrabbed())
                    collisionTracker.enabled = true;
            }
        }
    }

    protected AnimatorStateInfo CutAnimStateInfo => animator.GetCurrentAnimatorStateInfo(0);

    public string toolID;

    public virtual string ToolID
    {
        get => toolID;
        set
        {
            toolID = value;
        }
    }

    public bool FixUsingAble => false;

    private int defaultNameHash;

    protected virtual void Awake()
    {
        animator = animator == null ? GetComponent<Animator>() : animator;

        toolTrans = toolTrans == null ? transform.Find("Tool") : toolTrans;

        interactObj = GetComponent<AdvancedInteractableObj>();

        collisionTracker = GetComponent<HandToolCollisionTracker>();
    }

    public virtual IEnumerator PlayAnim(Transform processedObj, MixedToolDAScript.DAAnimType animType, float outLevel)
    {
        if (defaultNameHash == 0)
            defaultNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

        IsUsing = true;

        var srcLayer = gameObject.layer;

        gameObject.ApplyLayer("Default");

        //预留参数
        //animator.SetInteger("torque", 1);

        if (CutAnimStateInfo.shortNameHash != defaultNameHash)
        {
            animator.SetTrigger("Reset");

            yield return null;
        }

        animator.SetFloat("PartsOut", outLevel);
        animator.SetInteger("AnimType", (int)animType);
        animator.SetTrigger("Play");

        //等待进入新的状态 无过度时间
        yield return null;

        Vector3 initProcessedPos = processedObj.position;

        Vector3 initToolPos = toolTrans.position;

        while (true)
        {
            yield return null;

            processedObj.position = initProcessedPos + (toolTrans.position - initToolPos);

            //动画播放完毕后会自动回退到default
            if (defaultNameHash == CutAnimStateInfo.shortNameHash)
            {
                break;
            }
        }

        yield return null;

        animator.SetTrigger("Reset");

        gameObject.ApplyLayer(srcLayer);

        yield return null;

        IsUsing = false;
    }
}
