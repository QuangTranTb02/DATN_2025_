using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AudioManager - Quản lý music và sound effects
/// Singleton, DontDestroyOnLoad
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip hubMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private AudioClip bossMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip landSFX;
    [SerializeField] private AudioClip coinCollectSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip checkpointSFX;
    [SerializeField] private AudioClip levelCompleteSFX;
    [SerializeField] private AudioClip starCollectSFX;
    [SerializeField] private AudioClip pauseSFX;
    [SerializeField] private AudioClip unpauseSFX;

    // SFX Dictionary để dễ access
    private Dictionary<string, AudioClip> sfxDictionary;

    // Volume settings
    private float masterVolume = 1f;
    private float musicVolume = 0.5f;
    private float sfxVolume = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // Create audio sources if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // Build SFX dictionary
        BuildSFXDictionary();

        // Load volume settings from save
        LoadVolumeSettings();
    }

    private void BuildSFXDictionary()
    {
        sfxDictionary = new Dictionary<string, AudioClip>
        {
            { "ButtonClick", buttonClickSFX },
            { "Jump", jumpSFX },
            { "Land", landSFX },
            { "CoinCollect", coinCollectSFX },
            { "Death", deathSFX },
            { "Checkpoint", checkpointSFX },
            { "LevelComplete", levelCompleteSFX },
            { "StarCollect", starCollectSFX },
            { "Pause", pauseSFX },
            { "Unpause", unpauseSFX }
        };
    }

    #region Music Control

    /// <summary>
    /// Play background music
    /// </summary>
    public void PlayMusic(string musicName)
    {
        AudioClip clip = null;

        switch (musicName.ToLower())
        {
            case "mainmenu":
                clip = mainMenuMusic;
                break;
            case "hub":
                clip = hubMusic;
                break;
            case "level":
                clip = levelMusic;
                break;
            case "boss":
                clip = bossMusic;
                break;
        }

        if (clip != null && musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stop music
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    /// <summary>
    /// Pause music
    /// </summary>
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    /// <summary>
    /// Resume music
    /// </summary>
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    /// <summary>
    /// Fade out music
    /// </summary>
    public void FadeOutMusic(float duration = 1f)
    {
        LeanTween.value(gameObject, musicSource.volume, 0f, duration)
            .setOnUpdate((float val) => {
                musicSource.volume = val;
            })
            .setOnComplete(() => {
                musicSource.Stop();
                musicSource.volume = musicVolume * masterVolume;
            });
    }

    /// <summary>
    /// Fade in music
    /// </summary>
    public void FadeInMusic(float duration = 1f)
    {
        musicSource.volume = 0f;
        musicSource.Play();

        LeanTween.value(gameObject, 0f, musicVolume * masterVolume, duration)
            .setOnUpdate((float val) => {
                musicSource.volume = val;
            });
    }

    #endregion

    #region SFX Control

    /// <summary>
    /// Play sound effect by name
    /// </summary>
    public void PlaySFX(string sfxName)
    {
        if (sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX '{sfxName}' not found!");
        }
    }

    /// <summary>
    /// Play custom audio clip
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
    }

    /// <summary>
    /// Play SFX with custom volume
    /// </summary>
    public void PlaySFX(string sfxName, float volumeMultiplier)
    {
        if (sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeMultiplier);
            }
        }
    }

    #endregion

    #region Volume Control

    /// <summary>
    /// Set master volume (0-1)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
    }

    /// <summary>
    /// Set music volume (0-1)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
    }

    /// <summary>
    /// Set SFX volume (0-1)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }

        // SFX volume is applied per-clip in PlayOneShot
    }

    private void LoadVolumeSettings()
    {
        if (SaveSystem.Instance != null)
        {
            //GameSaveData saveData = SaveSystem.Instance.CurrentSaveData;
            //masterVolume = saveData.settings.masterVolume;
            //musicVolume = saveData.settings.musicVolume;
            //sfxVolume = saveData.settings.sfxVolume;

            ApplyVolumeSettings();
        }
    }

    #endregion

    #region Getters

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    #endregion
}