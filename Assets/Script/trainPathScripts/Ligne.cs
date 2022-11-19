using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ligne : MonoBehaviour
{
    List<Vector3Int> points;
    Lien L1,L2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Init(Lien l1, Lien l2, List<Vector3Int> pts){
        points=pts;
        L1=l1;
        L2=l2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
