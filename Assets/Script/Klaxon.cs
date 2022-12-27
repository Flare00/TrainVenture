using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Klaxon : MonoBehaviour
{
    public AudioSource audioS;
    public Vector3 pushPos;
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    public void Enter()
    {
        if (audioS != null)
            audioS.Play();
        transform.localPosition = pushPos;
    }
    public void Exit()
    {
        transform.localPosition = initialPos;
    }
}
