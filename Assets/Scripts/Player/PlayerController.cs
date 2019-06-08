using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FirstPersonCamera))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour, ICharacter
{
    [SerializeField] CharacterName playerName = default;
    [SerializeField] Sprite[] characterSprites = default;
    [SerializeField] [Range(0f, 3f)] float reEnableInteractionDelay = 2f;

    FirstPersonCamera firstPersonCamera;
    PlayerMovement playerMovement;
    Camera playerCamera;
    List<ClueInfo> cluesGathered = new List<ClueInfo>();
    bool canInteract = true;
    bool foundClueInLastDialogue;
    bool startedInvestigationInLastDialogue;

    UnityEvent onClueFound = new UnityEvent();
    UnityEvent onStartedInvestigation = new UnityEvent();

    void Awake()
    {
        firstPersonCamera = GetComponent<FirstPersonCamera>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        DialogueManager.Instance.OnDialogueAreaEnable.AddListener(Disable);
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(Enable);
    }

    bool CheckNotificationsDisplay()
    {
        bool shouldDelayInteractionEnable = false;

        if (foundClueInLastDialogue)
        {
            shouldDelayInteractionEnable = true;
            foundClueInLastDialogue = false;
            onClueFound.Invoke();
        }

        if (startedInvestigationInLastDialogue)
        {
            shouldDelayInteractionEnable = true;
            startedInvestigationInLastDialogue = false;
            onStartedInvestigation.Invoke();
        }

        return shouldDelayInteractionEnable;
    }

    void EnableInteraction()
    {
        canInteract = true;
    }

    public void Enable()
    {
        firstPersonCamera.enabled = true;
        playerMovement.enabled = true;

        bool delayInteractionEnable = CheckNotificationsDisplay();
        float interactionEnableDelay = (delayInteractionEnable) ? reEnableInteractionDelay : 0f;
        Invoke("EnableInteraction", interactionEnableDelay);
    }

    public void Disable()
    {
        firstPersonCamera.enabled = false;
        playerMovement.enabled = false;
        canInteract = false;
    }

    public void DeactivateCamera()
    {
        playerCamera.gameObject.SetActive(false);
    }

    public void AddClue(ClueInfo clueInfo)
    {
        if (!cluesGathered.Contains(clueInfo))
        {
            foundClueInLastDialogue = true;
            cluesGathered.Add(clueInfo);
        }
    }

    public void StartInvestigation()
    {
        startedInvestigationInLastDialogue = true;
    }

    public bool HasClue(ref ClueInfo clueInfo)
    {    
        return (cluesGathered.Contains(clueInfo));
    }

    public bool IsMovementEnabled()
    {
        return playerMovement.enabled;
    }

    public CharacterName GetCharacterName()
    {
        return playerName;
    }

    public Sprite GetSprite(CharacterEmotion characterEmotion)
    {
        return characterSprites[(int)characterEmotion];
    }

    #region Properties

    public FirstPersonCamera FirstPersonCamera
    {
        get { return firstPersonCamera; }
    }

    public bool CanInteract
    {
        get { return canInteract; }
    }

    public List<ClueInfo> CluesGathered
    {
        get { return cluesGathered; }
    }

    public UnityEvent OnClueFound
    {
        get { return onClueFound; }
    }

    public UnityEvent OnStartedInvestigation
    {
        get { return onStartedInvestigation; }
    }
    
    #endregion
}