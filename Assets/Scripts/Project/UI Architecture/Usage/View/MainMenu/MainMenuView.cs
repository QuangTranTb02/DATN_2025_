using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UIArchitecture.Usage{
    public sealed class MainMenuView : AppView<MainMenuViewState>
    {
        [SerializeField] MainMenuButtonView continueButtonView;
        [SerializeField] MainMenuButtonView startButtonView;
        [SerializeField] MainMenuButtonView exitButtonView;
        [SerializeField] MainMenuButtonView settingsButtonView;
        [SerializeField] Button backButton;
        protected override IEnumerator InternalInitialize(MainMenuViewState viewState)
        {
            yield return continueButtonView.Initialize(viewState.ContinueButtonState);
            yield return startButtonView.Initialize(viewState.StartButtonState);
            yield return exitButtonView.Initialize(viewState.ExitButtonState);
            yield return settingsButtonView.Initialize(viewState.SettingsButtonState);

            var internalState = (IMainMenuState)viewState;
            if(backButton != null){
                backButton.onClick.AddListener(internalState.InvokeBackButtonClicked);
            }
        }

        void OnDestroy(){
            if(backButton != null){
                backButton.onClick.RemoveAllListeners();
            }
        }
    }
}