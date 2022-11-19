using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gare : MonoBehaviour, ITrainPath
{
    string cityName;
    Vector3Int position;
    List<Ligne> liaisons;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Init(string n,Vector3Int p,List<Ligne> l){
        name = n;
        position = p;
        liaisons = l;
    }

    List<ITrainPath> goToAnotherGare(Gare target){
        //...
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
