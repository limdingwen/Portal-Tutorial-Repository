using UnityEngine;

public class PortalRenderer : MonoBehaviour
{
    public Camera portalCamera;
    public int maxRecursions = 2;
    
    public int debugTotalRenderCount;

    private Camera mainCamera;
    private Portal[] allPortals;

    private void Start()
    {
        mainCamera = Camera.main;
        allPortals = FindObjectsOfType<Portal>();
    }

    private void OnPreRender()
    {
        debugTotalRenderCount = 0;

        foreach (var portal in allPortals)
        {
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

    private void OnPostRender()
    {
        RenderTexturePool.Instance.ReleaseAllTextures();
    }
}