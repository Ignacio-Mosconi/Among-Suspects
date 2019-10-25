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
    bool foundClueInLastDialogue = false;
    bool startedInvestigationInLastDialogue = false;

    UnityEvent onStartedInvestigation = new UnityEvent();
    UnityEvent onClueFound = new UnityEvent();
    UnityEvent onAllCluesFound = new UnityEvent();
    UnityEvent onItemCollected = new UnityEvent();

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

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKey(KeyCode.I))
            GatherAllClues();
    }
#endif

    bool TriggeredNotification()
    {
        bool triggeredNotification = false;

        if (foundClueInLastDialogue)
        {
            triggeredNotification = true;
            foundClueInLastDialogue = false;
            onClueFound.Invoke();
            if (ChapterManager.Instance.CluesAmount == cluesGathered.Count)
                onAllCluesFound.Invoke();
        }

        if (startedInvestigationInLastDialogue)
        {
            triggeredNotification = true;
            startedInvestigationInLastDialogue = false;
            onStartedInvestigation.Invoke();
        }

        return triggeredNotification;
    }

    public void Enable()
    {
        firstPersonCamera.enabled = true;
        playerMovement.enabled = true;
        canInteract = !TriggeredNotification() && !IsInvoking("ReEnableInteractionDelayed");
    }

    public void Disable()
    {
        firstPersonCamera.enabled = false;
        playerMovement.enabled = false;
        canInteract = false;
    }

    public void ReEnableInteractionDelayed()
    {
        canInteract = true;
    }

    public void DeactivateCamera()
    {
        playerCamera.gameObject.SetActive(false);
    }

    public void AddClue(ClueInfo clueInfo)
    {
        if (!cluesGathered.Find(ci => ci.clueID == clueInfo.clueID))
        {
            ClueInfo clueInfoToAdd = ChapterManager.Instance.GetChapterClueInfo(clueInfo.clueID);
            
            foundClueInLastDialogue = true;
            cluesGathered.Add(clueInfoToAdd);
        }
    }

    public void ReloadCluesGathered(ClueInfo[] newClueInfos)
    {
        foreach (ClueInfo newClueInfo in newClueInfos)
        {
            ClueInfo clueInfoInList = cluesGathered.Find(ci => ci.clueID == newClueInfo.clueID);
            
            if (clueInfoInList)
            {
                cluesGathered.Add(newClueInfo);
                cluesGathered.Remove(clueInfoInList);
            }
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

    public UnityEvent OnStartedInvestigation
    {
        get { return onStartedInvestigation; }
    }

    public UnityEvent OnClueFound
    {
        get { return onClueFound; }
    }

    public UnityEvent OnAllCluesFound
    {
        get { return onAllCluesFound; }
    }

    public UnityEvent OnItemCollected
    {
        get { return onItemCollected; }
    }
    
    #endregion

#if UNITY_EDITOR
    #region Development Cheats

    public void GatherAllClues()
    {
        ChapterManager.Instance.TriggerInvestigationPhase();
        for (int i = 0; i < ChapterManager.Instance.CluesAmount; i++)
            cluesGathered.Add(ChapterManager.Instance.GetChapterClueInfo(i));
    }
    #endregion
#endif
}