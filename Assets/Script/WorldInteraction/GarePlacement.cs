using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GarePlacement : MonoBehaviour
{
    public GameObject terrain;
    public SplineContainer spline;

    public List<float> garePosition;

    // Start is called before the first frame update
    void Start()
    {
        foreach(float pos in garePosition)
        {
            GameObject g = GameObject.Instantiate(Resources.Load("Prefabs/Gare/Gare") as GameObject);
            g.transform.SetParent(terrain.transform, false);
            Vector3 v= spline.EvaluatePosition(pos);
            Vector3 vDir= spline.EvaluatePosition(pos + 0.01f);


            Vector3 right = Vector3.Cross(Vector3.up, v - vDir);
            Vector3 up = Vector3.Cross(v - vDir, right);

            g.transform.SetPositionAndRotation(v, Quaternion.LookRotation(right, up));
        }
    }

}
