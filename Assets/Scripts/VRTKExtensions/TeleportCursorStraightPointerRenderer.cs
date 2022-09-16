using UnityEngine;
using VRTK;

public class TeleportCursorStraightPointerRenderer : VRTK_StraightPointerRenderer
{
    public GameObject teleportTargetObj;

    private GameObject actualTeleportTargetObj;
    
    private VRTK_BasicTeleport currentTeleport;

    public override bool IsCursorVisible()
    {
        if (!base.IsCursorVisible())
            return false;

        if (destinationHit.transform == null)
            return false;

        bool policyValid = false;

        if (playareaCursor != null)
            policyValid = !VRTK_PolicyList.Check(destinationHit.transform.gameObject, playareaCursor.targetListPolicy);
         
        return policyValid;
    }

    protected override void CreatePointerObjects()
    {
        base.CreatePointerObjects();

        if (teleportTargetObj != null)
        {
            actualTeleportTargetObj = Instantiate(teleportTargetObj);
            actualTeleportTargetObj.transform.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "StraightPointerRenderer_TeleportTarget");
            actualTeleportTargetObj.transform.SetParent(actualContainer.transform);
            VRTK_PlayerObject.SetPlayerObject(actualTeleportTargetObj, VRTK_PlayerObject.ObjectTypes.Pointer);

            if (currentTeleport == null)
                currentTeleport = Object.FindObjectOfType<VRTK_BasicTeleport>();

            actualTeleportTargetObj.gameObject.SetActive(false);
        }
    }

    protected override void UpdateDependencies(Vector3 location)
    {
        base.UpdateDependencies(location);

        if (actualTeleportTargetObj != null)
        {
            actualTeleportTargetObj.transform.rotation = Quaternion.identity;
            actualTeleportTargetObj.transform.position = location;
        }
    }

    protected override void ToggleRenderer(bool pointerState, bool actualState)
    {
        base.ToggleRenderer(pointerState, actualState);
         
        if (actualTeleportTargetObj != null && !pointerState && actualTeleportTargetObj.gameObject.activeSelf)
            actualTeleportTargetObj.gameObject.SetActive(false);
    }

    protected override void CheckRayHit(bool rayHit, RaycastHit pointerCollidedWith)
    {
        base.CheckRayHit(rayHit, pointerCollidedWith);

        if (currentTeleport == null || actualTeleportTargetObj == null)
            return;

        bool actualState = false;

        if (rayHit && destinationHit.transform != null)
        {
            actualState = !VRTK_PolicyList.Check(destinationHit.transform.gameObject, currentTeleport.targetListPolicy);
        }

        if (actualTeleportTargetObj.activeSelf != actualState)
            actualTeleportTargetObj.SetActive(actualState);
    }
}
