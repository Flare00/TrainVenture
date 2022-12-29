using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public ParticleSystem smoke;

    public GameObject content;
    public float maxHeight = 1.96f;
    public float minHeight = 1.90f;


    public float maxPression = 16.0f;
    public float pression = 1.0f;

    public int maxQuantity = 100;
    public int quantity = 0;

    public float timeConsumption = 2.0f;
    private float timeSinceLastConsumption = 0;
    public float temperature = 16.0f;
    
    // Update is called once per frame
    void Update()
    {
        timeSinceLastConsumption += Time.deltaTime;
        if(timeSinceLastConsumption > timeConsumption)
        {
            timeSinceLastConsumption = 0;
            if(quantity > 0)
            {
                quantity--;
                RefreshLevelCoal();
            }
        }

        if(temperature < 100 && quantity > 0)
        {
            temperature += (Mathf.Pow(quantity, 2.0f) / 1000.0f)*Time.deltaTime;

            if(temperature > 100)
            {
                temperature = 100;
            }
        } else if (quantity == 0 && temperature > 16.0f)
        {
            temperature -= Time.deltaTime * 0.1f;
        }

        if(temperature > 80)
        {
            pression += Time.deltaTime * 0.5f;
        } else if(pression > 1.0f)
        {
            pression -= Time.deltaTime * 0.1f;
        }

        if(smoke != null)
        {
            var emission = smoke.emission;
            emission.rateOverTimeMultiplier = pression > 3 ? pression : 0;
        }

        if(pression > maxPression)
        {
            pression = maxPression; //For the futur, make it boom.
        }
    }

    /*
     * Return the left coal if max Quantity is reached.
     */
    public int PutCoal(int number)
    {
        int res = 0;
        this.quantity += number;
        if(this.quantity  > maxQuantity)
        {
            res = this.quantity % maxQuantity;
            this.quantity = maxQuantity;
        }
        RefreshLevelCoal();
        return res;
    }

    public void RefreshLevelCoal()
    {
        if(content != null)
        {
            if(quantity > 0)
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
