using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] Interactable interactable = default;
    
    PlayerController playerController;

    void Start()
    {
        playerController = CharacterManager.Instance.PlayerController;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == playerController.gameObject)
        {
            interactable.Interact();
            gameObject.SetActive(false);
        }
    }
}