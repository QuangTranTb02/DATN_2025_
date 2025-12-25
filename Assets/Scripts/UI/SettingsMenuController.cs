using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullScrennToggle;
    [SerializeField] private Button backButton;

    private void Start()
    {
        LoadSettings();

        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void LoadSettings()
    {
        //GameSaveData saveData = SaveSystem.Instance.CurrentSaveData;
        //masterVolumeSlider.value = saveData.settings.masterVolume;
        //musicVolumeSlider.value = saveData.settings.musicVolume;
        //sfxVolumeSlider.value = saveData.settings.sfxVolume;
    }

    private void OnMasterVolumeChanged(float value)
    {
        //AudioManager.Instance.SetMasterVolume(value);
        //SaveSystem.Instance.CurrentSaveData.settings.masterVolume = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        //AudioManager.Instance.SetMusicVolume(value);
        //SaveSystem.Instance.CurrentSaveData.settings.musicVolume = value;
    }

    private void OnSFXVolumeChanged(float value)
    {
        //AudioManager.Instance.SetSFXVolume(value);
        //SaveSystem.Instance.CurrentSaveData.settings.sfxVolume = value;
    }

    private void OnBackClicked()
    {
        //SaveSystem.Instance.SaveGame(); // Save settings
        UIManager.Instance.HideSettings();
    }
}