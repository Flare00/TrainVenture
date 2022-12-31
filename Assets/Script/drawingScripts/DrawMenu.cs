using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrawMenu : MonoBehaviour
{
    ///////////////////////////////figure out later
    public GameObject mainMenu;

    public Camera cam;
    ///////////////////////////////figure out later

    //mine : 
    public TextMesh info;
    public InputField input;
    [SerializeField]
    public Material GareTexture;
    private GareDrawing lastGare;
    public GameObject PopupBackground;


    private void Start()
    {
        Controls.FORCE_RAYCAST = true;
        info.gameObject.SetActive(false);
    }

    private void Update()
    {
    }

    public void MakeGare()
    {

    }

    public void OpenOptions()
    {
    }

    public void Exit()
    {
        Application.Quit();
    }
}
