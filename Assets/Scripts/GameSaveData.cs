using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure được serialize để lưu vào file
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public int version = 1; // Để upgrade save file sau này
    public string saveDate;
    public float totalPlayTime;

    // Progress data cho từng level
    public List<LevelProgress> levelProgressList = new List<LevelProgress>();

    // Settings
    public GameSettings settings = new GameSettings();
}

[System.Serializable]
public class LevelProgress
{
    public string levelID; // Unique ID từ LevelData
    public float bestTime;
    public int totalDeaths;
    public bool isCompleted;
    public bool isUnlocked;
    public int stars; // 0-3

    // Constructor
    public LevelProgress(string id)
    {
        levelID = id;
        bestTime = 0f;
        totalDeaths = 0;
        isCompleted = false;
        isUnlocked = false;
        stars = 0;
    }
}

[System.Serializable]
public class GameSettings
{
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool showFPS = false;
    public int qualityLevel = 2;
}