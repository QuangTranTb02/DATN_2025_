using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string Name;
    public Sprite Icon;
    public bool Stackable;
    public ToolAction OnAction;
    public ToolAction OnTileMapAction;
    public ToolAction OnItemUsed;
    public Crop crop;
    public bool iconHightlight;
    public GameObject itemPrefab;
    public bool isWeapon;
    public int damage;
    public bool canSold;
    public int price = 100;
}
