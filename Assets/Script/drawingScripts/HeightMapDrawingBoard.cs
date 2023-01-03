using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;


public class HeightMapDrawingBoard : MonoBehaviour
{
    public GameObject pencil;
    public float map_altitude;
    private Material pencilMaterial;
    private float colorIntensity=0.0f;

    public RenderTexture map;
    // Start is called before the first frame update
    void Start()
    {
        pencil = GameObject.Find("pencil");
        pencilMaterial = pencil.GetComponent<Renderer>().material;
        print(pencilMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        //x position on board is pencil.transform.position[2] and y is pencil.transform.position[1].

        Vector3 worldTransform = PencilPosition();
        float pencilX = -worldTransform[2];//minus to accomodate for 180 degrees rotation.
        float pencilY = worldTransform[1];

        if(pencilIsDrawing()){
            //...
        }
    }

    public void changeElevation(){
        print("change elevation");
        colorIntensity = (colorIntensity+0.1f)%1.0f;
        pencilMaterial.color = new Color(colorIntensity,colorIntensity,colorIntensity,1.0f);
        print(pencilMaterial.color);
    }
/*
        should do this to make the UI more convenient, but I can't find a out-of-box unity way of blurring a 
                    renderTexture and implementing it myself is overkill
    public void smoothMap(){
        //try to use graphics.Blit();
        RenderTexture rt1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4);
        RenderTexture rt2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4);
        Graphics.Blit(source, rt1, 0);

        //  otherwise : 
        //get tex as array
        //create a new one
        //copy the values
        //smooth them
        //overwrite former values.
    }*/
    Texture2D toTexture2D(RenderTexture rTex){
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public void save(){
        byte[] bytes = toTexture2D(map).EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/Resources/RunTimeImages/HeightMap.png", bytes);
    }

    private Vector3 PencilPosition(){//for inside use
        return (pencil.transform.position-new Vector3(0.0f,1.0f,0.0f))*5.0f;//have to multiply by 5 because plane has scale 0.2
    }
    public bool pencilIsDrawing(){
        Vector3 worldTransform = PencilPosition();
        return System.Math.Abs(worldTransform[0])<0.2 && System.Math.Abs(worldTransform[1])<5 && System.Math.Abs(worldTransform[2])<5;
    }
    public Vector3 LocalPencilPosition(){//for outside use
        return GameObject.Find("Manche").transform.localPosition;
    }
}
