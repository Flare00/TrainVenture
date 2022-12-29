using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public class Train : MonoBehaviour
{
    private static readonly float[] MAX_SPEED_VALUES = { 14.0f, 20.0f, 30.0f, 41.0f, 55.0f, 72.0f }; //environ { 50, 75, 110, 150 , 200, 260 } kmh
    private static readonly float[] ACCELERATION_VALUES = { 0.1f, 0.15f, 0.25f, 0.4f, 0.6f, 0.9f };
    private static readonly float[] BRAKE_VALUES = { 0.3f, 0.6f, 1.0f, 1.5f, 2.2f, 3.0f };

    public GameObject terrain;
    public SplineContainer chemin;

    public LeverValue speedLever;
    private Furnace four;

    public List<Wagon> wagons;
    private List<float> distWagon;

    public float avancement = 0.0f;
    public bool direction = true;
    public float speed = 0.0f; //in meter per seconds
    public float maxSpeed = 5.0f; //in meter per second 1m/s = 3.6km/h
    public float throttle = 0.0f; // si < 0 : Freine / recule, si = 0 : ne bouge pas / ralentit a cause du poid, si > 0 et suffisant : accélère (ralentit a cause du poid si insuffisant)

    public float acceleratorMultiplicator = 0.1f;
    public float brakeMultiplicator = 0.3f;
    private float avancementByMeter = 0.0f;

    public Compteur compteurVitesse = null;
    public Compteur compteurTemperature = null;
    public Compteur compteurPression = null;
    public Train()
    {
        wagons = new List<Wagon>();

    }

    private void Start()
    {
        ShopData.GetInstance().SubscribeUpgradeChange(LoadUpgradeData);
        UpdateData();
    }

    /*private void FixedUpdate()
    {
        foreach (Wagon wagon in wagons)
        {
            wagon.SetToInitial();
        }
    }*/


    private void Update()
    {
        if (speedLever != null)
        {
            this.throttle = speedLever.valeur;
        }
        float multiplicator = this.throttle >= 0 ? acceleratorMultiplicator : brakeMultiplicator;
        float tmpSpeed = speed + (throttle * multiplicator * (four.pression / 10.0f));
        if (tmpSpeed < maxSpeed)
        {
            speed = tmpSpeed;
        }
        else
        {
            speed = maxSpeed;
        }
        if (speed < 0)
        {
            speed = 0;
        }

        avancement += (speed * avancementByMeter) * Time.deltaTime;
        if (avancement >= 1.0f)
        {
            avancement %= 1.0f;
        }

        //locomotive.ComputePositionRotation(this.chemin, this.avancement, this.avancementByMeter);
        if (wagons.Count > 0)
        {

            for (int i = 0, max = wagons.Count; i < max; i++)
            {
                Wagon.Result r = wagons[i].ComputePositionRotation(this.chemin, this.avancement - (distWagon[i] * this.avancementByMeter), this.avancementByMeter);
                if (i > 0)
                {
                    wagons[i].transform.SetPositionAndRotation(r.position, r.rotation);
                }
                else
                {
                    terrain.transform.Translate(-r.position);
                    wagons[i].transform.rotation = r.rotation;
                }
            }
        }

        if (compteurVitesse != null)
        {
            compteurVitesse.SetValue(speed * 3.6f);
        }
        if (this.compteurPression != null)
        {
            this.compteurPression.SetValue(four.pression);
        }
        if (this.compteurTemperature != null)
        {
            this.compteurTemperature.SetValue(four.temperature);
        }
    }

    public void SetData(GameObject terrain, SplineContainer chemin, float avancement = 0.0f, bool direction = true, float maxSpeed = 5.0f, float speed = 0.0f, float throttle = 0.0f)
    {
        this.terrain = terrain;
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

        if (wagons.Count > 0)
        {
            Transform wagonInterior = wagons[0].transform.Find("WagonInterior");

            this.speedLever = wagonInterior.Find("SpeedLever").GetComponentInChildren<LeverValue>();
            this.compteurVitesse = wagonInterior.Find("Compteurs").Find("Vitesse").GetComponent<Compteur>();

            this.compteurPression = wagonInterior.Find("Compteurs").Find("Pression").GetComponent<Compteur>();
            this.compteurTemperature = wagonInterior.Find("Compteurs").Find("Temp").GetComponent<Compteur>();
            if (this.compteurVitesse != null)
            {
                this.compteurVitesse.SetLimit(this.maxSpeed * 3.6f);
            }


            if (this.compteurPression != null)
            {
                this.compteurPression.SetLimit(16.0f);
            }

            if (this.compteurTemperature != null)
            {
                this.compteurTemperature.SetLimit(100);
            }

            this.four = wagons[0].transform.Find("Four").GetComponent<Furnace>();

            LocomotiveInteraction li = wagons[0].GetComponent<LocomotiveInteraction>();
            if(li != null)
            {
                li.train = this;
            }


        }
    }

    public void LoadUpgradeData()
    {

        int tmpval = ShopData.GetInstance().upgrade.maxSpeedSelected - 1;
        int max = MAX_SPEED_VALUES.Length;
        this.maxSpeed = MAX_SPEED_VALUES[tmpval < 0 ? 0 : tmpval >= max ? max - 1 : tmpval];
        if (compteurVitesse != null)
        {
            compteurVitesse.SetLimit(this.maxSpeed * 3.6f);
        }

        tmpval = ShopData.GetInstance().upgrade.accelerationSelected - 1;
        max = ACCELERATION_VALUES.Length;
        this.acceleratorMultiplicator = ACCELERATION_VALUES[tmpval < 0 ? 0 : tmpval >= max ? max - 1 : tmpval];

        tmpval = ShopData.GetInstance().upgrade.brakeSelected - 1;
        max = BRAKE_VALUES.Length;
        this.brakeMultiplicator = BRAKE_VALUES[tmpval < 0 ? 0 : tmpval >= max ? max - 1 : tmpval];
    }





}
