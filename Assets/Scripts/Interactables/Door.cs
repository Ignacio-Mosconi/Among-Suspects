using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Door : Interactable
{
    [Header("Door Properties")]
    [SerializeField] bool isLocked = true;
    [Header("Door Sounds")]
    [SerializeField] AudioClip openSound = default; 
    [SerializeField] AudioClip closeSound = default;
    
    Animator animator;
    AudioSource audioSource;
    ThoughtInfo lockedThoughtInfo;
    float openAnimationDuration;
    float closeAnimationDuration;
    bool isOpen;

    protected override void Awake()
    {
        base.Awake();
        
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
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
        GameManager.Instance.OnLanguageChanged.AddListener(LoadThought);
    }

    void LoadThought()
    {
        string languagePath = Enum.GetName(typeof(Language), GameManager.Instance.CurrentLanguage);

        lockedThoughtInfo = Resources.Load("Thoughts/" + languagePath + "/Generic/Door Thought") as ThoughtInfo;
    }

    void Open()
    {
        isOpen = true;
        animator.SetTrigger("Open");
        PlayDoorSound(openSound, openAnimationDuration);
        Invoke("EnableInteraction", openAnimationDuration);
    }

    void Close()
    {
        isOpen = false;
        animator.SetTrigger("Close");
        PlayDoorSound(closeSound, closeAnimationDuration);
        Invoke("EnableInteraction", closeAnimationDuration);
    }

    void PlayDoorSound(AudioClip sound, float doorAnimationDuration)
    {
        audioSource.clip = sound;
        
        if (sound.length < doorAnimationDuration)
            audioSource.PlayDelayed(doorAnimationDuration - sound.length);
        else
            audioSource.Play();
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
            DialogueManager.Instance.StartDialogue(lockedThoughtInfo, interactionPoint.position);
    }

    public override string GetInteractionKind()
    {
        return (!isOpen) ? "open the door" : "close the door";
    }
}