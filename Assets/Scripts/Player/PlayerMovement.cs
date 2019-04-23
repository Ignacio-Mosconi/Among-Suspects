using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Range(2f, 5f)] float movementSpeed = 3f;
	[SerializeField] [Range(2f, 3f)] float runMultiplier = 2f;
    private float mult = 1.0f;

    CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 inputVector;
        Vector3 movement = Vector3.zero;
        float horMovement = Input.GetAxis("Horizontal");
        float forMovement = Input.GetAxis("Vertical");

        inputVector = new Vector3(horMovement, 0f, forMovement).normalized;
        if (Input.GetAxisRaw("Run") == 1){
            mult = runMultiplier;
        }
        else mult = 1.0f;
        movement += (transform.right * inputVector.x + transform.forward * inputVector.z) * movementSpeed * mult;
        characterController.Move(movement * Time.deltaTime);
    }
}