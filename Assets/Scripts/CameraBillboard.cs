using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    private Camera camera;
    
    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        transform.forward = camera.transform.forward;
    }
}
