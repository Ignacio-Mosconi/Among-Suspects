using UnityEngine;
using UnityEngine.Events;

public class PlayerLoookingEvent : UnityEvent<bool> {}

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] [Range(3f, 5f)] float interactionRadius;
    
    Transform player;
    Transform fpsCamera;
    bool isPlayerLooking = false;

    PlayerLoookingEvent onPlayerToggleLooking = new PlayerLoookingEvent();

    virtual protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fpsCamera = player.GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        Vector3 distance = player.position - transform.position;

        if (distance.sqrMagnitude < interactionRadius * interactionRadius)
        {
            if (Physics.Raycast(fpsCamera.position, fpsCamera.forward, interactionRadius))
            {
                if (!isPlayerLooking)
                    StartLooking();
                if (Input.GetButtonDown("Interact"))
                    Interact();
            }
            else
            {
                if (isPlayerLooking)
                    StopLooking();
            }
        }
    }

    void StartLooking()
    {
        isPlayerLooking = true;
        onPlayerToggleLooking.Invoke(true);
    }

    void StopLooking()
    {
        isPlayerLooking = false;
        onPlayerToggleLooking.Invoke(false);
    }

    protected abstract void Interact();

    public PlayerLoookingEvent OnPlayerToggleLooking
    {
        get { return onPlayerToggleLooking; }
    }
}