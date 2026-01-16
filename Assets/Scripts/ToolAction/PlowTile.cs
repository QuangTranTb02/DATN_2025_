using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Data/Tool Action/Plow")]
public class PlowTile : ToolAction
{
    [SerializeField] List<TileBase> canPlow ;
    [SerializeField] AudioClip onPlowUsed;
    public override bool OnApplyOnTileMap(Vector3Int gridPos,
        TileMapReadController tileMapManager, Item item)
    {
        TileBase tileToPlow = tileMapManager.GetTileBase(gridPos);

        if(canPlow.Contains(tileToPlow) == false) 
        {
            return false;
        }
        tileMapManager.cropManager.Plow(gridPos);
        AudioManager.Instance.Play(onPlowUsed);
        return true;
    }
}
