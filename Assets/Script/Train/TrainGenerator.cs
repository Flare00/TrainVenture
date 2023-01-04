using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainGenerator
{
    public static Train GenerateTrain(TrainData data, GameObject terrain, TrainPath trainPath)
    {
        return GenerateTrain(terrain, trainPath, data.direction, data.avancement, data.maxSpeed, data.speed, data.throttle, data.wagons);
    }

    public static Train GenerateTrain(GameObject terrain, TrainPath trainPath, bool direction, float avancement, float maxSpeed, float speed, float throttle, List<TrainData.WagonType> types)
    {
        GameObject tGo = new("Train");

        Train train = tGo.AddComponent<Train>();

        train.SetData(terrain, trainPath,0, avancement, direction, maxSpeed, speed, throttle);
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
