using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabObject : MonoBehaviour
{
    private Transform attachment;

    private void FixedUpdate()
    {
        if (attachment != null)
        {
            transform.position = attachment.position;

        }
    }

    public void Select(SelectEnterEventArgs args)
    {
        attachment= args.interactorObject.transform.Find("Anchor");
    }

    public void Unselect(SelectExitEventArgs args)
    {
        attachment = null;
    }
}
