using UnityEngine;

public class Object : Interactable
{
    [SerializeField] ThoughtInfo thoughtInfo = default;
    [SerializeField] Sprite objectSprite = default;

    protected override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(thoughtInfo, interactionPoint.position, objectSprite, enableImage: true);
    }
}