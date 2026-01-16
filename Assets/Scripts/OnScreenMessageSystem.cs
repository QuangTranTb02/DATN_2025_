using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class OnScreenMessage
{
    public GameObject go;
    public float timeToLive;

    public OnScreenMessage(GameObject go)
    {
        this.go = go;
    }
}
public class OnScreenMessageSystem : MonoBehaviour
{
    [SerializeField] GameObject textPrefab;

    List<OnScreenMessage> onScreenMessagesList;
    List<OnScreenMessage> openList;

    [SerializeField] float horizontalScatter = 0.5f;
    [SerializeField] float verticalScatter = 1.0f;
    [SerializeField] float timeToLive = 0.5f;
    private void Awake()
    {
        onScreenMessagesList = new List<OnScreenMessage>();
        openList = new List<OnScreenMessage>();
    }
    private void Update()
    {
        for(int i = onScreenMessagesList.Count - 1; i >= 0; i--)
        {
            onScreenMessagesList[i].timeToLive -= Time.deltaTime;
            if (onScreenMessagesList[i].timeToLive < 0)
            {
                onScreenMessagesList[i].go.SetActive(false);
                openList.Add(onScreenMessagesList[i]);
                onScreenMessagesList.RemoveAt(i);
            }
        }
    }
    public void PostMessage(Vector3 worldPosition, string message)
    {
        worldPosition.z = -1f;
        worldPosition.x += Random.Range(-horizontalScatter, horizontalScatter);
        worldPosition.y += Random.Range(-verticalScatter, verticalScatter);

        if (openList.Count > 0)
        {
            ReUseObjectFromOpenList(worldPosition, message);
        }
        else
        {
            CreateNewOnScreenMessageObject(worldPosition, message);
        }
    }

    private void ReUseObjectFromOpenList(Vector3 worldPosition, string message)
    {
        OnScreenMessage osm = openList[0];
        osm.go.SetActive(true);
        osm.timeToLive = timeToLive;
        osm.go.GetComponent<TextMeshPro>().text = message;
        osm.go.transform.position = worldPosition;
        openList.RemoveAt(0);
        onScreenMessagesList.Add(osm);
    }

    private void CreateNewOnScreenMessageObject(Vector3 worldPosition, string message)
    {

        GameObject textGO = Instantiate(textPrefab, transform);
        textGO.transform.position = worldPosition;

        TextMeshPro tmp = textGO.GetComponent<TextMeshPro>();
        tmp.text = message;

        OnScreenMessage onScreenMessage = new OnScreenMessage(textGO);

        onScreenMessage.timeToLive = timeToLive;
        onScreenMessagesList.Add(onScreenMessage);
    }
}
