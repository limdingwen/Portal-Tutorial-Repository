using UnityEngine;

public class PortalRenderer : MonoBehaviour
{
    public Camera portalCamera;
    public int maxRecursions = 2;
    
    public int debugTotalRenderCount;

    private Camera mainCamera;
    private PortalOcclusionVolume[] occlusionVolumes;

    private void Start()
    {
        mainCamera = Camera.main;
        occlusionVolumes = FindObjectsOfType<PortalOcclusionVolume>();
    }

    private void OnPreRender()
    {
        debugTotalRenderCount = 0;

        PortalOcclusionVolume currentOcclusionVolume = null;
        foreach (var occlusionVolume in occlusionVolumes)
        {
            if (occlusionVolume.collider.bounds.Contains(mainCamera.transform.position))
            {
                currentOcclusionVolume = occlusionVolume;
                break;
            }
        }

        if (currentOcclusionVolume != null)
        {
            var cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

            foreach (var portal in currentOcclusionVolume.portals)
            {
                if (!portal.ShouldRender(cameraPlanes)) continue;
                
                portal.RenderViewthroughRecursive(
                    mainCamera.transform.position,
                    mainCamera.transform.rotation,
                    out _,
                    out _,
                    out var renderCount,
                    portalCamera,
                    0,
                    maxRecursions);

                debugTotalRenderCount += renderCount;
            }
        }
    }

    private void OnPostRender()
    {
        RenderTexturePool.Instance.ReleaseAllTextures();
    }
}