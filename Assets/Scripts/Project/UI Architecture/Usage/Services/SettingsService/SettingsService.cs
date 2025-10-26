using System;
using System.Collections;
using Project.Persistence.Settings;

namespace Project.UIArchitecture.Usage.Services{
    public sealed class SettingsService : ISettingsService
    {
        private readonly ISettingsPersistentAPI m_settingsPersistentAPI;
        public SettingsModel Model {get;}
        public SettingsService(SettingsModel model, ISettingsPersistentAPI settingsPersistentAPI){
            m_settingsPersistentAPI = settingsPersistentAPI;
            Model = model;
        }
        public IEnumerator FetchSettingsData()
        {
            yield return m_settingsPersistentAPI.LoadSettingsCoroutine(OnFetchedSettingsData);
        }

        private void OnFetchedSettingsData(SettingsLoadRequest request)
        {
            Model.SetBGMValues(request.BGMVolume, request.MuteBGM);
            Model.SetSFXValues(request.SFXVolume, request.MuteSFX);
        }

        public IEnumerator SaveSettingsData(in SettingsSaveRequest request)
        {
            Model.SetBGMValues(request.BGMVolume, request.MuteBGM);
            Model.SetSFXValues(request.SFXVolume, request.MuteSFX);
            return m_settingsPersistentAPI.SaveSettingsCoroutine(
                new Persistence.Settings.SettingsSaveRequest(
                    request.BGMVolume, request.SFXVolume, request.MuteBGM, request.MuteSFX
                )
            );
        }
    }
}