using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftControls : Controls
{

    public InputActionProperty menuAction;

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
    }
}
