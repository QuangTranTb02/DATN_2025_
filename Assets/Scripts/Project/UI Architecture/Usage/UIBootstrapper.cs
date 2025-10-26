using Project.Persistence.Settings;
using Project.UIArchitecture.Usage.Services;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Project.UIArchitecture.Usage
{
    public sealed class UIBootstrapper : MonoBehaviour{
        [SerializeField] private PageContainer mainPageContainer;
        [SerializeField] string startUpResourceKey, mainMenuResourceKey, settingsResourceKey;
        private ITransitionService m_transitionService;
        private ISettingsService m_settingsService;
        void Start(){
            m_transitionService = new GeneralTransitionService(mainPageContainer, 
                startUpResourceKey: startUpResourceKey,
                mainMenuResourceKey: mainMenuResourceKey,
                settingsResourceKey: settingsResourceKey,
                startUpPresenterFactory: OnStartUpPageInitialized,
                mainMenuPresenterFactory: OnMainMenuPageInitialized,
                settingsPresenterFactory: OnSettingsPageInitialized
            );
            
            m_settingsService = new SettingsService(new SettingsModel(), new PlayerPrefsSettingsAPI());

            m_transitionService.OnApplicationStarted();
        }

        private void OnSettingsPageInitialized((string pageId, Page page) pageInfo)
        {
            SettingsPresenter presenter = new((SettingsPage)pageInfo.page, m_transitionService, m_settingsService);
            presenter.Initialize();
        }

        private void OnMainMenuPageInitialized((string pageId, Page page) pageInfo)
        {
            MainMenuPresenter presenter = new((MainMenuPage)pageInfo.page, m_transitionService);
            presenter.Initialize();
        }

        private void OnStartUpPageInitialized((string pageId, Page page) pageInfo){
            StartupPresenter presenter = new((StartupPage)pageInfo.page, m_transitionService);
            presenter.Initialize();
        }
    }
}