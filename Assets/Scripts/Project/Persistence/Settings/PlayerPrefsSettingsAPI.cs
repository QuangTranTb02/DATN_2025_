using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Persistence.Settings{
    public sealed class PlayerPrefsSettingsAPI : ISettingsPersistentAPI
    {

        private const string BgmVolumePrefsKey = "BGM_Volume";
        private const string SeVolumePrefsKey = "SFX_Volume";
        private const string IsBgmMutedPrefsKey = "BGM_Muted";
        private const string IsSeMutedPrefsKey = "SFX_Muted";

        private static float BgmVolume
        {
            get => PlayerPrefs.GetFloat(BgmVolumePrefsKey, 1.0f);
            set => PlayerPrefs.SetFloat(BgmVolumePrefsKey, value);
        }

        private static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(SeVolumePrefsKey, 1.0f);
            set => PlayerPrefs.SetFloat(SeVolumePrefsKey, value);
        }

        private static bool BgmMuted{
            get => PlayerPrefs.GetInt(IsBgmMutedPrefsKey, 0) == 1;
            set => PlayerPrefs.SetInt(IsBgmMutedPrefsKey, value ? 1 : 0); 
        }

        private static bool SfxMuted
        {
            get => PlayerPrefs.GetInt(IsSeMutedPrefsKey, 0) == 1;
            set => PlayerPrefs.SetInt(IsSeMutedPrefsKey, value ? 1 : 0);
        }

        public Task<SettingsLoadRequest> LoadSettingsAsync()
        {
            return Task.FromResult(new SettingsLoadRequest(
                BgmVolume, SfxVolume, BgmMuted, SfxMuted
            ));
        }

        public IEnumerator LoadSettingsCoroutine(Action<SettingsLoadRequest> callback)
        {
            yield return null;
            callback?.Invoke(new SettingsLoadRequest(
                BgmVolume, SfxVolume, BgmMuted, SfxMuted
            ));
        }

        public Task SaveSettingsAsync(in SettingsSaveRequest request)
        {
            BgmVolume = request.BGMVolume;
            SfxVolume = request.SFXVolume;
            BgmMuted = request.MuteBGM;
            SfxMuted = request.MuteSFX;
            return Task.CompletedTask;
        }

        public IEnumerator SaveSettingsCoroutine(SettingsSaveRequest request)
        {
            yield return null;

            BgmVolume = request.BGMVolume;
            SfxVolume = request.SFXVolume;
            BgmMuted = request.MuteBGM;
            SfxMuted = request.MuteSFX;
        }
    }
}