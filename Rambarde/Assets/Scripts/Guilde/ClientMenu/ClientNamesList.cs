using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ClientNamesList : MonoBehaviour
{
    private List<string> clientNames = new List<string>();
    private int namesCount;

    public void Init()
    {
        string path = "TextAssets/ClientNames";

        StringReader reader = new StringReader(Resources.Load<TextAsset>(path).text);

        while (reader.Peek() >= 0)
            clientNames.Add(reader.ReadLine());
        reader.Close();

        namesCount = clientNames.Count;
    }

    public string generateClientName()
    {
        int nName = Mathf.FloorToInt(Random.Range(0,namesCount));
        return clientNames[nName]; 
    }


}
