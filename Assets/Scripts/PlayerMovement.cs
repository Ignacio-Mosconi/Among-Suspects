using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Range(5f, 10f)] float movementSpeed = 5f;

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
        movement += (transform.right * inputVector.x + transform.forward * inputVector.z) * movementSpeed;

        characterController.Move(movement * Time.deltaTime);
    }
}