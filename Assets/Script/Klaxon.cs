using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Klaxon : MonoBehaviour
{
    public AudioSource audioS;
    public float enclenchementValue = 0.9f;
    public LeverValue lever;
    public Furnace four;

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
                if(four != null)
                {
                    if(four.pression > 1.0f)
                    {
                        four.pression -= Time.deltaTime * 0.4f;
                    }
                }
            }
        }
    }
}
