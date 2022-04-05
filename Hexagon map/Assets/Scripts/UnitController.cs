using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] bool playerUnit;
    [SerializeField] float speed = 5f;
    [SerializeField] float heightStep = .5f;
    [SerializeField] float unitHeight = 0f;
    [SerializeField] bool flyingUnit = false;
    [SerializeField] int visionRange = 3;
    HexGrid hexGrid;
    HexCoordinates hexCoords;
    UnitCombatController combatController;
    [SerializeField] Hex currentHex;

    [SerializeField] LayerMask enemyLayers;
    private Hex targetHex;
    List<Hex> path = new List<Hex>();

    [SerializeField] List<GameObject> visibleComponents;
    [SerializeField] GameObject selected;
    
    private List<Hex> visibleHexes;
    
    private void Awake()
    {
        visibleHexes = new List<Hex>();
        if (this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
        else { hexCoords = this.gameObject.AddComponent<HexCoordinates>(); }
        hexCoords.MoveToGridCords();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        combatController = this.GetComponent<UnitCombatController>();
        Invoke("UpdateVision", 1f);
        if (!playerUnit) { SetVisible(false); }
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
        Vector3 heightDiff = new Vector3(0, unitHeight, 0);
        if (Vector3.Distance(transform.position, path[0].GetTargetPoint() + heightDiff) > .05)
        {
            MoveToHex(path[0]);
        }
        else
        {

            currentHex.SetOccupent(null);
            hexCoords.MoveToGridCords();

            Vector3 pos = transform.position;
            pos.y = path[0].GetTargetPoint().y + unitHeight;
            transform.position = pos;

            hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex);
            currentHex.SetOccupent(this.gameObject);

            UpdateVision();

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
    void UpdateVision()
    {
        if (!playerUnit) { return; }
        List<Hex> newVision = hexGrid.GetHexesInRange(visionRange, currentHex);
        foreach(Hex h in visibleHexes)
        {
            h.SetVisible(false);
        }
        foreach(Hex h in newVision)
        {
            h.SetVisible(true);
        }
        visibleHexes = newVision;
    }
    void RemoveVision()
    {
        foreach (Hex h in visibleHexes)
        {
            h.SetVisible(false);
        }
    }
    void MoveToHex(Hex destination)
    {
        hexCoords.ConvertToHexCords();
        Hex currentHex;
        if (!hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex)){
            currentHex = destination; //not perfect
        }
        Vector3 heightDiff = new Vector3(0, unitHeight, 0);
        if (flyingUnit)
        {
            transform.Translate(((destination.GetTargetPoint() + heightDiff - transform.position).normalized
            * speed
            * Time.deltaTime));
        }
        else {
            transform.Translate(((destination.GetTargetPoint() + heightDiff - transform.position).normalized
                * speed
                * Time.deltaTime)
                / currentHex.GetMoveDificulty());
        }
        Physics.SyncTransforms();
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
        RemoveVision();
        this.gameObject.SetActive(false);
    }

    public void SetVisible(bool isvis)
    {
        foreach(GameObject c in visibleComponents)
        {
            c.SetActive(isvis);
        }
        
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
