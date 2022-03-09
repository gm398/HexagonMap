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
    GameObject target;

    UnitController controller;

    private void Awake()
    {
        controller = this.gameObject.GetComponent<UnitController>();
        enemyLayers = controller.GetEnemyLayers();
        if(range < 1) { range = 1; }
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
        Hex currentHex = controller.GetcurrentHex();
        GameObject closest = null;
        bool targetInRange = false;
        if (target != null)
        {
            targetInRange = target.GetComponentInParent<UnitController>().GetcurrentHex().DistanceFromHex(currentHex) <= range;
        }
        if (!targetInRange)
        {
            Collider[] enemys = Physics.OverlapSphere(this.transform.position, 1 + (range * 2), enemyLayers);

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
        }
        else {
            if (enemyLayers == (enemyLayers | (1 << target.layer)))
            {
                closest = target;
            }
        }
        
        if (closest != null)
        {
            //Debug.Log("looking at a target");
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

    public bool SetTarget(GameObject newTarget) {
        if(newTarget == null) { target = null; return false; }
        Health h = newTarget.GetComponentInParent<Health>();
        if(h != null) { target = newTarget; return true; }
        else { Debug.Log("Invalid target"); return false; }
    }


    public GameObject GetTarget() { return target; }
    public int GetRange() { return range; }
    public bool EnemyInRange()
    {
        Hex currentHex = controller.GetcurrentHex();
        UnitController enemyController = target.GetComponentInParent<UnitController>();
        if (enemyController != null)
        {
            if (enemyController.GetcurrentHex() != null)
            {
                return enemyController.GetcurrentHex().DistanceFromHex(currentHex) <= range;
            }
        }
        return false;
    }
}
