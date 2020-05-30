using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObjectClone : MonoBehaviour
{
    public CloningBounds cloningBounds;
    private readonly HashSet<Portal> currentlyTouchingPortals = new HashSet<Portal>();
    private PortalableObject portalableObject;
    private AbstractClone[] clones;

    public bool local;
    public static PortalableObjectClone LocalInstance;

    public Portal ClosestTouchingPortal
    {
        get
        {
            var currentMin = (portal: (Portal) null, distance: float.PositiveInfinity);
            var referencePosition = cloningBounds.referenceTransform.position;
            foreach (var portal in currentlyTouchingPortals)
            {
                var closestPointOnPlane = portal.plane.ClosestPointOnPlane(referencePosition);
                var distance = Vector3.Distance(closestPointOnPlane, referencePosition);
                if (distance < currentMin.distance) currentMin = (portal, distance);
            }
            return currentMin.portal;
        }
    }
    
    private void Awake()
    {
        if (local)
            LocalInstance = this;
        
        portalableObject = GetComponent<PortalableObject>();
        portalableObject.HasTeleported += OnTeleported;
        
        clones = GetComponentsInChildren<AbstractClone>();
        foreach (var clone in clones)
            clone.OnCloneAwake();
        
        cloningBounds.PortalEnter += OnEnterPortal;
        cloningBounds.PortalExit += OnExitPortal;
    }
    
    private void OnDestroy()
    {
        portalableObject.HasTeleported -= OnTeleported;
        cloningBounds.PortalEnter -= OnEnterPortal;
        cloningBounds.PortalExit -= OnExitPortal;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            try
            {
                if (currentlyTouchingPortals.Count != 0)
                    UpdateClones();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    private void OnEnterPortal(Portal sender)
    {
        // Only call OnCloneEnable for the first portal entered
        
        if (currentlyTouchingPortals.Count == 0)
            foreach (var clone in clones)
                clone.OnCloneEnable(sender, sender.targetPortal);

        currentlyTouchingPortals.Add(sender);

        UpdateClones(); // Force update after portal change for new transforms
    }

    private void OnExitPortal(Portal sender)
    {
        currentlyTouchingPortals.Remove(sender);
        
        // Only call OnCloneDisable if all portals have been exited
        
        if (currentlyTouchingPortals.Count == 0)
            foreach (var clone in clones) 
                clone.OnCloneDisable(sender, sender.targetPortal);
    }

    private void OnTeleported(Portal sender, Portal destination, Vector3 newPosition, Quaternion newRotation)
    {
        // OnTrigger events won't fire until next tick (Portal crossing calculations happen after OnTrigger events),
        // so if a frame is going to be rendered after this tick, we need the currentlyTouchingPortals to be correct.
        // This means manually editing it with the known teleport info. Since it is a HashMap, this should not
        // have any ill effects when OnTrigger happens next tick.
        
        currentlyTouchingPortals.Remove(sender);
        currentlyTouchingPortals.Add(destination);
        UpdateClones(); // Force update after portal change for new transforms
    }
    
    private void UpdateClones()
    {
        var closestPortal = ClosestTouchingPortal;
        if (closestPortal == null)
            throw new Exception("No touching portals found when trying to update clones.");
        
        foreach (var clone in clones)
            clone.OnCloneUpdate(closestPortal, closestPortal.targetPortal);
    }
}
