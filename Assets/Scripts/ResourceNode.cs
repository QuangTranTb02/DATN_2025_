using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ResourceNode : ToolHit
{
    [SerializeField] GameObject DropItem;
    
    [SerializeField] float Spread = 5f;

    [SerializeField] int itemCountInOneDrop;

    [SerializeField] Item item;

    [SerializeField] int DropCount = 5;

    [SerializeField] ResourceNodeType nodeType;

    public override void Hit()
    {
        while (DropCount > 0)
        {
            DropCount--;

            Vector3 position = transform.position;
            position.x += Spread * Random.value - Spread / 2;
            position.y += Spread * Random.value - Spread / 2;
            
            ItemSpawManager.Instance.SpawnItem(position, item, itemCountInOneDrop);
        }
        SpawnedObject spawnedObject = GetComponent<SpawnedObject>();

        if(spawnedObject != null)
        {
            spawnedObject.SpawnedObjectDestroyed();
        }
        Destroy(gameObject);
    }
    public override bool CanBeHit(List<ResourceNodeType> types)
    {
        return types.Contains(nodeType);
    }
}
