using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool PAUSE_ENABLED = false;
    private bool status = false;
    public GameObject pauseMenu;
    public GameObject Camera;

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
    }

    private void ActivatePause()
    {
        pauseMenu.SetActive(true);
        pauseMenu.transform.SetPositionAndRotation(Camera.transform.position + (Camera.transform.forward * 0.75f), Quaternion.LookRotation(Camera.transform.forward));
        Controls.FORCE_RAYCAST = true;
    }

    private void DesactivatePause()
    {
        pauseMenu.SetActive(false);

        Controls.FORCE_RAYCAST = false;

    }
}
