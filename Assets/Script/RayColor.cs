using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class RayColor : MonoBehaviour
{
    public Color valid;
    public Color invalid;
    public GameObject ray;
    public XRRayInteractor rayInteractor;

    private Renderer mat;
    private void Start()
    {
        mat = ray.GetComponent<Renderer>();
        ExitHover();
    }

    private void Update()
    {
        RaycastHit? hit3D;
        int index3D;
        RaycastResult? hitUI;
        int indexUI;
        bool closest;
        rayInteractor.TryGetCurrentRaycast(out hit3D, out index3D, out hitUI, out indexUI, out closest);

        bool hover = false;
        if(hit3D.HasValue)
        {
            hover = true;
        }

        if(hitUI.HasValue)
        {
            hover = hover || hitUI.Value.gameObject.tag == "UIButton";
        }

        if (hover)
        {
            Hover();
        } else
        {
            ExitHover();
        }
    }

    public void Hover()
    {
        mat.material.color = valid;
    }

    public void ExitHover()
    {

        mat.material.color = invalid;
    }
}
