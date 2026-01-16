using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Data/Tool Action/Fishing")]
public class Fishing : ToolAction
{
    [SerializeField] List<TileBase> CanFishing;
    [SerializeField] AudioClip onPlowUsed;
    public override bool OnApplyOnTileMap(Vector3Int gridPos,
        TileMapReadController tileMapManager, Item item)
    {
        TileBase tileToFishing = tileMapManager.GetTileBase(gridPos);

        if (CanFishing.Contains(tileToFishing) == false)
        {
            return false;
        }
        GameManager.Instance.FishingMiniGame.SetActive(true);
        return true;
    }
}
