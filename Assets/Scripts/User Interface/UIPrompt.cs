using System;
using System.Collections;
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
        AnimationClip showAnim = Array.Find(animations, a => a.name.ToLower().Contains("show"));
        AnimationClip idleAnim = Array.Find(animations, a => a.name.ToLower().Contains("idle"));
        AnimationClip hideAnim = Array.Find(animations, a => a.name.ToLower().Contains("hide"));

        showAnimationDuration = showAnim.length;
        idleAnimationDuration = idleAnim.length;
        hideAnimationDuration = hideAnim.length;
    }

    IEnumerator InvokeHidingRealTime()
    {
        yield return new WaitForSecondsRealtime(showAnimationDuration + idleAnimationDuration);
        Hide();
    }

    IEnumerator InvokeDeactivationRealTime()
    {
        yield return new WaitForSecondsRealtime(hideAnimationDuration);
        Deactivate();
    }

    public void Show()
    {
        gameObject.SetActive(true); 
        if (!keepOnScreen)
            StartCoroutine(InvokeHidingRealTime());
    } 

    public void Hide()
    {
        promptAnimator.SetTrigger("Hide");
        StartCoroutine(InvokeDeactivationRealTime());
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