using UnityEngine;

public class NpcController : Interactable
{
    [SerializeField] DialogueInfo dialogueInfo;

    protected override void Interact()
    {
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo);
    }
}