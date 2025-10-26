namespace Project.Persistence.Settings{
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

    public readonly struct SettingsLoadRequest{

        public readonly float BGMVolume, SFXVolume;
        public readonly bool MuteBGM, MuteSFX;
        public SettingsLoadRequest(float bgmVolume, float sfxVolume, bool muteBGM, bool muteSFX){
            BGMVolume = bgmVolume;
            SFXVolume = sfxVolume;
            MuteBGM = muteBGM;
            MuteSFX = muteSFX;
        }
    }
}