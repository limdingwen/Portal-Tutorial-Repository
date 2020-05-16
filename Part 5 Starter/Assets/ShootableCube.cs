using UnityEngine;

public class ShootableCube : MonoBehaviour
{
    private new Renderer renderer;
    private bool isGreen;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void OnShoot()
    {
        isGreen = !isGreen;
        renderer.material.color = isGreen ? Color.green : Color.white;
    }
}
