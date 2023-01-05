using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultForceRayCast : MonoBehaviour
{
    public bool defaultStatus = false;
    // Start is called before the first frame update
    void Start()
    {
        Controls.FORCE_RAYCAST = defaultStatus;       
    }
}
