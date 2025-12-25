// ========================================
// UIManager.cs - Main UI Controller
// ========================================
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Prefabs")]
    public GameObject hudPrefab;
    public GameObject pauseMenuPrefab;
    public GameObject settingsMenuPrefab;
    public GameObject levelCompleteScreenPrefab;

    public SceneField mainMenuScene;
    public SceneField levelSelectScene;

    [Header("Canvas Settings")]
    [SerializeField] private int sortOrderBase = 100;

    // Active UI instances
    private GameObject _currentHUD;
    private GameObject _currentPauseMenu;
    private GameObject _currentSettings;
    private GameObject _currentLevelComplete;

    // UI Stack
    private Stack<GameObject> _uiStack = new Stack<GameObject>();

    // State
    public bool IsPaused { get; private set; }
    public bool IsAnyMenuOpen => _uiStack.Count > 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    #region Input Handling

    private void HandleInput()
    {
        // ESC để pause/unpause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (GameSceneManager.Instance.IsInGameplay())
            {
                if (IsPaused)
                {
                    // Nếu đang mở Settings, đóng Settings trước
                    if (_currentSettings != null)
                    {
                        HideSettings();
                    }
                    else
                    {
                        ResumeGame();
                    }
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    #endregion

    #region HUD Management

    public void ShowHUD()
    {
        if (_currentHUD != null) return;

        if (hudPrefab != null)
        {
            _currentHUD = Instantiate(hudPrefab);
            Canvas canvas = _currentHUD.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = sortOrderBase;
            }
            Debug.Log("[UIManager] HUD shown");
        }
    }

    public void HideHUD()
    {
        if (_currentHUD != null)
        {
            Destroy(_currentHUD);
            _currentHUD = null;
            Debug.Log("[UIManager] HUD hidden");
        }
    }

    public HUDController GetHUD()
    {
        if (_currentHUD != null)
        {
            return _currentHUD.GetComponent<HUDController>();
        }
        return null;
    }

    #endregion

    #region Pause Menu

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPrefab != null)
        {
            _currentPauseMenu = Instantiate(pauseMenuPrefab);
            PushToStack(_currentPauseMenu);

            Canvas canvas = _currentPauseMenu.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = sortOrderBase + _uiStack.Count * 10;
            }

            // Play pause sound
            AudioManager.Instance?.PlaySFX("Pause");

            Debug.Log("[UIManager] Game paused");
        }
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        if (_currentPauseMenu != null)
        {
            PopFromStack();
            Destroy(_currentPauseMenu);
            _currentPauseMenu = null;

            // Play unpause sound
            AudioManager.Instance?.PlaySFX("Unpause");

            Debug.Log("[UIManager] Game resumed");
        }
    }

    #endregion

    #region Settings Menu

    public void ShowSettings()
    {
        if (_currentSettings != null) return;

        if (settingsMenuPrefab != null)
        {
            _currentSettings = Instantiate(settingsMenuPrefab);
            PushToStack(_currentSettings);

            Canvas canvas = _currentSettings.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = sortOrderBase + _uiStack.Count * 10;
            }

            Debug.Log("[UIManager] Settings shown");
        }
    }

    public void HideSettings()
    {
        if (_currentSettings != null)
        {
            PopFromStack();
            Destroy(_currentSettings);
            _currentSettings = null;

            Debug.Log("[UIManager] Settings hidden");
        }
    }

    #endregion

    #region Level Complete Screen

    public void ShowLevelComplete(LevelData levelData)
    {
        if (_currentLevelComplete != null) return;

        if (levelCompleteScreenPrefab != null)
        {
            _currentLevelComplete = Instantiate(levelCompleteScreenPrefab);
            PushToStack(_currentLevelComplete);

            Canvas canvas = _currentLevelComplete.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = sortOrderBase + _uiStack.Count * 10;
            }

            // Initialize
            LevelCompleteController completeScreen = _currentLevelComplete.GetComponent<LevelCompleteController>();
            if (completeScreen != null)
            {
                completeScreen.Initialize(levelData);
            }

            // Play complete sound
            AudioManager.Instance?.PlaySFX("LevelComplete");

            Debug.Log("[UIManager] Level complete screen shown");
        }
    }

    public void HideLevelComplete()
    {
        if (_currentLevelComplete != null)
        {
            PopFromStack();
            Destroy(_currentLevelComplete);
            _currentLevelComplete = null;

            Debug.Log("[UIManager] Level complete screen hidden");
        }
    }

    #endregion

    #region UI Stack Management

    private void PushToStack(GameObject ui)
    {
        _uiStack.Push(ui);
    }

    private void PopFromStack()
    {
        if (_uiStack.Count > 0)
        {
            _uiStack.Pop();
        }
    }

    public void CloseAllUI()
    {
        while (_uiStack.Count > 0)
        {
            GameObject ui = _uiStack.Pop();
            Destroy(ui);
        }

        _currentPauseMenu = null;
        _currentSettings = null;
        _currentLevelComplete = null;
    }

    #endregion

    #region Scene Transitions

    public void OnEnterGameplay()
    {
        CloseAllUI();
        ShowHUD();
    }

    public void OnExitGameplay()
    {
        HideHUD();
        CloseAllUI();

        Time.timeScale = 1f;
        IsPaused = false;
    }

    #endregion
}