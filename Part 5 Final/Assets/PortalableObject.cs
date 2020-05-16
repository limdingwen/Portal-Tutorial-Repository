using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    public delegate void HasTeleportedHandler(Portal sender, Portal destination, Vector3 newPosition, Quaternion newRotation);
    public event HasTeleportedHandler HasTeleported;

    public void OnHasTeleported(Portal sender, Portal destination, Vector3 newPosition, Quaternion newRotation)
    {
        HasTeleported?.Invoke(sender, destination, newPosition, newRotation);
    }
}