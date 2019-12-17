using System;
using System.Collections.Generic;
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

    Dictionary<string, Action<float>> activeEventCallbacks = new Dictionary<string, Action<float>>();

    void ProcessEventCallback(object cookie, AkCallbackType type, object info)
    {
        string eventName = (string)cookie;

        switch (type)
        {
            case AkCallbackType.AK_EndOfEvent:
                if (activeEventCallbacks.ContainsKey(eventName))
                    activeEventCallbacks.Remove(eventName);
                break;

            case AkCallbackType.AK_Duration:
                AkDurationCallbackInfo callbackInfo = (AkDurationCallbackInfo)info;

                if (activeEventCallbacks.ContainsKey(eventName) && activeEventCallbacks[eventName] != null)
                    activeEventCallbacks[eventName].Invoke(callbackInfo.fDuration);
                break;

            default:
                break;
        }  
    }

    public void PostEvent(string eventName, GameObject go = null, Action<float> durationCallback = null)
    {
        if (activeEventCallbacks.ContainsKey(eventName))
            activeEventCallbacks.Remove(eventName);

        activeEventCallbacks.Add(eventName, durationCallback);

        AkSoundEngine.PostEvent(eventName, 
                                go ? go : gameObject, 
                                (uint)(AkCallbackType.AK_EndOfEvent | AkCallbackType.AK_Duration),
                                ProcessEventCallback, 
                                eventName);
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

    public bool IsPlayingEvent(string eventName)
    {
        return activeEventCallbacks.ContainsKey(eventName);
    }
}