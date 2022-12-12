using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject trail;
    private void OnCollisionEnter(Collision collision)
    {
        if (explosion != null)
        {
            GameObject e = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(e, 3f);
            foreach(Collider hit in Physics.OverlapSphere(transform.position, 1f))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    rb.AddForce((hit.transform.position - transform.position).normalized * 5, ForceMode.Impulse);
                }
            }
        }
        if(trail != null)
        {
            trail.transform.parent = null;
            trail.GetComponent<ParticleSystem>().Stop();
            Destroy(trail, 3f);
        }
        Destroy(this.gameObject);
    }
}
