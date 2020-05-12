using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private PortalableObject portalableObject;

    public float moveSpeed = 5;
    public float turnSpeed = 5;

    private float turnRotation;

    public Transform playerCamera;
    private float verticalRotationAbsolute;
    
    public LayerMask shootingMask;
    private bool shoot;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        portalableObject = GetComponent<PortalableObject>();
        portalableObject.HasTeleported += PortalableObjectOnHasTeleported;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        
        // Turn player (up/down)

        playerCamera.localRotation = Quaternion.Euler(verticalRotationAbsolute, 0, 0);
        
        // Move player
        
        characterController.SimpleMove(
            transform.forward * Input.GetAxis("Vertical") * moveSpeed +
            transform.right * Input.GetAxis("Horizontal") * moveSpeed);
        
        // Shoot

        if (shoot)
        {
            if (Portal.RaycastRecursive(playerCamera.position,
                playerCamera.forward,
                shootingMask.value,
                8,
                out var hitInfo))
            {
                hitInfo.collider.SendMessage("OnShoot", SendMessageOptions.DontRequireReceiver);
            }

            shoot = false;
        }
    }

    private void Update()
    {
        turnRotation += Input.GetAxis("Mouse X");
        
        verticalRotationAbsolute += Input.GetAxis("Mouse Y") * -turnSpeed;
        verticalRotationAbsolute = Mathf.Clamp(verticalRotationAbsolute, -89, 89);

        if (Input.GetButtonDown("Fire1")) shoot = true;
    }

    private void OnDestroy()
    {
        portalableObject.HasTeleported -= PortalableObjectOnHasTeleported;
    }
}