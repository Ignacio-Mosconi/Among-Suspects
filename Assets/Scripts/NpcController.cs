using UnityEngine;

public class NpcController : Interactable
{
    protected override void Interact()
    {
        DialogueSystem.Instance.EnableDialogueArea();
    }
}