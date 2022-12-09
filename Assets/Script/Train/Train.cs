using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class Train : MonoBehaviour
{
    public SplineContainer chemin;

    public LeverValue speedLever;

    public List<Wagon> wagons;
    private List<float> distWagon;

    public float avancement = 0.0f;
    public bool direction = true;
    public float speed = 0.0f; //in meter per seconds
    public float maxSpeed = 5.0f; //in meter per second 1m/s = 3.6km/h
    public float throttle = 0.0f; // si < 0 : Freine / recule, si = 0 : ne bouge pas / ralentit a cause du poid, si > 0 et suffisant : accélère (ralentit a cause du poid si insuffisant)

    //private Wagon locomotive;
    private float avancementByMeter = 0.0f;

    public Train()
    {
        wagons = new List<Wagon>();
    }

    private void Start()
    {
        UpdateData();
    }


    private void Update()
    {
        if(speedLever != null)
        {
            this.throttle = speedLever.valeur;
        }
        float tmpSpeed = speed + (throttle *0.1f);
        if (tmpSpeed < maxSpeed)
        {
            speed = tmpSpeed;
        } else { 
            speed = maxSpeed;
        }
        if(speed < 0)
        {
            speed = 0;
        }

        avancement += (speed * avancementByMeter) * Time.deltaTime;
        if (avancement >= 1.0f)
        {
            avancement %= 1.0f;
        }

        //locomotive.ComputePositionRotation(this.chemin, this.avancement, this.avancementByMeter);
        for (int i = 0, max = wagons.Count; i < max; i++)
        {
            wagons[i].ComputePositionRotation(this.chemin, this.avancement - (distWagon[i] * this.avancementByMeter), this.avancementByMeter);
        }
    }

    public void SetData(SplineContainer chemin, float avancement = 0.0f, bool direction = true, float maxSpeed = 5.0f, float speed = 0.0f,  float throttle = 0.0f)
    {
        this.chemin = chemin;
        this.avancement = avancement;
        this.direction = direction;
        this.maxSpeed = maxSpeed;
        this.speed = speed;
        this.throttle = throttle;
    }

    public void UpdateData()
    {
        distWagon = new List<float>();
        //locomotive = GetComponent<Wagon>();
        avancementByMeter = 1.0f / this.chemin.CalculateLength();
        float distCumul = 0;//locomotive.GetLength()/2;
        for (int i = 0, max = wagons.Count; i < max; i++)
        {
            wagons[i].SetTrain(this);

            float tmpDist = wagons[i].GetLength();
            distWagon.Add(distCumul + (tmpDist / 2.0f));
            distCumul += tmpDist;
        }

        this.speedLever = gameObject.GetComponentInChildren<LeverValue>();
    }


}
