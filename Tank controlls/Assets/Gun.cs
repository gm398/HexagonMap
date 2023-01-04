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
    Transform 
        muzzel,
        rotateBarrel;
    [SerializeField]
    int
        numOfBarrels;
    
    private void Update()
    {
        if(rotateBarrel == null)
        { return; }
        if (timeOfNextShot > Time.time)
        {
            float deg = 360 / numOfBarrels * shotsPerSec * Time.deltaTime;
            rotateBarrel.Rotate(0, 0, deg);
        }
        else
        { rotateBarrel.localRotation = Quaternion.identity; }
    }
    public void Shoot()
    {
        if (!gameObject.activeSelf)
        { return; }
        if (timeOfNextShot < Time.time)
        {
            if (smoke != null)
            {
                GameObject s = Instantiate(smoke, muzzel.position, muzzel.rotation);
                s.transform.localScale = transform.lossyScale;
                Destroy(s, 1.5f);
            }
            GameObject b = Instantiate(bullet, muzzel.position, muzzel.rotation);
            Rigidbody rb = b.GetComponent<Rigidbody>();
            rb.AddForce(muzzel.forward * force, ForceMode.Impulse);
            timeOfNextShot = Time.time + 1 / shotsPerSec;
            //Destroy(b, 3f);
        }
    }
}
