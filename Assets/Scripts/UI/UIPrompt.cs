using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UIPrompt : MonoBehaviour
{
    [SerializeField] [Range(0f, 3f)] float onScreenDuration = 1.5f;
    [SerializeField] bool keepOnScreen = false;
    
    Animator promptAnimator;

    void Awake()
    {
        promptAnimator = GetComponent<Animator>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (!keepOnScreen)
        {
            float showAnimationDur = promptAnimator.GetCurrentAnimatorStateInfo(0).length;    
            Invoke("Hide", onScreenDuration + showAnimationDur);
        }
    } 

    public void Hide()
    {
        promptAnimator.SetTrigger("Hide");
        float hideAnimationDur = promptAnimator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("Deactivate", hideAnimationDur);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}