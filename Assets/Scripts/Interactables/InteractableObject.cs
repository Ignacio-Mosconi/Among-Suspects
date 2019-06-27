using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableObject : Interactable
{
    [SerializeField] Sprite objectSprite = default;

    ThoughtInfo thoughtInfo;
    
    protected override void Start()
    {
        base.Start();
        thoughtInfo = Resources.Load("Thoughts/" + SceneManager.GetActiveScene().name + "/" + gameObject.name + " Thought") as ThoughtInfo;
    }

    public override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.StartDialogue(thoughtInfo, interactionPoint.position, objectSprite, enableImage: true);
    }

    public override string GetInteractionKind()
    {
        return "inspect";
    }
}