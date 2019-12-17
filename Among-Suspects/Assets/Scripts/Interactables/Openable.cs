using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AkGameObj))]
public class Openable : Interactable
{
    [Header("Openable Properties")]
    [SerializeField] string[] openableNameByLanguage = new string[(int)Language.Count];
    [SerializeField] bool isLocked = true;
    [Header("Openable Sounds")]
    [SerializeField] string openEventName = default; 
    [SerializeField] string closeEventName = default;
    
    Dictionary<Language, ThoughtInfo> lockedThoughtInfoByLanguage = new Dictionary<Language, ThoughtInfo>();
    Animator animator;
    float openAnimationDuration;
    float closeAnimationDuration;
    bool isOpen;

    string[] openInteractionByLanguage = new string[(int)Language.Count];
    string[] closeInteractionByLanguage = new string[(int)Language.Count];

    protected override void Awake()
    {
        base.Awake();
        
        animator = GetComponent<Animator>();

        openInteractionByLanguage[0] = "open the " + openableNameByLanguage[0].ToLower();
        openInteractionByLanguage[1] = "abrir " + openableNameByLanguage[1].ToLower();
        closeInteractionByLanguage[0] = "close the " + openableNameByLanguage[0].ToLower();
        closeInteractionByLanguage[1] = "cerrar " + openableNameByLanguage[1].ToLower();
    }

    protected override void Start()
    {
        base.Start();
        
        AnimationClip[] animations = animator.runtimeAnimatorController.animationClips;
        AnimationClip openAnim = Array.Find(animations, a => a.name.ToLower().Contains("open"));
        AnimationClip closeAnim = Array.Find(animations, a => a.name.ToLower().Contains("close"));

        openAnimationDuration = openAnim.length;
        closeAnimationDuration = closeAnim.length;

        LoadThought();
    }

    void LoadThought()
    {
        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            string languagePath = Enum.GetName(typeof(Language), language);
            ThoughtInfo thoughtInfo = Resources.Load("Thoughts/" + languagePath + "/Generic/Door Thought") as ThoughtInfo;

            lockedThoughtInfoByLanguage.Add(language, thoughtInfo);
        }
    }

    void Open()
    {
        isOpen = true;
        animator.SetTrigger("Open");
        if (!String.IsNullOrEmpty(openEventName))
            AudioManager.Instance.PostEventDelayed(openEventName, openAnimationDuration * 0.5f, gameObject);
        Invoke("EnableInteraction", openAnimationDuration);
    }

    void Close()
    {
        isOpen = false;
        animator.SetTrigger("Close");
        if (!String.IsNullOrEmpty(closeEventName))
            AudioManager.Instance.PostEventDelayed(closeEventName, closeAnimationDuration * 0.5f, gameObject);
        Invoke("EnableInteraction", closeAnimationDuration);
    }

    public override void Interact()
    {
        DisableInteraction();

        if (!isLocked)
        {
            if (!isOpen)
                Open();
            else
                Close();
        }
        else
            DialogueManager.Instance.StartDialogue(lockedThoughtInfoByLanguage, interactionPoint.position);
    }

    public override string GetInteractionKind()
    {
        Language language = GameManager.Instance.CurrentLanguage;

        return (!isOpen) ? openInteractionByLanguage[(int)language] : closeInteractionByLanguage[(int)language];
    }
}