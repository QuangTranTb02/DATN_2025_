using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class PlaceableObjectManager : MonoBehaviour
{
    [SerializeField] PlaceableObjectContainer placeableObjContainer;

    [SerializeField] Tilemap targetTileMap;

    private void Start()
    {
        GameManager.Instance.GetComponent<PlaceableObjectReferenceManager>().placeableObjectManager = this;
        VisualizeMap();
    }
    private void OnDestroy()
    {
        for(int i = 0;i< placeableObjContainer.placeableObjects.Count;i++)
        {
            if (placeableObjContainer.placeableObjects[i].targetObj == null) { continue; }

            IPersistant persistant = placeableObjContainer.placeableObjects[i].targetObj.GetComponent<IPersistant>();
            if (persistant != null)
            {
                string jsonString = persistant.Read();
                placeableObjContainer.placeableObjects[i].objectState = jsonString;
            }
            placeableObjContainer.placeableObjects[i].targetObj = null;
        }
    }

    private void VisualizeMap()
    {
        for(int i = 0; i < placeableObjContainer.placeableObjects.Count; i++)
        {
            VisualizeItem(placeableObjContainer.placeableObjects[i]);
        }
    }

    private void VisualizeItem(PlaceableObject placeableObject)
    {
        GameObject go = Instantiate(placeableObject.placedItem.itemPrefab);

        go.transform.parent = transform;
        Vector3 pos = targetTileMap.CellToWorld(placeableObject.positionOnGrid) 
            + targetTileMap.cellSize / 2;
        pos -= Vector3.forward * 0.1f;
        go.transform.position = pos;

        IPersistant persistant = go.GetComponent<IPersistant>();
        if(persistant != null )
        {
            persistant.Load(placeableObject.objectState);
        }
        placeableObject.targetObj = go.transform;
    }
    public bool Check(Vector3Int pos)
    {
        return placeableObjContainer.Get(pos) != null;
    }
    public void Place(Item item, Vector3Int positionGrid)
    {
        if(Check(positionGrid) == true) {  return; }
        PlaceableObject placeableObject = new PlaceableObject(item, positionGrid);
        VisualizeItem(placeableObject);
        placeableObjContainer.placeableObjects.Add(placeableObject);
    }

    internal void PickUp(Vector3Int gridPos)
    {
        PlaceableObject placedObject = placeableObjContainer.Get(gridPos);

        if(placedObject == null) { return; }
        ItemSpawManager.Instance.SpawnItem(targetTileMap.CellToWorld(gridPos), 
            placedObject.placedItem,
            1);
        Destroy(placedObject.targetObj.gameObject);

        placeableObjContainer.Remove(placedObject);
    }
}
