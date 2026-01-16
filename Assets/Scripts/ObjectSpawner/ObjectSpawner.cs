using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] float SpawnArea_height = 1f;
    [SerializeField] float SpawnArea_width = 1f;

    [SerializeField] GameObject[] spawn;

    [SerializeField] float probability = 0.1f;
    [SerializeField] int spawnCount = 1;
    [SerializeField] int objectSpawnLimit = -1;
    int lenght;

    [SerializeField] bool oneTime = false;

    List<SpawnedObject> spawnedObjects;
    [SerializeField] JsonStringList targetSaveJsonList;
    [SerializeField] int idInList = -1;

    private void Start()
    {
        lenght = spawn.Length;
        if(oneTime == false)
        {
            TimeAgent time = GetComponent<TimeAgent>();
            time.OnTimeTick += UpdateSpawnedObjectCount;
            time.OnTimeTick += Spawn;
            spawnedObjects = new List<SpawnedObject>();

            LoadData();
        }
        else
        {
            Spawn(null);
            //Destroy(gameObject);
        }
    }
    void UpdateSpawnedObjectCount(DayTimeController dayTimeController)
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
            }
        }
    }


    internal void SpawnedObjectDestroy(SpawnedObject spawnedObject)
    {
        spawnedObjects.Remove(spawnedObject);
    }
    void Spawn(DayTimeController dayTimeController)
    {
        if (Random.value > probability) { return; }
        if(spawnedObjects != null)
        {
            if (objectSpawnLimit <= spawnedObjects.Count && objectSpawnLimit != -1) { return; }
        }
        for (int i = 0; i < spawnCount; i++)
        {
            int id = Random.Range(0, lenght);
            GameObject go = Instantiate(spawn[Random.Range(0, lenght)]);
            Transform t = go.transform;
            t.SetParent(transform);
            if (oneTime == false)
            {

                SpawnedObject spawnedObject = go.AddComponent<SpawnedObject>();
                spawnedObjects.Add(spawnedObject);
                spawnedObject.objectID = id;
            }

            Vector3 positon = transform.position;
            positon.x += Random.Range(-SpawnArea_width, SpawnArea_width);
            positon.y += Random.Range(-SpawnArea_height, SpawnArea_height);

            t.position = positon;
        }
    }
    public class ToSave
    {
        public List<SpawnedObject.SaveSpawedObjectData> spawedObjectDatas;

        public ToSave()
        {
            spawedObjectDatas = new List<SpawnedObject.SaveSpawedObjectData> ();
        }
    }
    string Read()
    {
        ToSave toSave = new ToSave();

        for(int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null) 
            {
                toSave.spawedObjectDatas.Add(
                new SpawnedObject.SaveSpawedObjectData(spawnedObjects[i].objectID,
                spawnedObjects[i].transform.position));
            }
        }
        return JsonUtility.ToJson(toSave);
    }
    public void Load(string json)
    {
        if (json == "" || json == "{}" || json == null)
        {
            return;
        }
        ToSave toLoad = JsonUtility.FromJson<ToSave>(json);

        for(int i = 0; i < toLoad.spawedObjectDatas.Count; i++)
        {
            SpawnedObject.SaveSpawedObjectData data = toLoad.spawedObjectDatas[i];
            GameObject go = Instantiate(spawn[data.objectID]);
            go.transform.position = data.worldPosition;
            go.transform.SetParent(transform);
            SpawnedObject so = go.AddComponent<SpawnedObject>();

            go.GetComponent<SpawnedObject>().objectID = data.objectID;
            spawnedObjects.Add(so);
        }
    }
    private void OnDestroy()
    {
        if(oneTime == true) { return; }
        SaveData();
    }

    private void SaveData()
    {
        if (CheckJSON() == false)
        {
            return;
        }
        string jsonString = Read();
        targetSaveJsonList.SetString(jsonString, idInList);
    }
    private void LoadData()
    {
        if (CheckJSON() == false)
        {
            return;
        }
        
        Load(targetSaveJsonList.GetString(idInList));
    }

    private bool CheckJSON()
    {
        if (oneTime == true) { return false; }
        if (targetSaveJsonList == null)
        {
            Debug.Log("target Json to Save spawned object is null");
            return false;
        }
        if (idInList == -1)
        {
            Debug.Log("id in list is not assigned data can't be save");
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(SpawnArea_width * 2, SpawnArea_height * 2));
    }

   
}
