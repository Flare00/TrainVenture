using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainSystem : MonoBehaviour
{
    public SplineContainer spline;
	public GameObject xr_rig;
	private Train train;

    void Awake()
    {
        ShopData.GetInstance(); //Create the Shop if does not exist (permit load of data).
    }
    void Start()
    {
        List<TrainData.WagonType> wagonList = new()
        {
            TrainData.WagonType.LOCOMOTIVE_1,
            TrainData.WagonType.WAGON_1,
            TrainData.WagonType.WAGON_1,
            TrainData.WagonType.WAGON_1,
            TrainData.WagonType.WAGON_1
        };

        TrainData data = new()
        {
            wagons = wagonList,
            speed = 0,
            maxSpeed = 10,
            avancement = 0,
            throttle = 0
        };

        train = TrainGenerator.GenerateTrain(data, spline);

        Transform interior = train.wagons[0].transform.Find("WagonInterior");
        if(interior != null)
        xr_rig.transform.SetParent(interior, false);
        xr_rig.transform.SetPositionAndRotation(interior.Find("XR_ANCHOR").position, interior.Find("XR_ANCHOR").rotation);
    }

    

}
