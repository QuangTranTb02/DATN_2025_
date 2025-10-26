using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UIArchitecture.Usage{
    public sealed class SoundSettingsView : AppView<SoundSettingsViewState>
    {
        [SerializeField] Slider bgmSlider, sfxSlider;
        [SerializeField] Toggle bgmToggle, sfxToggle;
        private SoundSettingsViewState m_viewState;
        protected override IEnumerator InternalInitialize(SoundSettingsViewState viewState)
        {
            m_viewState = viewState;
            bgmSlider.value = m_viewState.BGMVolume;
            sfxSlider.value = m_viewState.SFXVolume;
            bgmSlider.interactable = m_viewState.IsMutedBGM == false;
            sfxSlider.interactable = m_viewState.IsMutedSFX == false;
            bgmToggle.isOn = m_viewState.IsMutedBGM == false;
            sfxToggle.isOn = m_viewState.IsMutedSFX == false;

            bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
            sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
            bgmToggle.onValueChanged.AddListener(ChangeBGMEnabled);
            sfxToggle.onValueChanged.AddListener(ChangeSFXEnabled);
            yield return null;
        }

        private void ChangeSFXEnabled(bool enabled)
        {
            sfxSlider.interactable = enabled;
            m_viewState.IsMutedSFX = !enabled;
        }

        private void ChangeBGMEnabled(bool enabled)
        {
            bgmSlider.interactable = enabled;
            m_viewState.IsMutedBGM = !enabled;
        }

        private void ChangeSFXVolume(float value) => m_viewState.SFXVolume = value;

        private void ChangeBGMVolume(float value) => m_viewState.BGMVolume = value;
    }
}