using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Bgm, Effect, MaxCount,
}

public class SoundManager
{
    AudioSource[] audioSources = new AudioSource[(int)Sound.MaxCount];

    // Effect 오디오를 캐싱할 딕셔너리
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject("@Sound");
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Sound));
            for (int i = 0; i < soundNames.Length-1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }
            // BGM의 경우 반복재생하도록 함
            audioSources[(int)Sound.Bgm].loop = true;
        }
    }

    // 사운드 경로를 받아 해당 사운드를 재생
    public void Play(string path, Sound type = Sound.Effect, float pitch =1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    // AudioClip을 받아 해당 사운드를 재생
    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null) return;

        if (type == Sound.Bgm)
        {
            AudioSource audioSource = audioSources[(int)Sound.Bgm];
            // 기존 BGM이 재생 중이라면, 멈추고 audioClip 재생
            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = audioSources[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    // 모든 사운드를 멈추고 클립을 제거
    public void Clear()
    {
        foreach(AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        audioClips.Clear();
    }

    // 딕셔너리에서 AudioClip을 찾고 리턴. 없다면, Resources경로에서 찾고 리턴.
    private AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Sound.Bgm)
        {
            // Audio Clip 가져오기
            audioClip = Resources.Load<AudioClip>(path);
            if (audioClip == null) { Debug.Log($"Audio Clip is missing! {path}"); }
        }
        else
        {
            // Audio Clip 가져오기
            if (audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                audioClips.Add(path, audioClip);
            }
            if (audioClip == null) { Debug.Log($"Audio Clip is missing! {path}"); }
        }
        return audioClip;
    }

    public float GetBgmVolume()
    {
        return audioSources[(int)Sound.Bgm].volume;
    }
    public float GetEffectVolume()
    {
        return audioSources[(int)Sound.Effect].volume;
    }

    public void SetBgmVolume(float volume)
    {
        audioSources[(int)Sound.Bgm].volume = volume;
    }
    public void SetEffectVolume(float volume)
    {
        audioSources[(int)Sound.Effect].volume = volume;
    }
}
