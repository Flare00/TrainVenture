
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SaveLoad
{
    private static SaveLoad instance;

    public static SaveLoad GetInstance()
    {
        if(instance == null)
        {
            instance = new SaveLoad();
        } 
        return instance;
    }

    private BinaryFormatter br;
    public string save_folder;


    SaveLoad(string save_folder = "Save/")
    {
        this.br = new BinaryFormatter();
        this.save_folder = save_folder;
        if (!Directory.Exists(save_folder))
        {
            Directory.CreateDirectory(save_folder);
        }
    }

    public void Save(IDataSave data, string name)
    {
        FileStream fs = new FileStream(this.save_folder + name, FileMode.Create);
        this.br.Serialize(fs, data);
        fs.Close();
    }

    public IDataSave Load(string name)
    {
        if (File.Exists(this.save_folder + name))
        {
            FileStream fs = new FileStream(this.save_folder + name, FileMode.Open);
            IDataSave res = this.br.Deserialize(fs) as IDataSave;
            fs.Close();
            return res;
        }
        return null;
    }
}
