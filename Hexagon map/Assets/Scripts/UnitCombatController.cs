using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombatController : MonoBehaviour
{

    [SerializeField] int range = 1;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float damage = 10;
    [SerializeField] bool healer = false;
    [SerializeField] bool canAttackAir = true;
    LayerMask enemyLayers;
    [SerializeField] bool canAttack = true;
    
    [SerializeField] GameObject target;
    bool canCheckTarget = true;

    HexGrid hexGrid;
    UnitController controller;

    private void Awake()
    {

        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        controller = this.gameObject.GetComponent<UnitController>();
        if (healer)
        {
            enemyLayers = LayerMask.GetMask(LayerMask.LayerToName(this.gameObject.layer));
        }
        else
        {
            enemyLayers = controller.GetEnemyLayers();
        }
        if(range < 1) { range = 1; }
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack)
        {
            AimAtTarget();
        }
        if (canCheckTarget)
        {
            Invoke("CheckTarget", 1f);
            canCheckTarget = false;
        }
    }


    void AimAtTarget()
    {
     
        Hex currentHex = controller.GetcurrentHex();
        if(currentHex == null) { return; }
        GameObject closest = null;
        bool targetInRange = false;
        if (target != null)
        {
            if (!target.activeInHierarchy) { target = null; }
            else {
                //targetInRange = target.GetComponentInParent<HexCoordinates>().GetcurrentHex().DistanceFromHex(currentHex) <= range;
                Hex hex;
                hexGrid.GetHex(target.GetComponentInParent<HexCoordinates>().GetHexCoordsRQS(), out hex);
                targetInRange = hex.DistanceFromHex(currentHex) <= range;
            }
        }
        if (!canAttackAir)
        {
            if (target.GetComponentInParent<UnitController>().CanFly())
            {
                targetInRange = false;
            }
        }
        if (!targetInRange)
        {
            Collider[] enemys = Physics.OverlapSphere(this.transform.position, 1 + (range * 2), enemyLayers);

            foreach (Collider c in enemys)
            {
                if (!c.transform.parent.gameObject.Equals(this.gameObject))
                {
                    UnitController enemyController = c.GetComponentInParent<UnitController>();
                    if (enemyController != null)
                    {
                        if (enemyController.GetcurrentHex() != null)
                        {
                            if (enemyController.GetcurrentHex().DistanceFromHex(currentHex) <= range &&
                                (canAttackAir || 
                                    (!canAttackAir && !enemyController.CanFly())))
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
        }
        else
        {
            if (enemyLayers == (enemyLayers & (1 << target.layer)))
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
                Invoke("ResetAttack", 1 / attackSpeed);
            }


        } //else {Debug.Log("no closest"); }
    }

    //checks to see if the current target is still in range and visible, if not then updates pathfinding
    void CheckTarget()
    {
        if (controller.IsDead()) { return; }
        canCheckTarget = true;
        if (target != null)
        {
            UnitController targetController = target.GetComponentInParent<UnitController>();
            if (targetController != null)
            {
                if (!targetController.IsVisible())
                {
                    target = null;
                    return;
                }
                if (!EnemyInRange())
                {
                    controller.SetTargetEnemy(target);
                }
            }
        }
    }
    void ResetAttack()
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


    public bool IsHealer() { return healer; }
    public LayerMask GetTargetLayers() { return enemyLayers; }
}
