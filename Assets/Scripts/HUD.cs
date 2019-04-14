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
            interactable.OnPlayerToggleLooking.AddListener(ToggleInteractTextPanel);
    }

    void ToggleInteractTextPanel(bool activate)
    {
        interactTextPanel.SetActive(activate);
    }

    public void SetVisibility(bool visible)
    {
        hudArea.SetActive(visible);
    }
}