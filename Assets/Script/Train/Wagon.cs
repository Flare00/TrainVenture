using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class Wagon : MonoBehaviour
{
    public class Result
    {
        public Vector3 position;
        public Quaternion rotation;

        public Result(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    private Vector3 initialPos;
    private Quaternion initialRot;

    private Train train;
    [SerializeField]
    private float distEssieuAvant;
    [SerializeField]
    private float distEssieuArriere;
    [SerializeField]
    private float distance;

    [SerializeField]
    private Transform wagonModelTransform;

    public Wagon()
    {

    }

    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;

        GetValues();
    }

    public void SetToInitial()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
    }

    public void GetValues()
    {
        this.distEssieuAvant = this.transform.Find("PosEssieuAvant").localPosition.x;
        this.distEssieuArriere = this.transform.Find("PosEssieuArriere").localPosition.x;
        this.distance = this.transform.Find("Dist").localPosition.x;
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
        return this.distance * 2.0f * this.transform.localScale.x;
    }

    private Vector3 posEssieuAvant = Vector3.zero;
    private Vector3 posEssieuArriere= Vector3.zero;

    public Result ComputePositionRotation(SplineContainer chemin, float avancement, float avancementByMeter)
    {
        float avEssAv = (avancement + (avancementByMeter * distEssieuAvant)) % 1.0f;
        float avEssAr = (avancement + (avancementByMeter * distEssieuArriere)) % 1.0f;
        if(avEssAv < 0)
            avEssAv = 1.0f + avEssAv;
        if (avEssAr < 0)
            avEssAr = 1.0f + avEssAr;
        posEssieuAvant = chemin.EvaluatePosition(avEssAv);
        posEssieuArriere = chemin.EvaluatePosition(avEssAr);
		Vector3 right = Vector3.Cross(Vector3.up, posEssieuArriere - posEssieuAvant);
		Vector3 up = Vector3.Cross(posEssieuArriere - posEssieuAvant, right);


        return new Result((posEssieuAvant + posEssieuArriere) * 0.5f, Quaternion.LookRotation(right, up));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(posEssieuArriere, posEssieuAvant);
    }

    
}
