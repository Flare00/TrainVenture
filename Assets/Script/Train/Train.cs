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

    private class LigneExtra
    {
        public Ligne ligne;
        public float avancementByMeter;
        bool direction;

        public LigneExtra(Ligne ligne, float length, bool direction)
        {
            this.ligne = ligne;
            this.avancementByMeter = 1.0f / length;
            this.direction = direction;
        }
    }
    private static readonly float[] MAX_SPEED_VALUES = { 14.0f, 20.0f, 30.0f, 41.0f, 55.0f, 72.0f }; //environ { 50, 75, 110, 150 , 200, 260 } kmh
    private static readonly float[] ACCELERATION_VALUES = { 0.1f, 0.15f, 0.25f, 0.4f, 0.6f, 0.9f };
    private static readonly float[] BRAKE_VALUES = { 0.3f, 0.6f, 1.0f, 1.5f, 2.2f, 3.0f };

    public GameObject terrain;

    public LeverValue speedLever;
    private Furnace four;
    private WindZone wind;


    public List<Wagon> wagons;
    private List<float> distWagon;

    public float avancement;
    public bool direction = true;
    public float speed = 0.0f; //in meter per seconds
    public float maxSpeed = 5.0f; //in meter per second 1m/s = 3.6km/h
    public float throttle = 0.0f; // si < 0 : Freine / recule, si = 0 : ne bouge pas / ralentit a cause du poid, si > 0 et suffisant : accélère (ralentit a cause du poid si insuffisant)

    public float acceleratorMultiplicator = 0.1f;
    public float brakeMultiplicator = 0.3f;

    public Compteur compteurVitesse = null;
    public Compteur compteurTemperature = null;
    public Compteur compteurPression = null;

    // -- TRAIN PATH
    public TrainPath trainPath;

    private LigneExtra currentLine;
    private List<LigneExtra> pastLines = new();

    public Train()
    {
        wagons = new List<Wagon>();

    }

    private void Start()
    {
        ShopData.GetInstance().SubscribeUpgradeChange(LoadUpgradeData);
        UpdateData();

            avancement = currentLine.avancementByMeter * distWagon[0] * 4.0f;

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
        Avancement();
        TrainPath.PossibleLigne possibility = this.trainPath.IsChangementPossible(currentLine.ligne, this.avancement, currentLine.avancementByMeter, direction);
        if (possibility != null)
        {

            // Test if we want to change line
            if ((!direction && this.avancement >= 1.0f - currentLine.avancementByMeter) || (direction && this.avancement <= currentLine.avancementByMeter))
            {
                pastLines.Insert(0, currentLine);
                currentLine = new LigneExtra(possibility.ligne, trainPath.FindSplineContainerByLigne(possibility.ligne).CalculateLength(), possibility.startAvancement > 0.5 ? false : true);
                avancement = possibility.startAvancement;
            }
        }

        UpdateWagons();
        UpdateCompteurs();
    }

    private void Avancement()
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

        avancement += (speed * currentLine.avancementByMeter) * Time.deltaTime;
    }

    private void UpdateWagons()
    {
        if (wagons.Count > 0)
        {
            float avancement = this.avancement;

            for (int i = 0; i < wagons.Count; i++)
            {
                LigneExtra l = currentLine;
                LigneExtra lar = currentLine;
                int pastCursor = -1;
                int pastCursorAR = -1;

                avancement -= ((distWagon[i] * 2.0f) * l.avancementByMeter);

                float avancementAV = avancement + (wagons[i].GetDistEssieuAvant() * l.avancementByMeter);

                while (avancementAV < 0.0)
                {
                    pastCursor++;
                    if (pastCursor < pastLines.Count)
                    {
                        float oldAvancement = l.avancementByMeter;
                        l = pastLines[pastCursor];
                        avancementAV = 1.0f - ((Math.Abs(avancementAV) / oldAvancement) * l.avancementByMeter);
                    }
                    else
                    {
                        avancementAV = 1.0f;
                    }
                }

                float avancementAR = avancement + (wagons[i].GetDistEssieuArriere() * l.avancementByMeter);

                while (avancementAR < 0.0)
                {
                    pastCursorAR++;
                    if (pastCursorAR < pastLines.Count)
                    {
                        float oldAvancement = lar.avancementByMeter;
                        lar = pastLines[pastCursorAR];
                        avancementAR = 1.0f - ((Math.Abs(avancementAR) / oldAvancement) * lar.avancementByMeter);
                    }
                    else
                    {
                        avancementAR = 1.0f;
                    }
                }

                if (pastCursor < pastLines.Count)
                {
                    //Wagon.Result r = wagons[i].ComputePositionRotation(this.trainPath.FindSplineContainerByLigne(l.ligne), avancement, l.avancementByMeter);

                    Vector3 posAV = this.trainPath.FindSplineContainerByLigne(l.ligne).EvaluatePosition(avancementAV);
                    Vector3 posAR = this.trainPath.FindSplineContainerByLigne(lar.ligne).EvaluatePosition(avancementAR);

                    Vector3 right = Vector3.Cross(Vector3.up, posAR - posAV);
                    Vector3 up = Vector3.Cross(posAR - posAV, right);

                    Vector3 pos = (posAV + posAR) * 0.5f;
                    Quaternion rotation = Quaternion.LookRotation(right, up);

                    if (i > 0)
                    {
                        wagons[i].transform.SetPositionAndRotation(pos, rotation);
                    }
                    else
                    {
                        terrain.transform.Translate(-pos);
                        //Debug.Log(rotation.eulerAngles);
                        
                        if (wagons[i].transform.rotation.eulerAngles != rotation.eulerAngles)
                        {
                            wagons[i].transform.rotation = rotation;
                        }
                    }
                }
            }
        }
    }

    private void UpdateCompteurs()
    {
        if (compteurVitesse != null)
        {
            compteurVitesse.SetValue(speed * 3.6f);
        }
        if (wind != null)
        {
            wind.windMain = speed / 5.0f;
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
    public void SetData(GameObject terrain, TrainPath trainPath, int ligne, float avancement = 0.0f, bool direction = true, float maxSpeed = 5.0f, float speed = 0.0f, float throttle = 0.0f)
    {
        this.avancement = avancement;
        this.terrain = terrain;
        this.trainPath = trainPath;
        this.direction = direction;
        this.maxSpeed = maxSpeed;
        this.speed = speed;
        this.throttle = throttle;

        Ligne l = trainPath.GetLignes()[ligne];
        this.currentLine = new LigneExtra(l, this.trainPath.FindSplineContainerByLigne(l).CalculateLength(), direction);
        TrainPath.PossibleLigne possibility = this.trainPath.IsChangementPossible(currentLine.ligne, this.avancement, currentLine.avancementByMeter, !direction);
        if (possibility != null)
        {
            this.pastLines.Add(new LigneExtra(possibility.ligne, this.trainPath.FindSplineContainerByLigne(possibility.ligne).CalculateLength(), possibility.startAvancement > 0.5 ? false : true));
        }
    }

    public void UpdateData()
    {
        distWagon = new List<float>();
        //locomotive = GetComponent<Wagon>();


        for (int i = 0, max = wagons.Count; i < max; i++)
        {
            wagons[i].SetTrain(this);
            float tmpDist = wagons[i].GetLength();
            distWagon.Add((tmpDist / 2.0f));
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
            this.wind = wagons[0].transform.GetComponentInChildren<WindZone>();

            LocomotiveInteraction li = wagons[0].GetComponent<LocomotiveInteraction>();
            if (li != null)
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
