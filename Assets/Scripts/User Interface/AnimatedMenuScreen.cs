using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedMenuScreen : MonoBehaviour
{
    Animator screenAnimator;
    float showAnimationDuration;
    float hideAnimationDuration;

    public void Awake()
    {
        screenAnimator = GetComponent<Animator>();

        AnimationClip[] animations = screenAnimator.runtimeAnimatorController.animationClips;
        AnimationClip showAnim = Array.Find(animations, a => a.name.ToLower().Contains("show"));
        AnimationClip hideAnim = Array.Find(animations, a => a.name.ToLower().Contains("hide"));

        showAnimationDuration = showAnim.length;
        hideAnimationDuration = hideAnim.length;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        screenAnimator.SetTrigger("Hide");
        Invoke("Deactivate", hideAnimationDuration);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}