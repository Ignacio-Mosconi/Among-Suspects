using System;
using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class UIPrompt : MonoBehaviour
{ 
    [SerializeField] bool keepOnScreen = false;
    [SerializeField] bool keepStateAtPause = false;

    Animator promptAnimator;
    float showAnimationDuration;
    float idleAnimationDuration;
    float hideAnimationDuration;
    bool isShowing = false;
    bool isHiding = false;

    void FinishShowing()
    {
        isShowing = false;
    }

    void FinishHiding()
    {
        isHiding = false;
    }

    public void SetUp()
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

    public void Show()
    {
        isShowing = true;
        gameObject.SetActive(true);
        promptAnimator.keepAnimatorControllerStateOnDisable = keepStateAtPause;
        Invoke("FinishShowing", showAnimationDuration);
        if (!keepOnScreen)
            Invoke("Hide", showAnimationDuration + idleAnimationDuration);
    } 

    public void Hide()
    {
        isHiding = true;
        promptAnimator.SetTrigger("Hide");
        if (promptAnimator.updateMode == AnimatorUpdateMode.Normal)
        {
            Invoke("FinishHiding", hideAnimationDuration);
            Invoke("Deactivate", hideAnimationDuration);
        }
        else
        {
            GameManager.Instance.InvokeMethodInRealTime(FinishHiding, hideAnimationDuration);
            GameManager.Instance.InvokeMethodInRealTime(Deactivate, hideAnimationDuration);
        }
    }

    public void Deactivate()
    {
        promptAnimator.keepAnimatorControllerStateOnDisable = false;
        gameObject.SetActive(false);
    }

    public float GetOnScreenDuration()
    {
        float duration = (!keepOnScreen) ? showAnimationDuration + idleAnimationDuration + hideAnimationDuration : -1f; 
        return duration;
    }

    #region Properties

    public float ShowAnimationDuration
    {
        get { return showAnimationDuration; }
    }

    public float HideAnimationDuration
    {
        get { return hideAnimationDuration; }
    }

    public bool IsShowing
    {
        get { return isShowing; }
    }

    public bool IsHiding
    {
        get { return isHiding; }
    }

    #endregion
}