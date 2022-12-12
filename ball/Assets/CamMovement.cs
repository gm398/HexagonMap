using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 200;
    [SerializeField] Transform camHolder;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        float rotY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
        transform.Rotate(-rotY, 0, 0);
        camHolder.Rotate(0, rotX, 0);
        
    }
}
