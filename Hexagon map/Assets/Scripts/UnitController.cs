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
    HexGrid hexGrid;
    HexCoordinates hexCoords;
    UnitCombatController combatController;
    VisionController visionController;
    [SerializeField] Hex currentHex;

    [SerializeField] LayerMask enemyLayers;
    Hex targetHex;
    List<Hex> path = new List<Hex>();
    
    [SerializeField] GameObject selected;
    
    
    private void Awake()
    {
        if (this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
        else { hexCoords = this.gameObject.AddComponent<HexCoordinates>(); }
        hexCoords.MoveToGridCords();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        combatController = this.GetComponent<UnitCombatController>();
        visionController = this.GetComponent<VisionController>();

        if (!playerUnit) { visionController.SetVisible(false); }
        else { Invoke("UpdateVision", 1f); }//updates vision once everything has been loaded
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
            ArriveAtNewHex();
        }
         
    }

    //called once each time the unit arrives at a new hex
    void ArriveAtNewHex()
    {
        currentHex.SetOccupent(null);
        hexCoords.MoveToGridCords();

        Vector3 pos = transform.position;
        pos.y = path[0].GetTargetPoint().y + unitHeight;
        transform.position = pos;

        hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex);
        currentHex.SetOccupent(this.gameObject);

        if (playerUnit)
        {
            UpdateVision();
        }
        

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

   
    void UpdateVision()
    {
        visionController.UpdateVision(heightStep, currentHex);
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

        LayerMask targetLayers = combatController.GetTargetLayers();
        if(targetLayers != (targetLayers & (1 << enemy.layer)) && targetHex != null) { SetTargetHex(targetHex); return; }

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
        if (playerUnit) { visionController.RemoveVision(); }
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
    
    public bool IsVisible() { return visionController.IsVisible(); }

}
