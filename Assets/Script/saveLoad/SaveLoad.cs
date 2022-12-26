
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Splines;

public class SaveLoad
{
    private static SaveLoad instance;

    public static SaveLoad GetInstance()
    {
        instance ??= new SaveLoad(); 
        return instance;
    }

    private readonly BinaryFormatter br;
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
        FileStream fs = new(this.save_folder + name, FileMode.Create);
        this.br.Serialize(fs, data);
        fs.Close();
    }

    public IDataSave Load(string name)
    {
        if (File.Exists(this.save_folder + name))
        {
            FileStream fs = new(this.save_folder + name, FileMode.Open);
            IDataSave res = this.br.Deserialize(fs) as IDataSave;
            fs.Close();
            return res;
        }
        return null;
    }
	
	public void SaveSpline(Spline spline, string name){
		FileStream fs = new(this.save_folder + name, FileMode.Create);
        this.br.Serialize(fs, spline);
        fs.Close();
	}
	
    public Spline LoadSpline(string name)
    {
        if (File.Exists(this.save_folder + name))
        {
            FileStream fs = new(this.save_folder + name, FileMode.Open);
            Spline res = this.br.Deserialize(fs) as Spline;
            fs.Close();
            return res;
        }
        return null;
    }
	
}
