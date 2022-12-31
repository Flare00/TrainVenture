using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class splineMesh : MonoBehaviour
{
    public Material mat;
    public int RESOLUTION_MAX=500;//number of "steps" in the mesh
    private int RESOLUTION;
    [SerializeField]
    public float WIDTH=0.05f;//width of path drawn

    Vector3[] newVertices;
    Vector2[] newUVs;
    int[] newTriangles;
    int formerSplineLength=0;

    Vector3 up = new Vector3(0.0f,1.0f,0.0f);

    public SplineContainer spline;

    // Start is called before the first frame update
    void Start()
    {
        print("start");
        newVertices = new Vector3[RESOLUTION_MAX*2];
        newUVs = new Vector2[RESOLUTION_MAX*2];
        newTriangles = new int[(RESOLUTION_MAX-1)*6];
        GetComponent<MeshRenderer>().GetComponent<Renderer>().material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        int newSplineLength = spline.Spline.Count;
        RESOLUTION = newSplineLength*5;
        if(RESOLUTION>RESOLUTION_MAX){RESOLUTION=RESOLUTION_MAX;}
        if (newSplineLength>1 && newSplineLength!=formerSplineLength){
            formerSplineLength=newSplineLength;
            fitMeshToCurve();
        }
    }


    void fitMeshToCurve(){
        //get first position and normal on spline
        Vector3 currentPoint = spline.Spline.EvaluatePosition(0);
        Vector3 nextPoint = spline.Spline.EvaluatePosition(1.0f/(float)RESOLUTION);
        Vector3 direction = currentPoint-nextPoint;
        Vector3 normal = Vector3.Cross(up,direction);
        if(normal.magnitude>0.0f){
            normal = normal * (WIDTH/normal.magnitude);
        }
        //get points to the two sides of the position
        Vector3 currentRight = currentPoint+normal;
        Vector3 currentLeft = currentPoint-normal;
        newVertices[0]=currentRight;
        newVertices[1]=currentLeft;
        newUVs[0]=new Vector2(0,1);
        newUVs[1]=new Vector2(0,0);

        for(int i=1;i<RESOLUTION;i++){
            float splineProgress = (float)(i+1)/(float)RESOLUTION;
            //get next position on spline
            Vector3 nextNextPoint = spline.Spline.EvaluatePosition(splineProgress);
            direction = nextNextPoint - nextPoint;
            //get normal to spline there
            normal = Vector3.Cross(up,direction).normalized*WIDTH;
            if(normal.magnitude>0.0f){
                normal = normal * (WIDTH/normal.magnitude);
            }
            //print("----norm of normal : "+normal.magnitude);
            //get the two sides there
            Vector3 nextRight = nextPoint+normal;
            Vector3 nextLeft = nextPoint-normal;
            //add the new two positions to newVertices
            newVertices[i*2]=nextRight;
            newVertices[i*2+1]=nextLeft;
            newUVs[i*2]=new Vector2(splineProgress*spline.Spline.Count/5,1);
            newUVs[i*2+1]=new Vector2(splineProgress*spline.Spline.Count/5,0);
            print("uvs at positions"+(i*2).ToString()+", "+(i*2+1).ToString()+" : "+newUVs[i*2].ToString()+newUVs[i*2+1].ToString());

            //add the two triangles to newTriangles          
            newTriangles[(i-1)*6+2]=i*2;//nextRight
            newTriangles[(i-1)*6+0]=(i-1)*2+1;//currentLeft 
            newTriangles[(i-1)*6+1]=(i-1)*2;//currentRight 

            newTriangles[(i-1)*6+5]=i*2+1;//nextLeft
            newTriangles[(i-1)*6+3]=(i-1)*2+1;//currentLeft
            newTriangles[(i-1)*6+4]=i*2;//nextRight

            //set currentPosition to NextPosition etc
            currentPoint = nextPoint;
            currentRight = nextRight;
            currentLeft = nextLeft;
            nextPoint = nextNextPoint;
        }
        GetComponent<MeshFilter>().mesh.Clear();
        //add the new data to the mesh
        GetComponent<MeshFilter>().mesh.vertices = newVertices;
        GetComponent<MeshFilter>().mesh.triangles = newTriangles;
        GetComponent<MeshFilter>().mesh.uv = newUVs;
        GetComponent<MeshFilter>().mesh.Optimize ();
        GetComponent<MeshFilter>().mesh.RecalculateNormals ();
    }
}