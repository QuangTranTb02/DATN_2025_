using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Areas", fileName = "New Area")]
public class AreaData : ScriptableObject
{
    public string AreaName;
    public List<LevelData> Levels = new List<LevelData>();
}
