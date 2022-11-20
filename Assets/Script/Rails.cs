using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Rails : MonoBehaviour
{
    public SplineContainer spline;
    private float length;
    // Start is called before the first frame update
    void Start()
    {
        length = spline.CalculateLength();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
