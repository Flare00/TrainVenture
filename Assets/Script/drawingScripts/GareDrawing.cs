using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class GareDrawing : MonoBehaviour
{
    public Material material;
    public Mesh gareMesh;
    //rest not implemented --

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(string text){
        GetComponent<TextChangeScript>().SetText(text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
