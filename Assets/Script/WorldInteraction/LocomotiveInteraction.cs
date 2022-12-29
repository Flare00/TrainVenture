using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LocomotiveInteraction : MonoBehaviour
{
    public CoalBox coalbox;
    public Train train;

    private bool inGare = false;
    private bool oneTime = false;

    private void Update()
    {
        if (coalbox != null && train != null )
        {
            if (inGare && train.speed <= 0.1f )
            {
                if (oneTime)
                {
                    coalbox.Refill();
                    oneTime = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gare")
        {
            inGare = true;
            oneTime = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gare")
        {
            inGare = false;
        }
    }
}
