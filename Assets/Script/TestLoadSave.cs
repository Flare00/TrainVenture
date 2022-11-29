using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TestLoadSave : MonoBehaviour
{
    public SplineContainer spline;
    void Start()
    {
        /*List<TrainData.WagonType> wagonList = new List<TrainData.WagonType>();

        wagonList.Add(TrainData.WagonType.LOCOMOTIVE_1);
        wagonList.Add(TrainData.WagonType.WAGON_1);
        wagonList.Add(TrainData.WagonType.WAGON_1);
        wagonList.Add(TrainData.WagonType.WAGON_1);
        wagonList.Add(TrainData.WagonType.WAGON_1);

        TrainData data = new TrainData();
        data.wagons = wagonList;
        data.speed = 0;
        data.maxSpeed = 50;
        data.avancement = 0;
        data.throttle = 0;

        //TrainGenerator.GenerateTrain(spline, true, 0, 50, 0, 0, wagonList);


        SaveLoad.GetInstance().Save(data, "test.data");*/

        TrainGenerator.GenerateTrain(SaveLoad.GetInstance().Load("test.data") as TrainData, spline);
    }


}
