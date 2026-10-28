using UnityEngine;

public class PoolAudioSources
{
    public AudioSource Source;

    public AudioData CurrentAudio;

    public float StartTime;

    public bool IsPlaying => Source.isPlaying;

    public PoolAudioSources(AudioSource source)
    {
        Source = source;
    }

    public void SetAudioData(AudioData audioData)
    {
        CurrentAudio = audioData;
        StartTime = Time.time;
    }

    public void Clear()
    {
        CurrentAudio = null;
        StartTime = 0f;
    }
}