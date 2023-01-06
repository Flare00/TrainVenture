using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GarePlacement : MonoBehaviour
{
    /*public GameObject terrain;
    public SplineContainer spline;

    public List<float> garePosition;*/

    // Start is called before the first frame update
    void Start()
    {
        /*foreach (float pos in garePosition)
        {
            GameObject g = GameObject.Instantiate(Resources.Load("Prefabs/Gare/Gare") as GameObject);
            g.transform.SetParent(terrain.transform, false);
            Vector3 v = spline.EvaluatePosition(pos);
            Vector3 vDir = spline.EvaluatePosition(pos + 0.01f);


            Vector3 right = Vector3.Cross(Vector3.up, v - vDir);
            Vector3 up = Vector3.Cross(v - vDir, right);

            g.transform.SetPositionAndRotation(v, Quaternion.LookRotation(right, up));
        }*/

        List<Gare> gares = new();
        List<Ligne> lignes = new();

        Gare g1 = new Gare("Oubliette", new Vector3(0.1f, 0, 0.1f));
        Gare g2 = new Gare("Montpellier", new Vector3(0.5f, 0, 0.3f));
        Gare g3 = new Gare("Avignon", new Vector3(0.7f, 0, 0.7f));

        gares.Add(g1); gares.Add(g2); gares.Add(g3);

        Ligne l1 = new Ligne(new Lien(g1, 0.0f), new Lien(g2, 1.0f), new Spline() { new BezierKnot(g1.position), new BezierKnot(new Vector3(0.3f, 0, 0.1f)), new BezierKnot(g2.position) }); 
        Ligne l2 = new Ligne(new Lien(g2, 0.0f), new Lien(g3, 1.0f), new Spline() { new BezierKnot(g2.position), new BezierKnot(new Vector3(0.8f, 0, 0.5f)), new BezierKnot(g3.position) }); 
        Ligne l3 = new Ligne(new Lien(g3, 0.0f), new Lien(g1, 1.0f), new Spline() { new BezierKnot(g3.position), new BezierKnot(new Vector3(0.0f, 0, 0.1f)), new BezierKnot(g1.position) });

        l1.spline.EditType = SplineType.CatmullRom;
        l2.spline.EditType = SplineType.CatmullRom;
        l3.spline.EditType = SplineType.CatmullRom;

        lignes.Add(l1); lignes.Add(l2); lignes.Add(l3);

        g1.liaisons.Add(l3);
        g1.liaisons.Add(l1);

        g2.liaisons.Add(l1);
        g2.liaisons.Add(l2);

        g3.liaisons.Add(l2);
        g3.liaisons.Add(l3);

        DataTrainPath dtp = new(gares, lignes);
        SaveLoad.GetInstance().Save(dtp, "TrainPath/std.data");


    }

}
