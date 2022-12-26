using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using Unity.VisualScripting;
using UnityEngine;

public class ShopData
{
    public static string FILENAME_UPGRADE = "upgrade.data";

    private List<Action> upgradeListeners = new();
    private static ShopData instance;

    public static ShopData GetInstance()
    {
        if (instance == null)
            instance = new ShopData();
        return instance;
    }

    public ShopData()
    {
        LoadAll();
    }

    public enum ShopType
    {
        Upgrade
    }

    [System.Serializable]
    public class Upgrade : IDataSave
    {
        public int maxSpeed = 1;
        public int maxSpeedSelected = 1;

        public int acceleration = 1;
        public int accelerationSelected = 1;

        public int brake = 1;
        public int brakeSelected = 1;
    }

    public Upgrade upgrade = new Upgrade();

    public void SaveAll()
    {
        Save(ShopType.Upgrade);
    }

    public void LoadAll()
    {
        Load(ShopType.Upgrade);
    }

    public void Save(ShopType type)
    {
        switch (type)
        {
            case ShopType.Upgrade:
                if (upgrade != null)
                {
                    SaveLoad.GetInstance().Save(upgrade, FILENAME_UPGRADE);
                    NotifyUpgradeChange();
                }
                break;
        }
    }

    public void Load(ShopType type)
    {
        IDataSave tmp;
        switch (type)
        {
            case ShopType.Upgrade:
                tmp = SaveLoad.GetInstance().Load(FILENAME_UPGRADE);
                upgrade = tmp == null ? new Upgrade() : (Upgrade)tmp;
                break;
        }
    }

    public void NotifyUpgradeChange()
    {
        foreach(Action callback in upgradeListeners)
        {
            callback();
        }
    }

    public void SubscribeUpgradeChange(Action a)
    {
        this.upgradeListeners.Add(a);
    }
}
