using UnityEngine;
using UnityEngine.Audio;
public enum AudioType
{
    SFX,
    Music,
    Voice
}

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Audio/Audio Data")]

public class AudioData : ScriptableObject
{
    [Header("Audio Clip")]
    public AudioClip[] clips;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    // Add more audio settings as needed, such as pitch

    [Header("Loopeable")]
    public bool loop;

    [Header("Couldown")]
    [Min(0f)]
    public float cooldown = 0f;

    [Header("Priority")]
    public int priority = 5;

    [Header("Type of Audio")]
    public AudioType audioType;

    [Header("Type of Mixer")]
    public AudioMixerGroup mixerGroup;
}