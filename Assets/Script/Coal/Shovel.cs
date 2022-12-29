using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Shovel : MonoBehaviour
{
    public GameObject coal;
    public int maxQuantity = 25;
    public int quantity = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HIT : " + other.tag);
        if(other.tag == "Coal")
        {
            Debug.Log("COAL");
            CoalBox c = other.gameObject.GetComponent<CoalBox>();
            quantity += c.GetCoal(maxQuantity - quantity);
            if(quantity > 0)
            {
                coal.SetActive(true);
            }
        } else if (other.tag == "Furnace")
        {
            Debug.Log("FOUR");
            Furnace f = other.gameObject.GetComponent<Furnace>();
            quantity = f.PutCoal(quantity);
            if(quantity <= 0) { 
                coal.SetActive(false);
            }
        }
    }

}
