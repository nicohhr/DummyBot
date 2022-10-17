using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting.FullSerializer;

public static class SaveSystem
{

    public static void SaveSettings(Settings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, settings);
        stream.Close();
    }
    
    public static Settings LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.fun";
        Debug.Log(path);

        // Conferindo se arquivo existe
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings settings = formatter.Deserialize(stream) as Settings;
            stream.Close();
            Debug.Log("Venv path -> " + settings.VenvPath);
            return settings;

        } else
        {
            Debug.Log("Setting file was not found in " + path);
            return null;
        }
    }
}
