using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisrtPersonMovement : MonoBehaviour
{

    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 10;
    
    bool isGrounded;
    
    Vector3 velocity;
    float hInput, vInput;

    bool isJumping;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float jumpGravity = -20;
    [SerializeField] float fallGravity = -40;
    float gravity = 0f;

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        isJumping = Input.GetButton("Jump");

        Gravity();
        GroundMovement();

        controller.Move(velocity * Time.deltaTime);
    }
    
    public void GroundMovement()
    {
        
        Vector3 move = transform.right * hInput + transform.forward * vInput;
        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        if(velocity.y > 0) { gravity = jumpGravity; }
        else { gravity = fallGravity; }
        if(isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * jumpGravity);
        }
            
    }

    public void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
    }

}
