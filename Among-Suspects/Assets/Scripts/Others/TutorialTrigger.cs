using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TutorialTrigger : MonoBehaviour
{
    PlayerController playerController;

    UnityEvent onTrigger = new UnityEvent();

    void Start()
    {
        playerController = CharacterManager.Instance.PlayerController;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == playerController.gameObject)
        {
            onTrigger.Invoke();
            gameObject.SetActive(false);
        }
    }

    #region Properties

    public UnityEvent OnTrigger
    {
        get { return onTrigger; }
    }

    #endregion
}