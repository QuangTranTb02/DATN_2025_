using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[Serializable]
public class CropTile
{
    public int GrowTimer;
    public Crop crop;
    public int GrowStage;
    public SpriteRenderer spriteRenderer;
    public float damage;
    public Vector3Int position;
    public bool complete
    {
        get
        {
            if (crop == null) { return  false; }
            return GrowTimer >= crop.TimeToGrow;
        }
    }

    internal void Harvested()
    {
        GrowTimer = 0;
        GrowStage = 0;
        crop = null;
        spriteRenderer.gameObject.SetActive(false);
        damage = 0;
    }
}
public class CropManager : MonoBehaviour
{
    public TileMapCropManager cropManager;
    public void PickUp(Vector3Int gridPos)
    {
        if(cropManager == null) {
            Debug.Log("No tile map crops manager are ref in the crop manager ");
            return; }
       cropManager.PickUp(gridPos);
    }

    public bool Check(Vector3Int pos)
    {
        if (cropManager == null)
        {
            Debug.Log("No tile map crops manager are ref in the crop manager ");
            return false;
        }
        return cropManager.Check(pos);
    }
    public void Seed(Vector3Int pos, Crop toSeed)
    {
        if (cropManager == null)
        {
            Debug.Log("No tile map crops manager are ref in the crop manager ");
            return;
        }
        cropManager.Seed(pos, toSeed);
    }

    public void Plow(Vector3Int pos)
    {
        if (cropManager == null) { return; }

        cropManager.Plow(pos);
    }
}
