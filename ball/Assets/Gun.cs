using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrel;
    [SerializeField] float force = 50f;
    [SerializeField] float attacksPerSec = 5f;
    [SerializeField] Transform ball;
    float attackCD = 0f;
    float attackTime = 0f;
    bool canAttack = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && canAttack)
        {
            GameObject g = Instantiate(bullet, barrel.position, barrel.rotation);
            g.GetComponent<Rigidbody>().velocity = ball.GetComponent<Rigidbody>().velocity;
            g.GetComponent<Rigidbody>().AddForce(barrel.forward * force, ForceMode.Impulse);
            ball.GetComponent<Rigidbody>().AddForce(barrel.forward * -force, ForceMode.Impulse);

            //Invoke("ResetAttack", 1 / attacksPerSec);
            canAttack = false;
        }
        
        if (attackCD > attackTime)
        {
            canAttack = true;
            attackTime = Time.time + 1 / attacksPerSec;
        }
        attackCD += Time.deltaTime;
    }


    void ResetAttack()
    {
        canAttack = true;
    }
}
