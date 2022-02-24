using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] bool playerUnit;
    [SerializeField] float speed = 5f;
    [SerializeField] float range = 10f;
    [SerializeField] float heightStep = .5f;
    [SerializeField] LayerMask enemyLayers;
    HexGrid hexGrid;
    HexCoordinates hexCoords;
    [SerializeField] Hex currentHex;
    [SerializeField] GameObject target;
    private Hex targetHex;
    private bool newTarget = false;
    List<Hex> path = new List<Hex>();

    [SerializeField] float attackSpeed = 2;
    bool canAttack = true;
    
    private void Awake()
    {
        if (this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
        else { hexCoords = this.gameObject.AddComponent<HexCoordinates>(); }
        hexCoords.MoveToGridCords();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHex == null) {
            hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex);
            currentHex.SetOccupent(this.gameObject);
        }
        CheckForTarget();
        GoToTarget();

        if (canAttack)
        {
            AimAtTarget();
            canAttack = false;
            Invoke("resetAttack", 1 / attackSpeed);
        }
        

    }

    void CheckForTarget()
    {
        if (newTarget)
        {
            UpdatePath();
            newTarget = false;
        }

    }
    void GoToTarget()
    {
        if(path == null) { return; }
        path.TrimExcess();
        if (path.Count <= 0) { return; }

        
        if(!(this.gameObject.Equals(path[0].GetOccupant()) || path[0].GetOccupant() == null))
        {
            UpdatePath();
            return;
        }
        if (Vector3.Distance(transform.position, path[0].GetTargetPoint()) > .05)
        {
            transform.Translate(((path[0].GetTargetPoint() - transform.position).normalized
                * speed
                * Time.deltaTime)
                / path[0].GetMoveDificulty());
        }
        else
        {
            currentHex.SetOccupent(null);
            hexCoords.MoveToGridCords();
            hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex);
            currentHex.SetOccupent(this.gameObject);
            if (path.Count > 0)
            {
                path.RemoveAt(0);
                path.TrimExcess();
            }
        }
         
    }

    void UpdatePath()
    {
        if(hexGrid.FindPath(currentHex, targetHex, this.gameObject, out path, heightStep))
        {

        }
    }

    void AimAtTarget()
    {
        GameObject closest = null;
        List<Hex> hexsInRange = hexGrid.GetHexesInRange((int)range, currentHex);
        if (hexsInRange.Count <= 0) { return; }
        foreach (Hex h in hexsInRange)
        {
            UnitController enemy;
            if (h.GetOccupant() != null)
            {
                if (h.GetOccupant().GetComponent<UnitController>() != null)
                {

                    enemy = h.GetOccupant().GetComponent<UnitController>();
                    if (closest == null) { closest = enemy.gameObject; }
                    else if (h.DistanceFromHex(currentHex) < enemy.GetcurrentHex().DistanceFromHex(currentHex))
                    {
                        closest = enemy.gameObject;
                    }
                }
            }

        }


        if (closest != null)
        {
            //this.transform.LookAt(closest.transform.position);
            Debug.Log("looking at a target");
        } //else {Debug.Log("no closest"); }
    }

    public void SetTarget(Hex hex)
    {
        targetHex = hex;
        newTarget = true;
    }

    void resetAttack()
    {
        canAttack = true;
    }

    public Hex GetcurrentHex() { return currentHex; }
    public bool isPlayerUnit() { return playerUnit; }

}
