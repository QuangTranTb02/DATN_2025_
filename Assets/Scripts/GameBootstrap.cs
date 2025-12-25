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
        Debug.LogWarning("[Bootstrap] Game Bootstrap completed.");
        // Load main menu
        if (loadMainMenuOnStart)
        {
            GameSceneManager.Instance.LoadMenuScene();
        }
    }

    private void InitializeManagers()
    {
        // Tất cả managers đã có Singleton pattern và DontDestroyOnLoad
        // Chỉ cần access để trigger Awake()

        _ = GameManager.Instance;
        _ = GameSceneManager.Instance;
        _ = UIManager.Instance;
        _ = SaveSystem.Instance;
        _ = AudioManager.Instance;

        Debug.Log("[Bootstrap] All managers initialized");
    }
}