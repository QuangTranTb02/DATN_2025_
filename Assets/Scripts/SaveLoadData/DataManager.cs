using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    [SerializeField] PlayerData PlayerData;
    private List<PlayerData> PlayerDataLists;
    private IDataService DataService = new JsonDataService();

    /*private void Start()
    {
        SaveData();
    }*/
    public void InitData()
    {

    }

    public void SaveData()
    {
        if (DataService.SaveData("/player-stats1.json", PlayerData, false))
        {
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, "player-stats.json"); ;
            Debug.Log(filePath);
        }
        else
        {
            Debug.LogError("Could not save file! Show something on the UI about it!");
        }
    }
    public void LoadData() 
    {

    }
}
