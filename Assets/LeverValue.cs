using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverValue : MonoBehaviour
{
	public XRSimpleInteractable interactor;
    public GameObject lever;
	public Transform centerTransform;
	public Transform frontTransform;
    public int axis = 0; // 0 = X, 1 = Y, 2 = Z
    public float minDist= -1.0f;
    public float maxDist = 1.0f;

    public float minValue = -1.0f;
    public float maxValue = 1.0f;


    public float valeur = 0.0f;
	
	bool select = false;

    private float last = 0.0f;
	private Vector3 defaultValue;
    // Start is called before the first frame update
    void Start()
    {
        defaultValue = lever.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
		if(select){
			Vector3 fV = frontTransform.position - centerTransform.position;
			Vector3 vec = interactor.interactorsSelecting[0].transform.position - centerTransform.position;
			Vector3 p = Vector3.Project(vec, fV);
			
			float v = Vector3.Distance(Vector3.zero, p);
			v = Vector3.Dot(fV, p) < 0 ? -v : v;
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

				Vector3 a = defaultValue;
				a[axis] = v;
				
				lever.transform.localPosition = a;
				
				
				Debug.Log(lever.transform.localPosition);
				valeur = (((v - minDist) / (maxDist - minDist)) * (maxValue - minValue)) + minValue;
			}
			
			
		}
    }
	
	public void Selection(){
		select = true;
	}
	public void Deselection(){
		select = false;
	}
}
