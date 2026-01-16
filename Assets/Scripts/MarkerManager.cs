using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MarkerManager : MonoBehaviour
{
    [SerializeField] Tilemap targetTilemap;
    [SerializeField] TileBase tile;

    public Vector3Int markedCellPosition;
    Vector3Int oldCellPositon;

    bool show;
    private void Update()
    {
        if(show == false) { return; }
        targetTilemap.SetTile(oldCellPositon, null);
        targetTilemap.SetTile(markedCellPosition, tile);
        oldCellPositon = markedCellPosition;
    }

    internal void Show(bool selectable)
    {
        show = selectable;
        targetTilemap.gameObject.SetActive(show);
    }
}
