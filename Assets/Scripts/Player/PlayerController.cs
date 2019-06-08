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
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(CheckNotificationsDisplay);
    }

    void CheckNotificationsDisplay()
    {
        if (foundClueInLastDialogue)
        {
            foundClueInLastDialogue = false;
            onClueFound.Invoke();
        }

        if (startedInvestigationInLastDialogue)
        {
            startedInvestigationInLastDialogue = false;
            onStartedInvestigation.Invoke();
        }
    }

    public void SetAvailability(bool enable)
    {
        firstPersonCamera.enabled = enable;
        playerMovement.enabled = enable;
        canInteract = enable;
    }

    public void SetCameraAvailability(bool enable)
    {
        playerCamera.gameObject.SetActive(enable);
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

    public bool IsMovementAvailable()
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