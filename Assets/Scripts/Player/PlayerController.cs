using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FirstPersonCamera))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterName playerName;

    FirstPersonCamera firstPersonCamera;
    PlayerMovement playerMovement;
    List<ClueInfo> cluesGathered = new List<ClueInfo>();
    bool canInteract = true;

    void Awake()
    {
        firstPersonCamera = GetComponent<FirstPersonCamera>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void SetAvailability(bool enable)
    {
        firstPersonCamera.enabled = enable;
        playerMovement.enabled = enable;
        canInteract = enable;
    }

    public void AddClue(ClueInfo clueInfo)
    {
        if (!cluesGathered.Contains(clueInfo))
            cluesGathered.Add(clueInfo);
    }

    public bool HasClue(ref ClueInfo clueInfo)
    {    
        return (cluesGathered.Contains(clueInfo));
    }

    #region Getters & Setters

    public CharacterName PlayerName
    {
        get { return playerName; }
    }

    public FirstPersonCamera FirstPersonCamera
    {
        get { return firstPersonCamera; }
    }

    public bool CanInteract
    {
        get { return canInteract; }
    }
    
    #endregion
}