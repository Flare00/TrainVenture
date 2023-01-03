using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Splines;

[System.Serializable]
public class Ligne
{

    [SerializeField]
    public Spline spline;

    [SerializeField]
    public Lien l1;

    [SerializeField]
    public Lien l2;

    public Ligne(Lien l1, Lien l2, Spline spline)
    {
        this.spline = spline;
        this.l1 = l1;
        this.l2 = l2;
    }

}
