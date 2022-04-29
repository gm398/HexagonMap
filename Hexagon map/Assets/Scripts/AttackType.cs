using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType : MonoBehaviour
{
    [SerializeField] AttackShape attackShape;
    [SerializeField] float damage = 5f;
    [SerializeField] GameObject attackGraphic;
    [SerializeField] float graphicTimer = 3f;
    [SerializeField] bool friendlyFire = false;
    [SerializeField] bool heal = false;

    HexGrid hexGrid;
    private void Awake()
    {
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        GameObject attackShapeObject = Instantiate(attackShape.gameObject);
        attackShapeObject.transform.parent = this.transform;
        attackShape = attackShapeObject.GetComponent<AttackShape>();
    }
    public List<GameObject> Attack(Vector3 center, int direction, LayerMask alliedLayers)
    {
        Vector3 direc = attackShape.GetDirectionVector(direction);
        return Attack(center, direc, alliedLayers);
    }
    public List<GameObject> Attack(Vector3 center, Vector3 direction, LayerMask alliedLayers)
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
                if (attackGraphic != null)
                {
                    GameObject graphic = Instantiate(attackGraphic, hex.GetTargetPoint(), Quaternion.identity);
                    Destroy(graphic, graphicTimer);
                }
                GameObject occupant = hex.GetOccupant();
                if (occupant != null)
                {
                    Health health = occupant.GetComponentInChildren<Health>();
                    if (health != null)
                    {
                        bool isEnemy = !((alliedLayers.value & (1 << occupant.layer)) > 0);
                        if (friendlyFire && !heal)
                        {
                            health.TakeDamage(damage);
                            targetsHit.Add(occupant);
                        }
                        else if (!heal && isEnemy)
                        {
                            health.TakeDamage(damage);
                            targetsHit.Add(occupant);
                        }
                        else if (heal && !isEnemy)
                        {
                            health.TakeDamage(damage);
                            targetsHit.Add(occupant);
                        }
                    }
                }
            }
        }
        return targetsHit;
    }
    
    public int CanHit(Vector3 center, LayerMask targetLayer)
    {
        List<Vector3> shape = new List<Vector3>();
        for (int i = 0; i < 6; i++)
        {
            shape = attackShape.GetRotatedShape(i);
            foreach (Vector3 coord in shape)
            {
                if (hexGrid.GetHex(coord + center, out Hex h))
                {
                    GameObject occupant = h.GetOccupant();
                    if (occupant != null)
                    {
                        if ((targetLayer.value & (1 << occupant.layer)) > 0)
                        {
                            return i;
                        }
                    }
                }
            }
        }
        return -1;
    }
    public int GetMaxRange() { return attackShape.GetMaxRange(); }
    public bool IsHeal() { return heal; }
}
