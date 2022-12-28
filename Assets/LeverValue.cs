using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverValue : MonoBehaviour
{

    public enum Axis
    {
        X, Y, Z
    }

    public enum Retour
    {
        False,
        Min,
        Max,
        Middle
    }
    public GameObject lever;
    public Axis axis = Axis.X;
    public bool invertAxis = false;
    public float minDist = -1.0f;
    public float maxDist = 1.0f;

    public float minValue = -1.0f;
    public float maxValue = 1.0f;

    public Retour retourAuto = Retour.False;

    public bool invertValue;
    public float offset = 0.0f;

    public float valeur = 0.0f;

    private bool select = false;

    private float last = 0.0f;

    private Transform attachment;

    private Vector3 defaultLocalValue;
    // Start is called before the first frame update
    void Start()
    {
        defaultLocalValue = lever.transform.localPosition;
        if (retourAuto != Retour.False)
        {
            RetourAuto();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (select)
        {
            float v = transform.InverseTransformPoint(attachment.position)[(int)axis] + offset;

            v = invertAxis ? -v : v;

            if (v != last)
            {
                last = v;
                if (v > maxDist)
                {
                    v = maxDist;
                }
                else if (v < minDist)
                {
                    v = minDist;
                }

                Vector3 a = defaultLocalValue;
                a[(int)axis] = v ;

                lever.transform.localPosition = a;

                valeur = (((v - minDist) / (maxDist - minDist)) * (maxValue - minValue)) + minValue;
                valeur = invertValue ? (maxValue - valeur) + minValue : valeur;
            }


        }
    }

    private void RetourAuto()
    {
        float v = 0;

        switch (retourAuto)
        {
            case Retour.Min:
                v = minValue;
                break;
            case Retour.Max:
                v = maxValue;
                valeur = maxValue;
                break;
            case Retour.Middle:
                v = ((maxValue - minValue) / 2.0f) + minValue;
                break;

        }

        valeur = v;

        v = invertValue ? (maxValue - v) + minValue : v; 
        float dist = (((v - minValue)/(maxValue - minValue)) * (maxDist - minDist)) + minDist;

        Vector3 a = defaultLocalValue;
        a[(int)axis] = dist;

        lever.transform.localPosition = a;
    }

    public void SelectEnter(SelectEnterEventArgs args)
    {
        select = true;
        attachment = args.interactorObject.transform;
    }
    public void SelectExit(SelectExitEventArgs args)
    {
        select = false;
        attachment = null;
        if (retourAuto != Retour.False)
        {
            RetourAuto();
        }

    }
}
