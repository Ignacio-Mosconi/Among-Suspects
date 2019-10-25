using UnityEngine;

public enum CharacterEmotion
{
    Normal, Alternative, Happy, Surprised,
    Angry, Mad, Accusing, Shocked, 
    Listening
}

public enum DialogueType
{
    Normal, Interactive, Group
}

[System.Serializable]
public struct DialogueSound
{
    public AudioClip audioClip;
    [Range(0f, 1f)] public float playDelay;
}

[System.Serializable]
public struct Dialogue
{
    public CharacterName speakerName;
    [TextArea(3, 10)] public string speech;
    public CharacterEmotion speakerEmotion;
    public ClueInfo clueInfo;
    public DialogueSound dialogueSound;
    public bool revealName;
    public bool playerThought;
    public bool triggerNiceImpression;
}

[System.Serializable]
public struct BranchedDialogue
{
    public Dialogue[] branchDialogue;
}


[System.Serializable]
public struct DialogueOption
{
    public string option;
    public string description;
}

[System.Serializable]
public struct InteractiveDialogue
{
    public Dialogue[] intro;
    public DialogueOption[] playerOptions;
    public BranchedDialogue[] resultingDialogues;
}

[System.Serializable]
public struct GroupDialogue
{
    public CharacterName leftSpeaker;
    public CharacterName rightSpeaker;
    public Dialogue[] dialogue;
    public bool cancelOtherGroupDialogues;
}

[System.Serializable]
public struct TutorialDialogue
{
    public bool isPlayerTalking;
    [TextArea(3, 10)] public string speech;
}

[CreateAssetMenu(fileName = "New Dialogue Info", menuName = "Dialogue Info", order = 1)]
public class DialogueInfo : ScriptableObject
{
    [Header("Intro Dialogue")]
    public Dialogue[] introLines;
    [Header("Interactive Dialogue")]
    public InteractiveDialogue interactiveConversation;
    [Header("'Already Interacted' Dialogue")]
    public Dialogue[] niceComment;
    public Dialogue[] rudeComment;
    [Header("Group Dialogue")]
    public GroupDialogue groupDialogue;
    [Header("Dialogue Order")]
    public DialogueType[] dialogueTypeOrder = new DialogueType[3];

    [HideInInspector] public bool introRead = false;
    [HideInInspector] public bool interactionOptionSelected = false;
    [HideInInspector] public bool groupDialogueRead = false;

    public Dialogue[] DetermineNextDialogueLines()
    {
        Dialogue[] nextLines = null;

        foreach (DialogueType dialogueType in dialogueTypeOrder)
        {
            switch (dialogueType)
            {
                case DialogueType.Normal:
                    if (introLines.Length > 0 && !introRead)
                        nextLines = introLines;
                    break;
                case DialogueType.Interactive:
                    if (interactiveConversation.intro.Length > 0 && !interactionOptionSelected)
                        nextLines = interactiveConversation.intro;
                    break;
                case DialogueType.Group:
                    if (groupDialogue.dialogue.Length> 0 && !groupDialogueRead)
                        nextLines = groupDialogue.dialogue;
                    break;
            }
            if (nextLines != null)
                break;
        }

        return nextLines;
    }
}