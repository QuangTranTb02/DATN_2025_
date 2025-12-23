using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Đặt script này vào scene ĐẦU TIÊN (PersistentScene)
/// Đảm bảo PersistentScene được load và chuyển đến MainMenu
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private bool loadMainMenuOnStart = true;

    private void Start()
    {
        // Initialize managers
        InitializeManagers();

        // Load main menu
        if (loadMainMenuOnStart)
        {
            //LevelManager.Instance.LoadLevel(SceneNames.MainMenu);
        }
    }

    private void InitializeManagers()
    {
        // Tất cả managers đã có Singleton pattern và DontDestroyOnLoad
        // Chỉ cần access để trigger Awake()

        _ = GameManager.Instance;
        _ = LevelManager.Instance;
        _ = UiManager.Instance;
        _ = SaveSystem.Instance;
        _ = AudioManager.Instance;

        Debug.Log("[Bootstrap] All managers initialized");
    }
}