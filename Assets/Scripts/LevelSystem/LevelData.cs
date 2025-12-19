using NUnit.Framework;
using System;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName = "Level ";
    public int levelNumber = 1;
    public string levelID; // Unique ID
    public SceneField Scene;

    [Header("Spawn")]
    public Vector2 playerSpawnPosition = Vector2.zero;

    [Header("Requirements")]
    public int totalCollectibles = 0; // Số items trong level

    [Header("Progress (Auto Save)")]
    public float bestTime = 0f;
    public int totalDeaths = 0;
    public bool isCompleted = false;
    public bool isUnlocked = false;
    public int stars = 0; // 0-3 sao

    [Header("UI Display")]
    [TextArea(2, 4)]
    public string description = "Complete the level!";
    public GameObject LevelButtonObj { get; set; } // Icon của level

    // Reset progress (dùng khi debug)
    public void ResetProgress()
    {
        bestTime = 0f;
        totalDeaths = 0;
        isCompleted = false;
        stars = 0;
    }
}
