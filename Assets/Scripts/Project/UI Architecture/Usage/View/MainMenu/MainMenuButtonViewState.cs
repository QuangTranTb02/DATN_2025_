namespace Project.UIArchitecture.Usage{
    public sealed class MainMenuButtonViewState : AppViewState, IMainMenuButtonViewState
    {
        public bool IsLocked;
        public event System.Action ClickEvent;

        protected override void InternalDispose()
        {
            ClickEvent = null;
        }

        void IMainMenuButtonViewState.OnClick(){
            ClickEvent?.Invoke();
        }
    }

    internal interface IMainMenuButtonViewState
    {
        void OnClick();
    }
}