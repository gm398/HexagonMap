using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] SpriteRenderer body;
    Rigidbody2D rb;
    bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float yVel = rb.velocity.y;
        if (Input.GetButtonDown("Jump"))
        {
            yVel = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        }
        rb.velocity =  new Vector3(Input.GetAxis("Horizontal") * speed, yVel, 0);
        if(rb.velocity.x > 0.01 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(rb.velocity.x < -0.01 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }
}
