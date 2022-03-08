using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] bool playerUnit;
    [SerializeField] float speed = 5f;
    [SerializeField] float heightStep = .5f;
    HexGrid hexGrid;
    HexCoordinates hexCoords;
    [SerializeField] Hex currentHex;
    [SerializeField] GameObject target;

    [SerializeField] LayerMask enemyLayers;
    private Hex targetHex;
    private bool newTarget = false;
    List<Hex> path = new List<Hex>();

    
    
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
        if (path.Count <= 0) { path = null; return; }
       
        if(!(this.gameObject.Equals(path[0].GetOccupant()) || path[0].GetOccupant() == null))
        {
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
        hexGrid.FindPath(currentHex, targetHex, this.gameObject, out path, heightStep);
    }

    

    public void SetTarget(Hex hex)
    {
        targetHex = hex;
        newTarget = true;
    }
    public void SetTarget(GameObject enemy)
    {
        hexGrid.GetHex(enemy.GetComponentInParent<HexCoordinates>().GetHexCoordsRQS(), out targetHex);
        hexGrid.FindPath(currentHex, targetHex, this.gameObject, out path, heightStep, enemyLayers);
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

}
