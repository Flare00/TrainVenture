using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class TrainData : IDataSave
{
    // --- TRAIN ---
    public enum WagonType
    {
        LOCOMOTIVE_1,
        WAGON_1
    }
    [SerializeField]
    public float avancement;

    [SerializeField]
    public float maxSpeed;

    [SerializeField]
    public float speed;

    [SerializeField]
    public float throttle;

    [SerializeField]
    public bool direction;

    // --- WAGONS ---

    [SerializeField]
    public List<WagonType> wagons;
}
