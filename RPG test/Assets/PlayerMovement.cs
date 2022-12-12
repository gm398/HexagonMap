using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    [SerializeField] float speed = 10f;

    Rigidbody rb;
    

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        float hor = speed * Input.GetAxis("Horizontal") * Time.deltaTime;
        float ver = speed * Input.GetAxis("Vertical") * Time.deltaTime;

        rb.AddForce(new Vector3(hor, 0, ver));
    }
}
