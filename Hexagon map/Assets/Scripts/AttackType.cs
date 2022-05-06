using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType : MonoBehaviour
{
    [SerializeField] GameObject attackShapeObject;
    AttackShape attackShape;
    [SerializeField] float damage = 5f;
    [SerializeField] GameObject attackGraphic;
    [SerializeField] float graphicTimer = 3f;
    [SerializeField] bool friendlyFire = false;
    [SerializeField] bool heal = false;


    HexGrid hexGrid;
    private void Awake()
    {
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        GameObject attackShapeObject = Instantiate(this.attackShapeObject);
        attackShapeObject.transform.parent = this.transform;
        attackShape = attackShapeObject.GetComponent<AttackShape>();
    }
    public List<GameObject> Attack(Vector3 center, int direction, LayerMask targetLayer, bool canAttackAir)
    {
        Vector3 direc = attackShape.GetDirectionVector(direction);
        return Attack(center, direc, targetLayer, canAttackAir);
    }
    public List<GameObject> Attack(Vector3 center, Vector3 direction, LayerMask targetLayer, bool canAttackAir)
    {
        if(direction == null)
        {
            direction = attackShape.GetDefaultDirection();
        }
        List<Vector3> effectedHexs = attackShape.GetShapeFacingDirection(direction);
        List<GameObject> targetsHit = new List<GameObject>();
        Hex hex = null;
        foreach(Vector3 pos in effectedHexs)
        {
            if (hexGrid.GetHex(pos + center, out hex))
            {
                GameObject occupant = hex.GetOccupant();
                if (occupant != null)
                {
                    bool isEnemy = (targetLayer.value & (1 << occupant.layer)) > 0;
                    Health health = occupant.GetComponentInChildren<Health>();
                    bool cantAttackOccupant = false;
                    if (!canAttackAir)
                    {
                        UnitController uc = occupant.GetComponent<UnitController>();
                        if(uc != null)
                        {
                            cantAttackOccupant = uc.CanFly();
                        }
                    }
                    if (health != null && !cantAttackOccupant)
                    {
                        if (friendlyFire || isEnemy)
                        {
                            health.TakeDamage(damage);
                            targetsHit.Add(occupant);
                            SpawnGraphic(hex);
                        }
                    }
                }
                else if(!heal)
                {
                    SpawnGraphic(hex);
                }
            }
        }
        return targetsHit;
    }
    public void PlayerAttack(Vector3 center, Vector3 direction, LayerMask targetLayer, bool canAttackAir)
    {
        List<Vector3> effectedHexs = attackShape.GetShapeFacingDirection(direction);
        Hex hex = null;
        foreach (Vector3 pos in effectedHexs)
        {
            if (hexGrid.GetHex(pos + center, out hex))
            {
                GameObject occupant = hex.GetOccupant();
                if (occupant != null)
                {
                    bool isEnemy = (targetLayer.value & (1 << occupant.layer)) > 0;
                    Health health = occupant.GetComponentInChildren<Health>();
                    bool cantAttackOccupant = false;
                    if (!canAttackAir)
                    {
                        UnitController uc = occupant.GetComponent<UnitController>();
                        if (uc != null)
                        {
                            cantAttackOccupant = uc.CanFly();
                        }
                    }
                    if (health != null && !cantAttackOccupant)
                    {
                        health.TakeDamage(damage);
                        SpawnGraphic(hex);
                    }
                        
                       
                }
                else 
                {
                    SpawnGraphic(hex);
                }
            }
        }
    }

    void SpawnGraphic(Hex hex)
    {
        GameObject graphic = Instantiate(attackGraphic, hex.GetTargetPoint(), Quaternion.identity);
        Destroy(graphic, graphicTimer);
    }
    

    //returns a list of ints repersenting directions that can hit an enemy
    public List<int> CanHit(Vector3 center, LayerMask targetLayer, bool canAttackAir)
    {
        List<Vector3> shape = new List<Vector3>();
        List<int> possibilities = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            shape = attackShape.GetRotatedShape(i);
            foreach (Vector3 coord in shape)
            {
                if (hexGrid.GetHex(coord + center, out Hex h))
                {
                    GameObject occupant = h.GetOccupant();
                    if (occupant != null && h.IsVisible())
                    {
                        bool cantAttackOccupant = false;
                        if (!canAttackAir)
                        {
                            UnitController uc = occupant.GetComponent<UnitController>();
                            if (uc != null)
                            {
                                cantAttackOccupant = uc.CanFly();
                            }
                        }
                        if ((targetLayer.value & (1 << occupant.layer)) > 0 && !cantAttackOccupant)
                        {
                            possibilities.Add(i);
                        }
                    }
                }
            }
        }
        return possibilities;
    }
    public Vector3 GetFacingDirection(float angle)
    {
        Vector3 direction = new Vector3(0, 0, 0);
        if(angle >= 0 && angle < 60)
        {
            direction = new Vector3(-1, 1, 0);
        }
        else if (angle >= 60 && angle < 120)
        {
            direction = new Vector3(0, 1, -1);
        }
        else if (angle >= 120 && angle < 180)
        {
            direction = new Vector3(1, 0, -1);
        }
        else if (angle >= 180 && angle < 240)
        {
            direction = new Vector3(1, -1, 0);
        }
        else if (angle >= 240 && angle < 300)
        {
            direction = new Vector3(0, -1, 1);
        }
        else if (angle >= 300 && angle <= 360)
        {
            direction = new Vector3(-1, 0, 1);
        }

        return direction;
    }
    public List<Vector3> GetShapeFacingDirection(Vector3 direction) { return attackShape.GetShapeFacingDirection(direction); }
    public int GetMaxRange() { return attackShape.GetMaxRange(); }
    public bool IsHeal() { return heal; }
}
