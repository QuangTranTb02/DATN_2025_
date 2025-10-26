namespace Project.UIArchitecture.Usage{
    public sealed class StartupViewState : AppViewState, IStartupViewState
    {
        public event System.Action ClickEvent;

        void  IStartupViewState.OnClicked()
        {
            ClickEvent?.Invoke();
        }

        protected override void InternalDispose()
        {
            ClickEvent = null;
        }
    }

    internal interface IStartupViewState
    {
        void OnClicked();
    }
}