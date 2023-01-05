using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CleanPath
{

    List<Spline> previousSplines;
    Spline spline;
    Vector3 lastDirection;
    Vector3 lastNormal;
    float totalAngle=0.0f;
    Vector3 up = new Vector3(0.0f,1.0f,0.0f);
    Vector3 startPos;
    Vector3 endPos;

    List<Vector3> path=new List<Vector3>();
    List<float> costs=new List<float>();

    private int RESOLUTION = 500;//steps per path

    public CleanPath(Spline spl, List<Spline> prevSpl){
        spline = spl;
        previousSplines = prevSpl;
        startPos=spl.EvaluatePosition(0);
        endPos=spl.EvaluatePosition(1);
    }

    List<Vector3> generateStepOptions(Vector3 startPoint){
        List<float> coeffs = getOptionsCoeff();
        List<Vector3> endpoints = new List<Vector3>();
        for(int i=0;i<coeffs.Count;i++){
            endpoints.Add(startPoint + lastDirection + lastNormal*coeffs[i]);
        }
        return endpoints;
    }

    Vector3 pickNextStep(Vector3 startPoint){
        int chosenIndex;
        List<Vector3> options = generateStepOptions(startPoint);
        double[] distances = new double[options.Count];
        for(int i=0;i<options.Count;i++){
            distances[i]=distanceToSpline(options[i]);
        }
        int[] args = new int[options.Count];
        for(int i=0;i<options.Count;i++){args[i]=i;}
        System.Array.Sort(distances, args);//argsort to get corresponding positions in options.

        int pick = Random.Range(0,100);//will serve to randomly pick an option with probas.
        if(pick<39){chosenIndex= args[0];}
        else if(pick<39+18){chosenIndex= args[1];}
        else if(pick<39+18+14){chosenIndex= args[2];}
        else if(pick<39+18+14+10){chosenIndex= args[3];}
        else if(pick<39+18+14+10+5){chosenIndex= args[4];}   //ugly but the purpose is clear.
        else if(pick<39+18+14+10+5+5){chosenIndex= args[5];}
        else if(pick<39+18+14+10+5+5+3){chosenIndex= args[6];}
        else if(pick<39+18+14+10+5+5+3+3){chosenIndex= args[7];}
        else{chosenIndex= args[8];}

        totalAngle += getOptionsCoeff()[args[8]];//accumulate angle, to check for loops.
        updateDirectionAndNormal(startPoint,options[chosenIndex]);
        return options[chosenIndex];
    }

    bool hasLoop(){
        if(System.Math.Abs(totalAngle)>270){
            Debug.Log("looking for loop !");
            for(int i=0;i<path.Count-1;i++){
                Vector3 path_start = path[i];
                Vector3 path_end = path[i+1];
                for(int s=0;s<previousSplines.Count;s++){
                    Spline currentSpline = previousSplines[s];
                    for(int k=0;k<currentSpline.Count-1;k++){
                        Vector3 spline_start = currentSpline[i].Position;
                        Vector3 spline_end = currentSpline[i+1].Position;
                        if(hasIntersection(path_start,path_end, spline_start,spline_end)){
                            Debug.Log("found loop !");
                            return true;
                        }
                    }
                }
            }
        }return false;
    }



    void updateDirectionAndNormal(Vector3 lastPos, Vector3 newPos){
        lastDirection = newPos-lastPos;
        lastNormal = Vector3.Cross(up,lastDirection);
    }

    double distanceToSpline(Vector3 point){
        float min_dist = 1000000.0f;
        if(spline.Count==0){
            Debug.Log("called distanceToSpline on an empty spline (in CleanPath.cs)");
            return min_dist;}
        for(int i=1;i<RESOLUTION;i++){
            Vector3 sp_point = spline.EvaluatePosition((float)(i)/(float)(RESOLUTION));
            float dist = (point-sp_point).magnitude;
            if(dist<min_dist){
                min_dist = dist;
            }
        }
        return min_dist;
    }

    List<float> getOptionsCoeff(){
        List<float> ret = new List<float>();
        ret.Add(-0.5f);ret.Add(-0.375f);ret.Add(-0.25f);
        ret.Add(-0.125f);ret.Add(0.0f);ret.Add(0.125f);
        ret.Add(0.25f);ret.Add(0.375f);ret.Add(0.5f);
        return ret;
    }

    bool hasIntersection(Vector3 path_start,Vector3 path_end, Vector3 spline_start,Vector3 spline_end){//just on x and z coords
        Vector3 seg1 = path_end-path_start;
        Vector3 seg2 = spline_end-spline_start;
        Vector3 seg3 = spline_start-spline_start;
        return ( determinant(seg1,seg2)*determinant(seg1,seg3) )<0;
    }

    float determinant(Vector3 seg1, Vector3 seg2){//just on x and z coords
        return seg1[0]*seg2[1]-seg1[1]*seg2[0];
    }

}
