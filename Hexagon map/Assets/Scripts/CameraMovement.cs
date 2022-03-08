using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float panSpeed = 100f;
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float maxZoom = 1f, minZoom = 10f;
    private float currentZoom;
    private Vector3 rotationPoint;

    private void Awake()
    {
        currentZoom = cam.orthographicSize;
        SetRotationPoint();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        Cursor.lockState = CursorLockMode.Confined;
        float mouseX = -Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        bool middleMouse = Input.GetKey("mouse 2");
        bool rightClick = Input.GetKey("mouse 1");
        if (!rightClick && middleMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Vector3 direction = transform.right * mouseX + transform.forward * mouseY;
            direction.y = 0;
            
            this.transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
        else if(rightClick && middleMouse)
        {
            SetRotationPoint();
            Cursor.lockState = CursorLockMode.Locked;
            mouseX = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;//uses panSpeed rather than moveSpeed
            //this.transform.Rotate(new Vector3(0, mouseX, 0), Space.World);
            this.transform.RotateAround(rotationPoint, Vector3.up, mouseX);
        }



        float zoom = Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        currentZoom -= zoom;
        
        if(currentZoom < maxZoom) { currentZoom = maxZoom; }
        else if(currentZoom > minZoom) { currentZoom = minZoom; }
        cam.orthographicSize = currentZoom;
    }


    void SetRotationPoint()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 50))
        {
            rotationPoint = hit.transform.position;
        }
        else
        {
            rotationPoint = this.transform.position;
        }
    }
}
