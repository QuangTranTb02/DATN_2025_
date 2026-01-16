using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapReadController : MonoBehaviour
{

    [SerializeField] Tilemap tileMap;
    public CropManager cropManager;
    public PlaceableObjectReferenceManager placeableObjectManager;

    public Vector3Int GetGridPosition(Vector2 pos, bool mousePositon = false) 
    {
        if (tileMap == null)
        {
            tileMap = GameObject.Find("GroundTileMap").GetComponent<Tilemap>();
        }
        if (tileMap == null) { return Vector3Int.zero; }

        Vector3 WorldPosition;
        if (mousePositon)
        {
            WorldPosition = Camera.main.ScreenToWorldPoint(pos);

        }
        else
        {
            WorldPosition = pos;
        }

        Vector3Int gridPositon = tileMap.WorldToCell(WorldPosition);

        return gridPositon;
    }
    public TileBase GetTileBase(Vector3Int gridPositon )
    {

        if (tileMap == null)
        {
            tileMap = GameObject.Find("GroundTileMap").GetComponent<Tilemap>();
        }
        if (tileMap == null) { return null; }
        TileBase tile = tileMap.GetTile(gridPositon);

        return tile;
    }
 
}
