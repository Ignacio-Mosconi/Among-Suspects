using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Range(2f, 5f)] float movementSpeed = 3f;
	[SerializeField] [Range(1f, 3f)] float runMultiplier = 2f;

    CharacterController characterController;
    float currentRunMul = 1f;
    float verticalSpeed = 0f;

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

        inputVector = new Vector3(horMovement, 0f, forMovement);
        if (inputVector.sqrMagnitude > 1f)
            inputVector.Normalize();
        currentRunMul = (Input.GetButton("Run")) ? runMultiplier : 1f;
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        
        movement += (transform.right * inputVector.x + transform.forward * inputVector.z) * movementSpeed * currentRunMul;
        movement += Vector3.up * verticalSpeed;

        characterController.Move(movement * Time.deltaTime);
             
        if (characterController.isGrounded || (characterController.collisionFlags & CollisionFlags.Above) != 0)
            verticalSpeed = 0f;
    }
}