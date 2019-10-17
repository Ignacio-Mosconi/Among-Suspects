using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : Interactable
{
    [Header("Interactable Object Properties")]
    [SerializeField] Sprite objectSprite = default;
    [SerializeField] InventoryItemInfo inventoryItemInfo = null;

    Dictionary<Language, ThoughtInfo> thoughtInfoByLanguage = new Dictionary<Language, ThoughtInfo>();

    static string[] interactionKindByLanguage = { "inspect", "inspeccionar" };

    protected override void Start()
    {
        base.Start();

        LoadThought();
    }

    void LoadThought()
    {
        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            string languagePath = Enum.GetName(typeof(Language), language);
            ThoughtInfo thoughtInfo = Resources.Load("Thoughts/" + languagePath + "/" + SceneManager.GetActiveScene().name + "/" + 
                                        gameObject.name + " Thought") as ThoughtInfo;

            thoughtInfoByLanguage.Add(language, thoughtInfo);
        }
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
        DialogueManager.Instance.StartDialogue(thoughtInfoByLanguage, interactionPoint.position, objectSprite, enableImage: true);

        if (inventoryItemInfo)
            DialogueManager.Instance.OnDialogueAreaDisable.AddListener(RemoveFromScene);
    }

    public override string GetInteractionKind()
    {
        return interactionKindByLanguage[(int)GameManager.Instance.CurrentLanguage];
    }
}