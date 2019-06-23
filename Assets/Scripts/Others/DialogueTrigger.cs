using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] NPC npc = default;
    
    PlayerController playerController;

    void Start()
    {
        playerController = CharacterManager.Instance.PlayerController;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == playerController.gameObject)
        {
            npc.Interact();
            gameObject.SetActive(false);
        }
    }
}