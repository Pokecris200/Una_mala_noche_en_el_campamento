using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void OnEnable()
    {
        LoadValues();
    }

    public void LoadValues()
    {
        SettingsManager settings = SettingsManager.Instance;

        if (settings == null) return;

        masterSlider.SetValueWithoutNotify(settings.MasterVolume);
        musicSlider.SetValueWithoutNotify(settings.MusicVolume);
        sfxSlider.SetValueWithoutNotify(settings.SFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        SettingsManager.Instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        SettingsManager.Instance.SetSFXVolume(value);
    }

    public void Save()
    {
        SettingsManager.Instance.SaveSettings();
    }

    public void Cancel()
    {
        SettingsManager.Instance.LoadSettings();
        LoadValues();
    }

    public void ResetAudio()
    {
        SettingsManager.Instance.ResetSettings();
        LoadValues();
    }
}