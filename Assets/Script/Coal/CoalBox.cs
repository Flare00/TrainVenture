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

    public int GetCoal(int max)
    {
        if(max < quantity)
        {
            quantity -= max;

            Vector3 pos = content.transform.localPosition;
            pos.y = (((float)quantity / (float)maxQuantity) * (maxHeight - minHeight)) + minHeight;
            content.transform.localPosition = pos;

            return max;
        } else
        {
            quantity = 0;
            content.SetActive(false);
            return quantity;
        }
    }


}
