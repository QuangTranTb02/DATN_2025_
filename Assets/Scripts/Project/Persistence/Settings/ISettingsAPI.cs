namespace Project.Persistence.Settings{
    public interface ISettingsPersistentAPI : IPersistentAPI{
        System.Threading.Tasks.Task SaveSettingsAsync(in SettingsSaveRequest request);
        System.Threading.Tasks.Task<SettingsLoadRequest> LoadSettingsAsync();
        System.Collections.IEnumerator SaveSettingsCoroutine(SettingsSaveRequest request);
        System.Collections.IEnumerator LoadSettingsCoroutine(System.Action<SettingsLoadRequest> callback);
    }
}