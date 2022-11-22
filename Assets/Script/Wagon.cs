using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class Wagon : MonoBehaviour
{
    private Train train;

    private float distEssieuAvant;
    private float distEssieuArriere;
    private BoxCollider boxCollider;
    private Transform wagonModelTransform;


    void Start()
    {
        this.distEssieuAvant = this.transform.Find("PosEssieuAvant").localPosition.x;
        this.distEssieuArriere = this.transform.Find("PosEssieuArriere").localPosition.x;
        this.boxCollider = this.GetComponentInChildren<BoxCollider>();
        this.wagonModelTransform = this.transform.Find("Wagon");
    }


    void Update()
    {
        
    }

    public void SetTrain(Train train)
    {
        this.train = train;
    }

    public float GetLength()
    {
        return this.boxCollider.size.x * wagonModelTransform.localScale.x * this.transform.localScale.x;
    }

    private Vector3 posEssieuAvant = Vector3.zero;
    private Vector3 posEssieuArriere= Vector3.zero;

    public void ComputePositionRotation(SplineContainer chemin, float avancement, float avancementByMeter)
    {
        float avEssAv = (avancement + (avancementByMeter * distEssieuAvant)) % 1.0f;
        float avEssAr = (avancement + (avancementByMeter * distEssieuArriere)) % 1.0f;
        if(avEssAv < 0)
            avEssAv = 1.0f + avEssAv;
        if (avEssAr < 0)
            avEssAr = 1.0f + avEssAr;
        posEssieuAvant = chemin.EvaluatePosition(avEssAv);
        posEssieuArriere = chemin.EvaluatePosition(avEssAr);
        this.transform.SetPositionAndRotation((posEssieuAvant + posEssieuArriere) * 0.5f, Quaternion.FromToRotation(Vector3.right, posEssieuAvant - posEssieuArriere));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(posEssieuArriere, posEssieuAvant);
    }
}
