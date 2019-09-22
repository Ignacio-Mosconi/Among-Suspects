using UnityEngine;

public class ItemRequiredInteractable : Interactable
{
    [Header("Item Required Interactable Object Properties")]
    [SerializeField] Interactable actualInteractable = default;
    [SerializeField] InventoryItemInfo requiredItem = default;
    [SerializeField] Sprite interactableSprite = default;

    ItemRequiredThoughtInfo itemRequiredThoughtInfo;

    void OnValidate()
    {
        if (actualInteractable)
        {
            interactionPoint = actualInteractable.InteractionPoint;
            interactionRadius = actualInteractable.InteractionRadius;
            hasToBeFaced = actualInteractable.HasToBeFaced;
        }
    }

    protected override void Start()
    {
        base.Start();
        
        itemRequiredThoughtInfo = Resources.Load("Thoughts/Item-Required/" + gameObject.name + " Thought") as ItemRequiredThoughtInfo;
        actualInteractable.enabled = false;
    }

    void ShowInventoryItemsAvailable()
    {
        DialogueManager.Instance.OnDialogueAreaDisable.RemoveListener(ShowInventoryItemsAvailable);
        InventoryManager.Instance.ShowItemSelectionScreen();
        InventoryManager.Instance.OnInventoryItemChosen.AddListener(AttemptInteractionUsingItem);
    }

    void AttemptInteractionUsingItem()
    {
        InventoryManager.Instance.OnInventoryItemChosen.RemoveListener(AttemptInteractionUsingItem);
        
        InventoryItemInfo itemInfo = InventoryManager.Instance.CurrentlySelectedItem;

        if (itemInfo == requiredItem)
        {
            DialogueManager.Instance.StartDialogue(itemRequiredThoughtInfo.useCorrectItemThought, interactionPoint.position,
                                                    interactableSprite, enableImage: false);
            actualInteractable.enabled = true;
            Destroy(this);
        }
        else
            DialogueManager.Instance.StartDialogue(itemRequiredThoughtInfo.useIncorrectItemThought, interactionPoint.position,
                                                    interactableSprite, enableImage: false);
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(itemRequiredThoughtInfo.interactionThought, interactionPoint.position, 
                                                interactableSprite, enableImage: true);
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(ShowInventoryItemsAvailable);
    }

    public override string GetInteractionKind()
    {
        return actualInteractable.GetInteractionKind();
    }
}