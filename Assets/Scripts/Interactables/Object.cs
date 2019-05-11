using UnityEngine;

public class Object : Interactable
{
    [SerializeField] Dialogue[] comments;

    protected override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(comments, interactionPoint.position);
    }
}