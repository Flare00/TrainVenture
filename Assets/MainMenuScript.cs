using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject optionsMenu;

    public Camera cam;

    public Transition transition;
    private AsyncOperation loader = null;
    private void Start()
    {
        Controls.FORCE_RAYCAST = true;
        transition.Show();
    }

    private void Update()
    {
        if(loader != null)
        {
            if (transition.GetValue() < 0.01f)
            {
                loader.allowSceneActivation = true;
            }
        }
    }

    public void Play()
    {
        loader = SceneManager.LoadSceneAsync("Game");
        loader.allowSceneActivation = false;
        Controls.FORCE_RAYCAST = false;
        transition.Hide();
    }

    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
