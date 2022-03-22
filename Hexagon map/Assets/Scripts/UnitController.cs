using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] bool playerUnit;
    [SerializeField] float speed = 5f;
    [SerializeField] float heightStep = .5f;
    [SerializeField] bool flyingUnit = false;
    HexGrid hexGrid;
    HexCoordinates hexCoords;
    UnitCombatController combatController;
    [SerializeField] Hex currentHex;

    [SerializeField] LayerMask enemyLayers;
    private Hex targetHex;
    List<Hex> path = new List<Hex>();

    [SerializeField] GameObject selected;
    
    private void Awake()
    {
        if (this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
        else { hexCoords = this.gameObject.AddComponent<HexCoordinates>(); }
        hexCoords.MoveToGridCords();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        combatController = this.GetComponent<UnitCombatController>();
       

    }

    // Update is called once per frame
    void Update()
    {
        if(currentHex == null) {
            Hex temp;
            if(hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out temp))
            {
                currentHex = temp;
                currentHex.SetOccupent(this.gameObject);
            }
        }
        GoToTarget();
    }

   
    void GoToTarget()
    {
        if(path == null) { return; }
        path.TrimExcess();
        if (path.Count <= 0) { path = null; return; }
       
        if(!(this.gameObject.Equals(path[0].GetOccupant()) || path[0].GetOccupant() == null))
        {
            GameObject target = null;
            if(combatController != null) { target = combatController.GetTarget(); }
            if (path[0].GetOccupant().Equals(target)) {
                targetHex = null;
                path = null;
                return;
            }
            UpdatePath();
            return;
        }
        else
        {
            path[0].SetOccupent(this.gameObject);
            currentHex.SetOccupent(null);
        }
        if (Vector3.Distance(transform.position, path[0].GetTargetPoint()) > .05)
        {
            MoveToHex(path[0]);
        }
        else
        {

            currentHex.SetOccupent(null);
            hexCoords.MoveToGridCords();
            Vector3 pos = transform.position;
            pos.y = path[0].GetTargetPoint().y;
            transform.position = pos;
            hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex);
            currentHex.SetOccupent(this.gameObject);

            if (path.Count > 0)
            {
                path.RemoveAt(0);
                path.TrimExcess();
            }
            else
            {
                targetHex = null;
                path = null;
            }
        }
         
    }
    
    void MoveToHex(Hex destination)
    {
        hexCoords.ConvertToHexCords();
        Hex currentHex;
        if (!hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex)){
            currentHex = destination; //not perfect
        }
        transform.Translate(((destination.GetTargetPoint() - transform.position).normalized
            * speed
            * Time.deltaTime)
            / currentHex.GetMoveDificulty());
        
    }

    void UpdatePath()
    {
        
        List<Hex> temp;
        if (hexGrid.FindPath(currentHex, targetHex, this.gameObject, out temp, heightStep))
        {
            path = temp;
        }
        else
        {
            foreach (Hex h in hexGrid.GetNeighbours(targetHex))
            {
                if (h.GetOccupant() == null)
                {
                    if (hexGrid.FindPath(currentHex, h, this.gameObject, out temp, heightStep))
                    {
                        path = temp;
                        return;
                    }
                }
            }
            targetHex = null;
            path = null;
        }
    }

  
    public void SetTargetHex(Hex hex)
    {
        targetHex = hex;
        if (combatController != null) { combatController.SetTarget(null); }
        UpdatePath();
    }
    public void SetTargetEnemy(GameObject enemy)
    {
        if(enemy == null) { return; }
        hexGrid.GetHex(enemy.GetComponentInParent<HexCoordinates>().GetHexCoordsRQS(), out targetHex);
        targetHex = enemy.GetComponentInParent<UnitController>().GetcurrentHex();
        if(enemyLayers != (enemyLayers & (1 << enemy.layer)) && targetHex != null) { SetTargetHex(targetHex); }
        List<Hex> temp;
        Debug.Log("target is an enemy");
        bool validTarget = false;
        if(combatController != null) { validTarget = combatController.SetTarget(enemy); }

        if (validTarget)
        {
            if (combatController.EnemyInRange()) {
                return;
            }
        }

        if (hexGrid.FindPath(currentHex, targetHex, this.gameObject, out temp, heightStep, enemyLayers, combatController.GetRange()))
        {
            path = temp;
        }
       
    }



    public void Dead()
    {
        Debug.Log("unit is dead");
        currentHex.SetOccupent(null);
        this.gameObject.SetActive(false);
    }

    public Hex GetcurrentHex() { return currentHex; }
    public bool isPlayerUnit() { return playerUnit; }
    public LayerMask GetEnemyLayers() { return enemyLayers; }

    public bool IsEnemy(int layer) { return enemyLayers == (enemyLayers & (1 << layer)); }
    public bool CanFly() { return flyingUnit; }


    public void SetSelected(bool isSelected)
    {
        if (selected != null)
        {
            selected.SetActive(isSelected);
        }
    }

}
