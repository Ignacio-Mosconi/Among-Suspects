using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        actualInteractable.enabled = false;
        
        LoadThought();
        GameManager.Instance.OnLanguageChanged.AddListener(LoadThought);
    }

    void LoadThought()
    {
        string languagePath = Enum.GetName(typeof(Language), GameManager.Instance.CurrentLanguage);

        itemRequiredThoughtInfo = Resources.Load("Thoughts/" + languagePath + "/" + SceneManager.GetActiveScene().name + "/" + 
                                                gameObject.name + " Thought") as ItemRequiredThoughtInfo;
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

        if (itemInfo.itemID == requiredItem.itemID)
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