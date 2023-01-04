using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectingRoclet : MonoBehaviour
{

    [SerializeField]
    float
        activationDelay = 2f,
        rocketForce = 1000f;
    [SerializeField]
    GameObject
        explosion,
        trail;
    public Vector3 target;

    float
        activationTime;
    public Rigidbody
        rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        activationTime = Time.time + activationDelay;
        trail.SetActive(false);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        transform.LookAt(transform.position + rb.velocity);
        if (activationTime < Time.time)
        {
            rb.AddForce((target - transform.position).normalized * rocketForce * Time.fixedDeltaTime);
            //rb.useGravity = false;
            trail.SetActive(true);
            
        }
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }
}
