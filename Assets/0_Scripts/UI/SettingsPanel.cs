using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {
    public AudioMixerGroup MusicAudioGroup, SoundAudioGroup;
    public Slider MusicSlider, SoundSlider;
    bool saveRequired = false;
    public void ChangeMusicVolume(float value) {
        UpdateMixerVolume(MusicAudioGroup.audioMixer, "MusicVolume", value);
        UserProgressController.Instance.PlayerSettings.MusicVolume = value;
    }
    public void ChangeSoundVolume(float value) {
        UpdateMixerVolume(MusicAudioGroup.audioMixer, "SoundVolume", value);
        UserProgressController.Instance.PlayerSettings.SoundVolume = value;
    }
    void UpdateMixerVolume(AudioMixer mixer, string paramName, float value) {
        saveRequired = true;
        mixer.SetFloat(paramName, Mathf.Log10(value) * 20);
    }
    public void SaveSettings() {
        if (saveRequired) {
            UserProgressController.Instance.SaveProgress();
            //HtmlBridge.Instance.SaveSettings();
            saveRequired = false;
        }
    }
    public void ApplyProgress(PlayerSettings settings) {
        MusicSlider.value = settings.MusicVolume;
        SoundSlider.value = settings.SoundVolume;
        ChangeMusicVolume(settings.MusicVolume);
        ChangeSoundVolume(settings.SoundVolume);
    }
}
