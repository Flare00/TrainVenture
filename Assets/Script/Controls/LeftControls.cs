using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftControls : Controls
{

    public InputActionProperty menuAction;
    public InputActionProperty teleportationAction;

    private bool teleportationPressed = false;
    private bool menuHold = false;

    override protected void Action()
    {
        base.Action();

        if ((menuAction.action?.ReadValue<float>() ?? 0.0f) > 0.5f)
        {
            if (!menuHold)
            {
                Pause.PAUSE_ENABLED = !Pause.PAUSE_ENABLED;
                menuHold = true;
            }
        }
        else
        {
            menuHold = false;
        }

        if (teleportationAction != null)// && teleportRay != null) 
        {

            Vector2 v = teleportationAction.action.ReadValue<Vector2>();
            if (!teleportationPressed)
            {
                if (Mathf.Abs(v.x) > 0.1f || Mathf.Abs(v.y) > 0.1f)
                {
                    teleportationPressed = true;
                    local_force_raycast = true;
                    rayInteractor.raycastMask = 1 << 10; //teleportation Area Mask
                }
            }
            else
            {
                if (Mathf.Abs(v.x) < 0.1f || Mathf.Abs(v.y) < 0.1f)
                {
                    /*RaycastHit r;
                    if (raycastInteractor.GetComponent<XRRayInteractor>().TryGetCurrent3DRaycastHit(out r))
                    {
                        TeleportationArea tp;
                        if (r.collider.TryGetComponent<TeleportationArea>(out tp))
                        {
                            tp.teleportTrigger = new BaseTeleportationInteractable.TeleportTrigger();
                        }
                    }*/
                    teleportationPressed = false;
                    local_force_raycast = false;

                    ResetRaycastMask();
                }
            }
        }
    }
}
