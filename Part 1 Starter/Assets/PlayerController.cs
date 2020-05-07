using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    public float moveSpeed = 5;
    public float turnSpeed = 5;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Turn player
        
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * turnSpeed);
        
        // Move player
        
        characterController.SimpleMove(
            transform.forward * Input.GetAxis("Vertical") * moveSpeed +
            transform.right * Input.GetAxis("Horizontal") * moveSpeed);
    }
}