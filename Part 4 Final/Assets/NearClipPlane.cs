using UnityEngine;

public class NearClipPlane : MonoBehaviour
{
    public float nearClipPlane = 0.0001f;
    
    private void Start()
    {
        GetComponent<Camera>().nearClipPlane = nearClipPlane;
    }
}