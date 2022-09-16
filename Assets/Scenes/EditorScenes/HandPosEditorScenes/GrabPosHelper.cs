using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK;
 
public class GrabPosHelper : MonoBehaviour
{
    public bool isLeftHand;

    public VRTK_InteractableObject targetObj;

    public float targetGrabAnimBlendVal = 0.6f;

    protected Rigidbody attachTo;

    protected Animator animator;

    private void UpdateGrabPose()
    {
        if (targetObj == null)
            return;

        if (targetObj.grabAttachMechanicScript == null)
            targetObj.grabAttachMechanicScript = targetObj.GetComponent<VRTK_BaseGrabAttach>();

        if (targetObj.grabAttachMechanicScript == null)
            return;

        if (attachTo == null)
            attachTo = GetComponentInChildren<Rigidbody>();

        Transform snapHandle = null;

        if (isLeftHand)
            snapHandle = targetObj.grabAttachMechanicScript.leftSnapHandle;
        else
            snapHandle = targetObj.grabAttachMechanicScript.rightSnapHandle;

        if (snapHandle == null)
        {
            targetObj.transform.rotation = attachTo.transform.rotation;

            targetObj.transform.position = attachTo.transform.position;
        }
        else
        {
            targetObj.transform.rotation = attachTo.transform.rotation * Quaternion.Inverse(snapHandle.transform.rotation) * targetObj.transform.rotation;
            targetObj.transform.position = attachTo.transform.position - (snapHandle.transform.position - targetObj.transform.position);
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrabPose();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        animator.SetFloat("Point", 0);
        animator.SetFloat("Trigger", targetGrabAnimBlendVal);
    }
}
