using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPack : MonoBehaviour
{
    [SerializeField]
    List<Transform> 
        muzzels;

    [SerializeField]
    GameObject
        rocket,
        smoke;
    [SerializeField]
    float 
        force,
        shotsPerSec;
    float
        timeOfNextShot = 0;

    int currentMuzzel = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        if (!gameObject.activeSelf)
        { return; }
        if (timeOfNextShot < Time.time)
        {
            if (smoke != null)
            {
                GameObject s = Instantiate(smoke, muzzels[currentMuzzel].position, muzzels[currentMuzzel].rotation);
                Destroy(s, 1.5f);
            }
            GameObject b = Instantiate(rocket, muzzels[currentMuzzel].position, muzzels[currentMuzzel].rotation);
            Rigidbody rb = b.GetComponent<Rigidbody>();
            rb.AddForce(muzzels[currentMuzzel].forward * force, ForceMode.Impulse);
            timeOfNextShot = Time.time + 1 / shotsPerSec;
            Destroy(b, 3f);

            currentMuzzel++;
            if(currentMuzzel >= muzzels.Count)
            {
                currentMuzzel = 0;
                timeOfNextShot += 1f;
            }
        }
    }
}
