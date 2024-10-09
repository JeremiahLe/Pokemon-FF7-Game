using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    [Tooltip("How fast does the camera pan left/right?")] 
    public float sensitivity = 2.5f;
    
    [Tooltip("Where does the panning begin? Less covers less screen area.")]
    public float screenEdge = 100f;
    
    [Tooltip("How far left/right are you allowed to pan?")]
    public float cameraXBounds = 5f;

    private float currentXPosition;
    private float initialXPosition;

    private void Awake()
    {
        currentXPosition = transform.position.x;
        initialXPosition = currentXPosition;
    }

    void LateUpdate()
    {
        if (Input.mousePosition.x>Screen.width-screenEdge && currentXPosition < initialXPosition + cameraXBounds)
        {
            currentXPosition += sensitivity * Time.deltaTime;
        }
        else if(Input.mousePosition.x<screenEdge && currentXPosition > initialXPosition - cameraXBounds)
        {
            currentXPosition -= sensitivity * Time.deltaTime;
        }
        
        transform.position = new Vector3(currentXPosition, transform.position.y, transform.position.z);
    }
}
