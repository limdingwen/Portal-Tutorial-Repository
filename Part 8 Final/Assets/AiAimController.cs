using System.Collections.Generic;
using UnityEngine;

public class AiAimController : MonoBehaviour
{
    public Transform target;

    public LayerMask layerMask;
    public int maxRecursions;
    public int maxApparentPositions;
    
    private static readonly List<TargetApparentPosition> AimingTargetApparentPositions = new List<TargetApparentPosition>();
    private static readonly Queue<List<Portal>> AimingPortalChainQueue = new Queue<List<Portal>>(); // Chain of SOURCE portals.
    
    private PortalOcclusionVolume[] occlusionVolumes;

    private void Start()
    {
        occlusionVolumes = FindObjectsOfType<PortalOcclusionVolume>();
    }

    private void FixedUpdate()
    {
        PortalOcclusionVolume currentOcclusionVolume = null;
        foreach (var occlusionVolume in occlusionVolumes)
        {
            if (occlusionVolume.collider.bounds.Contains(transform.position))
            {
                currentOcclusionVolume = occlusionVolume;
                break;
            }
        }

        var bestApparentPosition = FindBestApparentPosition(
            transform.position,
            target.position,
            "Player Visibility Checker",
            layerMask,
            currentOcclusionVolume,
            maxRecursions,
            maxApparentPositions
            );
        if (bestApparentPosition != null)
        {
            transform.LookAt(bestApparentPosition.ApparentPosition);
        }
    }
    
    public static TargetApparentPosition FindBestApparentPosition(
        Vector3 origin,
        Vector3 target,
        string targetTag,
        LayerMask layerMask,
        PortalOcclusionVolume occlusionVolume,
        int maxRecursions,
        int maxApparentPositions)
    {
        AimingPortalChainQueue.Clear();
        AimingTargetApparentPositions.Clear();

        // Breadth first search

        AimingPortalChainQueue.Enqueue(new List<Portal>());
        while (AimingPortalChainQueue.Count > 0)
        {
            var currentChain = AimingPortalChainQueue.Dequeue();
            
            // Calculate apparent location

            var targetApparentPosition = target;
            foreach (var portal in currentChain)
                targetApparentPosition = Portal.TransformPositionBetweenPortals(portal.targetPortal, portal,
                    targetApparentPosition);

            var aimDirection = targetApparentPosition - origin;
            aimDirection.Normalize();

            // Calculate visibility
            // If visible, add to target apparent positions

            if (Portal.RaycastRecursive(
                origin,
                aimDirection,
                currentChain.Count,
                null,
                (x, y) => layerMask,
                out var hitInfo))
            {
                if (hitInfo.collider.CompareTag(targetTag))
                {
                    AimingTargetApparentPositions.Add(new TargetApparentPosition
                    {
                        ApparentPosition = targetApparentPosition,
                        AimDirection = aimDirection
                    });
                }
            }

            // Return if enough for an accurate heuristic

            if (maxApparentPositions > 0)
                if (AimingTargetApparentPositions.Count >= maxApparentPositions)
                    break;
            
            // Continue search; add to queue

            if (currentChain.Count >= maxRecursions) continue;
            foreach (var visiblePortal in currentChain.Count > 0 ? currentChain[currentChain.Count-1].visiblePortals : occlusionVolume == null ? new Portal[0] : occlusionVolume.portals)
                AimingPortalChainQueue.Enqueue(new List<Portal>(currentChain) { visiblePortal });
        }

        // Use heuristic (closest apparent) to find which of the apparent positions should the AI shoot at

        TargetApparentPosition bestApparentPosition = null;
        var minDistance = Mathf.Infinity;
        foreach (var aimingTargetApparentPosition in AimingTargetApparentPositions)
        {
            var distance = Vector3.Distance(aimingTargetApparentPosition.ApparentPosition, origin);
            if (distance >= minDistance) continue;
            minDistance = distance;
            bestApparentPosition = aimingTargetApparentPosition;
        }

        return bestApparentPosition;
    }
    
    public class TargetApparentPosition
    {
        public Vector3 ApparentPosition;
        public Vector3 AimDirection;
    }
}
