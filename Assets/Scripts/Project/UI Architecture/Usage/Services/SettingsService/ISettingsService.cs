using System.Collections;

namespace Project.UIArchitecture.Usage.Services{

    public interface ISettingsService{
        SettingsModel Model { get; }
        IEnumerator FetchSettingsData();
        IEnumerator SaveSettingsData(in SettingsSaveRequest request);
    }

    internal sealed class NullSettingsService : ISettingsService
    {
        public SettingsModel Model {get;}
        private NullSettingsService() {
            Model = new SettingsModel();
        }
        public static readonly NullSettingsService NullService = new();

        public IEnumerator FetchSettingsData()
        {
            return null;
        }

        public IEnumerator SaveSettingsData(in SettingsSaveRequest request){
            return null;
        }
    }

    public readonly struct SettingsSaveRequest{
        public readonly float BGMVolume, SFXVolume;
        public readonly bool MuteBGM, MuteSFX;

        public SettingsSaveRequest(float bgmVolume, float sfxVolume, bool muteBGM, bool muteSFX){
            BGMVolume = bgmVolume;
            SFXVolume = sfxVolume;
            MuteBGM = muteBGM;
            MuteSFX = muteSFX;
        }
    }

    public sealed class SettingsModel{
        public float BGMVolume, SFXVolume;
        public bool MuteBGM, MuteSFX;

        public void SetBGMValues(float bGMVolume, bool muteBGM)
        {
            BGMVolume = bGMVolume;
            MuteBGM = muteBGM;
        }

        public void SetSFXValues(float sFXVolume, bool muteSFX)
        {
            SFXVolume = sFXVolume;
            MuteSFX = muteSFX;
        }
    }
}