using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LeverValue : MonoBehaviour
{
    public GameObject leverParent;

    public GameObject lever;
    public int axis = 0; // 0 = X, 1 = Y, 2 = Z
    public float minDist= -1.0f;
    public float maxDist = 1.0f;

    public float minValue = -1.0f;
    public float maxValue = 1.0f;


    public float value = 0.0f;

    private Vector3 last;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(leverParent)
        Vector3 v = lever.transform.localPosition;
        Debug.Log(v);
        if (v != last)
        {
            last = v;
            if (v[axis] > maxDist)
            {
                v[axis] = maxDist;
                lever.transform.localPosition = v;
            }
            else if (v[axis] < minDist)
            {
                v[axis] = minDist;
                lever.transform.localPosition = v;
            }

            value = (((v[axis] - minDist) / (maxDist - minDist)) * (maxValue - minValue)) + minValue;

        }
    }
}
