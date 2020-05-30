using UnityEngine;

public abstract class AbstractClone : MonoBehaviour
{
    // Called when awake.
    public virtual void OnCloneAwake() { }

    // Called when the clone object is enabled.
    public virtual void OnCloneEnable(Portal sender, Portal destination) { }

    // Called on FixedUpdate and PortalChange. May be called multiple times, so do not rely on state and fixedDeltaTime.
    // Update your clone object here.
    public virtual void OnCloneUpdate(Portal sender, Portal destination) { }

    // Called when the clone object is disabled.
    public virtual void OnCloneDisable(Portal sender, Portal destination) { }
}