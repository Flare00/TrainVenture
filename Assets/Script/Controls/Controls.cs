using System;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;




public class Controls : MonoBehaviour
{
    public static bool FORCE_RAYCAST = false;
    public GameObject raycastInteractor;
    public GameObject anchor;

    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;

    private bool raycast_status = false;

    private void Start()
    {

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

        if (!FORCE_RAYCAST)
        {
            if (trigger > 0.1f)
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
