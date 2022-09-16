using System;
using UnityEngine;
using VRTK.GrabAttachMechanics;

using DG.Tweening;

/// <summary>
/// 抓取后  物体飞到目的地。
/// </summary>
public class FlyTargetGrabAttach : VRTK_ChildOfControllerGrabAttach
{
    public bool autoDisableColliders = true;

    private Sequence dotweenSeq;

    public event Action<bool> IsGrab;
 
    public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
    {
        if (dotweenSeq != null)
            dotweenSeq.Kill(false);

        var res = base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);

        if (res && autoDisableColliders)
        {
            SetCollidersEnable(grabbedObjectScript, false);
        }
        return res;
    }

    public override void StopGrab(bool applyGrabbingObjectVelocity)
    {
        if (dotweenSeq != null)
            dotweenSeq.Kill(false);

        if (grabbedObjectScript != null && autoDisableColliders)
            SetCollidersEnable(grabbedObjectScript, true);
        IsGrab?.Invoke(false);
        base.StopGrab(applyGrabbingObjectVelocity);
    }

    private void SetCollidersEnable(Component target, bool enable)
    {
        var colliders = target.GetComponentsInChildren<Collider>();

        Array.ForEach(colliders, (c) => {
            c.enabled = enable;
        });
    }
     
    protected override void SnapObjectToGrabToController(GameObject obj)
    {
        obj.transform.SetParent(controllerAttachPoint.transform);

        var fromLocalPos = obj.transform.localPosition;
        var fromLocalRotation = obj.transform.localRotation;

        if (!precisionGrab)
        {
            SetSnappedObjectPosition(obj);
        }

        var toLocalPos = obj.transform.localPosition;
        var toLocalR = obj.transform.localRotation;

        obj.transform.localPosition = fromLocalPos;
        obj.transform.localRotation = fromLocalRotation;
         
        dotweenSeq = DOTween.Sequence();

        dotweenSeq.Join(obj.transform.DOLocalMove(toLocalPos, 0.1f));
        dotweenSeq.Join(obj.transform.DOLocalRotateQuaternion(toLocalR, 0.1f));
        IsGrab?.Invoke(true);
    }

}
