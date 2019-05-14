using UnityEngine;

public class DebateController : Interactable
{
    [SerializeField] DebateInfo debateInfo;
    [SerializeField] GameObject characterSprites;

    Camera debateCamera;

    void Awake()
    {
        debateCamera = GetComponentInChildren<Camera>(includeInactive: true);
    }

    protected override void Interact()
    {
        DisableInteraction();
        playerController.SetAvailability(enable: false);
        GameManager.Instance.SetCursorAvailability(enable: true);
    }

    public void StartDebate()
    {
        playerController.SetCameraAvailability(enable: false);
        
        characterSprites.SetActive(true);
        debateCamera.gameObject.SetActive(true);
    }

    public void CancelDebate()
    {
        EnableInteraction();
        playerController.SetAvailability(enable: true);
        GameManager.Instance.SetCursorAvailability(enable: false);
    }
}