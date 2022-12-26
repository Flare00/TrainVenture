using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainGenerator
{
    public static Train GenerateTrain(TrainData data, SplineContainer path = null)
    {
        return GenerateTrain(path, data.direction, data.avancement, data.maxSpeed, data.speed, data.throttle, data.wagons);
    }

    public static Train GenerateTrain(SplineContainer path, bool direction, float avancement, float maxSpeed, float speed, float throttle, List<TrainData.WagonType> types)
    {
        GameObject tGo = new GameObject("Train");

        Train train = tGo.AddComponent<Train>();

        train.SetData(path, avancement, direction, maxSpeed, speed, throttle);
        train.LoadUpgradeData();

        for (int i = 0; i < types.Count; i++)
        {
            Wagon wag = GenerateWagon(types[i]);
            if (wag != null)
            {
                train.wagons.Add(wag);
            }
        }
        train.UpdateData();
        return train;
    }

    public static Wagon GenerateWagon(TrainData.WagonType type)
    {
        GameObject wagonGO = null;
        switch (type)
        {
            case TrainData.WagonType.LOCOMOTIVE_1:
                wagonGO = GameObject.Instantiate(Resources.Load("Prefabs/Locomotive") as GameObject);
                break;
            case TrainData.WagonType.WAGON_1:
                wagonGO = GameObject.Instantiate(Resources.Load("Prefabs/Wagon_simple") as GameObject);
                break;
        }
        if (wagonGO != null)
        {
            Wagon wagon = wagonGO.AddComponent<Wagon>();
            wagon.GetValues();
            return wagon;
        }
        return null;
    }

}
