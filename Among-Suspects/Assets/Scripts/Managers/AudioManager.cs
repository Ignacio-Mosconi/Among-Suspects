using System;
using UnityEngine;

public enum MixerType
{
    Sfx, Music
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

    public void PostEvent(string eventName, GameObject go = null, Action<float> durationCallback = null)
    {
        AkSoundEngine.PostEvent(eventName, go ? go : gameObject);
    }

    public void PostEventDelayed(string eventName, float delay, GameObject go = null, Action<float> durationCallback = null)
    {
        GameManager.Instance.InvokeMethodInScaledTime(() => PostEvent(eventName, go, durationCallback), delay);
    }

    public void SetState(string stateGroup, string stateName)
    {
        AkSoundEngine.SetState(stateGroup, stateName);
    }

    public void SetSwitch(string switchGroup, string switchName)
    {
        AkSoundEngine.SetSwitch(switchGroup, switchName, gameObject);
    }

    public void SetRTPCValue(string rtcpName, float value)
    {
        AkSoundEngine.SetRTPCValue(rtcpName, value);
    }

    public void SetMixerVolume(MixerType mixerType, float value)
    {
        SetRTPCValue(Enum.GetName(typeof(MixerType), mixerType), value);
    }
}