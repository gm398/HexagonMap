using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
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
        if(currentHex == null) { hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out currentHex); }
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
        if (path.Count <= 0) { return; }

        if(path[0].GetOccupant().Equals(this.gameObject) || path[0].GetOccupant() == null)
        {

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
        if(hexGrid.FindPath(currentHex, targetHex, out path, heightStep))
        {

        }
    }

    public void SetTarget(Hex hex)
    {
        targetHex = hex;
        newTarget = true;
    }

}
