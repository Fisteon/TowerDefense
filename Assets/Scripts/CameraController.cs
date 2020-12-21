using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0.25f, 1f)]
    public float panningSpeed;
    private int borderDelta = 5;

    public int cameraLimitLeft;
    public int cameraLimitRight;
    public int cameraLimitUp;
    public int cameraLimitDown;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ARROW PANNING
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left, panningSpeed);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right, panningSpeed);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward, panningSpeed);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.back, panningSpeed);
        }
         
        /*// EDGE PANNING
            // Up
        if (Input.mousePosition.y >= Screen.height - borderDelta && transform.position.x > cameraLimitUp)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left, panningSpeed);
        }
            // Down
        if (Input.mousePosition.y <= 0 + borderDelta && transform.position.x < cameraLimitDown) 
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right, panningSpeed);
        }
            // Left
        if (Input.mousePosition.x <= 0 + borderDelta && transform.position.z > cameraLimitLeft)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.back, panningSpeed);
        }
            // Right
        if (Input.mousePosition.x >= Screen.width - borderDelta && transform.position.z < cameraLimitRight)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward, panningSpeed);
        }*/
    }
}
