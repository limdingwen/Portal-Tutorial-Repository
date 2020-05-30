using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    private NavMeshAgent agent;
    private PortalableObject portalableObject;
    private Vector3? currentDestination;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        portalableObject = GetComponent<PortalableObject>();
        portalableObject.HasTeleported += PortalableObjectOnHasTeleported;
    }

    private void OnDestroy()
    {
        portalableObject.HasTeleported -= PortalableObjectOnHasTeleported;
    }

    private void PortalableObjectOnHasTeleported(Portal sender, Portal destination, Vector3 newPosition, Quaternion newRotation)
    {
        if (!agent.Warp(newPosition))
            Debug.LogWarning($"Warp failed for {gameObject.name} NavMeshAgent.");

        if (currentDestination != null)
        {
            var path = new NavMeshPath();
            agent.CalculatePath(currentDestination.Value, path);
            agent.SetPath(path);
        }
    }

    private void FixedUpdate()
    {
        if (agent.isOnOffMeshLink)
        {
            // Move character towards OffMeshLink start point
            // Should only really be used if the AI reaches the link without finishing going through the portal.

            transform.Translate(Vector3.ProjectOnPlane(agent.currentOffMeshLinkData.startPos - transform.position, Vector3.up).normalized * (agent.speed * Time.fixedDeltaTime));
        }
    }

    public void Goto(Vector3 destination)
    {
        currentDestination = destination;
        agent.SetDestination(destination);
    }
}
