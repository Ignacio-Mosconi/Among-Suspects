using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class UIPrompt : MonoBehaviour
{ 
    [SerializeField] bool keepOnScreen = false;

    Animator promptAnimator;
    float showAnimationDuration;
    float idleAnimationDuration;
    float hideAnimationDuration;

    public void Awake()
    {
        promptAnimator = GetComponent<Animator>();

        AnimationClip[] animations = promptAnimator.runtimeAnimatorController.animationClips;
        AnimationClip showAnim = Array.Find(animations, a => a.name.Contains("Show"));
        AnimationClip idleAnim = Array.Find(animations, a => a.name.Contains("Idle"));
        AnimationClip hideAnim = Array.Find(animations, a => a.name.Contains("Hide"));

        showAnimationDuration = showAnim.length;
        idleAnimationDuration = idleAnim.length;
        hideAnimationDuration = hideAnim.length;
    }

    public void Show()
    {
        gameObject.SetActive(true); 
        if (!keepOnScreen)
            Invoke("Hide", showAnimationDuration + idleAnimationDuration);
    } 

    public void Hide()
    {
        promptAnimator.SetTrigger("Hide");
        Invoke("Deactivate", hideAnimationDuration);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public float GetOnScreenDuration()
    {
        return (showAnimationDuration + idleAnimationDuration + hideAnimationDuration);
    }
}