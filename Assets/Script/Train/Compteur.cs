using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compteur : MonoBehaviour
{
    public enum Axis
    {
        X, Y, Z
    };
    public GameObject aiguille;
    public GameObject aiguilleLimit;

    public float minCompteur;
    public float maxCompteur;

    public float rotationMin = 90;
    public float rotationMax = 270;
    public float rotationMaxLimit = 280;

    public Axis axis = Axis.Z;

    protected float limit = 0;
    protected float value = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetValue(float value)
    {
        this.value = value;
        SetRotation(aiguille, this.value);
    }

    public void SetLimit(float value)
    {
        limit = value;
        SetRotation(aiguilleLimit, limit);
    }

    public float GetValue()
    {
        return this.value;
    }
    public float GetLimit()
    {
        return this.limit;
    }

    private void SetRotation(GameObject aiguille, float value)
    {
        Vector3 tmp = aiguille.transform.rotation.eulerAngles;
        float val = GetRotationByValue(value);
        switch (axis)
        {
            case Axis.X:
                tmp.x = val;
                break;
            case Axis.Y:
                tmp.y = val;
                break;
            case Axis.Z:
                tmp.z = val;
                break;
        }

        aiguille.transform.rotation = Quaternion.Euler(tmp.x, tmp.y, tmp.z);
    }

    private float GetRotationByValue(float value)
    {
        float res = (((value - minCompteur) / (maxCompteur - minCompteur)) * (rotationMax - rotationMin)) + rotationMin;

        if(res < rotationMin) return rotationMin;
        if(res > rotationMaxLimit) return rotationMaxLimit;
        return res;
    }
}
