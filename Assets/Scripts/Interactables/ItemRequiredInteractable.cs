using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemRequiredInteractable : Interactable
{
    [Header("Item Required Interactable Object Properties")]
    [SerializeField] Interactable actualInteractable = default;
    [SerializeField] InventoryItemInfo requiredItem = default;
    [SerializeField] Sprite interactableSprite = default;

    Dictionary<Language, ItemRequiredThoughtInfo> itemRequiredThoughtInfoByLanguage = new Dictionary<Language, ItemRequiredThoughtInfo>();

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
    }

    void LoadThought()
    {
        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            string languagePath = Enum.GetName(typeof(Language), language);
            ItemRequiredThoughtInfo thoughtInfo = Resources.Load("Thoughts/" + languagePath + "/" + SceneManager.GetActiveScene().name + "/" +
                                        gameObject.name + " Thought") as ItemRequiredThoughtInfo;

            itemRequiredThoughtInfoByLanguage.Add(language, thoughtInfo);
        }
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

        if (itemInfo != null && itemInfo.itemID == requiredItem.itemID)
        {
            DialogueManager.Instance.StartDialogue(itemRequiredThoughtInfoByLanguage, 
                                                    itemRequiredThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage].useCorrectItemThought,
                                                    interactionPoint.position,
                                                    interactableSprite, 
                                                    enableImage: false);
            actualInteractable.enabled = true;
            Destroy(this);
        }
        else
            DialogueManager.Instance.StartDialogue(itemRequiredThoughtInfoByLanguage,
                                                    itemRequiredThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage].useIncorrectItemThought,
                                                    interactionPoint.position,
                                                    interactableSprite, enableImage: false);
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(itemRequiredThoughtInfoByLanguage,
                                                itemRequiredThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage].interactionThought,
                                                interactionPoint.position, 
                                                interactableSprite, enableImage: true);
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(ShowInventoryItemsAvailable);
    }

    public override string GetInteractionKind()
    {
        return actualInteractable.GetInteractionKind();
    }
}