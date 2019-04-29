using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Transform interactionPoint;
    [SerializeField] [Range(1f, 5f)] float interactionRadius;

    FirstPersonCamera firstPersonCamera;
    bool isPlayerLookingAt = false;

    UnityEvent onStartLookingAt = new UnityEvent();
    UnityEvent onStopLookingAt = new UnityEvent();
    UnityEvent onInteraction = new UnityEvent();

    virtual protected void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        cameraTransform = playerObject.GetComponentInChildren<Camera>().transform;
        firstPersonCamera = playerObject.GetComponent<FirstPersonCamera>();
    }

    void Update()
    {
        Vector3 diff = interactionPoint.position - firstPersonCamera.transform.position;

        if (diff.sqrMagnitude < interactionRadius * interactionRadius)
        {
            if (Vector3.Angle(firstPersonCamera.transform.forward, diff.normalized) < firstPersonCamera.InteractionFOV * 0.5f)
            {
                if (!isPlayerLookingAt)
                    StartLookingAt();
                if (Input.GetButtonDown("Interact"))
                {
                    isPlayerLookingAt = false;
                    Interact();
                    onInteraction.Invoke();
                }
            }
            else
            {
                if (isPlayerLookingAt)
                    StopLookingAt();
            }
        }
        else
            if (isPlayerLookingAt)
                StopLookingAt();
    }

    void StartLookingAt()
    {
        isPlayerLookingAt = true;
        onStartLookingAt.Invoke();
    }

    void StopLookingAt()
    {
        isPlayerLookingAt = false;
        onStopLookingAt.Invoke();
    }

    protected abstract void Interact();

    #region Getters & Setters
    
    public UnityEvent OnStartLookingAt
    {
        get { return onStartLookingAt; }
    }

    public UnityEvent OnStopLookingAt
    {
        get { return onStopLookingAt; }
    }

    public UnityEvent OnInteraction
    {
        get { return onInteraction; }
    }
    
    #endregion
}