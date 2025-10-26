using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UIArchitecture.Usage{
    public sealed class SettingsView : AppView<SettingsViewState>
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private SoundSettingsView _soundSettingsView;
        protected override IEnumerator InternalInitialize(SettingsViewState viewState)
        {
            ISettingsViewState internalState = viewState;
            if(_backButton != null){
                _backButton.onClick.AddListener(internalState.InvokeCloseButtonClicked);
            }
            yield return _soundSettingsView.Initialize(viewState.SoundSettingsState);
        }

        void OnDestroy(){
            if(_backButton != null){
                _backButton.onClick.RemoveAllListeners();
            }
        }
    }
}