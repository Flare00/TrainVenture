using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gare : ITrainPath, IDataSave
{
    [SerializeField]
    public string cityName;

    [SerializeField]
    public Vector3 position;

    [SerializeField]
    public List<Ligne> liaisons;

    public Gare(string n,Vector3 p,List<Ligne> l){
        cityName = n;
        position = p;
        liaisons = l;
    }

    List<ITrainPath> goToAnotherGare(Gare target){
        //...
        return null;
    }

}
