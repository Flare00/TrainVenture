using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
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
    public class PossibleLigne{
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

    public string trainPathName = "one";
    
    private DataTrainPath dataTrainPath;
    private List<LigneSplineContainer> lignesSplineContainers = new();

    // Start is called before the first frame update
    public void Initialisation(float size, int nbSubdivision)
    {
        dataTrainPath = (DataTrainPath)SaveLoad.GetInstance().Load("TrainPath/" + trainPathName + ".data");

        foreach (Gare g in dataTrainPath.gares)
        {
            GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Gare/Gare") as GameObject);
            go.name = g.cityName;
            go.transform.SetParent(garesContainer.transform, false);
            go.transform.localPosition = g.position * size;


        }

        int increment = 0;
        foreach (Ligne l in dataTrainPath.lignes)
        {
            GameObject go = new GameObject("" + increment);
            go.transform.SetParent(lignesContainer.transform, false);
            
            SplineContainer sc = go.AddComponent<SplineContainer>();
            for (int i = 0; i < l.spline.Count; i++)
            {
                BezierKnot b = l.spline[i];
                b.Position[0] = b.Position[0] * size;
                b.Position[2] = b.Position[2] * size;
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

        for(int i = 0; i < this.lignesSplineContainers.Count && res == null; i++)
        {
            if (this.lignesSplineContainers[i].ligne.GetHashCode() == l.GetHashCode())
            {
                res = this.lignesSplineContainers[i].splineContainer;
            }
        }

        return res;
    }


}
