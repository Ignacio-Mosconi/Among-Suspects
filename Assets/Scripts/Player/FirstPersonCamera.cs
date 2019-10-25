using System.Collections;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] [Range(90f, 135f)] float rotationSpeed = 100f;
    [SerializeField] [Range(50f, 90f)] float verticalRange = 80f;
    [SerializeField] [Range(60f, 90f)] float interactionFOV = 90f;
    [SerializeField] [Range(90f, 120f)] float focusSpeed = 110f;

    Transform fpsCamera;
    Coroutine focusing;
    float horAngle = 0f;
    float verAngle = 0f;

    void Awake()
    {
        fpsCamera = GetComponentInChildren<Camera>().transform;
    }

    void Start()
    {
        horAngle = transform.eulerAngles.y;
    }

    void Update()
    {
        float horRotation = Input.GetAxis("Mouse X");
        float verRotation = Input.GetAxis("Mouse Y");

        horAngle += horRotation * rotationSpeed * Time.deltaTime;
        verAngle -= verRotation * rotationSpeed * Time.deltaTime;

        verAngle = Mathf.Clamp(verAngle, -verticalRange, verticalRange);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, horAngle, transform.eulerAngles.z);
        fpsCamera.eulerAngles = new Vector3(verAngle, fpsCamera.eulerAngles.y, fpsCamera.eulerAngles.z);
    }

    IEnumerator RotateViewProgressively(Vector3 targetPosition)
    {
        Vector3 horDiff = targetPosition - transform.position;
        Vector3 verDiff = targetPosition - fpsCamera.position;

        Vector3 targetDir = new Vector3(horDiff.x, transform.forward.y, horDiff.z).normalized;
        Vector3 localForward = transform.InverseTransformDirection(transform.forward);
        Vector3 localTargetDir = transform.InverseTransformDirection(targetDir);
        float angleBetweenDirs = Vector3.Angle(transform.forward, targetDir);
        float targetHorAngle = (localTargetDir.x > localForward.x) ? horAngle + angleBetweenDirs : horAngle - angleBetweenDirs;
        
        Vector3 targetCameraDir = new Vector3(fpsCamera.forward.x, verDiff.y / verDiff.magnitude, fpsCamera.forward.z).normalized;
        float angleBetweenCamDirs = Vector3.Angle(fpsCamera.forward, targetCameraDir);
        float targetVerAngle = (targetCameraDir.y > fpsCamera.forward.y) ? verAngle - angleBetweenCamDirs : verAngle + angleBetweenCamDirs;

        float angleSum = angleBetweenDirs + angleBetweenCamDirs;
        float horSpeed = Mathf.Lerp(0f, focusSpeed, angleBetweenDirs / angleSum);
        float verSpeed = Mathf.Lerp(0f, focusSpeed, angleBetweenCamDirs / angleSum);

        while (horAngle != targetHorAngle || verAngle != targetVerAngle)
        {
            horAngle = Mathf.MoveTowardsAngle(horAngle, targetHorAngle, horSpeed * Time.deltaTime);
            verAngle = Mathf.MoveTowardsAngle(verAngle, targetVerAngle, verSpeed * Time.deltaTime);
            
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, horAngle, transform.eulerAngles.z);
            fpsCamera.eulerAngles = new Vector3(verAngle, fpsCamera.eulerAngles.y, fpsCamera.eulerAngles.z);

            yield return new WaitForEndOfFrame();
        }
    }

    public void FocusOnPosition(Vector3 position)
    {
        if (focusing != null)
            StopCoroutine(focusing);
        focusing = StartCoroutine(RotateViewProgressively(position));
    }

    public float GetFocusDuration(Vector3 position)
    {
        Vector3 diff = position - transform.position;
        float angle = Vector3.Angle(transform.forward, diff);
        float focusDuration = angle / focusSpeed;

        return focusDuration;
    }

    #region Properties
    
    public float InteractionFOV
    {
        get { return interactionFOV; }
    }

    public float FocusSpeed
    {
        get { return focusSpeed; }
    }
    
    #endregion
}