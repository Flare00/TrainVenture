using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;


public class DrawingBoard : MonoBehaviour
{
    public GameObject pencil;
    static double SAMPLE_THRESHOLD=0.25;
    private double cumulDist;
    private Vector2 lastPosition;
    private Spline spline;
    [SerializeField]
    public SplineContainer sContainer;
    // Start is called before the first frame update
    void Start()
    {
        cumulDist=0;
        pencil = GameObject.Find("pencil");
        lastPosition = new Vector2(0.0f,0.0f);
        lastPosition[0]=pencil.transform.position[2];
        lastPosition[1]=pencil.transform.position[1];
        spline = new Spline();
        sContainer.Spline=spline;
    }

    // Update is called once per frame
    void Update()
    {
        //x position on board is pencil.transform.position[2] and y is pencil.transform.position[1].

        Vector3 worldTransform = PencilPosition();
        float pencilX = -worldTransform[2];//minus to accomodate for 180 degrees rotation.
        float pencilY = worldTransform[1];

        if(pencilIsDrawing()){
            cumulDist += System.Math.Sqrt(
                            System.Math.Pow(pencilX-lastPosition[0],2)
                            +System.Math.Pow(pencilY-lastPosition[1],2)
                            );
        }

        if(cumulDist>SAMPLE_THRESHOLD){
            cumulDist=0.0f;
            //X goes to the knot's z and y goes to the knot's X. 
            spline.Add(new BezierKnot(new float3(pencilY,0.0f,pencilX)));
            //print("added knot with position "+spline[spline.Count-1].Position.ToString());
        }

        lastPosition[0]=pencilX;
        lastPosition[1]=pencilY;
    }

    public bool pencilIsDrawing(){
        Vector3 worldTransform = PencilPosition();
        return System.Math.Abs(worldTransform[0])<0.2;
    }
    public Vector3 PencilPosition(){
        return (pencil.transform.position-new Vector3(0.0f,1.0f,0.0f))*5.0f;//have to multiply by 5 because plane has scale 0.2
    }
    public Vector3 LocalPencilPosition(){
        return pencil.transform.localPosition;
    }
}
