using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lien : MonoBehaviour, ITrainPath
{
    ITrainPath linkedPath;
    Vector3Int linkPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Init(ITrainPath path, Vector3Int pos){
        linkPoint = pos;
        linkedPath = path;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
