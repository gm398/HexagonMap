using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombatController : MonoBehaviour
{

    [SerializeField] int range = 1;
    [SerializeField] bool rangedUnit = false;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float damage = 10;
    [SerializeField] bool healer = false;
    [SerializeField] bool canAttackAir = true;
    
    LayerMask enemyLayers;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isMoving = false;
    [SerializeField] GameObject target;
    bool canCheckTarget = true;

    HexGrid hexGrid;
    UnitController controller;


    [SerializeField] AttackType attackType;

    private void Awake()
    {

        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        controller = this.gameObject.GetComponent<UnitController>();
        GameObject attackTypeObject = Instantiate(attackType.gameObject);
        attackTypeObject.transform.parent = this.transform;
        attackType = attackTypeObject.GetComponent<AttackType>();
        healer = attackType.IsHeal();
        if (healer)
        {
            enemyLayers = LayerMask.GetMask(LayerMask.LayerToName(this.gameObject.layer));
        }
        else
        {
            enemyLayers = controller.GetEnemyLayers();
        }
        if (!rangedUnit)
        {
            range = attackType.GetMaxRange();
        }
        if(range < 1) { range = 1; }
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack && !isMoving)
        {
            //AimAtTarget();
            UseAttackShape();
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



    void UseAttackShape()
    {

        //Collider[] enemys = Physics.OverlapSphere(this.transform.position, 1 + (range * 2), enemyLayers);
        if (Physics.CheckSphere(this.transform.position, 1 + (range * 2), enemyLayers))
        {
            if (rangedUnit)
            {
                Collider[] enemys = Physics.OverlapSphere(this.transform.position, 1 + (range * 2), enemyLayers);
                ChooseClosestTarget(enemys, out GameObject target);
                if(target != null)
                {
                    Vector3 centerOfAttack = target.GetComponentInChildren<HexCoordinates>().GetHexCoordsRQS();
                    List<GameObject> targetsHit = attackType.Attack(centerOfAttack, new Vector3(0,0,0), this.gameObject.layer);
                }
            }
            else
            {
                Vector3 currentHexCoords = controller.GetcurrentHex().GetHexCoordinates().GetHexCoordsRQS();
                int rotations = attackType.CanHit(currentHexCoords, enemyLayers);
                if (rotations >= 0)
                {
                    if (attackType == null) { Debug.Log("attacktype null"); }
                    LayerMask layer = this.gameObject.layer;
                    List<GameObject> targetsHit = attackType.Attack(currentHexCoords, rotations, layer);
                    if (targetsHit.Count > 0)
                    {
                        Invoke("ResetAttack", 1 / attackSpeed);
                        canAttack = false;
                    }
                }
            }
        }
    }
    bool ChooseClosestTarget(Collider[] enemys, out GameObject closest)
    {
        closest = null;
        Vector3 currentCoords = this.GetComponent<HexCoordinates>().GetHexCoordsRQS();
        hexGrid.GetHex(currentCoords, out Hex currentHex);
        foreach (Collider c in enemys)
        {
            if(c.gameObject.GetComponentInChildren<Health>() != null)
            {
                if(closest == null) { closest = c.gameObject; }
                else
                {
                    HexCoordinates hexCoords = c.gameObject.GetComponentInChildren<HexCoordinates>();
                    HexCoordinates closestCoords = closest.GetComponentInChildren<HexCoordinates>();
                    
                    if(hexCoords != null && closestCoords != null)
                    {
                        hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out Hex hex);
                        hexGrid.GetHex(closestCoords.GetHexCoordsRQS(), out Hex closestHex);
                        if(hex.DistanceFromHex(closestHex) > hex.DistanceFromHex(hex))
                        {
                            closest = c.gameObject;
                        }
                    }
                }
            }
        }
        if(closest == null) { return false; }
        hexGrid.GetHex(closest.GetComponentInChildren<HexCoordinates>().GetHexCoordsRQS(), out Hex targetHex);
        if(currentHex.DistanceFromHex(targetHex) > range)
        {
            closest = null;
            return false;
        }

        return true;
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

    public void SetIsMoving(bool isMoving) { this.isMoving = isMoving; }
    public bool IsHealer() { return healer; }
    public LayerMask GetTargetLayers() { return enemyLayers; }
}
