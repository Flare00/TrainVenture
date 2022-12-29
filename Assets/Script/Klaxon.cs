using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Klaxon : MonoBehaviour
{
    public AudioSource audioS;
    public float enclenchementValue = 0.9f;
    public LeverValue lever;

    void Start()
    {
    }

    private void Update()
    {
        if (audioS != null)
        {
            if (lever.valeur > enclenchementValue )
            {
                if(!audioS.isPlaying)
                {
                    audioS.Play();
                }
            }
        }
    }
}
