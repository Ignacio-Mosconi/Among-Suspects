using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DebateCameraController : MonoBehaviour
{
    [SerializeField] [Range(50f, 100f)] float cameraRotSpeed = 75f;
    [SerializeField] [Range(100f, 200f)] float spinningSpeed = 120f;

    Camera debateCamera;
    Quaternion currentCamTargetRot;

    Coroutine focusingRoutine;
    Coroutine spinningRoutine;

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

    IEnumerator SpinAround(float spinDuration)
    {
        float timer = 0f;

        while (timer < spinDuration)
        {
            timer += Time.deltaTime;
            debateCamera.transform.Rotate(Vector3.up, spinningSpeed * Time.deltaTime, Space.Self);

            yield return new WaitForEndOfFrame();
        }

        spinningRoutine = null;
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

    public void StartSpinning(float spinDuration)
    {
        spinningRoutine = StartCoroutine(SpinAround(spinDuration));
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

    public void StopSpinning()
    {
        if (spinningRoutine != null)
        {
            StopCoroutine(spinningRoutine);
            spinningRoutine = null;
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