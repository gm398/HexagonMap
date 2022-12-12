using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    GameObject 
        bullet,
        smoke;
    [SerializeField]
    float
        force = 20,
        shotsPerSec = 5;
    float
        timeOfNextShot = 0;
    [SerializeField]
    Transform muzzel;

      

    public void Shoot()
    {
        if (timeOfNextShot < Time.time)
        {
            if (smoke != null)
            {
                GameObject s = Instantiate(smoke, muzzel.position, muzzel.rotation);
                Destroy(s, 1.5f);
            }
            GameObject b = Instantiate(bullet, muzzel.position, muzzel.rotation);
            Rigidbody rb = b.GetComponent<Rigidbody>();
            rb.AddForce(muzzel.forward * force, ForceMode.Impulse);
            timeOfNextShot = Time.time + 1 / shotsPerSec;
        }
    }
}
