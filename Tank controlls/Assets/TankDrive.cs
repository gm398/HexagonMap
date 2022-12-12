using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDrive : MonoBehaviour
{
    [SerializeField] float driveForce = 3, turnSpeed = 10;
    [SerializeField] Transform camHolder;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(camHolder == null)
        { return; }
        Vector2 direc = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            Vector2 dir = new Vector2(camHolder.forward.x, camHolder.forward.z);
            // MoveTank(dir);
            direc += dir;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector2 dir = -new Vector2(camHolder.forward.x, camHolder.forward.z);
            //MoveTank(-dir);
            direc += dir;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Vector2 dir = new Vector2(camHolder.right.x, camHolder.right.z);
            //MoveTank(dir);
            direc += dir;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Vector2 dir = -new Vector2(camHolder.right.x, camHolder.right.z);
            // MoveTank(-dir);
            direc += dir;
        }

        int forward = 1;
        if(camHolder.localEulerAngles.y > 90 && camHolder.localEulerAngles.y < 270)
        {
            forward = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector2 dir = forward * new Vector2(transform.right.x, transform.right.z) * 100 + new Vector2(camHolder.forward.x, camHolder.forward.z);
            direc += dir.normalized;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector2 dir = forward *  -new Vector2(transform.right.x, transform.right.z) * 100 + new Vector2(camHolder.forward.x, camHolder.forward.z);
            direc += dir.normalized;
        }

        if (direc != new Vector2(0, 0))
        {
            MoveTank(direc);
        }

    }

    void MoveTank(Vector2 direction)
    {
        Vector2 facing = new Vector2(transform.forward.x, transform.forward.z);
        float angle =  Vector2.SignedAngle(facing, direction); //camHolder.localEulerAngles.y;
        angle *= Mathf.Deg2Rad;
        
        int x = 1;
        if (Mathf.Cos(angle) < 0)
        { x = -1; }
        transform.Rotate(0, x * -Mathf.Sin(angle) * turnSpeed * Time.deltaTime, 0);
        rb.velocity = transform.forward * Mathf.Cos(angle) * driveForce;
    }

    public void SetCamHolder(Transform cam)
    {
        camHolder = GetComponentInChildren<TurretController>().transform;
    }
}
