using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingBoard : MonoBehaviour
{
    public GameObject pencil;
    // Start is called before the first frame update
    void Start()
    {
        pencil = GameObject.Find("pencil");
    }

    // Update is called once per frame
    void Update()
    {
        print("x : "+pencil.transform.position[2].ToString());

        print("y : "+pencil.transform.position[1].ToString());
    }
}
