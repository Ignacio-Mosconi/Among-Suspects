using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    #region Singleton
    static DialogueSystem instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static DialogueSystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DialogueSystem>();
                if (!instance)
                    Debug.LogError("There is no 'DialogueSystem' in the scene");
            }

            return instance;
        }
    }
    #endregion

    [SerializeField] GameObject dialogueArea;

    public void EnableDialogueArea()
    {
        HUD.Instance.SetVisibility(false);
        dialogueArea.SetActive(true);
    }
}