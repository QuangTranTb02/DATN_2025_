using System;
using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Rendering.FilterWindow;

public class LevelManager : Singleton<LevelManager>
{

    [Header("Level Data")]
    [SerializeField] private LevelData[] _allLevels;

    private LevelData _currentLevel;
    private int _currentLevelIndex = -1;

    // Tracking trong level hiện tại
    private float _levelStartTime;
    private int _deathsThisLevel = 0;
    private int _collectiblesThisLevel = 0;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLogs = true;

    // Events (cho UI và systems khác)
    public System.Action<LevelData> OnLevelLoaded;
    public System.Action<LevelData> OnLevelCompleted;
    public System.Action OnPlayerDied;

    private void Awake()
    {
       
    }

    private void Start()
    {
        // Load progress từ save file
        LoadAllProgress();

        // Unlock level đầu tiên
        if (_allLevels.Length > 0)
        {
            _allLevels[0].isUnlocked = true;
            SaveProgress(_allLevels[0]);
        }
    }

    // ===== LOAD LEVEL =====

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= _allLevels.Length)
        {
            LogError($"Level index {levelIndex} out of range!");
            return;
        }

        LevelData level = _allLevels[levelIndex];

        if (!level.isUnlocked)
        {
            LogWarning($"Level {level.levelName} is locked!");
            return;
        }

        _currentLevelIndex = levelIndex;
        _currentLevel = level;

        Log($"Loading level: {level.levelName}");

        StartCoroutine(LoadLevelAsync(level.Scene));
    }
    public void LoadNextLevel()
    {
        int nextIndex = _currentLevelIndex + 1;

        if (nextIndex < _allLevels.Length)
        {
            LoadLevel(nextIndex);
        }
        else
        {
            Log("All levels completed!");
            // TODO: Load credits scene hoặc main menu
        }
    }

    public void ReloadCurrentLevel()
    {
        if (_currentLevel != null)
        {
            LoadLevel(_currentLevelIndex);
        }
        else
        {
            LogError("No current level to reload!");
        }
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
        Log($"Starting async load of scene: {sceneName}");

        // ===== FADE OUT =====
        if (SceneTransition.Instance != null)
        {
            yield return SceneTransition.Instance.FadeOut();
        }

        // ===== LOAD SCENE =====
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Đợi loading xong
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // TODO: Update loading bar nếu có
            yield return null;
        }

        // Scene loaded, đợi 1 frame để scene setup
        yield return new WaitForSeconds(0.1f);

        // ===== INITIALIZE LEVEL =====
        InitializeLevel();

        // Đợi thêm 0.2s cho player spawn ổn định
        yield return new WaitForSeconds(0.2f);

        // ===== FADE IN =====
        if (SceneTransition.Instance != null)
        {
            yield return SceneTransition.Instance.FadeIn();
        }

        Log("Scene transition complete!");
    }

    private void InitializeLevel()
    {
        if (_currentLevel == null)
        {
            LogError("Cannot initialize level: _currentLevel is null!");
            return;
        }

        Log($"Initializing level: {_currentLevel.levelName}");

        // Reset stats
        _levelStartTime = Time.time;
        _deathsThisLevel = 0;
        _collectiblesThisLevel = 0;

        // Spawn player ở vị trí đúng
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = _currentLevel.playerSpawnPosition;

            // Reset player state
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.Velocity = Vector2.zero;
            }

            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                // Reset health nếu cần
            }
        }
        else
        {
            LogWarning("Player not found in scene!");
        }

        // Set checkpoint
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(_currentLevel.playerSpawnPosition);
        }

        // Invoke event
        OnLevelLoaded?.Invoke(_currentLevel);

        Log($"Level {_currentLevel.levelName} ready to play!");
    }

    // ===== LEVEL COMPLETION =====

    public void CompleteLevel()
    {
        if (_currentLevel == null)
        {
            LogError("Cannot complete level: _currentLevel is null!");
            return;
        }

        Log($"Level {_currentLevel.levelName} completed!");

        // Tính stats
        float completionTime = Time.time - _levelStartTime;

        // Update best time
        if (_currentLevel.bestTime == 0f || completionTime < _currentLevel.bestTime)
        {
            _currentLevel.bestTime = completionTime;
            Log($"New best time: {completionTime:F2}s");
        }

        // Update total deaths
        _currentLevel.totalDeaths += _deathsThisLevel;

        // Calculate stars
        int stars = CalculateStars(completionTime, _deathsThisLevel);
        if (stars > _currentLevel.stars)
        {
            _currentLevel.stars = stars;
            Log($"New star rating: {stars}/3");
        }

        // Mark completed
        _currentLevel.isCompleted = true;

        // Unlock next level
        UnlockNextLevel();

        // Save progress
        SaveProgress(_currentLevel);

        // Invoke event
        OnLevelCompleted?.Invoke(_currentLevel);
    }

    private int CalculateStars(float time, int deaths)
    {
        int stars = 3;

        // Mất sao nếu chết nhiều
        if (deaths > 3) stars = 2;
        if (deaths > 7) stars = 1;

        // TODO: Có thể thêm logic dựa trên time

        return Mathf.Max(1, stars);
    }

    private void UnlockNextLevel()
    {
        int nextIndex = _currentLevelIndex + 1;

        if (nextIndex < _allLevels.Length)
        {
            if (!_allLevels[nextIndex].isUnlocked)
            {
                _allLevels[nextIndex].isUnlocked = true;
                SaveProgress(_allLevels[nextIndex]);
                Log($"Unlocked: {_allLevels[nextIndex].levelName}");
            }
        }
    }

    // ===== TRACKING =====

    public void RegisterPlayerDeath()
    {
        _deathsThisLevel++;
        Log($"Deaths this level: {_deathsThisLevel}");

        OnPlayerDied?.Invoke();
    }

    public void RegisterCollectible()
    {
        _collectiblesThisLevel++;
        Log($"Collectibles: {_collectiblesThisLevel}/{_currentLevel.totalCollectibles}");
    }

    public bool HasCollectedAll()
    {
        return _collectiblesThisLevel >= _currentLevel.totalCollectibles;
    }

    // ===== SAVE/LOAD IN GAME =====

    private void SaveProgress(LevelData level)
    {
        string key = $"Level_{level.levelNumber}";

        PlayerPrefs.SetFloat($"{key}_BestTime", level.bestTime);
        PlayerPrefs.SetInt($"{key}_Deaths", level.totalDeaths);
        PlayerPrefs.SetInt($"{key}_Completed", level.isCompleted ? 1 : 0);
        PlayerPrefs.SetInt($"{key}_Unlocked", level.isUnlocked ? 1 : 0);
        PlayerPrefs.SetInt($"{key}_Stars", level.stars);

        PlayerPrefs.Save();

        Log($"Saved progress for {level.levelName}");   
    }

    private void LoadProgress(LevelData level)
    {
        string key = $"Level_{level.levelNumber}";

        level.bestTime = PlayerPrefs.GetFloat($"{key}_BestTime", 0f);
        level.totalDeaths = PlayerPrefs.GetInt($"{key}_Deaths", 0);
        level.isCompleted = PlayerPrefs.GetInt($"{key}_Completed", 0) == 1;
        level.isUnlocked = PlayerPrefs.GetInt($"{key}_Unlocked", level.levelNumber == 1 ? 1 : 0) == 1;
        level.stars = PlayerPrefs.GetInt($"{key}_Stars", 0);
    }

    private void LoadAllProgress()
    {
        foreach (LevelData level in _allLevels)
        {
            LoadProgress(level);
        }

        Log("Loaded all progress");
    }

    public void ResetAllProgress()
    {
        foreach (LevelData level in _allLevels)
        {
            level.ResetProgress();
            SaveProgress(level);
        }

        // Unlock level đầu
        if (_allLevels.Length > 0)
        {
            _allLevels[0].isUnlocked = true;
            SaveProgress(_allLevels[0]);
        }

        Log("Reset all progress");
    }

    // ===== GETTERS =====

    public LevelData GetCurrentLevel() => _currentLevel;
    public LevelData[] GetAllLevels() => _allLevels;
    public int GetCurrentLevelIndex() => _currentLevelIndex;

    public int SetCurrentLevelIndex(int index)
    {
        if (index < 0 || index >= _allLevels.Length)
        {
            LogError($"Level index {index} out of range!");
            return -1;
        }
        _currentLevelIndex = index;
        _currentLevel = _allLevels[index];
        return _currentLevelIndex;
    }
        
    public int GetTotalStars()
    {
        int total = 0;
        foreach (LevelData level in _allLevels)
        {
            total += level.stars;
        }
        return total;
    }

    public int GetCompletedLevels()
    {
        int count = 0;
        foreach (LevelData level in _allLevels)
        {
            if (level.isCompleted) count++;
        }
        return count;
    }

    public float GetLevelTime()
    {
        return Time.time - _levelStartTime;
    }

    public int GetDeathsThisLevel()
    {
        return _deathsThisLevel;
    }

    public int GetCollectiblesThisLevel()
    {
        return _collectiblesThisLevel;
    }

    // ===== DEBUG =====

    private void Log(string message)
    {
        if (_showDebugLogs)
            UnityEngine.Debug.Log($"[LevelManager] {message}");
    }

    private void LogWarning(string message)
    {
        if (_showDebugLogs)
            UnityEngine.Debug.LogWarning($"[LevelManager] {message}");
    }

    private void LogError(string message)
    {
        UnityEngine.Debug.LogError($"[LevelManager] {message}");
    }
    
}
