using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.XR.OpenXR.Input;
using static TrainPath;
using static Unity.Burst.Intrinsics.X86.Avx;

public class TrainPath : MonoBehaviour
{

    public struct LigneSplineContainer
    {
        public Ligne ligne;
        public SplineContainer splineContainer;

        public LigneSplineContainer(Ligne ligne, SplineContainer splineContainer)
        {
            this.ligne = ligne;
            this.splineContainer = splineContainer;
        }
    }

    public class PossibleLigne
    {
        public Ligne ligne;
        public float startAvancement;

        public PossibleLigne(Ligne ligne, float startAvancement)
        {
            this.ligne = ligne;
            this.startAvancement = startAvancement;
        }
    }

    public Terrain terrain;
    public GameObject lignesContainer;
    public GameObject garesContainer;

    private string customTrainPathName = "one";
    private string trainPathName = "std";

    private DataTrainPath dataTrainPath;
    private List<LigneSplineContainer> lignesSplineContainers = new();

    // Start is called before the first frame update
    public void Initialisation(float size, int nbSubdivision)
    {
        dataTrainPath = (DataTrainPath)SaveLoad.GetInstance().Load("TrainPath/" + (DataBetweenScene.custom ? customTrainPathName : trainPathName) + ".data");

        float decalage = size;

        size /= 5.0f;

        int increment = 0;
        foreach (Ligne l in dataTrainPath.lignes)
        {
            GameObject go = new GameObject("" + increment);
            go.transform.SetParent(lignesContainer.transform, false);

            SplineContainer sc = go.AddComponent<SplineContainer>();

            for (int i = 0; i < l.spline.Count; i++)
            {
                BezierKnot b = l.spline[i];

                b.Position[0] = (b.Position[0] * size) + decalage;
                b.Position[2] = (b.Position[2] * -size) + decalage;

                l.spline[i] = b;
            }
            sc.Spline = l.spline;
            lignesSplineContainers.Add(new LigneSplineContainer(l, sc));

            l.spline = SplineSubdivide.Subdivide(sc, nbSubdivision);


            for (int i = 0; i < l.spline.Count; i++)
            {
                BezierKnot b = l.spline[i];
                float height = terrain.SampleHeight(new Vector3(b.Position[0], 0, b.Position[2]));
                b.Position[1] = height;
                l.spline[i] = b;
            }

            sc.Spline = l.spline;
            increment++;
        }

        foreach (Gare g in dataTrainPath.gares)
        {
            GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Gare/Gare") as GameObject);
            go.name = g.cityName;
            go.transform.SetParent(garesContainer.transform, false);

            TextMeshPro[] tmps = go.GetComponentsInChildren<TextMeshPro>();

            for(int i = 0; i < tmps.Length; i++)
            {
                tmps[i].SetText(g.cityName);
            }

            g.position = (g.position * size) + new Vector3(decalage, 0, decalage);
            g.position[1] = terrain.SampleHeight(new Vector3(g.position[0], 0, g.position[2]));
            go.transform.localPosition = g.position;

            bool found = false;
            bool dir = false;
            Vector3 pos = go.transform.localPosition + Vector3.right;
            for (int i = 0; i < g.liaisons.Count && !found; i++)
            {
                if ((g.liaisons[i].l1.linkedPath == g && g.liaisons[i].l1.linkPoint <= 0.1f) || (g.liaisons[i].l2.linkedPath == g && g.liaisons[i].l2.linkPoint <= 0.1f))
                {
                    pos = FindSplineContainerByLigne(g.liaisons[i]).EvaluatePosition(0.0001f);
                    found = true;
                    dir = true;
                }
                else if ((g.liaisons[i].l1.linkedPath == g && g.liaisons[i].l1.linkPoint >= 0.9f) || (g.liaisons[i].l2.linkedPath == g && g.liaisons[i].l2.linkPoint >= 0.9f))
                {
                    pos = FindSplineContainerByLigne(g.liaisons[i]).EvaluatePosition(0.9999f);
                    found = true;
                    dir = false;
                }
            }
            if (found)
            {
                go.transform.localRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, dir ? go.transform.localPosition - pos : pos - go.transform.localPosition), Vector3.up);
            }
        }



    }

    public PossibleLigne IsChangementPossible(Ligne l, float avancement, float avancementByMeter, bool direction)
    {
        PossibleLigne res = null;

        List<PossibleLigne> lignes = new List<PossibleLigne>();
        ITrainPath iTrainPath = null;

        if (!direction && avancement > 1.0 - avancementByMeter)
        {
            iTrainPath = l.l2.linkedPath;
        }
        else if (direction && avancement < avancementByMeter)
        {
            iTrainPath = l.l1.linkedPath;
        }

        if (iTrainPath != null)
        {

            if (iTrainPath.GetType() == typeof(Gare))
            {
                Gare tmp = (Gare)iTrainPath;
                for (int i = 0; i < tmp.liaisons.Count; i++)
                {
                    if (tmp.liaisons[i].GetHashCode() != l.GetHashCode())
                    {
                        lignes.Add(new PossibleLigne(tmp.liaisons[i], tmp.liaisons[i].l1.linkedPath.GetHashCode() == iTrainPath.GetHashCode() ? 0.0f : 1.0f));

                    }
                }
            }
            else
            {
                Ligne tmpL = (Ligne)iTrainPath;
                lignes.Add(new PossibleLigne(tmpL, tmpL.l1.GetHashCode() == l.GetHashCode() ? 0.0f : 1.0f));
            }
        }


        if (lignes.Count == 0)
        {
            foreach (Ligne ligne in dataTrainPath.lignes)
            {
                Lien self = null;
                float startpoint = 0.0f;
                if (ligne.l1.linkedPath.GetHashCode() == l.GetHashCode())
                {
                    self = ligne.l1;
                    startpoint = 0.0f;
                }
                else if (ligne.l2.linkedPath.GetHashCode() == l.GetHashCode())
                {
                    self = ligne.l2;

                    startpoint = 1.0f;
                }

                if (self != null)
                {
                    if (self.linkPoint >= avancement - avancementByMeter && self.linkPoint <= avancement + avancementByMeter)
                    {
                        lignes.Add(new PossibleLigne(ligne, startpoint));
                    }
                }
            }

        }


        if (lignes.Count > 0)
        {
            res = lignes[0];
        }

        return res;
    }

    public List<Gare> GetGares()
    {
        return dataTrainPath.gares;
    }

    public List<Ligne> GetLignes()
    {
        return dataTrainPath.lignes;
    }

    public SplineContainer FindSplineContainerByLigne(Ligne l)
    {
        SplineContainer res = null;

        for (int i = 0; i < this.lignesSplineContainers.Count && res == null; i++)
        {
            if (this.lignesSplineContainers[i].ligne.GetHashCode() == l.GetHashCode())
            {
                res = this.lignesSplineContainers[i].splineContainer;
            }
        }

        return res;
    }


}
