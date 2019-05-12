using UnityEngine;

public class Object : Interactable
{
    [SerializeField] Dialogue[] thoughts;
    [SerializeField] Sprite objectSprite;

    protected override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(thoughts, interactionPoint.position, objectSprite, enableImage: true);
    }
}