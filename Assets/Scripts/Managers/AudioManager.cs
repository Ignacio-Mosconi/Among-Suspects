using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum MixerType
{
    Sfx, Music, Count
}

public class AudioManager : MonoBehaviour
{
    #region Singleton

    static AudioManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            AwakeSetUp();
        }
    }

    public static AudioManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<AudioManager>();
                if (!instance)
                {
                    GameObject audioManagerPrefab = Resources.Load("Game Management/Audio Manager") as GameObject;
                    instance = Instantiate(audioManagerPrefab).GetComponentInChildren<AudioManager>();
                }
            }

            return instance;
        }
    }

    #endregion

    [Header("Audio Mixers")]
    [SerializeField] AudioMixer[] audioMixers = new AudioMixer[(int)MixerType.Count];
    [Header("Audio Clips")]
    [SerializeField] AudioClip[] soundsUI = default;
    [SerializeField] AudioClip[] themes = default;
    [SerializeField] AudioClip[] ambientSounds = default;
    [Header("Audio Sources")]
    [SerializeField] AudioSource soundsUISource = default;
    [SerializeField] AudioSource musicSource = default;
    [SerializeField] AudioSource ambientSoundSource = default;

    const float MixerMultiplier = 11.5f;
    const float MuteValue = -80f;

    Dictionary<MixerType, AudioMixer> audioMixersDic = new Dictionary<MixerType, AudioMixer>();

    void AwakeSetUp()
    {
        for (int i = 0; i < audioMixers.Length; i++)
            audioMixersDic.Add((MixerType)i, audioMixers[i]);
    }

    public void PlaySound(string soundName, bool oneShot = true)
    {
        AudioClip audioClip = Array.Find(soundsUI, clip => clip.name == soundName);
        if (!audioClip)
        {
            Debug.LogError("There are no sounds named " + soundName + " registered in the Audio Manager.", gameObject);
            return;
        }
        soundsUISource.clip = audioClip;
        if (oneShot)
            soundsUISource.PlayOneShot(audioClip);
        else
            soundsUISource.Play();
    }

    public void PlaySoundDelayed(AudioClip audioClip, float delay)
    {
        AudioClip clip = Array.Find(soundsUI, c => c == audioClip);
        if (!audioClip)
        {
            Debug.LogError("There are no sounds named " + audioClip.name + "registered in the Audio Manager.", gameObject);
            return;
        }
        soundsUISource.clip = audioClip;

        soundsUISource.PlayDelayed(delay);
    }

    public void PlayTheme(string themeName)
    {
        AudioClip audioClip = Array.Find(themes, theme => theme.name == themeName);
        if (!audioClip)
        {
            Debug.LogError("There are no themes named " + themeName + "registered in the Audio Manager.", gameObject);
            return;
        }
        musicSource.clip = audioClip;
        musicSource.Play();
    }

    public void PlayAmbientSound(string ambientSoundName)
    {
        AudioClip audioClip = Array.Find(ambientSounds, ambientSound => ambientSound.name == ambientSoundName);
        if (!audioClip)
        {
            Debug.LogError("There are no ambient sounds named " + ambientSoundName + "registered in the Audio Manager.", gameObject);
            return;
        }
        ambientSoundSource.clip = audioClip;
        ambientSoundSource.Play();
    }

    public void PauseAmbientSound()
    {
        ambientSoundSource.Pause();
    }

    public void ResumeAmbientSound()
    {
        ambientSoundSource.UnPause();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void SetMixerVolume(MixerType mixerType, float volume)
    {
        float desiredMixerLevel = (volume > 0f) ? Mathf.Max(Mathf.Log(volume) * MixerMultiplier, MuteValue) : MuteValue;
        audioMixersDic[mixerType].SetFloat("Volume", desiredMixerLevel);
    }

    public bool IsPlayingSound(string soundName)
    {
        return (soundsUISource.isPlaying && soundsUISource.clip.name == soundName);
    }
}