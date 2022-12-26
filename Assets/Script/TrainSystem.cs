using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainSystem : MonoBehaviour
{
    public SplineContainer spline;
	public GameObject xr_rig;
	private Train train;
    void Start()
    {
        ShopData.GetInstance();
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

        this.train = TrainGenerator.GenerateTrain(data, spline);
		xr_rig.transform.SetParent(this.train.wagons[0].transform, false);
        /*  xr_rig.transform.position = this.train.wagons[0].transform.Find("XR_ANCHOR").position;
		    xr_rig.transform.rotation = this.train.wagons[0].transform.Find("XR_ANCHOR").rotation; */
		xr_rig.transform.SetPositionAndRotation(this.train.wagons[0].transform.Find("XR_ANCHOR").position, this.train.wagons[0].transform.Find("XR_ANCHOR").rotation);
        //SaveLoad.GetInstance().Save(data, "test.data");

        //TrainGenerator.GenerateTrain(SaveLoad.GetInstance().Load("test.data") as TrainData, spline);
    }


}
