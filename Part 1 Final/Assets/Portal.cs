using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal targetPortal;
    
    public Transform normalVisible;
    public Transform normalInvisible;

    public Camera portalCamera;
    public Renderer viewthroughRenderer;
    private RenderTexture viewthroughRenderTexture;
    private Material viewthroughMaterial;

    private Camera mainCamera;

    private Vector4 vectorPlane;

    public static Vector3 TransformPositionBetweenPortals(Portal sender, Portal target, Vector3 position)
    {
        return
            target.normalInvisible.TransformPoint(
                sender.normalVisible.InverseTransformPoint(position));
    }

    public static Quaternion TransformRotationBetweenPortals(Portal sender, Portal target, Quaternion rotation)
    {
        return
            target.normalInvisible.rotation *
            Quaternion.Inverse(sender.normalVisible.rotation) *
            rotation;
    }

    private void Start()
    {
        // Create render texture
        
        viewthroughRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        viewthroughRenderTexture.Create();
        
        // Assign render texture to portal material (cloned)
        
        viewthroughMaterial = viewthroughRenderer.material;
        viewthroughMaterial.mainTexture = viewthroughRenderTexture;
        
        // Assign render texture to portal camera
        
        portalCamera.targetTexture = viewthroughRenderTexture;
        
        // Cache the main camera
        
        mainCamera = Camera.main;
        
        // Generate bounding plane

        var plane = new Plane(normalVisible.forward, transform.position);
        vectorPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
    }

    private void LateUpdate()
    {
        // Calculate portal camera position and rotation

        var virtualPosition = TransformPositionBetweenPortals(this, targetPortal, mainCamera.transform.position);
        var virtualRotation = TransformRotationBetweenPortals(this, targetPortal, mainCamera.transform.rotation);
        
        // Position camera
        
        portalCamera.transform.SetPositionAndRotation(virtualPosition, virtualRotation);
        
        // Calculate projection matrix
        
        var clipThroughSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix))
            * targetPortal.vectorPlane;
        
        // Set portal camera projection matrix to clip walls between target portal and portal camera
        // Inherits main camera near/far clip plane and FOV settings
        
        var obliqueProjectionMatrix = mainCamera.CalculateObliqueMatrix(clipThroughSpace);
        portalCamera.projectionMatrix = obliqueProjectionMatrix;
    }

    private void OnDestroy()
    {
        // Release render texture from GPU
        
        viewthroughRenderTexture.Release();
   
        // Destroy cloned material and render texture
        
        Destroy(viewthroughMaterial);
        Destroy(viewthroughRenderTexture);
    }
}
