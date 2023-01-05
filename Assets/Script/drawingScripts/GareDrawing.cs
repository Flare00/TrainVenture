using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class GareDrawing : MonoBehaviour
{
    public GameObject gareMesh;
    public string strText;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void init(Transform parent, Vector3 garePos, string text,Vector3 railNormal){
        strText=text;
        GetComponent<TextChangeScript>().SetText(text);

        transform.SetParent(parent);
        transform.localPosition=new Vector3(garePos[0],0.05f,garePos[2]);
        transform.Rotate(new Vector3(0,-90,0));
        transform.Rotate(new Vector3(90,0,0));
        transform.localScale*=0.03f;

        addGareMesh(parent);

        //PlaceNextTo(railNormal);
    }
    private void addGareMesh(Transform parent){
        gareMesh = (GameObject)(Instantiate(Resources.Load("Prefabs/text/GareMesh")));
        gareMesh.transform.SetParent(parent);
        gareMesh.transform.localPosition = new Vector3(transform.localPosition[0],
                                                        transform.localPosition[1]-0.03f,
                                                        transform.localPosition[2]);
        gareMesh.transform.localScale = transform.localScale*0.2f;
        gareMesh.transform.localRotation = transform.localRotation;
    }

    private void PlaceNextTo(Vector3 railNormal){//no longer in use as not the splines are disconnected.
        gareMesh.transform.localPosition += railNormal*5.0f;
        transform.localPosition+=railNormal*5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
