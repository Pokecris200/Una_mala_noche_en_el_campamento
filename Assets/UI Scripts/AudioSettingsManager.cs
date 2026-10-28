using UnityEngine;
using UnityEngine.Audio;


[DefaultExecutionOrder(-1000)]
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] AudioMixer audioMixer;

    const string MasterKey = "MasterVolume";
    const string MusicKey = "MusicVolume";
    const string SFXKey = "SFXVolume";

    const string MasterParameter = "MasterVolume";
    const string MusicParameter = "MusicVolume";
    const string SFXParameter = "SFXVolume";

    const float DefaultVolume = 1f;

    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        ApplyVolume(MasterParameter, MasterVolume);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        ApplyVolume(MusicParameter, MusicVolume);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        ApplyVolume(SFXParameter, SFXVolume);
    }

    void ApplyVolume(string parameter, float volume)
    {
        float decibels = volume > 0f
            ? Mathf.Log10(volume) * 20f
            : -80f;

        if (!audioMixer.SetFloat(parameter, decibels))
            Debug.LogWarning($"No se pudo aplicar el parámetro: {parameter}");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(MasterKey, MasterVolume);
        PlayerPrefs.SetFloat(MusicKey, MusicVolume);
        PlayerPrefs.SetFloat(SFXKey, SFXVolume);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MasterKey, DefaultVolume));
        SetMusicVolume(PlayerPrefs.GetFloat(MusicKey, DefaultVolume));
        SetSFXVolume(PlayerPrefs.GetFloat(SFXKey, DefaultVolume));
    }

    public void ResetSettings()
    {
        SetMasterVolume(DefaultVolume);
        SetMusicVolume(DefaultVolume);
        SetSFXVolume(DefaultVolume);

        SaveSettings();
    }
}