namespace Project.UIArchitecture.Usage
{
    public sealed class SoundSettingsViewState : AppViewState
    {
        public float BGMVolume;
        public float SFXVolume;
        public bool IsMutedBGM;
        public bool IsMutedSFX;

        protected override void InternalDispose(){}
    }
}