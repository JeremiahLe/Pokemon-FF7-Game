using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    public float sensitivity = 2.5f;
    public float screenEdge = 100f;
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
