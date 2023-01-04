using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataTrainPath : IDataSave
{
    [SerializeField]
    public List<Gare> gares = new();

    [SerializeField]
    public List<Ligne> lignes = new();

    public DataTrainPath(){}

    public DataTrainPath(List<Gare> gares, List<Ligne> lignes)
    {
        this.gares = gares;
        this.lignes = lignes;
    }
}
