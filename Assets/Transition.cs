using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Transition : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    private bool hide = false;
    private bool show = false;

    private float value = 0.0f;

    private ColorAdjustments filter;
    // Start is called before the first frame update
    void Start()
    {
        if(!volumeProfile.TryGet<ColorAdjustments>(out filter))
        {
            filter = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            value += Time.deltaTime;
            if(value >= 1.0f)
            {
                value = 1.0f;
                show= false;
            }

            UpdateValue();
        }
        else if (hide)
        {
            value -= Time.deltaTime;
            if (value <= 0.0f)
            {
                value = 0.0f;
                hide = false;
            }
            UpdateValue();
        }
    }

    private void UpdateValue()
    {
        if (filter != null)
        {
            filter.colorFilter.value = new Color(value, value, value);
        }
    }

    public void Show()
    {
        value = 0.0f;
        show = true;
    }

    public void Hide()
    {
        value = 1.0f;
        hide = true;
    }

    public void SetValue(float val)
    {
        value = val;
        UpdateValue();
    }

    public float GetValue() { return value; }
}
