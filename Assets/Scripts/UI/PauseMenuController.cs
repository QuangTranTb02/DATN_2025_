using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button mainMenuButton;

    private void Awake()
    {
        // Setup button listeners
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnResumeClicked()
    {
        UIManager.Instance.ResumeGame();
    }

    private void OnRestartClicked()
    {
        UIManager.Instance.ResumeGame(); // Unpause trước
        GameSceneManager.Instance.RestartCurrentLevel();
    }

    private void OnSettingsClicked()
    {
        //UIManager.Instance.ShowSettings();
    }

    private void OnMainMenuClicked()
    {
        // Show confirmation popup
        //UIManager.Instance.ShowConfirmation(
        //    "Return to Main Menu?",
        //    onConfirm: () =>
        //    {
        //        UIManager.Instance.ResumeGame();
        //        //GameSceneManager.Instance.LoadMainMenu();
        //    }
        //);
    }

    // Optional: Animation on open
    private void OnEnable()
    {
        // Fade in, scale up, etc.
        transform.localScale = Vector3.zero;
        //LeanTween.scale(gameObject, Vector3.one, 0.3f).setEaseOutBack().setIgnoreTimeScale(true);
    }
}