using System.Collections;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] [Range(90f, 135f)] float rotationSpeed = 100f;
    [SerializeField] [Range(50f, 90f)] float verticalRange = 80f;
    [SerializeField] [Range(60f, 90f)] float interactionFOV = 90f;
    [SerializeField] [Range(60f, 90f)] float focusSpeed = 75f;

    Transform fpsCamera;
    float horAngle = 0f;
    float verAngle = 0f;

    void Awake()
    {
        fpsCamera = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float horRotation = Input.GetAxis("Mouse X");
        float verRotation = Input.GetAxis("Mouse Y");

        horAngle += horRotation * rotationSpeed * Time.deltaTime;
        verAngle -= verRotation * rotationSpeed * Time.deltaTime;

        verAngle = Mathf.Clamp(verAngle, -verticalRange, verticalRange);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, horAngle, transform.eulerAngles.z);
        fpsCamera.localEulerAngles = new Vector3(verAngle, fpsCamera.localEulerAngles.y, fpsCamera.localEulerAngles.z);
    }

    IEnumerator RotateViewProgressively(Vector3 targetPosition)
    {
        Vector3 horTargetPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Quaternion targetHorRotation = Quaternion.LookRotation(horTargetPos - transform.position);

        while (transform.rotation != targetHorRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetHorRotation, focusSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        
        horAngle = transform.eulerAngles.y;
    }

    public void FocusOnObject(Vector3 position)
    {
        StartCoroutine(RotateViewProgressively(position));
    }

    #region Getters & Setters
    
    public float InteractionFOV
    {
        get { return interactionFOV; }
    }
    
    #endregion
}