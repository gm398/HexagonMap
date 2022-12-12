using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float force = 1.5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform cam;
    [SerializeField] LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(KeyCode.W, cam.forward * force);
        Move(KeyCode.S, cam.forward * -force);
        Move(KeyCode.D, cam.right * force);
        Move(KeyCode.A, cam.right * -force);
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, .6f, groundLayer);
    }
    void Move(KeyCode k, Vector3 direction)
    {
        if (Input.GetKey(k))
        {
            rb.AddForce(direction * Time.deltaTime);
        }
    }
}
