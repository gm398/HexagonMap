using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float range = 10f;
    [SerializeField] float heightStep = .5f;
    [SerializeField] LayerMask layers;
    HexGrid hexGrid;
    HexCoordinates hexCoords;
    [SerializeField] Hex currentHex;
    [SerializeField] GameObject target;
    private Hex targetHex;
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
        
    }
    void GoToTarget()
    {
        
       
        
        
    }

}
