using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float maxZoom = 1f, minZoom = 10f;
    private float currentZoom;

    private void Awake()
    {
        currentZoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        
        if (Input.GetKey("mouse 2"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            float mouseX = -Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            float mouseY = -Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            this.transform.Translate(new Vector3(mouseX, 0, mouseY), Space.World);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }



        float zoom = Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        currentZoom -= zoom;
        
        if(currentZoom < maxZoom) { currentZoom = maxZoom; }
        else if(currentZoom > minZoom) { currentZoom = minZoom; }
        cam.orthographicSize = currentZoom;
    }
}
