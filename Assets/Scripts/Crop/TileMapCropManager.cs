using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileMapCropManager : TimeAgent
{
    [SerializeField] TileBase Plowed;
    [SerializeField] TileBase Seeded;

    Tilemap targetTilemap;

    [SerializeField] GameObject CropsSpriteprefab;

    [SerializeField] CropContainer container;
    private void Start()
    {
        GameManager.Instance.GetComponent<CropManager>().cropManager = this;
        targetTilemap = GetComponent<Tilemap>();
        OnTimeTick += Tick;
        Init();
        VisualizeMap();
    }

    private void VisualizeMap()
    {
        for(int i = 0;i<container.crops.Count;i++) 
        {
            VisualizeTile(container.crops[i]);
        }
    }

    private void OnDestroy()
    {
        for(int i = 0; i < container.crops.Count; i++)
        {
            container.crops[i].spriteRenderer = null;
        }
    }
    public void Tick(DayTimeController dayTimeController)
     {
        if (targetTilemap == null) { return; }

        foreach (CropTile cropTile in container.crops)
        {
            if (cropTile.crop == null) continue;
            cropTile.damage += 0.02f;

            if (cropTile.damage > 1f)
            {
                cropTile.Harvested();
                targetTilemap.SetTile(cropTile.position, Plowed);
                continue;
            }
            if (cropTile.complete)
            {
                Debug.Log("I an done growing");
                continue;
            }

            cropTile.GrowTimer += 1;

            if (cropTile.GrowTimer >= cropTile.crop.GrowStageTime[cropTile.GrowStage])
            {
                cropTile.spriteRenderer.gameObject.SetActive(true);
                cropTile.spriteRenderer.sprite = cropTile.crop.sprites[cropTile.GrowStage];
                cropTile.GrowStage += 1;
            }
        }
    }
    internal bool Check(Vector3Int pos)
    {
        return container.Get(pos) != null;
    }
    public void Plow(Vector3Int position)
    {
        if (Check(position) == true) return;
        CreatPlowedTile(position);
    }
    public void Seed(Vector3Int position, Crop toSeed)
    {
        CropTile tile = container.Get(position);

        if (tile == null) { return; }

        targetTilemap.SetTile(position, Seeded);

        tile.crop = toSeed;
    }
    private void CreatPlowedTile(Vector3Int position)
    {
        CropTile crop = new CropTile();
        container.Add(crop);
        
        crop.position = position;

        VisualizeTile(crop);

        targetTilemap.SetTile(position, Plowed);
    }

    internal void PickUp(Vector3Int gridPos)
    {
        Vector2Int position = (Vector2Int)gridPos;
        CropTile tile = container.Get(gridPos);
        if (tile == null) { return; }

        if (tile.complete)
        {
            ItemSpawManager.Instance.SpawnItem(
                targetTilemap.CellToWorld(gridPos),
                tile.crop.yield,
                tile.crop.count);
            tile.Harvested();
            VisualizeTile(tile);
        }
    }
    public void VisualizeTile(CropTile cropTile)
    {
        targetTilemap.SetTile(cropTile.position,cropTile.crop != null ? Seeded : Plowed);
        
        if(cropTile.spriteRenderer == null)
        {
            GameObject go = Instantiate(CropsSpriteprefab,transform);
            go.transform.position = targetTilemap.CellToWorld(cropTile.position);
            go.transform.position -= Vector3.forward * 0.01f;
            cropTile.spriteRenderer = go.GetComponent<SpriteRenderer>();
        }
        bool growing = cropTile.crop != null && cropTile.GrowTimer >= cropTile.crop.GrowStageTime[0];
        cropTile.spriteRenderer.gameObject.SetActive(growing);
        if(growing == true)
            cropTile.spriteRenderer.sprite = cropTile.crop.sprites[cropTile.GrowStage - 1];
    }    
}
