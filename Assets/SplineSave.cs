using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class SplineSave : MonoBehaviour, IDataSave
{
	public SplineContainer spline;
    // Start is called before the first frame update
    void Start()
    {
		/*Spline trueSpline = spline.Spline;
		for(int i = 0; i < trueSpline.Count; i++){
			BezierKnot b = trueSpline[i];
			b.Position[0] = b.Position[0] / 2048.0f;
			b.Position[1] = b.Position[1] / 2048.0f;
			b.Position[2] = b.Position[2] / 2048.0f;
			trueSpline[i] = b;
		}
		SaveLoad.GetInstance().SaveSpline(spline.Spline, "spline.data");*/
		//spline.Spline = SaveLoad.GetInstance().LoadSpline("spline.data");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
