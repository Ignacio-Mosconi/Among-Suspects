using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Transform interactionPoint;
    [SerializeField] [Range(1f, 5f)] float interactionRadius;
    [SerializeField] bool hasToBeFaced = false;

    FirstPersonCamera firstPersonCamera;
    Transform cameraTransform;
    bool isPlayerLookingAt = false;

    UnityEvent onStartLookingAt = new UnityEvent();
    UnityEvent onStopLookingAt = new UnityEvent();
    UnityEvent onInteraction = new UnityEvent();

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        firstPersonCamera = playerObject.GetComponent<FirstPersonCamera>();
        cameraTransform = firstPersonCamera.GetComponentInChildren<Camera>().transform;

        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(EnableInteraction);
    }

    void Update()
    {
        Vector3 diff = interactionPoint.position - cameraTransform.position;

        if (diff.sqrMagnitude < interactionRadius * interactionRadius)
        {
            float angleBetweenObjs = Vector3.Angle(cameraTransform.forward, diff);
            float viewRange = firstPersonCamera.InteractionFOV * 0.5f / diff.magnitude;
            float interactableFwdOrient = firstPersonCamera.transform.InverseTransformDirection(transform.forward).z;

            if (angleBetweenObjs < viewRange && (!hasToBeFaced || interactableFwdOrient < -0.5f))
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

    protected virtual void EnableInteraction()
    {
        enabled = true;
    }

    protected virtual void DisableInteraction()
    {
        enabled = false;
    }

    protected abstract void Interact();

    #region Getters & Setters

    public Vector3 InteractionPosition
    {
        get { return interactionPoint.position; }
    }

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