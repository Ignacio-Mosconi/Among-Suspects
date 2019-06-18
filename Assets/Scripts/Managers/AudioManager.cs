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
            DontDestroyOnLoad(gameObject);
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

    Dictionary<MixerType, AudioMixer> audioMixersDic = new Dictionary<MixerType, AudioMixer>();

    void Start()
    {
        for (int i = 0; i < audioMixers.Length; i++)
            audioMixersDic.Add((MixerType)i, audioMixers[i]);
    }

    public void PlaySound(string soundName)
    {
        AudioClip audioClip = Array.Find(soundsUI, clip => clip.name == soundName);
        if (!audioClip)
        {
            Debug.LogError("There are no sounds named " + soundName + "registered in the Audio Manager.", gameObject);
            return;
        }
        soundsUISource.clip = audioClip;
        soundsUISource.PlayOneShot(audioClip);
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

    public bool IsPlayingSound(string soundName)
    {
        return (soundsUISource.isPlaying && soundsUISource.clip.name == soundName);
    }
}