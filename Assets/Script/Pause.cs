using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool PAUSE_ENABLED = false;
    private bool status = false;
    public GameObject pauseGO;
    public GameObject Camera;

    public GameObject pauseMenu;
    public GameObject upgradeMenu;
    public GameObject optionsMenu;

    public Transition transition;
    private AsyncOperation loader = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PAUSE_ENABLED)
        {
            if (status)
            {
                DesactivatePause();
                status = false;
            }
        }
        else
        {
            if (!status)
            {
                ActivatePause();
                status = true;
            }
        }

        if(loader != null)
        {
            if(transition.GetValue() < 0.01f)
            {
                loader.allowSceneActivation = true;
            }
        }
    }

    private void ActivatePause()
    {
        this.upgradeMenu.SetActive(false);
        this.optionsMenu.SetActive(false);
        this.pauseMenu.SetActive(true);

        pauseGO.SetActive(true);
        pauseGO.transform.SetPositionAndRotation(Camera.transform.position + (Camera.transform.forward * 0.75f), Quaternion.LookRotation(Camera.transform.forward));
        Controls.FORCE_RAYCAST = true;
    }

    private void DesactivatePause()
    {
        pauseGO.SetActive(false);

        Controls.FORCE_RAYCAST = false;
    }
    public void ClosePause()
    {
        PAUSE_ENABLED = false;
    }
    public void OpenUpgrade()
    {
        this.pauseMenu.SetActive(false);
        this.upgradeMenu.SetActive(true);
    }

    public void CloseUpgrade()
    {
        this.pauseMenu.SetActive(true);
        this.upgradeMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        this.pauseMenu.SetActive(false);
        this.optionsMenu.SetActive(true);
    }
    public void CloseOptions()
    {
        this.pauseMenu.SetActive(true);
        this.optionsMenu.SetActive(false);
    }

    public void MainMenu()
    {
        loader = SceneManager.LoadSceneAsync("MainMenu");
        loader.allowSceneActivation = false;
        Controls.FORCE_RAYCAST = false;
        transition.Hide();
    }


}
