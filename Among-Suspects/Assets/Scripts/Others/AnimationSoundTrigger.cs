using UnityEngine;

public class AnimationSoundTrigger : MonoBehaviour
{
    public void TriggerSound(string eventName)
    {
        AudioManager.Instance.PostEvent(eventName);
    }
}