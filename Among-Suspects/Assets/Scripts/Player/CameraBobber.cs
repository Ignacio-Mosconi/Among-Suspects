using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class CameraBobber : MonoBehaviour
{
    [SerializeField, Range(0f, 20f)] float bobbingSpeed = 10f;
    [SerializeField, Range(0f, 1f)] float bobbingIntensity = 0.2f;
    
    PlayerMovement playerMovement;
    Transform fpsCamera;
    float verticalMidpoint;
    float bobbingTimer = 0f;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        fpsCamera = GetComponentInChildren<Camera>().transform;
    }

    void Start()
    {
        verticalMidpoint = fpsCamera.localPosition.y;
    }

    void Update()
    {
        Vector3 cameraPosition = new Vector3(fpsCamera.localPosition.x, verticalMidpoint, fpsCamera.localPosition.z);
        float totalInput = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));

        if (totalInput == 0f)
            bobbingTimer = 0f;
        else
        {
            float wave = Mathf.Sin(bobbingTimer);
            float fullCircle = Mathf.PI * 2;
            
            bobbingTimer += bobbingSpeed * playerMovement.CurrentRunMultiplier * Time.deltaTime;
            if (bobbingTimer > fullCircle)
                bobbingTimer -= fullCircle;

            if (wave != 0f)
            {
                float translateAmount = wave * bobbingIntensity * Mathf.Clamp01(totalInput);          
                cameraPosition.y = verticalMidpoint + translateAmount;
            }
        }

        fpsCamera.localPosition = cameraPosition;
    }
}
