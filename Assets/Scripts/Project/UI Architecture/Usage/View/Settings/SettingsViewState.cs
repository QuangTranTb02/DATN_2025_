namespace Project.UIArchitecture.Usage
{
    public sealed class SettingsViewState : AppViewState, ISettingsViewState
    {
        public event System.Action CloseEvent;
        public readonly SoundSettingsViewState SoundSettingsState = new();
        protected override void InternalDispose()
        {
            SoundSettingsState.Dispose();
            CloseEvent = null;
        }

        void ISettingsViewState.InvokeCloseButtonClicked()
        {
            CloseEvent?.Invoke();
        }
    }

    internal interface ISettingsViewState{
        void InvokeCloseButtonClicked();
    }
}