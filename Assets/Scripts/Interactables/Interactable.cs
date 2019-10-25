using System;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class StartLookingEvent : UnityEvent<string> {}

public abstract class Interactable : MonoBehaviour
{
    [Header("Basic Interaction Properties")]
    [SerializeField] protected Transform interactionPoint = default;
    [SerializeField] [Range(1f, 5f)] protected float interactionRadius = default;
    [SerializeField] protected bool hasToBeFaced = false;

    protected PlayerController playerController;
    
    Transform cameraTransform;
    LayerMask interactionRaycastLayerMask;
    Collider[] interactionColliders;
    bool isPlayerLookingAt = false;

    StartLookingEvent onStartLookingAt = new StartLookingEvent();
    UnityEvent onStopLookingAt = new UnityEvent();
    UnityEvent onInteraction = new UnityEvent();

    protected virtual void Awake()
    {
        interactionColliders = GetComponentsInChildren<Collider>();
    }

    protected virtual void Start()
    {
        playerController = CharacterManager.Instance.PlayerController;
        cameraTransform = playerController.GetComponentInChildren<Camera>().transform;
        interactionRaycastLayerMask = ~LayerMask.GetMask(LayerMask.LayerToName(cameraTransform.gameObject.layer));

        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(EnableInteraction);
    }

    void Update()
    {
        if (!playerController.CanInteract)
            return;

        Vector3 diff = interactionPoint.position - cameraTransform.position;

        if (diff.sqrMagnitude < interactionRadius * interactionRadius)
        {
            float angleBetweenObjs = Vector3.Angle(cameraTransform.forward, diff);
            float viewRange = playerController.FirstPersonCamera.InteractionFOV * 0.5f / diff.magnitude;
            float interactableFwdOrient = playerController.transform.InverseTransformDirection(transform.forward).z;

            RaycastHit hitInfo;
            Vector3 cameraPosition = cameraTransform.position;
            Vector3 interactionPointPosition = interactionPoint.position;
            Vector3 raycastDirection = (interactionPointPosition - cameraPosition).normalized;
            
            bool isObjectOccluded = true;
            if (Physics.Raycast(cameraPosition, raycastDirection, out hitInfo, interactionRadius, interactionRaycastLayerMask))
                if (Array.Find(interactionColliders, c => c == hitInfo.collider))
                    isObjectOccluded = false;
            
            if (angleBetweenObjs < viewRange && (!hasToBeFaced || interactableFwdOrient < -0.5f) && !isObjectOccluded)
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
        onStartLookingAt.Invoke(GetInteractionKind());
    }

    void StopLookingAt()
    {
        isPlayerLookingAt = false;
        onStopLookingAt.Invoke();
    }

    public virtual void EnableInteraction()
    {
        enabled = true;
    }

    public virtual void DisableInteraction()
    {
        enabled = false;
    }

    public virtual string GetInteractionKind()
    {
        return "interact";
    }

    public abstract void Interact();

    #region Properties

    public Transform InteractionPoint
    {
        get { return interactionPoint; }
    }

    public float InteractionRadius
    {
        get { return interactionRadius; }
    }

    public bool HasToBeFaced
    {
        get { return hasToBeFaced; }
    }

    public Vector3 InteractionPosition
    {
        get { return interactionPoint.position; }
    }

    public StartLookingEvent OnStartLookingAt
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