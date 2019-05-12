using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FirstPersonCamera))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] string playerName;

    FirstPersonCamera firstPersonCamera;
    PlayerMovement playerMovement;
    List<ClueInfo> cluesGathered = new List<ClueInfo>();

    void Awake()
    {
        firstPersonCamera = GetComponent<FirstPersonCamera>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void SetMovemeventAvailability(bool enableMovement)
    {
        firstPersonCamera.enabled = enableMovement;
        playerMovement.enabled = enableMovement;
    }

    public void FocusOnPosition(Vector3 position)
    {
        firstPersonCamera.FocusOnPosition(ref position);
    }

    public void AddClue(ClueInfo clueInfo)
    {
        if (!cluesGathered.Contains(clueInfo))
            cluesGathered.Add(clueInfo);
    }

    #region Getters & Setters

    public string PlayerName
    {
        get { return playerName; }
    }
    
    #endregion
}