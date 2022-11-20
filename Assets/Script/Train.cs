using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class Train : MonoBehaviour
{
    [SerializeField]
    public SplineContainer chemin;

    [SerializeField]
    public List<Wagon> wagons;
    private List<float> distWagon;

    [SerializeField]
    public float avancement = 0.0f;

    [SerializeField]
    public bool direction = true;

    [SerializeField]
    public float speed = 0.0f; //in meter per seconds

    [SerializeField]
    public float maxSpeed = 5.0f; //in meter per second 1m/s = 3.6km/h

    [SerializeField]
    public float throttle = 0.1f; // si < 0 : Freine / recule, si = 0 : ne bouge pas / ralentit a cause du poid, si > 0 et suffisant : accélère (ralentit a cause du poid si insuffisant)

    //private Wagon locomotive;
    private float avancementByMeter = 0.0f;

    private void Start()
    {
        distWagon= new List<float>();
        //locomotive = GetComponent<Wagon>();
        avancementByMeter = 1.0f / this.chemin.CalculateLength();
        float distCumul = 0;//locomotive.GetLength()/2;
        for(int i = 0, max = wagons.Count; i < max; i++)
        {
            wagons[i].SetTrain(this);

            float tmpDist = wagons[i].GetLength();
            distWagon.Add(distCumul + (tmpDist / 2.0f));
            distCumul += tmpDist;
        }
    }

    private void Update()
    {
        float tmpSpeed = speed + (throttle * 0.1f);
        if(Math.Abs(tmpSpeed) <= maxSpeed)
        {
            speed = maxSpeed;
        }

        avancement += (speed  * avancementByMeter) * Time.deltaTime ;
        if(avancement >= 2.9f)
        {
            avancement %= 1.0f;
        }

        //locomotive.ComputePositionRotation(this.chemin, this.avancement, this.avancementByMeter);
        for(int i = 0, max = wagons.Count; i < max; i++)
        {
            wagons[i].ComputePositionRotation(this.chemin, this.avancement - (distWagon[i] * this.avancementByMeter), this.avancementByMeter);
        }
    }


}
