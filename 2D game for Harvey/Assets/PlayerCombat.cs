using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius = 1f;
    [SerializeField] float attacksPerSecond = 2f;
    [SerializeField] GameObject particles;
    bool canAttack = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1") && canAttack)
        {
            Destroy(
                Instantiate(particles, attackPoint.position, Quaternion.identity),
                1f);
            canAttack = false;
            Invoke("ResetAttack", 1 / attacksPerSecond);
        }
    }


    void ResetAttack()
    {
        canAttack = true;
    }
}
