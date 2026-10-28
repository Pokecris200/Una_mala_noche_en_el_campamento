using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField]
    private int initialSFXSources = 10;

    [SerializeField]
    private int maxSFXSources = 25;

    [Header("Audio Data")]
    [SerializeField]
    private AudioData[] audioData;

    private Dictionary<string, AudioData>
        audioDictionary;

    private Dictionary<string, float>
        cooldownTracker;

    private List<PoolAudioSources>
        sfxPool;

    private AudioSource musicSource;

    private Coroutine musicFadeCoroutine;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Initialize();
    }


    private void Initialize()
    {
        BuildAudioDictionary();
        InitializeCooldownTracker();
        SetupMusicSource();
        SetupSFXPool();
    }


    private void BuildAudioDictionary()
    {
        audioDictionary =
            new Dictionary<string, AudioData>();

        foreach (AudioData data in audioData)
        {
            if (data == null)
                continue;

            if (audioDictionary.ContainsKey(data.name))
            {
                Debug.LogWarning(
                    $"Duplicate AudioData: {data.name}"
                );

                continue;
            }

            audioDictionary.Add(data.name, data);
        }
    }


    private void InitializeCooldownTracker()
    {
        cooldownTracker =
            new Dictionary<string, float>();
    }


    private void SetupMusicSource()
    {
        musicSource =
            gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }


    private void SetupSFXPool()
    {
        sfxPool =
            new List<PoolAudioSources>();

        for (int i = 0;
             i < initialSFXSources;
             i++)
        {
            CreateSFXSource();
        }
    }


    private PoolAudioSources CreateSFXSource()
    {
        GameObject sourceObject = new GameObject($"SFX_{sfxPool.Count}");

        sourceObject.transform.SetParent(transform);

        AudioSource source = sourceObject.AddComponent<AudioSource>();

        source.playOnAwake = false;

        source.spatialBlend = 0f;

        PoolAudioSources pooledSource = new PoolAudioSources(source);

        sfxPool.Add(pooledSource);

        return pooledSource;
    }


    private bool TryGetAudioData(string audioName, out AudioData data)
    {
        if (!audioDictionary.TryGetValue(audioName, out data))
        {
            Debug.LogWarning($"Audio '{audioName}' not found.");

            return false;
        }

        return true;
    }


    private bool CheckCooldown(AudioData data)
    {
        if (data.cooldown <= 0f)
            return true;

        if (!cooldownTracker.TryGetValue(data.name, out float lastPlayed))
        {
            cooldownTracker[data.name] = Time.time;

            return true;
        }

        if (Time.time - lastPlayed >= data.cooldown)
        {
            cooldownTracker[data.name] = Time.time;

            return true;
        }

        return false;
    }


    private PoolAudioSources GetAvailableSource(int requestedPriority)
    {
        foreach (PoolAudioSources source in sfxPool)
        {
            if (!source.IsPlaying)
                return source;
        }

        if (sfxPool.Count < maxSFXSources)
        {
            return CreateSFXSource();
        }

        return FindReplacementSource(
            requestedPriority
        );
    }


    private PoolAudioSources FindReplacementSource(int requestedPriority)
    {
        PoolAudioSources candidate = null;

        foreach (PoolAudioSources source in sfxPool)
        {
            if (source.CurrentAudio == null)
                continue;

            int priority = source.CurrentAudio.priority;

            if (priority >= requestedPriority)
                continue;

            if (candidate == null)
            {
                candidate = source;
                continue;
            }

            int candidatePriority = candidate.CurrentAudio.priority;

            if (priority < candidatePriority)
            {
                candidate = source;
            }
            else if (priority == candidatePriority && source.StartTime < candidate.StartTime)
            {
                candidate = source;
            }
        }

        return candidate;
    }


    private AudioClip GetRandomClip(AudioData data)
    {
        if (data.clips == null || data.clips.Length == 0)
        {
            Debug.LogWarning($"AudioData '{data.name}' has no clips.");

            return null;
        }

        return data.clips[Random.Range(0, data.clips.Length)];
    }


    private void ConfigureSource(AudioSource source, AudioData data, bool is3D)
    {
        source.outputAudioMixerGroup = data.mixerGroup;

        source.spatialBlend = is3D ? 1f : 0f;

        source.loop = data.loop;

        source.volume = 1f;
    }


    public void Play(string audioName)
    {
        if (!TryGetAudioData(audioName, out AudioData data))
            return;

        if (data.audioType == AudioType.Music)
        {
            PlayMusic(data);
            return;
        }

        if (!CheckCooldown(data))
            return;

        PoolAudioSources pooledSource = GetAvailableSource(data.priority);

        if (pooledSource == null)
            return;

        ConfigureSource(pooledSource.Source, data, false);

        AudioClip clip = GetRandomClip(data);

        if (clip == null)
            return;

        pooledSource.SetAudioData(data);

        pooledSource.Source.PlayOneShot(clip, data.volume);
    }


    public void Play3D(string audioName, Vector3 position)
    {
        if (!TryGetAudioData(audioName, out AudioData data))
            return;

        if (!CheckCooldown(data))
            return;

        PoolAudioSources pooledSource = GetAvailableSource(data.priority);
        if (pooledSource == null) return;

        AudioSource source = pooledSource.Source;
        ConfigureSource(source, data, true);

        source.transform.position = position;

        source.rolloffMode = AudioRolloffMode.Linear;

        source.minDistance = 3f;
        source.maxDistance = 25f;

        AudioClip clip = GetRandomClip(data);

        if (clip == null)
            return;

        pooledSource.SetAudioData(data);

        source.PlayOneShot(clip, data.volume);
    }


    public void PlayMusic(string audioName)
    {
        if (!TryGetAudioData(audioName, out AudioData data))
            return;

        if (data.audioType != AudioType.Music)
        {
            Debug.LogWarning($"'{data.name}' no es música.");
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        musicFadeCoroutine = StartCoroutine(ChangeMusicCoroutine(data, 0.5f));
    }

    private IEnumerator ChangeMusicCoroutine(AudioData data, float duration)
    {
        float startVolume = musicSource.volume;


        float elapsed = 0f;

        while (elapsed < duration && musicSource.isPlaying)
        {
            elapsed += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                startVolume,
                0f,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        musicSource.Stop();

        AudioClip clip = GetRandomClip(data);

        if (clip == null)
        {
            musicFadeCoroutine = null;
            yield break;
        }

        musicSource.outputAudioMixerGroup = data.mixerGroup;
        musicSource.clip = clip;
        musicSource.loop = data.loop;
        musicSource.volume = 0f;
        musicSource.Play();

        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            musicSource.volume = Mathf.Lerp(
                0f,
                data.volume,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        musicSource.volume = data.volume;
        musicFadeCoroutine = null;
    }



    private void PlayMusic(AudioData data)
    {
        AudioClip clip = GetRandomClip(data);

        if (clip == null)
            return;

        musicSource.outputAudioMixerGroup = data.mixerGroup;

        musicSource.clip = clip;
        musicSource.volume = data.volume;
        musicSource.loop = data.loop;

        musicSource.Play();
    }


    public void StopMusic()
    {
        musicSource.Stop();
    }


    public void FadeMusic(float targetVolume, float duration)
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeMusicCoroutine(targetVolume, duration));
    }


    private IEnumerator FadeMusicCoroutine(float target, float duration)
    {
        float start = musicSource.volume;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            musicSource.volume = Mathf.Lerp(start, target, t);

            yield return null;
        }

        musicSource.volume = target;

        musicFadeCoroutine = null;
    }
}