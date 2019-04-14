using UnityEngine;

public class NpcController : Interactable
{
    [SerializeField] DialogueInfo dialogue;

    protected override void Interact()
    {
        DialogueSystem.Instance.EnableDialogueArea(dialogue);
    }
}