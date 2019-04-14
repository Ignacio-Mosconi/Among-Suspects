using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    #region Singleton
    static HUD instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static HUD Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<HUD>();
                if (!instance)
                    Debug.LogError("There is no 'HUD' in the scene");
            }

            return instance;
        }
    }
    #endregion

    [SerializeField] GameObject hudArea;
    [SerializeField] GameObject interactTextPanel;

    void Start()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();

        foreach (Interactable interactable in interactables)
        {
            interactable.OnStartLookingAt.AddListener(ShowInteractTextPanel);
            interactable.OnStopLookingAt.AddListener(HideInteractTextPanel);
            interactable.OnInteraction.AddListener(HideInteractTextPanel);
        }
    }

    void ShowInteractTextPanel()
    {
        interactTextPanel.SetActive(true);
    }

    void HideInteractTextPanel()
    {
        interactTextPanel.SetActive(false);
    }

    public void SetVisibility(bool visible)
    {
        hudArea.SetActive(visible);
    }
}