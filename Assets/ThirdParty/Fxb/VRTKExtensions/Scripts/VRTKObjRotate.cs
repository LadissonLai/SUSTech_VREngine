using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VRTKObjRotate : MonoBehaviour
{
    public VRTK_InteractableObject interactableObj;

    public float restartRotateDelay;

    [Tooltip("角度/秒")]
    public float rotateSpeed = 30f;
    [Tooltip("默认自转")]
    public Transform rotateCenter;

    protected float restartRotateFlag;

    protected bool isAllowRotate;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (interactableObj == null)
            interactableObj = GetComponent<VRTK_InteractableObject>();
        rotateCenter = rotateCenter ? rotateCenter : transform;
        isAllowRotate = true;
    }


    protected virtual void LateUpdate()
    {
        if (interactableObj.IsTouched() || interactableObj.IsGrabbed() || !isAllowRotate)
        {
            restartRotateFlag = restartRotateDelay;

            return;
        }

        if (restartRotateFlag > 0)
        {
            restartRotateFlag -= Time.deltaTime;

            return;
        }

        //旋转
        transform.RotateAround(rotateCenter.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
