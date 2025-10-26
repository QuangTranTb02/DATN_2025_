using System.Collections;
using Project.UIArchitecture.Usage.Services;

namespace Project.UIArchitecture.Usage{
    public sealed class SettingsPresenter : AppPagePresenter<SettingsPage, SettingsView, SettingsViewState>
    {
        private readonly ITransitionService m_service;
        private readonly ISettingsService m_settingsService;
        public SettingsPresenter(SettingsPage view, ITransitionService transitionService, ISettingsService settingsService) : base(view)
        {
            m_service = transitionService;
            m_settingsService = settingsService;
        }

        protected override IEnumerator ViewDidLoad(SettingsPage view, SettingsViewState state)
        {
            yield return m_settingsService.FetchSettingsData();
            SettingsModel model = m_settingsService.Model;
            
            // fetch sound settings
            var soundState = state.SoundSettingsState;
            soundState.BGMVolume = model.BGMVolume;
            soundState.SFXVolume = model.SFXVolume;
            soundState.IsMutedBGM = model.MuteBGM;
            soundState.IsMutedSFX = model.MuteSFX;
            
            state.CloseEvent += m_service.ExecutePopCommand;
        }

        protected override IEnumerator ViewWillPopExit(SettingsPage view, SettingsViewState state)
        {
            yield return OnViewExit(view, state);
        }

        protected override IEnumerator ViewWillPushExit(SettingsPage view, SettingsViewState state)
        {
            yield return OnViewExit(view, state);
        }

        protected override IEnumerator ViewWillDestroy(SettingsPage view, SettingsViewState state)
        {
            state.CloseEvent -= m_service.ExecutePopCommand;
            yield return OnViewExit(view, state);
        }

        private IEnumerator OnViewExit(SettingsPage _, SettingsViewState state)
        {
            if(IsDirty(state, m_settingsService.Model)){
                var soundState = state.SoundSettingsState;
                yield return m_settingsService.SaveSettingsData(new SettingsSaveRequest(
                    soundState.BGMVolume, 
                    soundState.SFXVolume, 
                    soundState.IsMutedBGM, 
                    soundState.IsMutedSFX
                ));
            }
        }

        private bool IsDirty(SettingsViewState state, SettingsModel model){
            var soundState = state.SoundSettingsState;
            if(soundState.IsMutedBGM != model.MuteBGM || soundState.IsMutedSFX != model.MuteSFX){
                return true;
            }

            if(soundState.BGMVolume != model.BGMVolume || soundState.SFXVolume != model.SFXVolume){
                return true;
            }

            return false;
        }
    }
}