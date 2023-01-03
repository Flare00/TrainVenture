using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lien : ITrainPath
{
    [SerializeField]
    public ITrainPath linkedPath;

    [SerializeField]
    public float linkPoint;


    public Lien(ITrainPath path, float point){
        linkPoint = point;
        linkedPath = path;
    }
}
