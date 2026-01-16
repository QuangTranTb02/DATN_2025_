using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Tool Action/ Place Obj")]
public class PlaceObject : ToolAction
{
    public override bool OnApplyOnTileMap(Vector3Int gridPos, TileMapReadController tileMapManager, Item item)
    {
        if (tileMapManager.placeableObjectManager.Check(gridPos) == true)
        {
            return false;
        }
        tileMapManager.placeableObjectManager.Place(item, gridPos);
        return true;
    }
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
       inventory.Remove(usedItem);
    }
}
