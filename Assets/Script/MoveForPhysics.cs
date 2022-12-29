using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForPhysics : MonoBehaviour
{
    private Vector3 initialPos;
    private Quaternion initialRotation;

    void Start()
    {
        initialPos = transform.position;
        initialRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        transform.position = initialPos;
        transform.rotation = initialRotation;
    }
}
