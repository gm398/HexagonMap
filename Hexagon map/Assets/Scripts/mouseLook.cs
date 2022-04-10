using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    private float xRotation = 0f;
    [SerializeField] string camZoom = "t";
    [SerializeField] Camera cam;
    [SerializeField] Transform CamAnchor;
    [SerializeField] Transform ThirdPersonAnchor;
    private bool isFP = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isFP = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        CamAnchor.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);

        if(Input.GetKey(camZoom))
        {
            cam.fieldOfView = 45;
        }
        else { cam.fieldOfView = 60; }
        if (Input.GetKeyDown("tab"))
        {
            SwitchCams();
        }
    }

    private void SwitchCams()
    {
        if (isFP)
        {
            transform.position = ThirdPersonAnchor.position;
        }
        else
        {
            transform.position = CamAnchor.position;
        }
        isFP = !isFP;
    }
}
