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
            transform.rotation = attachment.rotation;
        }
    }

    public void SelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("SELECT");
        attachment = args.interactorObject.transform;
        transform.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void SelectExit(SelectExitEventArgs args)
    {
        Debug.Log("UN SELECT");

        attachment = null;
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }
}
