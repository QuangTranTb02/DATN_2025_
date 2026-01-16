using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SceneName
{
    Farm,
    Beach,
    Cave,
    CaveInto,
    Town,
    Forest,
    Maze
}
[CreateAssetMenu(menuName = "Data/Plyer Data")]
public class PlayerData : ScriptableObject
{
    public string characterName = "Quang";
    public Gender characterGender = Gender.Male;
    public int gold = 1000;
    public int saveSlotId = 0;

    public Season season;
    public DayOfWeek dayOfWeek;
    public int Day;
    public float time;

    public List<CropContainer> cropContainers;
    public List<PlaceableObjectContainer> placeableObjectContainers;
    public List<JsonStringList> jsonStringLists;

    public ItemContainer inventory;
}
