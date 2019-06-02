using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DebateCameraController : MonoBehaviour
{
    [SerializeField] [Range(50f, 100f)] float cameraRotSpeed = 75f;

    Camera debateCamera;
    Quaternion currentCamTargetRot;

    Coroutine focusingRoutine;

    UnityEvent onFocusFinish = new UnityEvent();

    IEnumerator FocusOnCharacter(Vector3 characterPosition)
    {
        Vector3 diff = characterPosition - debateCamera.transform.position;

        Vector3 targetDir = new Vector3(diff.x, debateCamera.transform.forward.y, diff.z);
        Quaternion fromRot = debateCamera.transform.rotation;

        currentCamTargetRot = Quaternion.LookRotation(targetDir, debateCamera.transform.up);

        float timer = 0f;
        float angleBetweenRots = Quaternion.Angle(fromRot, currentCamTargetRot);
        float rotDuration = angleBetweenRots / cameraRotSpeed;

        while (timer < rotDuration)
        {
            timer += Time.deltaTime;
            debateCamera.transform.rotation = Quaternion.Slerp(fromRot, currentCamTargetRot, timer / rotDuration);

            yield return new WaitForEndOfFrame();
        }

        onFocusFinish.Invoke();

        focusingRoutine = null;
    }

    public void SetUpDebateCamera(Camera camera)
    {
        debateCamera = camera;
    }

    public void SetDebateCameraAvailability(bool enable)
    {
        debateCamera.gameObject.SetActive(enable);
    }

    public void StartFocusing(Vector3 characterPosition)
    {
        focusingRoutine = StartCoroutine(FocusOnCharacter(characterPosition));
    }

    public void StopFocusing()
    {
        if (focusingRoutine != null)
        {
            StopCoroutine(focusingRoutine);
            debateCamera.transform.rotation = currentCamTargetRot;
            onFocusFinish.Invoke();
            focusingRoutine = null;
        }
    }

    public bool IsFocusing()
    {
        return (focusingRoutine != null);
    }

    #region Properties

    public UnityEvent OnFocusFinish
    {
        get { return onFocusFinish; }
    }

    #endregion
}