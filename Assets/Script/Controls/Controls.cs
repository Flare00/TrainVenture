using System;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;




public class Controls : MonoBehaviour
{
    public static bool FORCE_RAYCAST = false;
    public GameObject raycastInteractor;

    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;

    protected bool local_force_raycast = false;
    protected bool raycast_status = false;
    protected XRRayInteractor rayInteractor;

    protected LayerMask defaultRayMask;

    private void Start()
    {
        rayInteractor = raycastInteractor.GetComponent<XRRayInteractor>();
        defaultRayMask = rayInteractor.raycastMask;
        raycastInteractor.SetActive(false);
    }
    private void Update()
    {
        Action();
    }

    virtual protected void Action()
    {
        float trigger = triggerAction.action?.ReadValue<float>() ?? 0.0f;
        float grip = gripAction.action?.ReadValue<float>() ?? 0.0f;

        Debug.Log("ForceRaycast : " + FORCE_RAYCAST);
        if (!FORCE_RAYCAST)
        {
            if (trigger > 0.1f | local_force_raycast)
            {
                if (!raycast_status)
                {
                    raycastInteractor.SetActive(true);
                    raycast_status = true;
                }
            }
            else
            {
                if (raycast_status)
                {
                    raycastInteractor.SetActive(false);
                    raycast_status = false;
                }
            }
        }
        else
        {
            if (!raycast_status)
            {
                raycastInteractor.SetActive(true);
                raycast_status = true;
            }
        }
    }

    public void ResetRaycastMask()
    {
        rayInteractor.raycastMask = defaultRayMask;
    }

    /*private void OnEnable()
    {
        selectAction.EnableDirectAction();
        selectValueAction.EnableDirectAction();
    }

    private void OnDisable()
    {
        selectAction.DisableDirectAction();
        selectValueAction.DisableDirectAction();
    }*/

}
