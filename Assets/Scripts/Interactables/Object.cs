using UnityEngine;

public class Object : Interactable
{
    [SerializeField] ThoughtInfo thoughtInfo;
    [SerializeField] Sprite objectSprite;

    protected override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(thoughtInfo, interactionPoint.position, objectSprite, enableImage: true);
    }
}