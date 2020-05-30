using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    private PortalRenderer portalRenderer;
    private TextMeshProUGUI textMesh;
    
    private void Start()
    {
        portalRenderer = FindObjectOfType<PortalRenderer>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        textMesh.text = $"# of Portal renders: {portalRenderer.debugTotalRenderCount}, total renders: {portalRenderer.debugTotalRenderCount + 1}";
    }
}
