using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tool Action/Seed") ]
public class SeedTile : ToolAction
{
    public override bool OnApplyOnTileMap(Vector3Int gridPos, 
        TileMapReadController tileMapManager,Item item)
    {
        if(tileMapManager.cropManager.Check(gridPos) == false) return false;

        tileMapManager.cropManager.Seed(gridPos,item.crop);
        return true;
    }
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        inventory.Remove(usedItem);
    }
}
