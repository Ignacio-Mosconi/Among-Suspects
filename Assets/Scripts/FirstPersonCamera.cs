using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] [Range(90f, 135f)] float rotationSpeed = 100f;
    [SerializeField] [Range(50f, 90f)] float verticalRange = 80f;
    [SerializeField] [Range(60f, 90f)] float interactionFOV = 90f;

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

        transform.eulerAngles = new Vector3(transform.rotation.x, horAngle, transform.rotation.z);
        fpsCamera.localEulerAngles = new Vector3(verAngle, fpsCamera.rotation.y, fpsCamera.rotation.z);
    }

    #region Getters & Setters
    
    public float InteractionFOV
    {
        get { return interactionFOV; }
    }
    
    #endregion
}