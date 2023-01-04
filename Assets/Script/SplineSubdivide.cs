using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class SplineSubdivide 
{

    /*public static Spline Subdivide(Spline spline, float frequency = 0.1f)
    {
        Spline res= new Spline();
        float freqSum = 0;
        while( freqSum <= 1.0f)
        {
            Vector3 pos = spline.EvaluatePosition(freqSum);

            Debug.Log("Freq : " + freqSum + " - " + pos);
            res.Add(new BezierKnot(pos));
            freqSum+= frequency;
        }
        res.EditType= SplineType.CatmullRom;
        return res;
    }*/

    public static Spline Subdivide(SplineContainer splineContainer, int number = 100)
    {
        Spline res = new Spline();
        float frequency = 1.0f / (float) number;
        float freqSum = 0;
        while (freqSum <= 1.0f)
        {
            res.Add(new BezierKnot(splineContainer.EvaluatePosition(freqSum)));
            freqSum += frequency;
        }
        res.EditType = SplineType.CatmullRom;
        return res;
    }
}
