using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Singleton quản lý save/load data
/// Sử dụng JSON để lưu trữ
/// </summary>
public class SaveSystem : MonoBehaviour
{
    private static SaveSystem _instance;
    public static SaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveSystem");
                _instance = go.AddComponent<SaveSystem>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private const string SAVE_FILE_NAME = "savefile.json";
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    public GameSaveData CurrentSaveData { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region Save/Load Core

    /// <summary>
    /// Lưu toàn bộ game data
    /// </summary>
    public void SaveGame()
    {
        try
        {
            CurrentSaveData.saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string json = JsonUtility.ToJson(CurrentSaveData, true);
            File.WriteAllText(SaveFilePath, json);

            Debug.Log($"[SaveSystem] Game saved successfully to: {SaveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Load game data từ file
    /// </summary>
    public void LoadGame()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                string json = File.ReadAllText(SaveFilePath);
                CurrentSaveData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log($"[SaveSystem] Game loaded successfully from: {SaveFilePath}");
            }
            else
            {
                Debug.Log("[SaveSystem] No save file found. Creating new save data.");
                CurrentSaveData = new GameSaveData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load game: {e.Message}");
            CurrentSaveData = new GameSaveData();
        }
    }

    /// <summary>
    /// Xóa save file
    /// </summary>
    public void DeleteSave()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                CurrentSaveData = new GameSaveData();
                Debug.Log("[SaveSystem] Save file deleted.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to delete save: {e.Message}");
        }
    }

    #endregion

    #region Level Progress Management

    /// <summary>
    /// Lấy progress của một level
    /// </summary>
    public LevelProgress GetLevelProgress(string levelID)
    {
        LevelProgress progress = CurrentSaveData.levelProgressList.Find(p => p.levelID == levelID);

        if (progress == null)
        {
            // Tạo mới nếu chưa có
            progress = new LevelProgress(levelID);
            CurrentSaveData.levelProgressList.Add(progress);
        }

        return progress;
    }

    /// <summary>
    /// Cập nhật progress của một level
    /// </summary>
    public void UpdateLevelProgress(string levelID, float time, int deaths, bool completed, int stars)
    {
        LevelProgress progress = GetLevelProgress(levelID);

        // Cập nhật best time (chỉ nếu tốt hơn hoặc chưa có)
        if (progress.bestTime == 0f || time < progress.bestTime)
        {
            progress.bestTime = time;
        }

        progress.totalDeaths += deaths;
        progress.isCompleted = completed;

        // Cập nhật stars (chỉ nếu cao hơn)
        if (stars > progress.stars)
        {
            progress.stars = stars;
        }

        SaveGame(); // Auto save sau khi update
    }

    /// <summary>
    /// Unlock một level
    /// </summary>
    public void UnlockLevel(string levelID)
    {
        LevelProgress progress = GetLevelProgress(levelID);
        progress.isUnlocked = true;
        SaveGame();
    }

    /// <summary>
    /// Kiểm tra xem level đã unlock chưa
    /// </summary>
    public bool IsLevelUnlocked(string levelID)
    {
        LevelProgress progress = GetLevelProgress(levelID);
        return progress.isUnlocked;
    }

    /// <summary>
    /// Sync data từ SaveSystem vào LevelData ScriptableObject
    /// Gọi khi cần hiển thị UI
    /// </summary>
    public void SyncToLevelData(LevelData levelData)
    {
        LevelProgress progress = GetLevelProgress(levelData.levelID);

        levelData.bestTime = progress.bestTime;
        levelData.totalDeaths = progress.totalDeaths;
        levelData.isCompleted = progress.isCompleted;
        levelData.isUnlocked = progress.isUnlocked;
        levelData.stars = progress.stars;
    }

    /// <summary>
    /// Sync data từ LevelData vào SaveSystem
    /// Gọi khi hoàn thành level
    /// </summary>
    public void SyncFromLevelData(LevelData levelData)
    {
        UpdateLevelProgress(
            levelData.levelID,
            levelData.bestTime,
            levelData.totalDeaths,
            levelData.isCompleted,
            levelData.stars
        );
    }

    #endregion

    #region Area Management

    /// <summary>
    /// Sync toàn bộ levels trong một Area
    /// </summary>
    public void SyncAreaToSave(AreaData area)
    {
        foreach (LevelData level in area.Levels)
        {
            SyncToLevelData(level);
        }
    }

    /// <summary>
    /// Unlock level tiếp theo trong area
    /// </summary>
    public void UnlockNextLevel(AreaData area, string currentLevelID)
    {
        int currentIndex = area.Levels.FindIndex(l => l.levelID == currentLevelID);

        if (currentIndex >= 0 && currentIndex < area.Levels.Count - 1)
        {
            string nextLevelID = area.Levels[currentIndex + 1].levelID;
            UnlockLevel(nextLevelID);
        }
    }

    #endregion

    #region Debug Tools

    /// <summary>
    /// Unlock tất cả levels (để test)
    /// </summary>
    public void UnlockAllLevels(List<AreaData> allAreas)
    {
        foreach (AreaData area in allAreas)
        {
            foreach (LevelData level in area.Levels)
            {
                UnlockLevel(level.levelID);
            }
        }
        Debug.Log("[SaveSystem] All levels unlocked!");
    }

    /// <summary>
    /// Reset tất cả progress
    /// </summary>
    public void ResetAllProgress()
    {
        CurrentSaveData = new GameSaveData();
        SaveGame();
        Debug.Log("[SaveSystem] All progress reset!");
    }

    #endregion
}