using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UIPrompt : MonoBehaviour
{
    [SerializeField] [Range(0f, 3f)] float onScreenDuration = 1.5f;
    
    Animator promptAnimator;

    void Awake()
    {
        promptAnimator = GetComponent<Animator>();
    }

    void Show()
    {
        gameObject.SetActive(true);
        float showAnimationDur = promptAnimator.GetCurrentAnimatorStateInfo(0).length;    
        Invoke("Hide", onScreenDuration + showAnimationDur);
    } 

    void Hide()
    {
        promptAnimator.SetTrigger("Hide");
        float hideAnimationDur = promptAnimator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("Deactivate", hideAnimationDur);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Display()
    {
        Show();
    }
}