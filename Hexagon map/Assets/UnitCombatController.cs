using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombatController : MonoBehaviour
{

    [SerializeField] int range = 1;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float damage = 10;
    LayerMask enemyLayers;
    bool canAttack = true;

    UnitController controller;

    private void Awake()
    {
        controller = this.gameObject.GetComponent<UnitController>();
        enemyLayers = controller.GetEnemyLayers();
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack)
        {
            AimAtTarget();
        }
    }


    void AimAtTarget()
    {
        GameObject closest = null;

        Collider[] enemys = Physics.OverlapSphere(this.transform.position, 1 + (range * 2), enemyLayers);
        Hex currentHex = controller.GetcurrentHex();
        foreach (Collider c in enemys)
        {
            UnitController enemyController = c.GetComponentInParent<UnitController>();
            if (enemyController != null)
            {
                if (enemyController.GetcurrentHex() != null)
                {
                    if (enemyController.GetcurrentHex().DistanceFromHex(currentHex) <= range)
                    {
                        if (closest == null) { closest = c.gameObject; }
                        else if (enemyController.GetcurrentHex().DistanceFromHex(currentHex)
                            < closest.GetComponentInParent<UnitController>().GetcurrentHex().DistanceFromHex(currentHex))
                        {
                            closest = c.gameObject;
                        }
                    }
                }
            }
        }
        
        if (closest != null)
        {
            //this.transform.LookAt(closest.transform.position);
            Debug.Log("looking at a target");
            Health enemyHealth = closest.GetComponentInParent<Health>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                canAttack = false;
                Invoke("resetAttack", 1 / attackSpeed);
            }


        } //else {Debug.Log("no closest"); }
    }

    void resetAttack()
    {
        canAttack = true;
    }

}
