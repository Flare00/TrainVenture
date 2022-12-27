using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForDraw : MonoBehaviour
{
    private Vector3 initialPos;
    private Quaternion initialRotation;

    public Transform anchor;

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

    private void Update()
    {
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
    }

}
