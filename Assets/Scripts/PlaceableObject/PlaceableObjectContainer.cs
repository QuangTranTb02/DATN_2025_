using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlaceableObject
{
    public Item placedItem;
    public Transform targetObj;
    public Vector3Int positionOnGrid;


    /// <summary>
    /// serialized JSON string Which contains the state of the object
    /// </summary>
    public string objectState;
    public PlaceableObject(Item item, Vector3Int pos)
    {
        this.placedItem = item;
        this.positionOnGrid = pos;
    }
}
[CreateAssetMenu(menuName = "Data/Placeable Object Conatainer")]
public class PlaceableObjectContainer : ScriptableObject
{
    [SerializeField] SceneName sceneName;
    public List<PlaceableObject> placeableObjects;

    internal PlaceableObject Get(Vector3Int pos)
    {
        return placeableObjects.Find(x => x.positionOnGrid == pos);
    }

    internal void Remove(PlaceableObject placedObject)
    {
        placeableObjects.Remove(placedObject);
    }
}
