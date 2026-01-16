using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSpawManager : Singleton<ItemSpawManager>
{
    [SerializeField] GameObject PickUpItemPrefab;
    public void SpawnItem(Vector3 position, Item item, int count)
    {
        GameObject go =  Instantiate(PickUpItemPrefab,position,Quaternion.identity);
        go.GetComponent<PickUpItem>().set(item, count);
    }

    public void SpawnItem(Transform transform, Vector3 position, Item item, int count)
    {
        GameObject go = Instantiate(PickUpItemPrefab, transform);
        go.transform.position = position;
        go.GetComponent<PickUpItem>().set(item, count);
    }
}
    