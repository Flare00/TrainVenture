using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Splines;
using TMPro;

public class HeightMapDrawMenu : MonoBehaviour
{
    public Camera cam;

    //these don't have to do with the UI : 
    public HeightMapDrawingBoard board;

    public void ChangeElevation(){
        board.changeElevation();
    }
    /*public void smooth(){
        board.smoothMap();
    }*/

    private void Start()
    {
        Controls.FORCE_RAYCAST = true;
    }

    private void Update()
    {
    }

    public void save(){
        board.save();
    }
}