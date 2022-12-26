using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    private readonly int MAX_UPGRADE = 6;

    public enum UpgradeType
    {
        MaxSpeed,
        Acceleration,
        Brake
    }

    public GameObject maxSpeed;
    public GameObject acceleration;
    public GameObject brake;


    private GameObject maxSpeedIndicators;
    private GameObject accelerationIndicators;
    private GameObject brakeIndicators;

    private ShopData data;

    public void Start()
    {
        data = ShopData.GetInstance();
        data.Load(ShopData.ShopType.Upgrade);

        maxSpeedIndicators = maxSpeed.transform.Find("indicator").gameObject;
        accelerationIndicators = acceleration.transform.Find("indicator").gameObject;
        brakeIndicators = brake.transform.Find("indicator").gameObject;

        SetButtons();

        UpdateIndicator(UpgradeType.MaxSpeed);
        UpdateIndicator(UpgradeType.Acceleration);
        UpdateIndicator(UpgradeType.Brake);
    }

    private void SetButtons()
    {
        for (int i = 0, max = Enum.GetNames(typeof(UpgradeType)).Length; i < max; i++)
        {
            UpgradeType type = (UpgradeType)i;
            GameObject go = null;

            switch (type)
            {
                case UpgradeType.MaxSpeed:
                    go = maxSpeed;
                    break;
                case UpgradeType.Acceleration:
                    go = acceleration;
                    break;
                case UpgradeType.Brake:
                    go = brake;
                    break;
            }
            if (go != null)
            {
                go.transform.Find("buttons").Find("unlock").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => { Upgrade(type); }));

                GameObject indicators = go.transform.Find("indicator").gameObject;
                for (int j = 0; j < MAX_UPGRADE; j++)
                {
                    int buff = j; //necessary for some obscure reason.
                    Button b = indicators.transform.GetChild(j).AddComponent<Button>();
                    b.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                        {
                            SelectUpgrade(type, buff + 1);
                        }
                    ));
                }
            }
        }
    }

    public void Upgrade(UpgradeType type)
    {
        if (true) // in the future will check if can buy the upgrade and remove the money
        {
            switch (type)
            {
                case UpgradeType.MaxSpeed:
                    if (data.upgrade.maxSpeed < MAX_UPGRADE)
                    {
                        data.upgrade.maxSpeed++;
                        data.upgrade.maxSpeedSelected = data.upgrade.maxSpeed;
                    }
                    break;
                case UpgradeType.Acceleration:
                    if (data.upgrade.acceleration < MAX_UPGRADE)
                    {
                        data.upgrade.acceleration++;
                        data.upgrade.accelerationSelected = data.upgrade.acceleration;
                    }
                    break;
                case UpgradeType.Brake:
                    if (data.upgrade.acceleration < MAX_UPGRADE)
                    {
                        data.upgrade.brake++;
                        data.upgrade.brakeSelected = data.upgrade.brake;
                    }
                    break;
            }
            data.Save(ShopData.ShopType.Upgrade);
        }

        UpdateIndicator(type);
    }

    public void SelectUpgrade(UpgradeType type, int value)
    {
        if (value > 0 && value <= MAX_UPGRADE)
        {
            switch (type)
            {
                case UpgradeType.MaxSpeed:
                    this.data.upgrade.maxSpeedSelected = value < data.upgrade.maxSpeed ? value : data.upgrade.maxSpeed;
                    break;
                case UpgradeType.Acceleration:
                    this.data.upgrade.accelerationSelected = value < data.upgrade.acceleration ? value : data.upgrade.acceleration;
                    break;
                case UpgradeType.Brake:
                    this.data.upgrade.brakeSelected = value < data.upgrade.brake ? value : data.upgrade.brake;
                    break;
            }
            data.Save(ShopData.ShopType.Upgrade);
        }
        

        UpdateIndicator(type);
    }

    public void UpdateIndicator(UpgradeType type)
    {
        GameObject elem = null;
        int max = 0, selected = 0;
        switch (type)
        {
            case UpgradeType.MaxSpeed:
                elem = maxSpeedIndicators;
                max = data.upgrade.maxSpeed;
                selected = data.upgrade.maxSpeedSelected;
                break;
            case UpgradeType.Acceleration:
                elem = accelerationIndicators;
                max = data.upgrade.acceleration;
                selected = data.upgrade.accelerationSelected;
                break;
            case UpgradeType.Brake:
                elem = brakeIndicators;
                max = data.upgrade.brake;
                selected = data.upgrade.brakeSelected;
                break;
        }

        if (elem != null)
        {
            for (int i = 0; i < MAX_UPGRADE; i++)
            {
                Image img = elem.transform.GetChild(i).gameObject.GetComponent<Image>();
                if (i >= max)
                {
                    img.color = Color.gray;
                }
                else if (i >= selected)
                {
                    img.color = Color.white;
                }
                else
                {
                    img.color = Color.yellow;
                }
            }
        }
    }
}
