using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller cho Main Menu Scene
/// Buttons: Play, Continue, Option, Quit
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        SetupButtons();
        CheckContinueButton();
        PlayBackgroundMusic();
    }

    private void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (optionButton != null)
            optionButton.onClick.AddListener(OnOptionClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void CheckContinueButton()
    {
        // Enable continue button nếu có save data
        if (continueButton != null && SaveSystem.Instance != null)
        {
            //bool hasSaveData = SaveSystem.Instance.CurrentSaveData.levelProgressList.Count > 0;
            //continueButton.interactable = hasSaveData;
        }
    }

    private void PlayBackgroundMusic()
    {
        AudioManager.Instance?.PlayMusic("MainMenu");
    }

    #region Button Callbacks

    private void OnPlayClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        // New game - có thể reset progress hoặc load từ đầu
        // Option 1: Reset tất cả
        // SaveSystem.Instance?.ResetAllProgress();

        // Option 2: Unlock level đầu tiên
        // SaveSystem.Instance?.UnlockLevel("level_1_1");

        // Load hub/level select
        GameSceneManager.Instance.LoadHub();
    }

    private void OnContinueClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        // Continue from last save
        GameSceneManager.Instance.LoadHub();
    }

    private void OnOptionClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");

        // Show settings menu
        UIManager.Instance.ShowSettings();
    }

    private void OnQuitClicked()
    {
        AudioManager.Instance?.PlaySFX("ButtonClick");
        Debug.Log("[MainMenu] Quit button clicked.");
        // Quit game
        Application.Quit();
    }

    #endregion

    private void OnDestroy()
    {
        // Cleanup
        if (playButton != null)
            playButton.onClick.RemoveAllListeners();

        if (continueButton != null)
            continueButton.onClick.RemoveAllListeners();

        if (optionButton != null)
            optionButton.onClick.RemoveAllListeners();

        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }
}