namespace Project.UIArchitecture.Usage{
    public sealed class MainMenuViewState : AppViewState, IMainMenuState
    {
        public readonly MainMenuButtonViewState ContinueButtonState = new();
        public readonly MainMenuButtonViewState StartButtonState = new();
        public readonly MainMenuButtonViewState ExitButtonState = new();
        public readonly MainMenuButtonViewState SettingsButtonState = new();
        public event System.Action BackButtonClickedEvent;

        void IMainMenuState.InvokeBackButtonClicked()
        {
            BackButtonClickedEvent?.Invoke();
        }

        protected override void InternalDispose()
        {
            ContinueButtonState.Dispose();
            StartButtonState.Dispose();
            ExitButtonState.Dispose();
            SettingsButtonState.Dispose();
            BackButtonClickedEvent = null;
        }
    }

    internal interface IMainMenuState
    {
        void InvokeBackButtonClicked();
    }
}