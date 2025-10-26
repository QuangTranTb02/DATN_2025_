using System;
using System.Collections;

namespace Project.UIArchitecture.Usage
{
    public sealed class MainMenuPresenter : AppPagePresenter<MainMenuPage, MainMenuView, MainMenuViewState>
    {
        private readonly ITransitionService m_service;
        public MainMenuPresenter(MainMenuPage view, ITransitionService transitionService) : base(view)
        {
            m_service = transitionService;
            // Khúc này có thể gắn repository để load features
            // ví dụ nếu feature mua skin chưa có sẵn thì SetActive(false) cái nút mua skin
            // Khá đỉnh :)))
        }

        protected override IEnumerator ViewDidLoad(MainMenuPage view, MainMenuViewState state)
        {
            yield return null;
            // khóa feature nếu chưa có, rồi qua bên view check nếu khóa thì SetActive(false)
            state.ContinueButtonState.IsLocked = true;
            state.SettingsButtonState.ClickEvent += OnSettingsButtonClicked;
            state.BackButtonClickedEvent += m_service.ExecutePopCommand;
            state.StartButtonState.ClickEvent += OnStartGameClicked;
        }

        private void OnStartGameClicked()
        {
            UnityEngine.Debug.Log("Start clicked");
        }

        protected override void ViewDidPopExit(MainMenuPage view, MainMenuViewState state)
        {
            state.SettingsButtonState.ClickEvent -= OnSettingsButtonClicked;
            state.BackButtonClickedEvent -= m_service.ExecutePopCommand;
        }

        private void OnSettingsButtonClicked()
        {
            m_service.SettingsShown();
        }
    }
}