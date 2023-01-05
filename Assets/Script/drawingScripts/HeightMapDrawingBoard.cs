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
    private float scale_factor=0.0f;
    private Vector3 origScale;

    public RenderTexture map;
    // Start is called before the first frame update
    void Start()
    {
        pencilMaterial = pencil.GetComponent<Renderer>().material;
        origScale = pencil.transform.localScale;
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

    public void changePencilSize(){
        scale_factor = (scale_factor+0.3f)%2.0f;
        pencil.transform.localScale = origScale*scale_factor;
        Debug.Log("pencil scale : "+pencil.transform.localScale.ToString());
    }


    Texture2D toTexture2D(RenderTexture rTex){
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public void save(){
        Texture2D tex =Blur(Resize(toTexture2D(map),100,100),5);
        Graphics.Blit(tex,map);
        byte[] bytes = toTexture2D(map).EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/Resources/RunTimeImages/HeightMap.png", bytes);
    }

    private Vector3 PencilPosition(){//for inside use
        return (pencil.transform.position-new Vector3(-0.5f,1.0f,0.0f))*(1.0f/0.1f);//have to multiply because plane is downscaled
    }
    public bool pencilIsDrawing(){
        Vector3 worldTransform = PencilPosition();
        return System.Math.Abs(worldTransform[0])<0.2 && System.Math.Abs(worldTransform[1])<5 && System.Math.Abs(worldTransform[2])<5;
    }
    public Vector3 LocalPencilPosition(){//for outside use
        return GameObject.Find("Manche").transform.localPosition;
    }


    private Texture2D Blur(Texture2D image, int blurSize){//credits for this function : 
                        //https://forum.unity.com/threads/contribution-texture2d-blur-in-c.185694/
        Texture2D blurred = new Texture2D(image.width, image.height);
     
        // look at every pixel in the blur rectangle
        for (int xx = 0; xx < image.width; xx++)
        {
            for (int yy = 0; yy < image.height; yy++)
            {
                float avgR = 0, avgG = 0, avgB = 0, avgA = 0;
                int blurPixelCount = 0;
     
                // average the color of the red, green and blue for each pixel in the
                // blur size while making sure you don't go outside the image bounds
                for (int x = xx; (x < xx + blurSize && x < image.width); x++)
                {
                    for (int y = yy; (y < yy + blurSize && y < image.height); y++)
                    {
                        Color pixel = image.GetPixel(x, y);
     
                        avgR += pixel.r;
                        avgG += pixel.g;
                        avgB += pixel.b;
                        avgA += pixel.a;
     
                        blurPixelCount++;
                    }
                }
     
                avgR = avgR / blurPixelCount;
                avgG = avgG / blurPixelCount;
                avgB = avgB / blurPixelCount;
                avgA = avgA / blurPixelCount;
     
                // now that we know the average for the blur size, set each pixel to that color
                for (int x = xx; x < xx + blurSize && x < image.width; x++)
                    for (int y = yy; y < yy + blurSize && y < image.height; y++)
                        blurred.SetPixel(x, y, new Color(avgR, avgG, avgB, avgA));
            }
        }
        blurred.Apply();
        return blurred;
    }
    Texture2D Resize(Texture2D texture2D,int targetX,int targetY){//credits : 
                     //https://stackoverflow.com/questions/56949217/how-to-resize-a-texture2d-using-height-and-width
        RenderTexture rt=new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        Texture2D result=new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }
}
