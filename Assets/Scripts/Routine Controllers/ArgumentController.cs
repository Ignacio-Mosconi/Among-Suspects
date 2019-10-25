using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ArgumentController : MonoBehaviour
{
    [SerializeField] GameObject argumentPanel = default;
    [SerializeField] [Range(1.5f, 2f)] float expandedScale = 2f;
    [SerializeField] [Range(0.5f, 1.5f)] float expansionDuration = 1f;

    Coroutine expandingRoutine;

    UnityEvent onArgumentFinish = new UnityEvent();

    IEnumerator ExpandArgument(bool lastArgument)
    {
        Vector3 initialScale = argumentPanel.transform.localScale;
        Vector3 targetScale = argumentPanel.transform.localScale * expandedScale;

        float timer = 0f;

        while (timer < expansionDuration)
        {
            timer += Time.deltaTime;
            argumentPanel.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / expansionDuration);

            yield return new WaitForEndOfFrame();
        }

        if (lastArgument)
            onArgumentFinish.Invoke();

        expandingRoutine = null;
    }

    public void StartExpanding(bool lastArgument)
    {
        expandingRoutine = StartCoroutine(ExpandArgument(lastArgument));
    }

    public void StopExpanding(bool lastArgument)
    {
        if (expandingRoutine != null)
        {
            StopCoroutine(expandingRoutine);
            argumentPanel.transform.localScale = new Vector3(expandedScale, expandedScale, expandedScale);
            if (lastArgument)
                onArgumentFinish.Invoke();
            expandingRoutine = null;
        }
    }

    public void ResetArgumentPanelScale()
    {
        argumentPanel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public bool IsExpanding()
    {
        return (expandingRoutine != null);
    }

    # region Properties

    public UnityEvent OnArgumentFinish
    {
        get { return onArgumentFinish; }
    }

    #endregion
}