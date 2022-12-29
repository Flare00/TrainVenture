using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.XR.Interaction.Toolkit;

public class TrainSystem : MonoBehaviour
{
    public Transition transition;

    public SplineContainer spline;
	public GameObject xr_rig;
	public GameObject terrain;
	private Train train;

    private bool showScreen = false;
    private bool hideScreen = false;
    private float fondu = 0.0f;

    void Awake()
    {
        ShopData.GetInstance(); //Create the Shop if does not exist (permit load of data).
        transition.SetValue(0.0f);
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

        train = TrainGenerator.GenerateTrain(data,terrain, spline);

        Transform interior = train.wagons[0].transform.Find("WagonInterior");
        if(interior != null)
        xr_rig.transform.SetParent(interior, false);
        xr_rig.transform.SetPositionAndRotation(interior.Find("XR_ANCHOR").position, interior.Find("XR_ANCHOR").rotation);

        if(train.wagons.Count > 0)
        {
            TeleportationArea[] tps = train.wagons[0].GetComponentsInChildren<TeleportationArea>();
            foreach (TeleportationArea tp in tps)
            {
                tp.teleportationProvider = xr_rig.GetComponentInChildren<TeleportationProvider>();
            }
        }

        transition.Show();
    }

}
