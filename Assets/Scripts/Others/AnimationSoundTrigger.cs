using UnityEngine;

public class AnimationSoundTrigger : MonoBehaviour
{
    public void TriggerSound(string soundName)
    {
        AudioManager.Instance.PlaySound(soundName);
    }
}