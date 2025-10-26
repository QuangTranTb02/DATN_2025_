namespace Project.UIArchitecture.Usage{

    /// <summary>
    /// Defines how app transitions between pages
    /// </summary>
    public interface ITransitionService{
        void OnApplicationStarted();
        void MainMenuShown();
        void SettingsShown();
        void ExecutePopCommand();
    }
}