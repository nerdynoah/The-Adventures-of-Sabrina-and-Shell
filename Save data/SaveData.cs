using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    private string fileName;
    private Dictionary<string, object> data;

    public void Start()
    {
        fileName = Path.Combine(Application.persistentDataPath, "game.json");
    }
    public void SaveGameState()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        using (FileStream stream = File.Create(fileName))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
        }
    }
    public void LoadGameState()
    {
        if (!File.Exists(fileName))
        {
            Debug.Log("No saved game");
            return;
        }
        using (FileStream stream = File.OpenRead(fileName))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            data = formatter.Deserialize(stream) as Dictionary<string, object>;
        }
    }

}
