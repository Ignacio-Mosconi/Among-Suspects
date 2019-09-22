using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : Interactable
{
    [Header("Interactable Object Properties")]
    [SerializeField] Sprite objectSprite = default;
    [SerializeField] InventoryItemInfo inventoryItemInfo = null;

    ThoughtInfo thoughtInfo;
    
    protected override void Start()
    {
        base.Start();
        thoughtInfo = Resources.Load("Thoughts/" + SceneManager.GetActiveScene().name + "/" + gameObject.name + " Thought") as ThoughtInfo;
    }

    void RemoveFromScene()
    {
        DialogueManager.Instance.OnDialogueAreaDisable.RemoveListener(RemoveFromScene);
        InventoryManager.Instance.AddInventoryItem(inventoryItemInfo);
        Destroy(gameObject);
    }

    public override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.StartDialogue(thoughtInfo, interactionPoint.position, objectSprite, enableImage: true);

        if (inventoryItemInfo)
            DialogueManager.Instance.OnDialogueAreaDisable.AddListener(RemoveFromScene);
    }

    public override string GetInteractionKind()
    {
        return "inspect";
    }
}