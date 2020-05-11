using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private PortalableObject portalableObject;

    public float moveSpeed = 5;
    public float turnSpeed = 5;

    private float turnRotation;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        portalableObject = GetComponent<PortalableObject>();
        portalableObject.HasTeleported += PortalableObjectOnHasTeleported;
    }

    private void PortalableObjectOnHasTeleported(Portal sender, Portal destination, Vector3 newposition, Quaternion newrotation)
    {
        // For character controller to update
        
        Physics.SyncTransforms();
    }

    private void FixedUpdate()
    {
        // Turn player
        
        transform.Rotate(Vector3.up * turnRotation * turnSpeed);
        turnRotation = 0; // Consume variable
        
        // Move player
        
        characterController.SimpleMove(
            transform.forward * Input.GetAxis("Vertical") * moveSpeed +
            transform.right * Input.GetAxis("Horizontal") * moveSpeed);
    }

    private void Update()
    {
        turnRotation += Input.GetAxis("Mouse X");
    }

    private void OnDestroy()
    {
        portalableObject.HasTeleported -= PortalableObjectOnHasTeleported;
    }
}