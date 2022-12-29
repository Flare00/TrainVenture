using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalBox : MonoBehaviour
{
    public GameObject content;
    public float maxHeight = 1.0f;
    public float minHeight = 0.05f;

    public int maxQuantity = 1000;
    public int quantity = 1000;

    // Start is called before the first frame update
    void Start()
    {
        if(quantity > maxQuantity)
        {
            quantity = maxQuantity;
        }
    }

    public void Refill()
    {
        this.quantity = maxQuantity;
        RefreshLevelCoal();
    }

    public int GetCoal(int max)
    {
        int res = max;
        if(max < quantity)
        {
            quantity -= max;
        } 
        else
        {
            res = quantity;
            quantity = 0;
        }

        RefreshLevelCoal();
        return res;
    }

    public void RefreshLevelCoal()
    {
        if (content != null)
        {
            if (quantity > 0)
            {
                content.SetActive(true);
                Vector3 pos = content.transform.localPosition;
                pos.y = (((float)quantity / (float)maxQuantity) * (maxHeight - minHeight)) + minHeight;
                content.transform.localPosition = pos;
            }
            else
            {
                content.SetActive(false);
            }
        }
    }

}
